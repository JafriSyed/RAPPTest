using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace RAPPTest
{
    public static class ScreenSaver
    {
        public static WPFWindow _screenSaverWindow;
        static Viewbox _dynamicViewbox = new Viewbox();

        public static bool IsOpen()
        {
            bool isOpen = false;
            if (_screenSaverWindow == null)
                isOpen = false;
            else
                isOpen = _screenSaverWindow.isOpen;
            return isOpen;
        }
  
        public static void InitializeActions()
        {
            _screenSaverWindow = new WPFWindow();
            _screenSaverWindow.KeyUp += new KeyEventHandler(KeyPress);
            _screenSaverWindow.MouseLeftButtonDown += new MouseButtonEventHandler(_screenSaverWindow_MouseLeftButtonDown);
            _screenSaverWindow.MouseRightButtonDown += new MouseButtonEventHandler(_screenSaverWindow_MouseLeftButtonDown);
        }


        private static void KeyPress(object sender, KeyEventArgs e)
        {
            // handling all the key events 
            StopVideo();
            _screenSaverWindow.Hide();
            _screenSaverWindow.Visibility = System.Windows.Visibility.Hidden;
            _screenSaverWindow.isOpen = false;
            
        }

        private static void StopVideo()
        {
            System.Windows.Controls.Grid myGrid = (Grid)_dynamicViewbox.Tag;
            if (myGrid.Children.Count > 0)
            {
                if (myGrid.Children[0] is MediaElement)
                {
                    MediaElement me = (MediaElement)myGrid.Children[0];
                    me.Stop();
                }
            }
        }

        private static void PlayVideo()
        {
            System.Windows.Controls.Grid myGrid = (Grid)_dynamicViewbox.Tag;
            if (myGrid.Children.Count > 0)
            {
                if (myGrid.Children[0] is MediaElement)
                {
                    MediaElement me = (MediaElement)myGrid.Children[0];
                    me.Position = TimeSpan.Zero;
                    me.LoadedBehavior = MediaState.Play;
                    me.Play();
                }
            }
        }

        private static void _screenSaverWindow_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            _screenSaverWindow.Hide();
            _screenSaverWindow.Visibility = System.Windows.Visibility.Hidden;
            _screenSaverWindow.isOpen = false;
        }

        public static void ShowScreenSaver()
        {
            if (_screenSaverWindow.isOpen)
                return;


            if (Application.Current.MainWindow is MainWindow)
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                if (window.WindowState == System.Windows.WindowState.Minimized)
                {
                    window.WindowState = System.Windows.WindowState.Maximized;
                }
            }


            if (Application.Current.MainWindow is MainWindow)
            {
                RappTestEntities rappRntity = new RappTestEntities();
                IEnumerable<Media> media = from m in rappRntity.Media
                                           where m.IsScreenSaver == true
                                           select new Media { FileName = m.FileName };
               
                System.Windows.Controls.Grid myGrid = new Grid();

                if (media.Count() > 0)
                {
                    foreach (Media m in media)
                    {
                        MainWindow mainwindow = (MainWindow)Application.Current.MainWindow;
                        _screenSaverWindow.EnterFullScreen(m);
                    }
                }
               
            }
        }

        ////loop to keep video playing continuously
        private static void me_MediaEnded(object sender, EventArgs e)
        {
            PlayVideo();
        }    

        private static void EnterFullScreen(Media e)
        {
            _screenSaverWindow.EnterFullScreen(e);
            _screenSaverWindow.isOpen = true;
        }

        public class WPFWindow : Window
        {
            public bool isOpen;
            public Viewbox dynamicViewbox;

            public WPFWindow()
            {
                //SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                SolidColorBrush brush = new SolidColorBrush(Colors.Black);

                //this.AllowsTransparency = true;
                //this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.Background = brush;
                this.Topmost = false;
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                dynamicViewbox = _dynamicViewbox;
                dynamicViewbox.StretchDirection = StretchDirection.Both;
                dynamicViewbox.Stretch = Stretch.Uniform;
               
                this.Content = dynamicViewbox;
            }


            public void EnterFullScreen(Media media)
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                this.Show();
                isOpen = true;
                if (media != null)
                {
                    this.SetViewboxContent(media);
                }
            }


            public void SetViewboxContent(Media media)
            {
                System.Windows.Controls.Grid myGrid = new Grid();
                if (this.isOpen)
                {
                    myGrid.Children.Add(media.GetUIElement());
                }
                dynamicViewbox.Tag = myGrid;
                dynamicViewbox.Child = myGrid;
                if (myGrid != null && myGrid.Children.Count > 0)
                {
                    if (myGrid.Children[0] is MediaElement)
                    {
                        MediaElement me = (MediaElement)myGrid.Children[0];
                        me.MediaEnded += new RoutedEventHandler(me_MediaEnded);
                    }
                }
            }
        }
    }
}


