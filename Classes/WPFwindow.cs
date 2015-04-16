using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace RAPPTest
{
    public class WPFWindow :  Window
    {   
        private bool _inStateChange;

            public WPFWindow()
            {
                SolidColorBrush brush = new SolidColorBrush(Colors.Black);

                Canvas canvas = new Canvas();
                canvas.Width = this.Width;
                canvas.Height = this.Height;
                canvas.Background = brush;

                this.AllowsTransparency = true;
                this.WindowStyle = WindowStyle.None;
                this.Background = brush;
                this.Topmost = true;

                this.Width = 400;
                this.Height = 300;
                this.Content = canvas;
            }

            private void Window_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == System.Windows.Input.Key.F11)
                {
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    ResizeMode = ResizeMode.NoResize;
                }
            }

            protected override void OnStateChanged(EventArgs e)
            {
                if (WindowState == WindowState.Maximized && !_inStateChange)
                {
                    _inStateChange = true;
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;
                    WindowState = WindowState.Maximized;
                    ResizeMode = ResizeMode.NoResize;
                    _inStateChange = false;
                }
                base.OnStateChanged(e);
            }
    }
}
