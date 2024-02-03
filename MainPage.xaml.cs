using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Point = System.Drawing.Point;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Dodge_Game_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer, timer2, timer3, timer4, timer5, timer6, timer7, timer8;
        int timeInterval;
        GameManager game_Manager;
        bool gameIsOn = true;
        MediaPlayer gameBackgroundSound = new MediaPlayer();

        //MediaPlayer youWinSound = new MediaPlayer();
        //MediaPlayer baddiescollisionsound = new MediaPlayer();

        public MainPage()
        {
            InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            btnSaveGame.Visibility = Visibility.Collapsed;
        }
        
        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {

            rbtnLoadGame.IsEnabled = false;
            rbtnLoadGame.Visibility = Visibility.Collapsed;
            btnSaveGame.IsEnabled = true;
            btnSaveGame.Visibility = Visibility.Collapsed;
            txtPause.Visibility = Visibility.Collapsed;                      //reomove start game image & text upon starting the game.
            txtResume.Visibility = Visibility.Collapsed;
            startBackground.Visibility = Visibility.Collapsed;
            btnStartGame.Visibility = Visibility.Collapsed;
            btnStartGame.IsEnabled = false;
            GameBackgroundSound();                               // add sound track
            game_Manager = new GameManager();

            if ((bool)!rbtnLoadGame.IsChecked)                        //if this is a new game, create all players and lives.
            {
                myCanvas.Children.Add(game_Manager.goodGuy._image);      //add player image

                for (int i = 0; i < game_Manager.badGuy.Count; i++)       //add enemies images
                {
                    myCanvas.Children.Add(game_Manager.badGuy[i]._image);
                }
                for (int i = 0; i < game_Manager.goodGuy.Lives.Count; i++)   //add players "lives" images.
                {
                    myCanvas.Children.Add(game_Manager.goodGuy.Lives[i]);
                }
            }
            else           //if this is a loded game, create only players who didn't die.

            {

                for (int i = 0; i < game_Manager.outOfTheGame.Length; i++)
                {
                    if (!game_Manager.outOfTheGame[i])
                        myCanvas.Children.Add(game_Manager.badGuy[i]._image);

                }
                for (int i = 0; i < game_Manager.goodGuy.Lives.Count; i++)   //add players "lives" images.
                {
                    myCanvas.Children.Add(game_Manager.goodGuy.Lives[i]);
                }

                myCanvas.Children.Add(game_Manager.goodGuy._image);             //add player's image.
            }

            #region Timers
            myCanvas.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///pictures/Field.png")) };
            timer = new DispatcherTimer();
            timeInterval = 50;
            timer.Interval = TimeSpan.FromMilliseconds(timeInterval);
            timer.Tick += TimerTickGeneral;                //game timer
            timer.Start();
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(15);     //enemies get faster every 15 seconds.
            timer2.Tick += TimerTickEnemyIncreasedSpeed;
            timer2.Start();
            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromSeconds(20);      //speed gift is thrown after 20seconds.
            timer3.Tick += TimerTickThrowSpeed;
            timer3.Start();
            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(timeInterval);
            timer4.Tick += TimerTickPlayerIncreasedSpeed;
            timer5 = new DispatcherTimer();
            timer5.Interval = TimeSpan.FromSeconds(10);
            timer5.Tick += TimerTickThrowSlow;               //slow gift is thrown after 10 seconds.
            timer5.Start();
            timer6 = new DispatcherTimer();
            timer6.Interval = TimeSpan.FromMilliseconds(timeInterval);
            timer6.Tick += TimerTickSlowDown;
            timer7 = new DispatcherTimer();
            timer7.Interval = TimeSpan.FromSeconds(15);
            timer7.Tick += TimerTickThrowBomb;                //throw bomb after 15 seconds.
            timer7.Start();
            timer8 = new DispatcherTimer();
            timer8.Interval = TimeSpan.FromMilliseconds(timeInterval);
            timer8.Tick += TimerTickYouDie;
            #endregion
        }




        private void TimerTickYouDie(object sender, object e)                  //checks intersection between player & bomb.
        {

            Rectangle rect1 = new Rectangle();               //represents player.
            Rectangle rect2 = new Rectangle();               //represents bomb image 
            rect1.X = (int)game_Manager.goodGuy.ImageLeft();
            rect1.Y = (int)game_Manager.goodGuy.ImageTop();
            rect1.Width = (int)game_Manager.goodGuy._image.Width / 2;
            rect1.Height = (int)game_Manager.goodGuy._image.Height / 2;

            rect2.X = (int)game_Manager.bomb.ImageLeft();
            rect2.Y = (int)game_Manager.bomb.ImageTop();
            rect2.Width = (int)game_Manager.bomb._image.Width / 2;
            rect2.Height = (int)game_Manager.bomb._image.Height / 2;

            if (rect1.IntersectsWith(rect2))
            {
                gameIsOn = false;
                myCanvas.Children.Remove(game_Manager.bomb._image);
                GameOver();
            }


        }

        private void TimerTickThrowBomb(object sender, object e)
        {
            game_Manager.InitializeBomb();                         //create bomb.
            myCanvas.Children.Add(game_Manager.bomb._image);       //throw bomb to canvas.
            timer8.Start();                                        //start checking for intersection.
            timer7.Stop();                                         //stop throwing bomb. 
        }

        private void TimerTickThrowSlow(object sender, object e)         //throw turtle & start timer that checks for intersection with it.
        {

            game_Manager.InitializeSlow();                        //create turtle.
            myCanvas.Children.Add(game_Manager.slow._image);       //throw turtle to canvas.
            timer6.Start();                                           //start checking for intersection.
            timer5.Stop();                                             //stop throwing turtle.
        }

        private void TimerTickSlowDown(object sender, object e)              //check  intersection with turtle.
        {
            Rectangle rect1 = new Rectangle();               //represents player.
            Rectangle rect2 = new Rectangle();               //represents slow image (turtle).
            rect1.X = (int)game_Manager.goodGuy.ImageLeft();
            rect1.Y = (int)game_Manager.goodGuy.ImageTop();
            rect1.Width = (int)game_Manager.goodGuy._image.Width / 2;
            rect1.Height = (int)game_Manager.goodGuy._image.Height / 2;

            rect2.X = (int)game_Manager.slow.ImageLeft();
            rect2.Y = (int)game_Manager.slow.ImageTop();
            rect2.Width = (int)game_Manager.slow._image.Width / 2;
            rect2.Height = (int)game_Manager.slow._image.Height / 2;

            if (rect1.IntersectsWith(rect2))
            {
                myCanvas.Children.Remove(game_Manager.slow._image);
                game_Manager.goodGuy.step -= game_Manager.goodGuy.step + 2;
            }
        }





        private void TimerTickThrowSpeed(object sender, object e)     //throw horse & start timer that checks for intersection with it.
        {
            game_Manager.InitializeSpeedGift();                       //create horse.
            myCanvas.Children.Add(game_Manager.speed._image);         //throw horse to canvas.
            timer4.Start();                                           //start checking for intersection
            timer3.Stop();                                            //stop throwing horse. 


        }
        private void TimerTickPlayerIncreasedSpeed(object sender, object e)      //check intersection with horse.
        {

            Rectangle rect1 = new Rectangle();               //represents player.
            Rectangle rect2 = new Rectangle();               //represents speed image (horse).
            rect1.X = (int)game_Manager.goodGuy.ImageLeft();
            rect1.Y = (int)game_Manager.goodGuy.ImageTop();
            rect1.Width = (int)game_Manager.goodGuy._image.Width / 2;
            rect1.Height = (int)game_Manager.goodGuy._image.Height / 2;

            rect2.X = (int)game_Manager.speed.ImageLeft();
            rect2.Y = (int)game_Manager.speed.ImageTop();
            rect2.Width = (int)game_Manager.speed._image.Width / 2;
            rect2.Height = (int)game_Manager.speed._image.Height / 2;

            if (rect1.IntersectsWith(rect2))
            {
                game_Manager.goodGuy.step += 2;                              //increase player speed upon intersection with horse. 
                myCanvas.Children.Remove(game_Manager.speed._image);
            }

        }

        public void GameBackgroundSound()
        {

            if (gameIsOn == true)
            {
                _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///sounds/01 Simpsons Theme Song.mp3"));
                gameBackgroundSound = _mediaPlayerElement.MediaPlayer;
                gameBackgroundSound.Play();
            }
        }
        private void TimerTickEnemyIncreasedSpeed(object sender, object e)          //baddies get faster every 15 seconds.
        {
            timer.Interval = TimeSpan.FromMilliseconds(timeInterval);
            for (int i = 0; i < game_Manager.badGuy.Count; i++)
            {
                game_Manager.badGuy[i].step += 3;
            }

        }

        private void TimerTickGeneral(object sender, object e)                   //general game timer.
        {
            for (int i = 0; i < game_Manager.badGuy.Count; i++)
            {

                Chase(game_Manager.badGuy[i]);
                BaddiesCollision();
                GoodieCollision();
            }
        }

        private void BaddiesCollision()                  //check collision between baddies.
        {

            Rectangle rect1 = new Rectangle();
            Rectangle rect2 = new Rectangle();
            for (int i = 0; i < game_Manager.badGuy.Count; i++)
            {
                rect1.X = (int)game_Manager.badGuy[i].ImageLeft();    //rect1 = badGuys list
                rect1.Y = (int)game_Manager.badGuy[i].ImageTop();
                rect1.Width = (int)game_Manager.badGuy[i]._width / 2;
                rect1.Height = (int)game_Manager.badGuy[i]._height / 2;

                for (int j = 0; j < game_Manager.badGuy.Count; j++)
                {
                    if (i != j)                                         //only if it's a different badGuy from the list.
                    {
                        rect2.X = (int)game_Manager.badGuy[j].ImageLeft();
                        rect2.Y = (int)game_Manager.badGuy[j].ImageTop();       //rect1 = badGuys list
                        rect2.Width = (int)game_Manager.badGuy[j]._width / 2;
                        rect2.Height = (int)game_Manager.badGuy[j]._height / 2;
                    }
                    if (rect1.IntersectsWith(rect2))                           //in case of a collision
                    {

                        rect1.X = 0;
                        rect1.Y = 0;
                        myCanvas.Children.Remove(game_Manager.badGuy[i]._image);             //remove from canvas.
                        game_Manager.badGuy.RemoveAt(i);                                     //remove from list.

                        if (game_Manager.badGuy.Count == 1)
                        {
                            gameIsOn = true;
                            YouWin();
                            gameIsOn = false;


                            //YouWinSound();



                        }

                    }
                }

            }
        }

        private void saveGame_Click(object sender, RoutedEventArgs e)
        {
            GameManager game_Manager = new GameManager();
            game_Manager.SaveGameData();
            //List<PlayerModel> allPlayers = GetAllPlayers();
            //SaveDataSchema.SaveToFile(allPlayers);
        }

        //private List<PlayerModel> GetAllPlayers()
        //{
        //    List<PlayerModel> allPlayers = new List<PlayerModel>();
        //    allPlayers.Add(new PlayerModel
        //    {
        //        PlayerXPosition = Canvas.GetLeft(game_Manager.goodGuy._image),
        //        PlayerYPosition = Canvas.GetTop(game_Manager.goodGuy._image),
        //        AmountOfLives = game_Manager.goodGuy.lifeCounter
        //    });

        //    for (int i = 0; i < game_Manager.badGuy.Count; i++)
        //    {
        //        allPlayers.Add(new PlayerModel
        //        {
        //            PlayerXPosition = Canvas.GetLeft(game_Manager.badGuy[i]._image),
        //            PlayerYPosition = Canvas.GetTop(game_Manager.badGuy[i]._image),
        //        });
        //    }
        //    return allPlayers;

        //}


        //private  void LoadGame_Click(object sender, RoutedEventArgs e)
        //{




        //    //List<PlayerModel> loadPlayers = await SaveDataSchema.LoadFromFile();
        //    //if(loadPlayers.Count == 0)
        //    //{
        //    //    MessageDialog messageDialog = new MessageDialog("No saved game to load");
        //    //    await messageDialog.ShowAsync();    
        //    //}

        //    //gameIsOn = true;
        //    //GameManager gameManager = new GameManager(myCanvas, loadPlayers);

        //}

        private void rbtnLoadGame_Checked(object sender, RoutedEventArgs e)
        {
            game_Manager = new GameManager();
            //myCanvas.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri("ms-appx:///pictures/Field.png")) };
            //timer = new DispatcherTimer();
            //timeInterval = 50;
            //timer.Interval = TimeSpan.FromMilliseconds(timeInterval);
            //timer.Tick += TimerTickGeneral;
            //timer.Start();
            //timer2 = new DispatcherTimer();
            //timer2.Interval = TimeSpan.FromSeconds(10);     //enemies get faster every 10 seconds.
            //timer2.Tick += TimerTickEnemyIncreasedSpeed;
            //timer2.Start();
            //timer3 = new DispatcherTimer();
            //timer3.Interval = TimeSpan.FromSeconds(20);      //speed gift is thrown after 20seconds.
            //timer3.Tick += TimerTickThrowSpeed;
            //timer3.Start();
            //timer4 = new DispatcherTimer();
            //timer4.Interval = TimeSpan.FromMilliseconds(timeInterval);
            //timer4.Tick += TimerTickPlayerIncreasedSpeed;
            //timer5 = new DispatcherTimer();
            //timer5.Interval = TimeSpan.FromSeconds(10);
            //timer5.Tick += TimerTickThrowSlow;               //slow gift is thrown after 10 seconds.
            //timer5.Start();
            //timer6 = new DispatcherTimer();
            //timer6.Interval = TimeSpan.FromMilliseconds(timeInterval);
            //timer6.Tick += TimerTickSlowDown;
            //timer7 = new DispatcherTimer();
            //timer7.Interval = TimeSpan.FromSeconds(15);
            //timer7.Tick += TimerTickThrowBomb;                //throw bomb after 15 seconds.
            //timer7.Start();
            //timer8 = new DispatcherTimer();
            //timer8.Interval = TimeSpan.FromMilliseconds(timeInterval);
            //timer8.Tick += TimerTickYouDie;
            game_Manager.LoadGame();
            //timer.Start();
        }





        //private void BaddiesCollisionSound()
        //{
        //    _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///sounds/explosion.mp3"));
        //    baddiescollisionsound = _mediaPlayerElement.MediaPlayer;
        //    baddiescollisionsound.Play();

        //}
        private void YouWin()                                             //winners video.
        {
            if (gameIsOn == true)
            {
                MediaPlayer youWin = new MediaPlayer();
                _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///videos/Win.mp4"));
                youWin = _mediaPlayerElement.MediaPlayer;
                youWin.Play();
                //myCanvas.Children.Clear();
                //btnStartGame.IsEnabled = true;
                //btnStartGame.Visibility = Visibility.Visible;
                //for (int i = 0; i < game_Manager.badGuy.Count; i++)
                //{
                //    game_Manager.badGuy.RemoveAt(i);
                //    game_Manager.badGuy.Clear();
                //}
                //game_Manager.InitializeBadPlayer();

            }
        }
        //private void YouWinSound()
        //{   _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///07 Stage Clear.mp3"));
        //    youWinSound = _mediaPlayerElement.MediaPlayer;
        //    youWinSound.Play();
        //}


        private void GoodieCollision()                  //check collision between palayer & baddie.
        {
            Rectangle rect1 = new Rectangle();
            Rectangle rect2 = new Rectangle();

            rect1.X = (int)game_Manager.goodGuy.ImageLeft();           //rect1 = the player
            rect1.Y = (int)game_Manager.goodGuy.ImageTop();
            rect1.Width = (int)game_Manager.goodGuy._width / 2;
            rect1.Height = (int)game_Manager.goodGuy._height / 2;

            for (int i = 0; i < game_Manager.badGuy.Count; i++)
            {
                rect2.X = (int)game_Manager.badGuy[i].ImageLeft();       //rect2 = enemies list.
                rect2.Y = (int)game_Manager.badGuy[i].ImageTop();
                rect2.Width = (int)game_Manager.badGuy[i]._width / 2;
                rect2.Height = (int)game_Manager.badGuy[i]._height / 2;

                if (rect1.IntersectsWith(rect2))                           //in case of collision between player & enemy.
                {
                    Canvas.SetTop(game_Manager.badGuy[i]._image, 0);
                    Canvas.SetLeft(game_Manager.badGuy[i]._image, 0);
                    game_Manager.goodGuy.lifeCounter--;
                    myCanvas.Children.Remove(game_Manager.goodGuy.Lives[game_Manager.goodGuy.lifeCounter]);//remove life from canvas
                    game_Manager.goodGuy.Lives.RemoveAt(game_Manager.goodGuy.lifeCounter);//remove life from counter itself.

                    if (game_Manager.goodGuy.lifeCounter == 0)
                    {
                        gameIsOn = false;
                        {
                            GameOver();
                            break;
                        }

                    }


                }
            }
        }
        private void GameOver()                                  //game over video
        {


            if (gameIsOn == false)
            {
                MediaPlayer gameOver = new MediaPlayer();
                _mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///videos/Game over.mp4"));
                gameOver = _mediaPlayerElement.MediaPlayer;
                gameOver.Play();
                //myCanvas.Children.Clear();
                //btnStartGame.IsEnabled = true;
                //btnStartGame.Visibility = Visibility.Visible;
                //for (int i = 0; i < game_Manager.badGuy.Count; i++)
                //{
                //    game_Manager.badGuy.RemoveAt(i);
                //}
                
                //game_Manager.InitializeBadPlayer();
                //myCanvas.Visibility = Visibility.Visible;



            }

        }


        void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)      //player's movement
        {
            if (gameIsOn == true)
            {
                VirtualKey movement = args.VirtualKey;
                switch (movement)
                {
                    case VirtualKey.Up:
                        if (game_Manager.goodGuy.ImageTop() >= 2)
                            Canvas.SetTop(game_Manager.goodGuy._image, game_Manager.goodGuy.ImageTop() - game_Manager.goodGuy.step - 5);  //up

                        break;
                    case VirtualKey.Down:
                        if (game_Manager.goodGuy.ImageTop() + game_Manager.goodGuy._image.Height <= myCanvas.ActualHeight + 30)
                            Canvas.SetTop(game_Manager.goodGuy._image, game_Manager.goodGuy.ImageTop() + game_Manager.goodGuy.step + 5);   //down

                        break;
                    case VirtualKey.Left:
                        if (game_Manager.goodGuy.ImageLeft() >= myCanvas.MinWidth - 30)
                            Canvas.SetLeft(game_Manager.goodGuy._image, game_Manager.goodGuy.ImageLeft() - game_Manager.goodGuy.step - 5);   //left

                        break;
                    case VirtualKey.Right:
                        if (game_Manager.goodGuy.ImageLeft() + game_Manager.goodGuy._image.Width <= myCanvas.ActualWidth + 15)
                            Canvas.SetLeft(game_Manager.goodGuy._image, game_Manager.goodGuy.ImageLeft() + game_Manager.goodGuy.step + 5);     //right

                        break;
                    case VirtualKey.Space:
                        Random rand = new Random();                                                                             //move to a random location.
                        Canvas.SetLeft(game_Manager.goodGuy._image, rand.Next(1200));
                        Canvas.SetTop(game_Manager.goodGuy._image, rand.Next(700));
                        break;

                    case VirtualKey.Enter:                                                                                     //pause the game.
                        PauseGame();
                        btnSaveGame.Visibility = Visibility.Visible;
                        break;

                    case VirtualKey.Shift:
                        ResumeGame();                                                                                         //resume the game.
                        break;

                }

            }





        }

        private void ResumeGame()
        {
            gameIsOn = true;
            timer.Start();
            timer2.Start();
            timer3.Start();
            timer5.Start();
            timer7.Start();
            gameBackgroundSound.Play();
        }
        private void PauseGame()
        {
            gameBackgroundSound.Pause();
            timer.Stop();
            timer2.Stop();
            timer3.Stop();
            timer5.Stop();
            timer7.Stop();
            gameIsOn = true;
            



        }


        private void Chase(BadPlayer enemy)          //make enemies chase player
        {
            if (gameIsOn == true)
            {


                double enemyLeft = enemy.ImageLeft();
                double enemyTop = enemy.ImageTop();

                double playerLeft = game_Manager.goodGuy.ImageLeft();
                double playerTop = game_Manager.goodGuy.ImageTop();

                var distance = new Point((int)(playerLeft - enemyLeft), (int)(playerTop - enemyTop));
                if (distance.X >= 0 && distance.Y >= 0)
                {
                    //down-right
                    Canvas.SetTop(enemy._image, enemyTop + enemy.step);
                    Canvas.SetLeft(enemy._image, enemyLeft + enemy.step);

                }
                else if (distance.X <= 0 && distance.Y <= 0)
                {
                    //up-left
                    Canvas.SetTop(enemy._image, enemyTop - enemy.step);
                    Canvas.SetLeft(enemy._image, enemyLeft - enemy.step);
                }
                else if (distance.X >= 0 && distance.Y <= 0)
                {
                    //up-right
                    Canvas.SetTop(enemy._image, enemyTop - enemy.step);
                    Canvas.SetLeft(enemy._image, enemyLeft + enemy.step);

                }
                else if (distance.X <= 0 && distance.Y >= 0)
                {
                    //down-left
                    Canvas.SetTop(enemy._image, enemyTop + enemy.step);
                    Canvas.SetLeft(enemy._image, enemyLeft - enemy.step);
                }
            }
        }


    }
}
