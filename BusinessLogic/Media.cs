using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace RAPPTest
{
    public class Media : INotifyPropertyChanged
    {
        #region Member Variables
        
        private Guid _mediaId;
        private Guid _mediaFolderId;
        private string _fileName;
        private string _title;
        private string _description;
        private string _notes;
        private int _sequence;
        private bool _isScreenSaver;
        private bool _isDeleted;
        private int _folderNum;
        private string _folderName;
        public ObservableCollection<Media> _mediaList = new ObservableCollection<Media>(); 

        #endregion

        #region Properties

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

        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        #endregion
    }
}
