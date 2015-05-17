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
        private static WPFWindow _screenSaverWindow;

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
            _screenSaverWindow.MouseMove += new MouseEventHandler(MouseMove);
        }


        private static void KeyPress(object sender, KeyEventArgs e)
        {
            // handling all the key events 
            _screenSaverWindow.Hide();
            _screenSaverWindow.Visibility = System.Windows.Visibility.Hidden;
            _screenSaverWindow.isOpen = false;

        }

        private static void MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            //{
            //    _screenSaverWindow.Hide();
            //    _screenSaverWindow.Visibility = System.Windows.Visibility.Hidden;
            //    _screenSaverWindow.isOpen = false;
            //}
        }

        public static void ShowScreenSaver()
        {
            if (_screenSaverWindow.isOpen)
                return;

            if (Application.Current.MainWindow is MainWindow)
            {
                RappTestEntities rappRntity = new RappTestEntities();
                IEnumerable<Media> media = from m in rappRntity.Media
                                           where m.IsScreenSaver == true
                                           select new Media { FileName = m.FileName };
                Viewbox dynamicViewbox = new Viewbox();
                System.Windows.Controls.Grid myGrid = new Grid();

                if (media.Count() > 0)
                {
                    foreach (Media m in media)
                    {
                        MainWindow window = (MainWindow)Application.Current.MainWindow;
                        _screenSaverWindow.EnterFullScreen(m);
                        myGrid.Children.Add(m.GetUIElement());
                    }
                }
                dynamicViewbox.Child = myGrid;
            }
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
                dynamicViewbox = new Viewbox();
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
                dynamicViewbox.Child = myGrid;
            }
        }
    }
}


