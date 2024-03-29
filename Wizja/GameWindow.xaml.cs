﻿using System.Media;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wizja.classes;
using Label = System.Windows.Controls.Label;

namespace Wizja
{
    /// <summary>
    /// Logika interakcji dla klasy GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        bool[] direction = new bool[4];
        bool isBpressed = false;
        //Testy przeciwników
        private ObjectLoader objectLoader;
        private Shop itemshop;
        private Rectangle skullIcon;
        private ImageBrush skullImage = new ImageBrush();
        public Label pathLabel;
        public Spawner spawner;
        public Player player;
        public HUD hud;
        int animTimer = 0;
        bool walkin = false;

        private System.Timers.Timer gameTimer = new System.Timers.Timer();
        public GameWindow()
        {
            InitializeComponent();

            //ładowanie mapy
            objectLoader = new ObjectLoader(gameCanvas);
            //dodawanie obiektu hudu
            
            hud = new HUD(100, 10, 0, statCanvas);
            CreateEnemyCounter();
            CreatePathLabel();
            //Tworzenie shop
            itemshop = new Shop(gameCanvas, shopCanvas, hud, this);
            KeyUp += KeyIsUp;
            KeyDown += KeyIsDown;
            GameStarter();
            itemshop.initPlayer(player);
            itemshop.timerShopCheck.Start();



            //inicjalizacja gameTimera
            gameTimer.Interval = 16; //16 MILISEKUND
            gameTimer.Elapsed += GameTick;
            gameTimer.Start(); //od razu zaczyna timer gameTimer
        }

        private void CreatePathLabel()
        {
            pathLabel = new Label
            {
                Content = $"TO SHOP",
                FontSize = 50,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(pathLabel, 3100);
            Canvas.SetTop(pathLabel, 1650);
            upperCanvas.Children.Add(pathLabel);
            pathLabel.Visibility = Visibility.Hidden;

        }
        private void GameTick(object sender, ElapsedEventArgs e)
        {
            try //timer obsługa
            {
                gameCanvas.Dispatcher.Invoke(() =>
                {
                    MovementHandler.Step(direction, objectLoader.GetListMovingObjects(), this.GetRectanglesByName("Enemy"), this.GetRectanglesByName("SpawnerObject"), objectLoader.GetListMapObjects());
                    //po przerwie spawnuje nowych przeciwników
                    if (hud.GetTime() == 0)
                    {
                        pathLabel.Visibility = Visibility.Hidden;
                        skullIcon.Visibility = Visibility.Visible;
                        spawner.Spawn();
                        spawner.MoveEveryOne(player, objectLoader.GetListMapObjects());
                        hud.UpdateCounterOfEnemy(spawner.enemies[spawner.currentRound].Count(), spawner.GetListOfLivingEnemies().Count());
                        EndOfGameLose();
                        EndOfGameWin();
                    }
                    else
                    {
                        skullIcon.Visibility = Visibility.Hidden;
                    }

                    animTimer = (animTimer+1)%32;

                    if(walkin)
                    {
                        if(animTimer >= 0 && animTimer < 16)
                        {
                            player.playerImage.Fill = new ImageBrush(player.rotatedPlayer1);
                        }
                        else
                        {
                            player.playerImage.Fill = new ImageBrush(player.rotatedPlayer2);
                        }
                    }
                    else
                    {
                        player.playerImage.Fill = new ImageBrush(player.rotatedPlayer);
                    }

                });
            }
            catch { }
        } 

       

        private void GameStarter()
        {
            player = new Player(gameCanvas, hud, objectLoader.GetListMapObjects());
            player.MouseMoveHandler(gameCanvas);
            MovementHandler.initialize(player, pathLabel);
            List<Point> enemiesSpawner = new List<Point>() { new Point(5500, 500), new Point(5500, 3500), new Point(500, 500), new Point(500, 3500) };
            int[][] enemyLists = new int[5][];


            spawner = new Spawner(enemiesSpawner, 5, gameCanvas, player, upperCanvas);
            enemyLists[0] = new int[] { 75, 25, 0, 0 };
            enemyLists[1] = new int[] { 55, 35, 15, 5 };
            enemyLists[2] = new int[] { 35, 35, 20, 10 };
            enemyLists[3] = new int[] { 15, 40, 35, 10 };
            enemyLists[4] = new int[] { 5, 40, 35, 20 };
            spawner.GenerateEnemies(enemyLists[0], 20, 0, 125);
            spawner.GenerateEnemies(enemyLists[1], 30, 1, 115);
            spawner.GenerateEnemies(enemyLists[2], 40, 2, 110);
            spawner.GenerateEnemies(enemyLists[3], 50, 3, 105);
            spawner.GenerateEnemies(enemyLists[4], 80, 4, 50);

            player.SetAllEnemies(spawner.GetAllEnemies());

        }

        private void EndOfGameLose()
        {
            if (player.healthPoints == 0)
            {
                ObjectLoader.hitObjects.Clear();
                DeathWindow deathWindow = new DeathWindow(hud);
                gameTimer.Stop();
                deathWindow.Show();
                this.Close();
            }
        }
        private void EndOfGameWin()
        {
            if(spawner.AllDead() && spawner.currentRound == 5)
            {
                WinWindow winWindow = new WinWindow(hud);
                gameTimer.Stop();
                winWindow.Show();
                this.Close();
            }
        }

        private void CreateEnemyCounter()
        {
            skullImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/skull.png"));
            skullIcon = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = skullImage
            };
            Canvas.SetLeft(skullIcon, 1115);
            Canvas.SetTop(skullIcon, 30);
            statCanvas.Children.Add(skullIcon);
            skullIcon.Visibility = Visibility.Hidden;
        }

        // Zwraca obiekty typu rectangle o podanej nazwie dla przeciwnika "Enemy"
        private List<Rectangle> GetRectanglesByName(string name)
        {
            var objects = gameCanvas.Children.OfType<Rectangle>().Where(x => x.Name.Contains(name));
            List<Rectangle> results = new List<Rectangle>();
            foreach (var rectangle in objects)
            {
                results.Add(rectangle);
            }
            return results;
        }

        public void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                direction[0] = true;
                walkin = true;
            }
            else if (e.Key == Key.A)
            {
                direction[1] = true;
                walkin = true;
            }
            else if (e.Key == Key.S)
            {
                direction[2] = true;
                walkin = true;
            }
            else if (e.Key == Key.D)
            {
                direction[3] = true;
                walkin = true;
            }
            else if (e.Key == Key.B)
            {
                isBpressed = true;
            }
        }

        public void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                direction[0] = false;
            }
            else if (e.Key == Key.A)
            {
                direction[1] = false;
            }
            else if (e.Key == Key.S)
            {
                direction[2] = false;
            }
            else if (e.Key == Key.D)
            {
                direction[3] = false;
            }
            else if (e.Key == Key.B)
            {
                isBpressed = false;
            }

            if (!direction.Contains(true))
            {
                walkin = false;
            }
        }
        public bool getB()
        {
            return isBpressed;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Pass the mouse click position to the player object to handle shooting
            player.MouseLeftButtonDown(sender, e);
        }
    }
}
