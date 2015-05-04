using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace RAPPTest
{
    public class Media : INotifyPropertyChanged
    {
        #region Member Variables

        /// <summary>
        /// /
        /// </summary>
        private Guid _mediaId;

        /// <summary>
        /// 
        /// </summary>
        private Guid _mediaFolderId;

        /// <summary>
        /// 
        /// </summary>
        private string _fileName;

        /// <summary>
        /// 
        /// </summary>
        private string _title;

        /// <summary>
        /// 
        /// </summary>
        private string _description;

        /// <summary>
        /// 
        /// </summary>
        private string _notes;

        /// <summary>
        /// 
        /// </summary>
        private int _sequence;

        /// <summary>
        /// 
        /// </summary>
        private bool _isScreenSaver;

        /// <summary>
        /// 
        /// </summary>
        private bool _isDeleted;

        /// <summary>
        /// 
        /// </summary>
        private int _folderNum;

        /// <summary>
        /// 
        /// </summary>
        private string _folderName;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Media> _mediaList = new ObservableCollection<Media>();

        private static string[] _imageFormats = new string[] { ".bmp", ".gif", ".jpg", ".jpeg", ".tif" };
        private static string[] _videoFormats = new string[] { ".avi", ".flv", ".mov", ".mp4", ".mpg", ".wmv" };

        #endregion

        #region Properties


        /// <summary>
        /// 
        /// </summary>
        public Guid MediaId
        {
            get { return this._mediaId; }
            set
            {
                if (this._mediaId != value)
                {
                    this._mediaId = value;
                    this.NotifyPropertyChanged("MediaId");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid MediaFolderId
        {
            get { return this._mediaFolderId; }
            set
            {
                if (this._mediaFolderId != value)
                {
                    this._mediaFolderId = value;
                    this.NotifyPropertyChanged("MediaFolderId");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return this._fileName; }
            set
            {
                if (this._fileName != value)
                {
                    this._fileName = value;
                    this.NotifyPropertyChanged("FileName");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            get { return this._title; }
            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.NotifyPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return this._description; }
            set
            {
                if (this._description != value)
                {
                    this._description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Notes
        {
            get { return this._notes; }
            set
            {
                if (this._notes != value)
                {
                    this._notes = value;
                    this.NotifyPropertyChanged("Notes");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Sequence
        {
            get { return this._sequence; }
            set
            {
                if (this._sequence != value)
                {
                    this._sequence = value;
                    this.NotifyPropertyChanged("Sequence");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsScreenSaver
        {
            get { return this._isScreenSaver; }
            set
            {
                if (this._isScreenSaver != value)
                {
                    this._isScreenSaver = value;
                    this.NotifyPropertyChanged("IsScreenSaver");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted
        {
            get { return this._isDeleted; }
            set
            {
                if (this._isDeleted != value)
                {
                    this._isDeleted = value;
                    this.NotifyPropertyChanged("IsDeleted");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int FolderNum
        {
            get { return this._folderNum; }
            set
            {
                if (this._folderNum != value)
                {
                    this._folderNum = value;
                    this.NotifyPropertyChanged("FolderNum");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FolderName
        {
            get { return this._folderName; }
            set
            {
                if (this._folderName != value)
                {
                    this._folderName = value;
                    this.NotifyPropertyChanged("FolderName");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Media> MediaList
        {
            get { return _mediaList; }
            set
            {
                if (this._mediaList != value)
                {
                    this._mediaList = value;
                    NotifyPropertyChanged("MediaList");
                }
            }
        }

        #endregion

        private static string GetName(string source, string targetWithoutExt)
        {
            string ext = Path.GetExtension(source);
            return targetWithoutExt + ext;
        }

        public bool IsImage()
        {
            //checking if file is an image
            return _imageFormats.Contains(Path.GetExtension(this.FileName).ToLower());
        }

        public bool IsVideo()
        {
            //checking if file is a video
            return _videoFormats.Contains(Path.GetExtension(this.FileName).ToLower());
        }

        // source can either refer to absolute path, in case its the original
        // or to relative path, in case its transferred into our file folder
        public string Source
        {
            get
            {

                // get parent directory from current directory
                string parentDirectory = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;

                // then add images folder to path to make full path
                string fullPath = Path.Combine(parentDirectory + "//Media//", this.FileName);

                return Path.GetFullPath(fullPath);

            }
        }
     
        private Image GetImage()
        {
            return CreateImage(new Uri(Source, UriKind.Absolute));
        }


        private MediaElement GetVideo()
        {
            return CreateVideo(new Uri(Source, UriKind.Absolute));
        }

        private Image GetImageByUri(Uri uri)
        {
            return CreateImage(uri);
        }

        public static bool IsImage(Uri uri)
        {
            return CreateImage(uri) != null;
        }

        public static bool IsVideo(Uri uri)
        {
            return CreateVideo(uri) != null;
        }

        public static Image CreateImage(Uri uri)
        {
            try
            {
                //creating image
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = uri;
                bmp.EndInit();

                Image image = new Image();
                image.Source = bmp;
                return image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
                return null;
            }
        }

        public static MediaElement CreateVideo(Uri uri)
        {
            try
            {
                //creating video
                MediaElement video = new MediaElement();

                video.BeginInit();
                video.Source = uri;

                return video;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
                return null;
            }
        }

        public UIElement GetUIElement()
        {
            if (IsImage())
            {
                return GetImage();
            }
            else if (IsVideo())
            {
                return GetVideo();
            }
            else
            {
                return null;
            }
        }

        #region INotifyPropertyChangeEvents

        /// <summary>
        /// Raises the PropertyChanged notification in a thread safe manner
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        #endregion
    }
}
