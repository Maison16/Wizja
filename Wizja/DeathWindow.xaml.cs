﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wizja.classes;

namespace Wizja
{
    /// <summary>
    /// Logika interakcji dla klasy DeathWindow.xaml
    /// </summary>
    public partial class DeathWindow : Window
    {
        HUD hud;
        public DeathWindow(HUD hud)
        {
            InitializeComponent();
            this.hud = hud;
            var deathBackground = new ImageBrush();
            deathBackground.ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/menu/menuBackground.png"));
            deathCanvas.Background = deathBackground;
            
            Label score = new Label();
            score.Content = "Score: " + hud.totalPoints.ToString();
            score.FontSize = 82;
            score.FontWeight = FontWeights.Bold;
            score.Foreground = Brushes.White;
            Canvas.SetLeft(score, 800);
            Canvas.SetTop(score, 450);
            deathCanvas.Children.Add(score);

            var logo = new ImageBrush();
            logo.ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/menu/defeat.png"));

            Rectangle recLogo = new Rectangle
            {
                Width = 1300,
                Height = 500,
                Fill = logo
            };
            Canvas.SetLeft(recLogo, 310);
            Canvas.SetTop(recLogo, 50);
            deathCanvas.Children.Add(recLogo);
            SoundPlayer deathSound = new SoundPlayer("sound/death.wav");
            deathSound.Play();

        }
        private void closeWindow(object sender, RoutedEventArgs e)
        {
            
            MainWindow backToMenu = new MainWindow();
            backToMenu.Show();
            this.Close();
        }
    }
}
