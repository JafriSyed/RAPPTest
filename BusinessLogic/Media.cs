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

        private static string[] IMAGE_FORMATS = new string[] { ".bmp", ".gif", ".jpg", ".jpeg", ".tif" };
        private static string[] VIDEO_FORMATS = new string[] { ".avi", ".flv", ".mov", ".mp4", ".mpg", ".wmv" };

        private string source;
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

        public Media()
        {
            
        }

        public bool IsImage()
        {
            return IMAGE_FORMATS.Contains(Path.GetExtension(this.FileName).ToLower());
        }

        public bool IsVideo()
        {
            return VIDEO_FORMATS.Contains(Path.GetExtension(this.FileName).ToLower());
        }

        // source can either refer to absolute path, in case its the original
        // or to relative path, in case its transferred into our file folder
        public string Source
        {
            get
            {
                if (IsSourceInternal)
                {
                    return Path.GetFullPath(Path.Combine(System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "//Images//", this.FileName));
                }
                else
                {
                    return source;
                }
            }
        }
        public bool IsSourceExternal
        {
            get { return File.Exists(source); }
        }

        public bool IsSourceInternal
        {
            get { return !IsSourceExternal; }
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


        public static bool IsValid(string path)
        {
            return FileValidator.IsValidFile(path);
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

        public Image GetImageThumbnail()
        {
            return GetImage();
        }

        public Image ScriptTabGetUIElement()
        {
            Uri sourceUri = new Uri(Source, UriKind.Absolute);
            string source = Source.ToString();
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(source);
            if (IsImage())
            {
                return GetImageByUri(sourceUri);
            }
            else
            {
                string fullPath = Path.Combine("", filenameWithoutExtension + ".jpg");
                return GetImageByUri(new Uri(fullPath));
            }
        }


        /* Siwei added end */


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
