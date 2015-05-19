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
        private TabItem _lastSelectedTabItem = null;
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
            _lastSelectedTabItem = (TabItem)this.tabControl.SelectedItem;
        }

        #endregion


        #region Private Methods

        private void LoadScreenSaverControl()
        {
            this.screenSaverTimer.ItemsSource = Utilities.Utilities.GetTimerList();
            this.screenSaverTimer.SelectedIndex = 0;
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
                ColumnDefinition colGrdUpper = new ColumnDefinition();
                ColumnDefinition colGrdLower = new ColumnDefinition();
                gridButtonUpper.ColumnDefinitions.Add(colGrdUpper);
                gridButtonUpperImport.ColumnDefinitions.Add(colGrdLower);
            }
            for (int i = 0; i < 13; i++)
            {
                Button btnGrdUpper = new Button();
                Button btnGrdLower = new Button();
                if (i == 0)
                    _selectedButton = btnGrdUpper;
                btnGrdUpper.Name = startLabel.ToString();
                btnGrdUpper.Content = "" + startLabel;
                btnGrdUpper.FontSize = 24;
                btnGrdUpper.FontFamily = new FontFamily("Arial");
                btnGrdUpper.Click += new RoutedEventHandler(btn_Click);
                btnGrdUpper.AllowDrop = true;
                btnGrdUpper.Drop += new DragEventHandler(OnFolderButtonDrop);
                btnGrdUpper.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                    btnGrdUpper.Margin = new Thickness(2, 0, 0, 0);

                btnGrdLower.Name = startLabel.ToString();
                btnGrdLower.Content = "" + startLabel;
                btnGrdLower.FontSize = 24;
                btnGrdLower.FontFamily = new FontFamily("Arial");
                btnGrdLower.Click += new RoutedEventHandler(btn_Click);
                btnGrdLower.AllowDrop = true;
                btnGrdLower.Drop += new DragEventHandler(OnFolderButtonDrop);
                btnGrdLower.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                    btnGrdLower.Margin = new Thickness(2, 0, 0, 0);
                gridButtonUpper.Children.Add(btnGrdUpper);
                gridButtonUpperImport.Children.Add(btnGrdLower);
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
                ColumnDefinition colGrdUpper = new ColumnDefinition();
                ColumnDefinition colGrdLower = new ColumnDefinition();
                gridButtonLower.ColumnDefinitions.Add(colGrdUpper);
                gridButtonLowerImport.ColumnDefinitions.Add(colGrdLower);
            }
            for (int i = 0; i < 13; i++)
            {
                Button btnGrdUpper = new Button();
                Button btnGrdLower = new Button();
                btnGrdUpper.Content = startLabel;
                btnGrdUpper.FontSize = 24;
                btnGrdUpper.FontFamily = new FontFamily("Arial");
                btnGrdUpper.Click += new RoutedEventHandler(btn_Click);
                btnGrdUpper.AllowDrop = true;
                btnGrdUpper.Drop += new DragEventHandler(OnFolderButtonDrop);
                btnGrdUpper.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                {
                    btnGrdUpper.Margin = new Thickness(2, 0, 0, 0);
                }
                btnGrdLower.Content = startLabel;
                btnGrdLower.FontSize = 24;
                btnGrdLower.FontFamily = new FontFamily("Arial");
                btnGrdLower.Click += new RoutedEventHandler(btn_Click);
                btnGrdLower.AllowDrop = true;
                btnGrdLower.Drop += new DragEventHandler(OnFolderButtonDrop);
                btnGrdLower.SetValue(Grid.ColumnProperty, i);
                if (i > 0)
                {
                    btnGrdLower.Margin = new Thickness(2, 0, 0, 0);
                }
                gridButtonLower.Children.Add(btnGrdUpper);
                gridButtonLowerImport.Children.Add(btnGrdLower);
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

                if (!string.IsNullOrEmpty(f.Title))
                {
                    txtImportHeader.Text = f.Title;
                    txtOrganizerHeader.Text = f.Title;
                    txtScriptHeader.Text = f.Title;
                }
                else
                {
                    txtImportHeader.Text = "Import images and videos";
                    txtOrganizerHeader.Text = "Organize your media collection";
                    txtScriptHeader.Text = "View and edit media data";
                }
            }
        }

        private void ShowScreenSaver()
        {
            if (!ScreenSaver.IsOpen())
            {
                ScreenSaver.InitializeActions();
                ScreenSaver.ShowScreenSaver();
            }
        }


        private void SaveMedia()
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
            Guid mediaFolderId = (Guid)lblMediaFolderId.Content;
            UpdateMediaFolderTitle(mediaFolderId, txtScriptHeader.Text);
        }

        //private void UpdateFolderTitle(string )

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
            if (GetIdleTime() > /*Convert.ToInt32(screenSaverTimer.SelectedValue)*/ 5 * 1000)  //10 secs, Time to wait before locking
            {
                ShowScreenSaver();
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_lastSelectedTabItem != null && lblMediaFolderId.Content != null)
            {
                SaveDetails();
            }

            _lastSelectedTabItem = this.tabControl.SelectedItem as TabItem;
        }

        private void ImagebucketTab_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //binding all media files on the image bucket tab UI.
            try
            {
                MediaView mv = new MediaView();
                ObservableCollection<Media> mediaObj = mv.GetAllImages();
                mediaListBox.ItemsSource = mediaObj;
                mediaListBox.Tag = mv.GetImageBucketItemCount(mediaObj);
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
                SaveDetails();

                //setting active folder on the bottom of the UI
                activeBtn.Content = btn.Content;
                activeBtnImport.Content = btn.Content;
                activeBtnScript.Content = btn.Content;

                if (activeBtn.Content != _selectedButton.Content)
                {
                    //styling buttons
                    btn.Background = Brushes.Purple;
                    btn.Foreground = Brushes.Yellow;
                    _selectedButton.ClearValue(Button.BackgroundProperty);
                    _selectedButton.ClearValue(Button.ForegroundProperty);
                    _selectedButton = btn;
                }

                _selectedButton.Background = Brushes.Purple;
                _selectedButton.Foreground = Brushes.Yellow;
                Guid mediaFolderId = (Guid)lblMediaFolderId.Content;
                //binding images after fetching them from the database by providing media folder id
                GetMediaFolderID(Convert.ToInt32(_openFolder.Content.ToString()), _selectedButton.Content.ToString());
                MediaImportControl ic = new MediaImportControl();
                ic.BindImages((Guid)lblMediaFolderId.Content);

                if (this.tabControl.SelectedIndex == 3)
                    this.tabControl.SelectedIndex = 2;


                if (mediaFolderId != (Guid)lblMediaFolderId.Content)
                {
                    lblScriptMediaId.Content = null;
                    lblOrganizerMediaId.Content = null;
                    EmptyFields();
                }
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
                //checking if it's a image on the grid and has not been dropped directly from windows explorer
                if (e.Data.GetData(e.Data.GetFormats()[0]) is List<object>)
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
        }


        /// <summary>
        /// Adding/Moving the image on dropping the folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFolderDrop(object sender, System.Windows.DragEventArgs e)
        {
            bool isSameFolder = false;
            int folderNum = Convert.ToInt32(((Label)sender).Content.ToString());
            if (folderNum.ToString() == _openFolder.Content.ToString())
                isSameFolder = true;

            Guid mediaFolderId = new Guid();
            IEnumerable<Folder> folder = MediaView.GetFolderId(folderNum, _selectedButton.Content.ToString());
            foreach (var f in folder)
            {
                mediaFolderId = f.MediaFolderId;
            }
            if (e.Data is DataObject)
            {
                if (e.Data.GetData(e.Data.GetFormats()[0]) is List<object>)
                {
                    List<object> droppedItem = (List<object>)e.Data.GetData(e.Data.GetFormats()[0]);
                    MoveImageToDifferentFolder(droppedItem, mediaFolderId);
                }

                else if (((DataObject)e.Data).ContainsFileDropList())
                {
                    MoveImagestoDatabaseFromExplorer((DataObject)e.Data, mediaFolderId, isSameFolder);
                }
            }
        }

        /// <summary>
        /// Adding/Moving the image on dropping the folder button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFolderButtonDrop(object sender, System.Windows.DragEventArgs e)
        {
            bool isSameFolder = false;
            string folderName = ((Button)sender).Content.ToString();
            if (folderName == _selectedButton.Content.ToString())
                isSameFolder = true;

            Guid mediaFolderId = new Guid();
            IEnumerable<Folder> folder = MediaView.GetFolderId(Convert.ToInt32(_openFolder.Content.ToString()), folderName);
            foreach (var f in folder)
            {
                mediaFolderId = f.MediaFolderId;
            }

            if (e.Data is DataObject)
            {
                if (e.Data.GetData(e.Data.GetFormats()[0]) is List<object>)
                {
                    List<object> droppedItem = (List<object>)e.Data.GetData(e.Data.GetFormats()[0]);
                    MoveImageToDifferentFolder(droppedItem, mediaFolderId);
                }

                else if (((DataObject)e.Data).ContainsFileDropList())
                {
                    MoveImagestoDatabaseFromExplorer((DataObject)e.Data, mediaFolderId, isSameFolder);
                }
            }
        }

        private void MoveImagestoDatabaseFromExplorer(DataObject data, Guid mediaFolderId, bool isSameFolder)
        { 
            //adding images to database after dropping them to the grid.
            StringBuilder sb = new StringBuilder();
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            RappTestEntities mediaEntity = new RappTestEntities();
            List<Media> lstMedia = new List<Media>();
            MediaView mv = new MediaView();
            var query = from m in
                            mediaEntity.Media
                        where m.MediaFolderId == mediaFolderId
                        select m.Sequence;
            int lastSequence = 0;
            if (query.Count() > 0)
                lastSequence = Convert.ToInt32(query.Max());

            //checking if any file has been dropped on the grid
            int counter = 0;
            foreach (string filePath in data.GetFileDropList())
            {
                try
                {
                    if (Utilities.Utilities.IsImage(filePath) || Utilities.Utilities.IsVideo(filePath))
                    {
                        lastSequence++;
                        var fileName = filePath.ToString();
                        Media m = new Media();
                        m.Sequence = lastSequence;
                        m.FileName = Utilities.Utilities.RenameAndMoveFile(filePath.ToString());
                        m.IsDeleted = false;
                        m.IsScreenSaver = false;
                        m.MediaFolderId = mediaFolderId;
                        lstMedia.Add(m);
                    }
                    else
                    {
                        if (counter == 0)
                        {
                            sb.Append("Following files you are trying to import appears to be corrupt: ");
                            sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                        }
                        sb.Append(System.IO.Path.GetFileName(filePath));
                        sb.Append(Environment.NewLine);
                        counter++;
                    }
                }
                catch (Exception ex)
                {
                    sb.Append(System.IO.Path.GetFileName(filePath));
                    sb.Append(Environment.NewLine);
                }
            }

            //inserting images and binding the grid again
            MediaView.InsertImages(lstMedia);

            if (isSameFolder)
            {
                MediaImportControl ic = new MediaImportControl();
                ic.BindImages(mediaFolderId);
            }

            if (sb.Length > 0)
                MessageBox.Show(sb.ToString());
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
            UpdateTitleandSequence();
        }


        /// <summary>
        /// moving the image to different Folder
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <param name="mediaFolderId"></param>
        private void MoveImageToDifferentFolder(List<object> mediaItem, Guid mediaFolderId)
        {
            Media m = (Media)mediaItem[0];
            Guid mediaId = (Guid)m.MediaId;
            MediaView mv = new MediaView();
            mv.UpdateImageLocation(mediaId, mediaFolderId);
            //binding UI again
            mv.GetAllMediaData((Guid)lblMediaFolderId.Content);
        }

        private void SaveDetails()         
        {
            if (_lastSelectedTabItem.Header.ToString() == "Import")
            {
                UpdateTitleandSequence();
            }
            else if (_lastSelectedTabItem.Header.ToString() == "Organize")
            {
                SaveOrganizerSettings();
            }
            else if (_lastSelectedTabItem.Header.ToString() == "Script")
            {
                SaveMedia();
            }
        }

        private void SaveOrganizerSettings()
        {
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

            UpdateMediaFolderTitle(mediaFolderId, txtOrganizerHeader.Text);
        }

        private void UpdateTitleandSequence()
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
            UpdateMediaFolderTitle(mediaFolderId, txtImportHeader.Text);
          
        }


        private void UpdateMediaFolderTitle(Guid mediaFolderId, string title)
        {
            MediaView mv = new MediaView();
            mv.UpdateMediaFolderTitle(mediaFolderId, title);           
            txtImportHeader.Text = title;
            txtScriptHeader.Text = title;
            txtOrganizerHeader.Text = title;
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
                SaveDetails();

                if (clickedFolder == _openFolder && this.tabControl.SelectedIndex == 3)
                    return;

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

                if (this.tabControl.SelectedIndex == 4)
                {
                    int folderNum = Convert.ToInt32(_openFolder.Content.ToString());
                    ObservableCollection<ImageBucket> lstBucket = (ObservableCollection<ImageBucket>)mediaListBox.Tag;
                    var query = from m in lstBucket
                                where m.FolderNum <= folderNum
                                select m;
                    double sumOffSet = 0;

                    if (folderNum == 1)
                        sumOffSet = 0;
                    else if (folderNum == 9)
                        sumOffSet = imgBucketScrollView.MaxHeight;
                    else
                        sumOffSet = lstBucket.Where(l => l.FolderNum <= folderNum).Sum(l => l.Percentage);

                    imgBucketScrollView.ScrollToVerticalOffset(sumOffSet);
                }

                if (this.tabControl.SelectedIndex == 3)
                    this.tabControl.SelectedIndex = 2;


                lblScriptMediaId.Content = null;
                lblOrganizerMediaId.Content = null;
                EmptyFields();
            }
        }

        private void EmptyFields()
        {
            txtDescription.Text = string.Empty;
            txtDescriptionOrganizer.Text = string.Empty;
            txtNotes.Text = string.Empty;
            txtNotesOrganizer.Text = string.Empty;
            txtTitle.Text = string.Empty;
            txtTitleOrganizer.Text = string.Empty;
            imgScript.Source = null;
        }

        private void btnMoreData_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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
            SaveOrganizerSettings();
        }

        private void btnSaveMedia_Click(object sender, RoutedEventArgs e)
        {
            SaveMedia();
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