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
using System.IO;
using System.Collections.ObjectModel;

using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Telerik.Windows.Controls.DragDrop;
using System.ComponentModel;
using Telerik.Windows.DragDrop;
using System.Collections;
using System.Windows.Threading;
using System.Timers;

namespace RAPPTest
{
    /// <summary>
    /// This class defines the Drag and Drop panel control that will hold the media items with drag and drop
    /// functionality.
    /// </summary>
    public partial class MediaImportControl : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Media> _mediaObj = new ObservableCollection<Media>();
        private Media _selectedModel;
        private ListBox _associatedObject;

        public MediaImportControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// AssociatedObject Property
        /// </summary>
        public ListBox AssociatedObject
        {
            get
            {
                return _associatedObject;
            }
            set
            {
                _associatedObject = value;
            }
        }

        protected virtual void Initialize()
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Binding listbox with image list when import control is loaded.
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            BindImages((Guid)window.lblMediaFolderId.Content);
        }

        private void OnDrop(object sender, System.Windows.DragEventArgs e)
        {
            //adding images to database after dropping them to the grid.
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            RappTestEntities mediaEntity = new RappTestEntities();
            List<Media> lstMedia = new List<Media>();
            MediaView mv = new MediaView();
            Guid mediaFolderId = (Guid)window.lblMediaFolderId.Content;
            var query = from m in
                            mediaEntity.Media
                        where m.MediaFolderId == mediaFolderId
                        select m.Sequence;
            int lastSequence = 0;
            if (query.Count() > 0)
                lastSequence = Convert.ToInt32(query.Max());

            //checking if any file has been dropped on the grid

            if (e.Data is DataObject && ((DataObject)e.Data).ContainsFileDropList())
            {
                foreach (string filePath in ((DataObject)e.Data).GetFileDropList())
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
                }
                //inserting images and binding the grid again
                MediaView.InsertImages(lstMedia);
                BindImages(mediaFolderId);
            }
        }

        private void theGrid_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_GiveFeedback(object sender, System.Windows.GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// this function binds the images to the list
        /// </summary>
        /// <param name="mediaFolderId"></param>
        public void BindImages(Guid mediaFolderId)
        {
            try
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                MediaView mv = new MediaView();
                MediaObj = mv.GetAllMediaData(mediaFolderId);
                if (window.tabControl.SelectedIndex == 1)
                    window.dndPanelImport.lstImageGallery.ItemsSource = MediaObj;
                else if (window.tabControl.SelectedIndex == 2)
                    window.dndPanel.lstImageGallery.ItemsSource = MediaObj;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// this event is called when user clicks on the image thumbnail and shows script tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstImageGallery_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            ListBox item = (ListBox)sender;
            Guid mediaId = ((Media)(item.SelectedItems[0])).MediaId;
            GetMediaByMediaId(mediaId);
            window.tabControl.SelectedIndex = 3;
            e.Handled = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstImageGallery_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            ListBox item = (ListBox)sender;
            if (item.SelectedItems.Count > 0)
            {
                Guid mediaId = ((Media)(item.SelectedItems[0])).MediaId;
                GetMediaByMediaId(mediaId);
            }
            if (window.tabControl.SelectedIndex == 2)
            {
                window.tabControl.SelectedIndex = 3;
                window.tabControl.SelectedIndex = 2;
            }
            e.Handled = true;
        }

        /// <summary>
        /// this function gets media by media id
        /// </summary>
        /// <param name="mediaId"></param>
        /// <param name="selectedIndex"></param>
        private void GetMediaByMediaId(Guid mediaId)
        {
            ShowData(mediaId);
        }

        /// <summary>
        /// this function shows image related data i.e. title, description etc.
        /// </summary>
        /// <param name="mediaId"></param>
        private void ShowData(Guid mediaId)
        {
            MediaView mv = new MediaView();
            Medium m = mv.GetMediaByMediaId(mediaId);
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            //checking if file is video then replace the video extension with jpg to show as thumbnail
            string fileName = Utilities.Utilities.IsVideo(m.FileName) ? Utilities.Utilities.ReplaceVideoExtension(m.FileName) : m.FileName;
            window.imgScript.Source = new BitmapImage(new Uri(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Media\\" + fileName));
            window.lblOrganizerMediaId.Content = mediaId;
            window.txtTitle.Text = m.Title;
            window.txtDescription.Text = m.Description;
            window.txtNotes.Text = m.Notes;
            window.lblScriptMediaId.Content = mediaId;
            window.txtTitleOrganizer.Text = m.Title;
            window.txtDescriptionOrganizer.Text = m.Description;
            window.txtNotesOrganizer.Text = m.Notes;
        }

        /// <summary>
        /// setting image as screen saver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstImageGallery_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox item = (ListBox)sender;
            MediaView mv = new MediaView();
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you want to set this image as screen saver?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Guid mediaId = ((Media)(item.SelectedItems[0])).MediaId;
                mv.SetImageAsScreenSaver(mediaId);
            }
            else
                return;
        }

        public ObservableCollection<Media> MediaObj
        {
            get { return _mediaObj; }
            set { _mediaObj = value; NotifyPropertyChanged("MediaObj"); }
        }

        public Media SelectedModel
        {
            get { return _selectedModel; }
            set { _selectedModel = value; NotifyPropertyChanged("SelectedModel"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private int m_ListBoxWidth = 350;

        public int ListBoxWidth
        {
            get { return m_ListBoxWidth; }
            set
            {
                m_ListBoxWidth = value;
                NotifyPropertyChanged("ListBoxWidth");
            }
        }

        private int m_ListBoxHeight = 150;

        public int ListBoxHeight
        {
            get { return m_ListBoxHeight; }
            set
            {
                m_ListBoxHeight = value;
                NotifyPropertyChanged("ListBoxHeight");
            }
        }
    }
}
