using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Dodge_Game_2
{
    public class GameManager
    {
        public GoodPlayer goodGuy;
        public List<BadPlayer> badGuy;
        public Gift speed, slow, bomb;
        Canvas canvas = new Canvas();
        public bool[] outOfTheGame = new bool[10];

        public Random random = new Random();


        public GameManager()
        {
            InitializeBadPlayer();
            InitializeGoodPlayer();
        }
        public void InitializeBomb()
        {
            bomb = new Gift(50, 50, random.Next(800), random.Next(500), "ms-appx:///pictures/bomb.png");
        }
        public void InitializeSlow()
        {
            slow = new Gift(50, 50, random.Next(800), random.Next(700), "ms-appx:///pictures/turtle.png");
        }
        public void InitializeSpeedGift()
        {
            speed = new Gift(50, 50, random.Next(800), random.Next(700), "ms-appx:///pictures/horse.png");
        }


        public void InitializeGoodPlayer()
        {
            goodGuy = new GoodPlayer(100, 100, 700, 650, "ms-appx:///pictures/Good guy.png");
        }

        public void InitializeBadPlayer()
        {
            badGuy = new List<BadPlayer>();
            for (int i = 0; i < 10; i++)
            {
                badGuy.Add(new BadPlayer(100, 100, random.Next(300), i * 150,
                    $"ms-appx:///pictures/bad guy{random.Next(1, 10)}.png"));
            }

        }

        //public GameManager(Canvas canvasPlayingArea, List<PlayerModel> loadPlayers)
        //{
        //    canvas = canvasPlayingArea;
        //    Image PlayerImage = new Image();
        //    PlayerImage.Source = new BitmapImage(new Uri("ms-appx:///pictures/Good guy.png"));
        //    goodGuy = new GoodPlayer(PlayerImage, canvas, loadPlayers[0].AmountOfLives);
        //    Canvas.SetLeft(PlayerImage, loadPlayers[0].PlayerXPosition);
        //    Canvas.SetTop(PlayerImage, loadPlayers[0].PlayerYPosition);
        //    PlayerImage.Width = 50;
        //    PlayerImage.Height = 50;
        //    canvasPlayingArea.Children.Add(PlayerImage);
        //    Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        //    Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        //    InitializeBadPlayer(loadPlayers);



        public async void SaveGameData()
        {
            StorageFolder saveFolder = ApplicationData.Current.LocalFolder;
            StorageFile saveFile = await saveFolder.CreateFileAsync("save.txt", CreationCollisionOption.ReplaceExisting);

            for (int i = 0; i < 10; i++)
            {
                if (!outOfTheGame[i])
                {
                    await FileIO.AppendTextAsync(saveFile, Canvas.GetLeft(badGuy[i]._image).ToString() + "\n");//save enemies positions
                    await FileIO.AppendTextAsync(saveFile, Canvas.GetTop(badGuy[i]._image).ToString() + "\n");

                }
                else
                {
                    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");   //killed enemies get 2000 mark. 
                    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");
                }
            }

            await FileIO.AppendTextAsync(saveFile, Canvas.GetLeft(goodGuy._image).ToString() + "\n");  //save player's position. 
            await FileIO.AppendTextAsync(saveFile, Canvas.GetTop(goodGuy._image).ToString() + "\n");
            await FileIO.AppendTextAsync(saveFile, goodGuy.lifeCounter + "\n"); //save player's remaining lives.

            //if (canvas.Children.Contains(speed._image))        //check if speed gift is on screen.
            //{
            //    await FileIO.AppendTextAsync(saveFile, Canvas.GetLeft(speed._image).ToString() + "\n");
            //    await FileIO.AppendTextAsync(saveFile, Canvas.GetTop(speed._image).ToString() + "\n");
            //}
            //else
            //{
            //    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");
            //    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");
            //}
            //if (canvas.Children.Contains(slow._image))                //check if slow gift is on screen.
            //{
            //    await FileIO.AppendTextAsync(saveFile, Canvas.GetLeft(slow._image).ToString() + "\n");
            //    await FileIO.AppendTextAsync(saveFile, Canvas.GetTop(slow._image).ToString() + "\n");
            //}
            //else
            //{
            //    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");
            //    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");
            //}
            //if (canvas.Children.Contains(bomb._image))          //check if bomb gift is on screen. 
            //{
            //    await FileIO.AppendTextAsync(saveFile, Canvas.GetLeft(bomb._image).ToString() + "\n");
            //    await FileIO.AppendTextAsync(saveFile, Canvas.GetTop(bomb._image).ToString() + "\n");
            //}
            //else
            //{
            //    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");
            //    await FileIO.AppendTextAsync(saveFile, 2000 + "\n");

            //}

        }
        public void LoadGame()
        {
            string filePath = @"C:\Users\elina\AppData\Local\Packages\4147f86a-fe2f-4f0a-af7e-8245766b7125_f9q874pxpfxk6\LocalState\save.txt";
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < 20; i += 2)
            {
                if (int.Parse(lines[i/2]) == 2000)
                {
                    outOfTheGame[i / 2] = true;
                }
                else
                {
                    outOfTheGame[i / 2] = false;
                }

            }

            for (int i = 0; i < badGuy.Count; i++)
            {
                Canvas.SetLeft(badGuy[i]._image, double.Parse(lines[i * 2]));
                Canvas.SetTop(badGuy[i]._image, double.Parse(lines[(i * 2) + 1]));
            }
            Canvas.SetLeft(goodGuy._image, double.Parse(lines[20]));
            Canvas.SetTop(goodGuy._image, double.Parse(lines[21]));


            canvas.Children.Clear();

            //if (int.Parse(lines[23]) != 2000)                                //create gifts that weren't already taken.
            //{
            //    Canvas.SetLeft(speed._image, double.Parse(lines[23]));
            //    Canvas.SetTop(speed._image, double.Parse(lines[24]));
            //    canvas.Children.Add(speed._image);
            //    Canvas.GetLeft(speed._image);
            //    Canvas.GetTop(speed._image);
            //}

            canvas.Children.Add(goodGuy._image);                        //create the player.

            for (int i = 0; i < badGuy.Count; i++)                       //create enemies.
            {
                if (!outOfTheGame[i])
                {
                    canvas.Children.Add(badGuy[i]._image);
                }
            }

        }
        


    }






}










