﻿using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Wizja.classes;

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
        public Spawner spawner;
        public Player player;
        public HUD hud;

        private System.Timers.Timer gameTimer = new System.Timers.Timer();
        public GameWindow()
        {
            InitializeComponent();


            //ładowanie mapy
            objectLoader = new ObjectLoader(gameCanvas);
            //dodawanie obiektu hudu
            
            hud = new HUD(100, 30, 300, statCanvas);
            //Tworzenie shop
            itemshop = new Shop(gameCanvas, shopCanvas, hud, this);

            KeyUp += KeyIsUp;
            KeyDown += KeyIsDown;
            testing_ERYK();
            itemshop.initPlayer(player);
            itemshop.timerShopCheck.Start();



            //inicjalizacja gameTimera
            gameTimer.Interval = 16; //16 MILISEKUND
            gameTimer.Elapsed += GameTick;
            gameTimer.Start(); //od razu zaczyna timer gameTimer



        }
        private void GameTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                gameCanvas.Dispatcher.Invoke(() =>
                {
                    MovementHandler.Step(direction, objectLoader.GetListMovingObjects());
                    MovementHandler.Step(direction, this.GetRectanglesByName("Enemy"));


                    //Testowanie Przeciwników
                    if (hud.GetTime() == 0)
                    {
                        spawner.Spawn();
                        spawner.MoveEveryOne(player, objectLoader.GetListMapObjects());
                        EndOfGame();
                    }

                });
            }
            catch { }
        }
        private void testing_ERYK()
        {
            player = new Player(gameCanvas, hud, objectLoader.GetListMapObjects());
            player.MouseMoveHandler(gameCanvas);
            MovementHandler.initialize(player);
            List<Point> enemiesSpawner = new List<Point>() { new Point(4000, 1500), new Point(4000, 2500), new Point(2000, 1500), new Point(2000, 2500) };
            int[][] enemyLists = new int[5][];


            spawner = new Spawner(enemiesSpawner, 5, gameCanvas, player);
            enemyLists[0] = new int[] { 75, 25, 0, 0 };
            enemyLists[1] = new int[] { 60, 35, 15, 0 };
            enemyLists[2] = new int[] { 40, 35, 25, 0 };
            enemyLists[3] = new int[] { 25, 35, 25, 5 };
            enemyLists[4] = new int[] { 10, 35, 25, 20 };
            spawner.GenerateEnemies(enemyLists[0], 12, 0, 125);
            spawner.GenerateEnemies(enemyLists[1], 24, 1, 115);
            spawner.GenerateEnemies(enemyLists[2], 36, 2, 110);
            spawner.GenerateEnemies(enemyLists[3], 48, 3, 105);
            spawner.GenerateEnemies(enemyLists[4], 60, 4, 100);

            player.SetAllEnemies(spawner.GetAllEnemies());

        }

        private void EndOfGame()
        {
            if (player.healthPoints == 0)
            {
                DeathWindow deathWindow = new DeathWindow(hud);
                gameTimer.Stop();
                deathWindow.Show();
                this.Close();
            }
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
            }
            else if (e.Key == Key.A)
            {
                direction[1] = true;
            }
            else if (e.Key == Key.S)
            {
                direction[2] = true;
            }
            else if (e.Key == Key.D)
            {
                direction[3] = true;
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
