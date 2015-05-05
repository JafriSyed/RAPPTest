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
using System.Windows.Controls.Primitives;

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

        //private Manager ;
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
            // initializing controls, loading all the buttons on the UI and populating screen saver control

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
       
        /// <summary>
        /// Gets system idle time to show screen saver
        /// </summary>
        /// <returns></returns>
        private static uint GetIdleTime()
        {
            // this function check the idle time of the application

            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            GetLastInputInfo(ref LastUserAction);
            return ((uint)Environment.TickCount - LastUserAction.dwTime);
        }

        /// <summary>
        /// Gets ticks count
        /// </summary>
        /// <returns></returns>
        private static long GetTickCount()
        {
            return Environment.TickCount;
        }

        /// <summary>
        /// Gets time when last input was done.
        /// </summary>
        /// <returns></returns>
        private static long GetLastInputTime()
        {

            //this function gets the last input time

            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(LastUserAction);
            if (!GetLastInputInfo(ref LastUserAction))
            {
                throw new Exception(GetLastError().ToString());
            }

            return LastUserAction.dwTime;
        }

        /// <summary>
        /// Starting time control
        /// </summary>
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
                ColumnDefinition colDefn2 = new ColumnDefinition();
                gridButtonUpper.ColumnDefinitions.Add(colDefn);
                gridButtonUpperImport.ColumnDefinitions.Add(colDefn2);
            }
            for (int i = 0; i < 13; i++)
            {
                Button btn = new Button();
                Button btn2 = new Button();
                if (i == 0)
                    _selectedButton = btn;
                btn.Name = startLabel.ToString();
                btn.Content = "" + startLabel;
                btn.FontSize = 24;
                btn.FontFamily = new FontFamily("Arial");
                btn.Click += new RoutedEventHandler(btn_Click);
                btn.AllowDrop = true;
                btn.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                    btn.Margin = new Thickness(2, 0, 0, 0);

                btn2.Name = startLabel.ToString();
                btn2.Content = "" + startLabel;
                btn2.FontSize = 24;
                btn2.FontFamily = new FontFamily("Arial");
                btn2.Click += new RoutedEventHandler(btn_Click);
                btn2.AllowDrop = true;
                btn2.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                    btn2.Margin = new Thickness(2, 0, 0, 0);
                gridButtonUpper.Children.Add(btn);
                gridButtonUpperImport.Children.Add(btn2);
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
                gridButtonLower.ColumnDefinitions.Add(colDefn);
                ColumnDefinition colDefn2 = new ColumnDefinition();
                gridButtonLowerImport.ColumnDefinitions.Add(colDefn2);
            }
            for (int i = 0; i < 13; i++)
            {
                Button btn = new Button();
                Button btn2 = new Button();
                btn.Content = startLabel;
                btn.FontSize = 24;
                btn.FontFamily = new FontFamily("Arial");
                btn.Click += new RoutedEventHandler(btn_Click);
                btn.AllowDrop = true;
                btn.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                {
                    btn.Margin = new Thickness(2, 0, 0, 0);
                }
                btn2.Content = startLabel;
                btn2.FontSize = 24;
                btn2.FontFamily = new FontFamily("Arial");
                btn2.Click += new RoutedEventHandler(btn_Click);
                btn2.AllowDrop = true;
                btn2.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                {
                    btn2.Margin = new Thickness(2, 0, 0, 0);
                }
                gridButtonLower.Children.Add(btn);
                gridButtonLowerImport.Children.Add(btn2);
                startLabel++;
            }
        }

        /// <summary>
        /// Gets Media Folder Id by folder number and name and setting its value on the page in a hidden control
        /// </summary>
        /// <param name="folderNum"></param>
        /// <param name="folderName"></param>
        private void GetMediaFolderID(int folderNum, string folderName)
        {
            //getting media folder id to bind it to the lblMediaFolderId, this lblMediaFolderId will be user to fetch media files from the database
            IEnumerable<Folder> folder = MediaView.GetFolderId(folderNum, folderName);
            foreach (var f in folder)
            {
                lblMediaFolderId.Content = f.MediaFolderId;
                if (this.tabControl.SelectedIndex == 1)
                {
                    if (!string.IsNullOrEmpty(f.Title))
                        txtImportHeader.Text = f.Title;
                    else
                        txtImportHeader.Text = "Import images and videos";
                }  
                else
                {
                    if (!string.IsNullOrEmpty(f.Title))
                        txtOrganizerHeader.Text = f.Title;
                    else
                        txtOrganizerHeader.Text = "Organize your media collection";
                }
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
            {
                ShowScreenSaver();
            }
        }

        private void ShowScreenSaver()
        {
            PlayView playView = new PlayView();
            RappTestEntities rappRntity = new RappTestEntities();
            IEnumerable<Media> media = from m in rappRntity.Media
                                       where m.IsScreenSaver == true
                                       select new Media { FileName = m.FileName };

            if (media.Count() > 0)
            { 
                foreach(Media m in media)
                    playView.EnterFullScreen(m);
            }
            
            //MessageBox.Show("Screen Saver...");
        }

        private void ImagebucketTab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //binding all media files on the image bucket tab UI.
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
                //setting active folder on the bottom of the UI
                activeBtn.Content = btn.Content;
                activeBtnImport.Content = btn.Content;

                //styling buttons
                btn.Background = Brushes.Purple;
                btn.Foreground = Brushes.Yellow;
                _selectedButton.ClearValue(Button.BackgroundProperty);
                _selectedButton.ClearValue(Button.ForegroundProperty);
                _selectedButton = btn;

                //binding images after fetching them from the database by providing media folder id
                GetMediaFolderID(Convert.ToInt32(_openFolder.Content.ToString()), _selectedButton.Content.ToString());
                MediaImportControl ic = new MediaImportControl();
                ic.BindImages((Guid)lblMediaFolderId.Content);
            }
        }
        
        /// <summary>
        /// Deleting the image on dropping the image to recylcle folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecycleDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data is DataObject)
            {
                //deleting the image from the database
                List<object> droppedItem = (List<object>)e.Data.GetData(e.Data.GetFormats()[0]);
                Media m = (Media)droppedItem[0];
                Guid mediaId = (Guid)m.MediaId;
                MediaView mv = new MediaView();
                mv.DeleteImage(mediaId);

                //binding UI again
                mv.GetAllMediaData((Guid)lblMediaFolderId.Content);
            }   
        }

        private void iconZoomOut_MouseUp(object sender, RoutedEventArgs e)
        {
            //chaning size of each image by looping through the list
            for (int i = 0; i < dndPanelImport.lstImageGallery.Items.Count; i++)
            {
                ListBoxItem lbi = (ListBoxItem)dndPanelImport.lstImageGallery.ItemContainerGenerator.ContainerFromItem(dndPanelImport.lstImageGallery.Items[i]);
                lbi.Width = 100;
            }
        }

        private void iconZoomIn_MouseUp(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dndPanelImport.lstImageGallery.Items.Count; i++)
            {  
                //chaning size of each image by looping through the list
                ListBoxItem lbi = (ListBoxItem)dndPanelImport.lstImageGallery.ItemContainerGenerator.ContainerFromItem(dndPanelImport.lstImageGallery.Items[i]);
                lbi.Width = 120;
                lbi.Height = 120;
            }
        }

        private void iconZoomMax_MouseUp(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < dndPanelImport.lstImageGallery.Items.Count; i++)
            {
                //chaning size of each image by looping through the list
                ListBoxItem lbi = (ListBoxItem)dndPanelImport.lstImageGallery.ItemContainerGenerator.ContainerFromItem(dndPanelImport.lstImageGallery.Items[i]);
                lbi.Width = 130;
                lbi.Height = 130;
            }
        }

        private void play_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //calling the play view UI when the play tab is selected from the menu
            
            PlayView playView = new PlayView();
            playView.EnterFullScreen();
            this.tabControl.SelectedIndex = 2;
            return;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //updating sequence of the images
            List<Medium> lstMedium = new List<Medium>();
            MediaView mv = new MediaView();

            Guid mediaFolderId = (Guid)lblMediaFolderId.Content;
            int sequence = 0;
            MediaImportControl mic = dndPanelImport;
            foreach (Media item in mic.lstImageGallery.Items)
            {
                sequence++;
                Medium mItem = new Medium();
                mItem.MediaId = item.MediaId;
                mItem.Sequence = sequence;
                mItem.FileName = item.FileName;
                lstMedium.Add(mItem);
            }
            mv.UpdateSequence(lstMedium);

            //binding list again.
            mic.BindImages(mediaFolderId);

            string title = mv.UpdateMediaFolderTitle(mediaFolderId, txtImportHeader.Text);
            txtImportHeader.Text = title;

        }
         
        /// <summary>
        /// showing images against respective folder when the user clicks on the folder icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void folder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //styling folder buttons when clicked
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

                //binding list again as now the folder has changed
                GetMediaFolderID(Convert.ToInt32(_openFolder.Content.ToString()), _selectedButton.Content.ToString());
                MediaImportControl ic = new MediaImportControl();
                ic.BindImages((Guid)lblMediaFolderId.Content);
            }
        }

        private void middleTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // in this event we are basically showing and hiding the note field 
            
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

        private void btnSaveOrganizer_Click(object sender, RoutedEventArgs e)
        {
            //saving description against the images and video files
            
            MediaView mv = new MediaView();
            Medium m = new Medium();
            List<Medium> lstMedium = new List<Medium>();

            Guid mediaFolderId = (Guid)lblMediaFolderId.Content;
            int sequence = 0;
            MediaImportControl mic = dndPanelImport;

            if (lblScriptMediaId.Content != null)
            {
                Guid mediaId = (Guid)lblScriptMediaId.Content;
               
                m.Description = txtDescriptionOrganizer.Text;
                m.Title = txtTitleOrganizer.Text;
                m.Notes = txtNotesOrganizer.Text;
                mv.UpdateImageData(mediaId, m);
            }
          
            foreach (Media item in mic.lstImageGallery.Items)
            {
                sequence++;
                Medium mItem = new Medium();
                mItem.MediaId = item.MediaId;
                mItem.Sequence = sequence;
                mItem.FileName = item.FileName;
                lstMedium.Add(mItem);
            }
            mv.UpdateSequence(lstMedium);
        }

        private void btnSaveMedia_Click(object sender, RoutedEventArgs e)
        {
            if (lblScriptMediaId.Content != null)
            {
                Guid mediaId = (Guid)lblScriptMediaId.Content;
                MediaView mv = new MediaView();
                Medium m = new Medium();
                m.Title = txtTitle.Text;
                m.Description = txtDescription.Text;
                m.Notes = txtNotes.Text;
                mv.UpdateImageData(mediaId, m);
            }
        }


        #endregion

        public TabItem PlayTabItem
        {
            get { return play; }
        }

        public Viewbox PlayViewbox
        {
            get { return playViewbox; }
        }



        #region Struct
       
        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        #endregion


    }
}