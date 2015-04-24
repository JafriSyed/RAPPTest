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
using System.Reflection;

namespace RAPPTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Member Variables

        private Label _openFolder = null;
        private Button _selectedButton = null;

        #endregion


        #region Static Variables

        //private Manager manager;
        [DllImport("User32.dll")]
        public static extern bool LockWorkStation();
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);
        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        #endregion


        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            LoadScreenSaverControl();
            CreateUpperGridButtons();
            CreateLowerGridButtons();
            expBrowser.NavigationTarget = ShellFileSystemFolder.FromFolderPath("C:\\");
        }

        #endregion

       
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


        private void GetMediaFolderID(int folderNum, string folderName)
        {
            IEnumerable<Folder> folder = MediaView.GetFolderId(folderNum, folderName);
            foreach (var f in folder)
            {
                lblMediaFolderId.Content = f.MediaFolderId;            
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
            GetMediaFolderID(Convert.ToInt32(_openFolder.Content.ToString()), _selectedButton.Content.ToString());
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

        private void textBorder_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void ImagebucketTab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MediaView mv = new MediaView();
                ObservableCollection<Media> mediaObj = mv.GetAllImages();
                mediaListBox.ItemsSource = mediaObj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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
                GetMediaFolderID(Convert.ToInt32(_openFolder.Content.ToString()), _selectedButton.Content.ToString());
                MediaImportControl ic = new MediaImportControl();
                ic.BindImages((Guid)lblMediaFolderId.Content);
            }
        }

        private void theSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
        }

        private void OnRecycleDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data is DataObject)
            {
                List<object> droppedItem = (List<object>)e.Data.GetData(e.Data.GetFormats()[0]);
                Media m = (Media)droppedItem[0];
                Guid mediaId = (Guid)m.MediaId;
                MediaView mv = new MediaView();
                mv.DeleteImage(mediaId);
                mv.GetAllMediaData((Guid)lblMediaFolderId.Content);
            }   
        }

        private void iconZoomOut_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

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
                GetMediaFolderID(Convert.ToInt32(_openFolder.Content.ToString()), _selectedButton.Content.ToString());
                MediaImportControl ic = new MediaImportControl();
                ic.BindImages((Guid)lblMediaFolderId.Content);
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
            //MessageBox.Show("Upper texbox");
        }

        private void middleTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Middle texbox");
        }

        private void middleTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            if (window.tabControl.SelectedIndex == 3)
            {
                if (this.lowerScrollView.Visibility == System.Windows.Visibility.Hidden)
                    this.lowerScrollView.Visibility = System.Windows.Visibility.Visible;
                else
                    this.lowerScrollView.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                if (this.lowerScrollViewOrganizer.Visibility == System.Windows.Visibility.Hidden)
                    this.lowerScrollViewOrganizer.Visibility = System.Windows.Visibility.Visible;
                else
                    this.lowerScrollViewOrganizer.Visibility = System.Windows.Visibility.Hidden;
            }
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

        private void btnSaveOrganizer_Click(object sender, RoutedEventArgs e)
        {
            Guid mediaId = (Guid)lblScriptMediaId.Content;
            MediaView mv = new MediaView();
            Medium m = new Medium();
            m.Description = txtDescriptionOrganizer.Text;
            m.Title = txtTitleOrganizer.Text;
            m.Notes = txtNotesOrganizer.Text;
            mv.UpdateImageData(mediaId, m);
        }

        private void btnSaveMedia_Click(object sender, RoutedEventArgs e)
        {
            Guid mediaId = (Guid)lblScriptMediaId.Content;
            MediaView mv = new MediaView();
            Medium m = new Medium();
            m.Title = txtTitle.Text;
            m.Description = txtDescription.Text;
            m.Notes = txtNotes.Text;
            mv.UpdateImageData(mediaId, m);
        }

        private void fullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            WPFWindow w = new WPFWindow();
            w.Show();
        }

        #endregion


        #region Struct
       
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        #endregion

       
    }
}