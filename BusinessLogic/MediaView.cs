using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Data.Objects.SqlClient;
using System.Collections;

namespace RAPPTest
{
    public class MediaView : INotifyCollectionChanged
    {

        public ObservableCollection<Media> _mediaList = new ObservableCollection<Media>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaFolderId"></param>
        /// <returns></returns>
        public ObservableCollection<Media> GetAllMediaData(Guid mediaFolderId)
        {
            try
            {
                // Getting all media elements by mediafolderId
                RappTestEntities rappEntity = new RappTestEntities();
                var query = from m in rappEntity.Media
                            where m.MediaFolderId == mediaFolderId
                            orderby m.Sequence
                            select new Media
                            {
                                MediaId = (Guid)m.MediaId,
                                FileName = m.FileName,
                                Sequence = (Int32)m.Sequence
                            };

                //transforming the collection to the list
                _mediaList = new ObservableCollection<Media>(query);

                //initially we just have the file name so we are concatenating the directory to the file name to display it on the UI.
                AddDirectoryToFileName(_mediaList);
                return _mediaList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public Medium GetMediaByMediaId(Guid mediaId)
        {
            try
            {
                //getting media elements by media id
                RappTestEntities rappEntity = new RappTestEntities();
                var query = from m in rappEntity.Media
                            where m.MediaId == mediaId
                            select m;

                return (Medium)query.ToList()[0];

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstMedia"></param>
        public void UpdateSequence(List<Medium> lstMedia)
        {
            try
            {
                //updating the sequence of the images.
                RappTestEntities rappEntity = new RappTestEntities();
                var query = (from d1 in rappEntity.Media.AsEnumerable()
                             join d2 in lstMedia.AsEnumerable() on d1.MediaId equals d2.MediaId
                             select new { d1, d2.Sequence }).ToList();

                query.ForEach(b => b.d1.Sequence = b.Sequence);

                rappEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        /// <param name="m"></param>
        public void UpdateImageData(Guid mediaId, Medium m)
        {
            try
            {
                //updating image details, this function is called from the script UI and organizer UI.
                RappTestEntities rappEntity = new RappTestEntities();
                var query = from media in rappEntity.Media
                            where media.MediaId == mediaId
                            select media;
                foreach (var media in query)
                {
                    media.Description = m.Description;
                    media.Title = m.Title;
                    media.Notes = m.Notes;
                }
                rappEntity.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Media> GetAllImages()
        {
            try
            {
                //Getting all the images to show on the image view UI.
                RappTestEntities rappEntity = new RappTestEntities();

                var query = from m in rappEntity.Media
                            join mf in rappEntity.MediaFolders
                            on m.MediaFolderId equals mf.MediaFolderId
                            select new Media
                            {
                                FileName = m.FileName,
                                FolderNum = (Int32)mf.FolderNum,
                                FolderName = mf.FolderName,
                                Sequence = (Int32)m.Sequence
                            };
                if (query.Count() > 0)
                {
                    _mediaList = new ObservableCollection<Media>(query);
                    AddDirectoryToFileName(_mediaList);
                }
                return _mediaList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaList"></param>
        public static void InsertImages(List<Media> mediaList)
        {
            try
            {
                //inserting images into the database
                RappTestEntities rappEntity = new RappTestEntities();
                foreach (Media media in mediaList)
                {
                    Medium m = new Medium();
                    m.MediaId = Guid.NewGuid();
                    m.MediaFolderId = media.MediaFolderId;
                    m.FileName = media.FileName;
                    m.Sequence = media.Sequence;
                    m.IsScreenSaver = false;
                    m.Title = string.Empty;
                    m.Description = string.Empty;
                    m.Notes = string.Empty;
                    rappEntity.Media.AddObject(m);
                    rappEntity.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        public void DeleteImage(Guid mediaId)
        {
            try
            {
                RappTestEntities rapp = new RappTestEntities();
                var media = (from m in rapp.Media
                             where m.MediaId == mediaId
                             select m).First();


                rapp.Media.DeleteObject(media);
                rapp.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaId"></param>
        public void SetImageAsScreenSaver(Guid mediaId)
        {
            try
            {
                //setting the image as screen saver, only one image can be set at a time
                RappTestEntities rappTest = new RappTestEntities();
                var mediaList = rappTest.Media;
                foreach (Medium m in mediaList)
                {
                    if (m.MediaId == mediaId)
                        m.IsScreenSaver = true;
                    else
                        m.IsScreenSaver = false;
                }
                rappTest.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderNum"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static IEnumerable<Folder> GetFolderId(int folderNum, string folderName)
        {
            // getting folder id by name and number, this basically binds to the lblmediafolder id placed on the mainview xaml file
            RappTestEntities mediaEntity = new RappTestEntities();
            var query = from m in
                            mediaEntity.MediaFolders
                        where m.FolderNum == folderNum
                              && m.FolderName == folderName
                        select new Folder
                        {
                            MediaFolderId = m.MediaFolderId,
                            Title = m.Title
                        };

            return query;
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
                    this.CollectionChanged(this._mediaList, new NotifyCollectionChangedEventArgs(
                                                                NotifyCollectionChangedAction.Reset));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void NotifyPropertyChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaList"></param>
        private void AddDirectoryToFileName(ObservableCollection<Media> mediaList)
        {
            // this function loops to the list and concatenates directory to each file name.
            string appDirectory = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string newFilePathForImages = appDirectory + "\\Media\\";
            foreach (Media m in mediaList)
            {
                string fileName = Utilities.Utilities.IsVideo(m.FileName) ? Utilities.Utilities.ReplaceVideoExtension(m.FileName) : m.FileName;
                m.FileName = string.Format("{0}{1}", newFilePathForImages, fileName);
            }        }
    }
}
