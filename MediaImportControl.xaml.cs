﻿using System;
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
        private static DispatcherTimer myClickWaitTimer =
         new DispatcherTimer(
             new TimeSpan(0, 0, 0, 1),
             DispatcherPriority.Background,
             mouseWaitTimer_Tick,
             Dispatcher.CurrentDispatcher);

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
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            BindImages((Guid)window.lblMediaFolderId.Content);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }

        private void OnDropQuery(object sender, Telerik.Windows.Controls.DragDrop.DragDropQueryEventArgs e)
        {
            e.QueryResult = e.Options.DropDataObject != null && e.Options.DropDataObject.ContainsFileDropList();
        }

        private void OnDrop(object sender, System.Windows.DragEventArgs e)
        {
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

                if (e.Data is DataObject && ((DataObject)e.Data).ContainsFileDropList())
                {
                    foreach (string filePath in ((DataObject)e.Data).GetFileDropList())
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
                    MediaView.InsertImages(lstMedia);
                    BindImages(mediaFolderId);
                }
                else
                {
                    List<Medium> lstMedium = new List<Medium>();
                    int sequence = 0;
                    foreach (Media item in lstImageGallery.Items)
                    {
                        sequence++;
                        Medium mItem = new Medium();
                        mItem.MediaId = item.MediaId;
                        mItem.Sequence = sequence;
                        mItem.FileName = item.FileName;
                        lstMedium.Add(mItem);
                    }
                    mv.UpdateSequence(lstMedium);
                    BindImages(mediaFolderId);
                }
        }


        private void OnDropInfo(object sender, Telerik.Windows.Controls.DragDrop.DragDropEventArgs e)
        {
           
        }

        private void theGrid_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_GiveFeedback(object sender, System.Windows.GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_DragOver(object sender, System.Windows.DragEventArgs e)
        {
          

        }

        private void theGrid_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
           
        }

        private void theGrid_Drop(object sender, System.Windows.DragEventArgs e)
        {
           
            
        }

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

        private void lstImageGallery_DragDropCompleted(object sender, Telerik.Windows.DragDrop.DragDropCompletedEventArgs e)
        {

        }

        private void lstImageGallery_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            myClickWaitTimer.Stop();
            ListBox item = (ListBox)sender;
            Guid mediaId = ((Media)(item.SelectedItems[0])).MediaId;
            GetMediaByMediaId(mediaId, 3);
            e.Handled = true;
          
        }

        private void lstImageGallery_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myClickWaitTimer.Start();
            ListBox item = (ListBox)sender;
            Guid mediaId = ((Media)(item.SelectedItems[0])).MediaId;
            GetMediaByMediaId(mediaId, 2);
            e.Handled = true;
        }

        private void lstImageGallery_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private static void mouseWaitTimer_Tick(object sender, EventArgs e)
        {
            myClickWaitTimer.Stop();

            // Handle Single Click Actions
            //Trace.WriteLine("Single Click");
        }

        private void GetMediaByMediaId(Guid mediaId, int selectedIndex)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            
            
            if (selectedIndex == 2)
            {
                window.tabControl.SelectedIndex = selectedIndex;
            }
            else if (selectedIndex == 3)
            {
                //window.imgScript.Source = new BitmapImage(new Uri(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Images\\" + m.FileName));
                window.tabControl.SelectedIndex = 3;
            }

            ShowData(mediaId);
        }

        private void ShowData(Guid mediaId)
        {
            MediaView mv = new MediaView();
            Medium m = mv.GetMediaByMediaId(mediaId); 
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            window.imgScript.Source = new BitmapImage(new Uri(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Images\\" + m.FileName));
            window.lblOrganizerMediaId.Content = mediaId;
            window.txtTitle.Text = m.Title;
            window.txtDescription.Text = m.Description;
            window.txtNotes.Text = m.Notes;
            window.lblScriptMediaId.Content = mediaId;
            window.txtTitleOrganizer.Text = m.Title;
            window.txtDescriptionOrganizer.Text = m.Description;
            window.txtNotesOrganizer.Text = m.Notes;
        }

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

      

    }
}
