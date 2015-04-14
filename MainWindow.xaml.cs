using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation;
using Microsoft.WindowsAPICodePack.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RAPPTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _nextMediaID = 0;
        Brush _txtInputBorderBrush = null;
        private Label _openFolder = null;
        private Button _selectedButton = null;

        private MediaItemControl _scriptMediaItemUC;
        
        private List<Label> _folderLabels;
        private List<Button> _keyButtons;
        private List<Button> _keyButtonsImport;

        //private Manager manager;
        [DllImport("User32.dll")]
        public static extern bool LockWorkStation();
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);
        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public MainWindow()
        {
            InitializeComponent();
            LoadScreenSaverControl();
            CreateUpperGridButtons();
            CreateLowerGridButtons();
            expBrowser.NavigationTarget = ShellFileSystemFolder.FromFolderPath("C:\\");
        }


        #region Private Methods

        private void LoadScreenSaverControl()
        {
            this.screenSaverTimer.ItemsSource = Utilities.Utilities.GetTimerList();
            this.screenSaverTimer.SelectedIndex = 2;
        }

        private static uint GetIdleTime()
        {
            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            GetLastInputInfo(ref LastUserAction);
            return ((uint)Environment.TickCount - LastUserAction.dwTime);
        }

        private static long GetTickCount()
        {
            return Environment.TickCount;
        }

        private static long GetLastInputTime()
        {
            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            if (!GetLastInputInfo(ref LastUserAction))
            {
                throw new Exception(GetLastError().ToString());
            }

            return LastUserAction.dwTime;
        }


        private void StartTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Create and display the buttons in first row at the bottom
        /// </summary>
        private void CreateUpperGridButtons()
        {
            char startLabel = 'A';
            for (int i = 0; i < 13; i++)
            {
                ColumnDefinition colDefn = new ColumnDefinition();
                gridButtonUpperImport.ColumnDefinitions.Add(colDefn);
            }
            for (int i = 0; i < 13; i++)
            {
                Button btn = new Button();
                if (i == 0)
                    _selectedButton = btn;
                btn.Name = startLabel.ToString();
                btn.Content = "" + startLabel;
                btn.FontSize = 24;
                btn.FontFamily = new FontFamily("Arial");
                btn.Click += new RoutedEventHandler(btn_Click);
                btn.AllowDrop = true;
                btn.Drop += new DragEventHandler(key_Drop);
                btn.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                    btn.Margin = new Thickness(2, 0, 0, 0);

                gridButtonUpperImport.Children.Add(btn);
                startLabel++;
            }
        }

        /// <summary>
        /// Create and display the buttons in second row at the bottom
        /// </summary>
        private void CreateLowerGridButtons()
        {
            char startLabel = 'N';
            for (int i = 0; i < 13; i++)
            {
                ColumnDefinition colDefn = new ColumnDefinition();
                gridButtonLowerImport.ColumnDefinitions.Add(colDefn);
            }
            for (int i = 0; i < 13; i++)
            {
                Button btn = new Button();
                btn.Content = startLabel;
                btn.FontSize = 24;
                btn.FontFamily = new FontFamily("Arial");
                btn.Click += new RoutedEventHandler(btn_Click);
                btn.AllowDrop = true;
                btn.Drop += new DragEventHandler(key_Drop);
                btn.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                {
                    btn.Margin = new Thickness(2, 0, 0, 0);
                }

                gridButtonLowerImport.Children.Add(btn);
                startLabel++;
            }

        }


        #endregion


        #region Public Methods

        public void EnterFullScreen()
        {
            WPFWindow w = new WPFWindow();
            w.Show();
        }


        #endregion


        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartTimer();
            _openFolder = this.folder1;
        }


        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (GetIdleTime() > Convert.ToInt32(screenSaverTimer.SelectedValue) * 1000)  //10 secs, Time to wait before locking
                MessageBox.Show("Show Screen Saver....");
        }


        protected override void OnClosed(EventArgs e)
        {

        }

        private void textBorder_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Hide border of textbox when mouse over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBorder_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void ImagebucketTab_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Handler for click event of all buttons on the bottom panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                activeBtn.Content = btn.Content;
                activeBtnImport.Content = btn.Content;
                btn.Background = Brushes.Purple;
                btn.Foreground = Brushes.Yellow;
                _selectedButton.ClearValue(Button.BackgroundProperty);
                _selectedButton.ClearValue(Button.ForegroundProperty);
                _selectedButton = btn;
            }
        }

        private void theSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }


        private void iconZoomOut_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Handler for click event of slider zoomIn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void iconZoomIn_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void folder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //dndPanel.UpdateItems();
            Label clickedFolder = sender as Label;
            if (clickedFolder != null)
            {
                if (clickedFolder != _openFolder)
                {

                    clickedFolder.Background = this.FindResource("openFolderIcon") as ImageBrush;
                    clickedFolder.Foreground = Brushes.Yellow;
                    _openFolder.Background = this.FindResource("closedFolderIcon") as ImageBrush;
                    _openFolder.Foreground = this.FindResource("Purple") as SolidColorBrush;
                    _openFolder = clickedFolder;
                }
            }
        }


        private void folder_Drop(object sender, DragEventArgs e)
        {

        }

        private void key_Drop(object sender, DragEventArgs e)
        {

        }

        private void upperTxtBox_Drop(object sender, DragEventArgs e)
        {

        }


        private void middleTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void middleTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void expBrowser_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void expBrowser_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void iconZoomOutImport_MouseUp(object sender, MouseButtonEventArgs e)
        {


        }

        private void theSliderImport_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void iconZoomInImport_MouseUp(object sender, MouseButtonEventArgs e)
        {


        }

        private void playViewbox_MouseEnter(object sender, MouseEventArgs e)
        {


        }

        private void organizer_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }


        private void Window_KeyUp(object sender, KeyEventArgs e)
        {


        }


        // all of the code below are not in use! //by Siwei
        private void fullScreenButton_Click(object sender, RoutedEventArgs e)
        {

            // Create new window, that window will have a view box, and its content will be whatever the selected item is, just read in the actual image

            WPFWindow w = new WPFWindow();
            w.Show();

        }

        #endregion


        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public class WPFWindow : Window
        {
            private bool _inStateChange;

            public WPFWindow()
            {
                //SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
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
}