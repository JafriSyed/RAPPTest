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


namespace RAPPTest
{
    /// <summary>
    /// This class defines the Drag and Drop panel control that will hold the media items with drag and drop
    /// functionality.
    /// </summary>
    public partial class MediaImportControl : UserControl
    {
       
        public MediaImportControl()
        {
            InitializeComponent();
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

        private void OnDropInfo(object sender, Telerik.Windows.Controls.DragDrop.DragDropEventArgs e)
        {
            if (e.Options.Status == DragStatus.DropComplete)
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                RappTestEntities mediaEntity = new RappTestEntities();
                List<Media> lstMedia = new List<Media>();
                Guid mediaFolderId = (Guid)window.lblMediaFolderId.Content;
                var fileList = e.Options.DropDataObject.GetFileDropList();
                var query = from m in
                                mediaEntity.Media
                            where m.MediaFolderId == mediaFolderId
                            select m.Sequence;
                int lastSequence = 0;
                if (query.Count() > 0)
                    lastSequence = Convert.ToInt32(query.Max());

                foreach (var file in fileList)
                {
                    var fileName = file.ToString();
                    Media m = new Media();
                    m.Sequence = lastSequence + 1;
                    m.FileName = file.ToString();
                    m.IsDeleted = false;
                    m.IsScreenSaver = false;
                    m.MediaFolderId = mediaFolderId;
                    lstMedia.Add(m);
                }
                MediaView.InsertImages(lstMedia);
                BindImages(mediaFolderId);
            }
        }

        private void theGrid_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_DragOver(object sender, DragEventArgs e)
        {
            Grid grid = sender as Grid;
            MediaItemControl mediaItemUC = new MediaItemControl();
            if (grid != null)
            {
                if (e.Data.GetDataPresent(mediaItemUC.GetType()))
                {
                    mediaItemUC = e.Data.GetData(mediaItemUC.GetType()) as MediaItemControl;
                }
            }

        }

        private void theGrid_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        private void theGrid_Drop(object sender, DragEventArgs e)
        {
           
            
        }

        public void BindImages(Guid mediaFolderId)
        {
            try
            {
                ObservableCollection<Media> mediaObj = MediaView.GetAllMediaData(mediaFolderId);
                //List<Media> lstMediaObj = MediaView.GetAllMediaData(mediaFolderId);
                if (mediaObj.Count() > 0)
                {
                    lstImageGallery.DataContext = mediaObj;
                    //lstImageGallery.Visibility = Visibility.Visible;
                }
                //else
                    //lstImageGallery.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void lstImageGallery_DragDropCompleted(object sender, Telerik.Windows.DragDrop.DragDropCompletedEventArgs e)
        {

        }

    }
}
