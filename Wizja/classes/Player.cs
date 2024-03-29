﻿using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wizja.classes.guns;
using Wizja.Enemies;
namespace Wizja.classes
{
    public class Player
    {
        public int healthPoints;
        public double movingSpeed;
        public Rectangle playerImage;
        public Rectangle flashLightImage;
        private List<Rectangle> obstacles;
        private List<Enemy> allEnemies;
        private Weapon weapon;
        public static ImageSource flashLightSource = new BitmapImage(new Uri("pack://application:,,,/res/flashlight.png"));
        public HUD hud;
        public int gameTick =0;
        //odtwarzanie  strzału
        int isWalking = 0;
        public TransformedBitmap rotatedPlayer = new TransformedBitmap();
        public TransformedBitmap rotatedPlayer1 = new TransformedBitmap();
        public TransformedBitmap rotatedPlayer2 = new TransformedBitmap();


        public SoundPlayer shoot;

        public void setWeapon(Weapon w)
        {
            this.weapon = w;
        }
        public Weapon getWeapon()
        {
            return weapon;
        }
        public Player(Canvas gameCanvas, HUD hud, List<Rectangle> obstacles)
        {
            healthPoints = 100;
            movingSpeed = 4.5;
            this.hud = hud;
            this.obstacles = obstacles;
            this.allEnemies = allEnemies;

            // tutaj wczytuje obraz playera nasz nowy player to rotated player
           
            rotatedPlayer.BeginInit();
            rotatedPlayer.Source = new BitmapImage(new Uri("pack://application:,,,/res/Player.png"));
            rotatedPlayer.Transform = new RotateTransform(-90);
            rotatedPlayer.EndInit();
            rotatedPlayer1.BeginInit();
            rotatedPlayer1.Source = new BitmapImage(new Uri("pack://application:,,,/res/PlayerWalk1.png"));
            rotatedPlayer1.Transform = new RotateTransform(-90);
            rotatedPlayer1.EndInit();
            rotatedPlayer2.BeginInit();
            rotatedPlayer2.Source = new BitmapImage(new Uri("pack://application:,,,/res/PlayerWalk2.png"));
            rotatedPlayer2.Transform = new RotateTransform(-90);
            rotatedPlayer2.EndInit();
            playerImage = new Rectangle()
            {
                Width = 75,
                Height = 47,
                Fill = new ImageBrush(rotatedPlayer)
            };
            flashLightImage = new Rectangle()
            {
                Width = 3000,
                Height = 3000,
                Fill = new ImageBrush(flashLightSource),
                Opacity = 0.97
            };
            Panel.SetZIndex(flashLightImage, int.MaxValue);
            Canvas.SetLeft(playerImage, 3000);
            Canvas.SetTop(playerImage, 2000);
            Canvas.SetLeft(flashLightImage, 1523.5);
            Canvas.SetTop(flashLightImage, 537.5);
            gameCanvas.Children.Add(playerImage);
            gameCanvas.Children.Add(flashLightImage);
            playerImage.RenderTransformOrigin = new Point(0.5, 0.5);

            this.weapon = new StartGun(); // bron na start
        }
        public void TakeDamage(int damage)
        {
            healthPoints = hud.GetHp();
            healthPoints -= damage;
            if (healthPoints <= 0)
            {
                hud.SetHp(0);
            }
            else
            {
                hud.SetHp(healthPoints);
            }
        }
        Canvas gameCanvas;
        public void MouseMoveHandler(Canvas gameCanvas)
        {

            this.gameCanvas = gameCanvas;
            gameCanvas.MouseMove += MouseMove;
            gameCanvas.MouseLeftButtonDown += MouseLeftButtonDown;

        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(gameCanvas);
            Point playerPosition = new Point(Canvas.GetLeft(flashLightImage) + flashLightImage.Width / 2, Canvas.GetTop(flashLightImage) + flashLightImage.Height / 2);
            double angle = Math.Atan2(mousePosition.Y - playerPosition.Y, mousePosition.X - playerPosition.X) * (180 / Math.PI);
            RotateDarkness(angle);
            RotatePlayer(angle);
        }
        private void RotateDarkness(double angle)
        {
            flashLightImage.RenderTransformOrigin = new Point(0.5, 0.5);
            RotateTransform rotateTransform = new RotateTransform(angle);
            flashLightImage.RenderTransform = rotateTransform;
        }
        private void RotatePlayer(double angle)
        {
            playerImage.RenderTransformOrigin = new Point(0.5, 0.5);
            RotateTransform rotateTransform = new RotateTransform(angle);
            TransformGroup transformGroup = new TransformGroup();

            //player.RenderTransform = rotateTransform;
            if (Math.Abs(angle) > 90)
            {
                ScaleTransform reflectTransform = new ScaleTransform(1, -1);
                transformGroup.Children.Add(reflectTransform);
                transformGroup.Children.Add(rotateTransform);
            }
            else
                transformGroup.Children.Add(rotateTransform);
            playerImage.RenderTransform = transformGroup;

        }

        public void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (gameTick > weapon.coolDown)
            {
                gameTick = 0;
                Point endPoint;
                Point playerPos = new Point(Canvas.GetLeft(playerImage), Canvas.GetTop(playerImage));

                playerPos = new Point(Canvas.GetLeft(playerImage) + playerImage.Width / 2, Canvas.GetTop(playerImage) + playerImage.Height / 2);

                // pojebana matma z chatu do endpointu strzalu
                double flashlightAngle = ((RotateTransform)flashLightImage.RenderTransform).Angle;
                Vector direction = new Vector(Math.Cos(flashlightAngle * Math.PI / 180), Math.Sin(flashlightAngle * Math.PI / 180));
                // double distance = 1000;
                // endPoint = new Point(playerPos.X + direction.X * distance, playerPos.Y + direction.Y * distance);
                ChoiceSound();
                shoot.Play();
                weapon.Shoot(playerPos, direction, obstacles, allEnemies, gameCanvas);
            }
        }
        public void ChoiceSound()
        {
            if (weapon.name == "Family Gun")
            {
                shoot = new SoundPlayer("sound/eeee.wav");
            }
            else if (weapon.name == "Pistol")
            {
                shoot = new SoundPlayer("sound/shoot.wav");
            }
            else if (weapon.name == "SMG")
            {
                shoot = new SoundPlayer("sound/smg.wav");

            }
            else if (weapon.name == "M4A1")
            {
                shoot = new SoundPlayer("sound/rfileShot.wav");

            }
            else if (weapon.name == "Shotgun")
            {
                shoot = new SoundPlayer("sound/ShotgunShot.wav");
            }
            else 
            {
                shoot = new SoundPlayer("sound/stoneDrop.wav");
            }
        }

        public void SetAllEnemies(List<Enemy> allEnemies)
        {
            this.allEnemies = allEnemies;
        }
    }
}
