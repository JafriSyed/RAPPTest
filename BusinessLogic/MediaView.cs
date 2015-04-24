﻿using System;
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

        public ObservableCollection<Media> mediaList = new ObservableCollection<Media>();

        public ObservableCollection<Media> GetAllMediaData(Guid mediaFolderId)
        {
            try
            {
                RappTestEntities rappEntity = new RappTestEntities();
                var query = from m in rappEntity.Media
                            where m.MediaFolderId == mediaFolderId
                            && m.IsDeleted == false
                            select new Media
                            {
                                MediaId = (Guid)m.MediaId,
                                FileName = m.FileName,
                                Sequence = (Int32)m.Sequence
                            };

                mediaList = new ObservableCollection<Media>(query);
                AddDirectoryToFileName(mediaList);
                return mediaList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Medium GetMediaByMediaId(Guid mediaId)
        {
            try
            {
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

        public void UpdateSequence(List<Medium> lstMedia)
        {
            try
            {
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

        public void UpdateImageData(Guid mediaId, Medium m)
        {
            try
            {
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

        public ObservableCollection<Media> GetAllImages()
        {
            try
            {
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
                    mediaList = new ObservableCollection<Media>(query);
                    AddDirectoryToFileName(mediaList);
                }
                return mediaList;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void InsertImages(List<Media> mediaList)
        {
            try
            {
                RappTestEntities rappEntity = new RappTestEntities();
                foreach (Media media in mediaList)
                {
                    Medium m = new Medium();
                    m.MediaId = Guid.NewGuid();
                    m.MediaFolderId = media.MediaFolderId;
                    m.FileName = media.FileName;
                    m.Sequence = media.Sequence;
                    m.IsScreenSaver = false;
                    m.IsDeleted = false;
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

        public void DeleteImage(Guid mediaId)
        {
            try
            {
                RappTestEntities rappEntity = new RappTestEntities();
                var query = from m in rappEntity.Media
                            where m.MediaId == mediaId
                            select m;

                foreach (var m in query)
                {
                    m.IsDeleted = true;
                    rappEntity.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void SetImageAsScreenSaver(Guid mediaId)
        {
            try
            {
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

        public static IEnumerable<Folder> GetFolderId(int folderNum, string folderName)
        {
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

        public ObservableCollection<Media> MediaList
        {
            get { return mediaList; }
            set
            {
                if (this.mediaList != value)
                {
                    this.mediaList = value;
                    this.CollectionChanged(this.mediaList, new NotifyCollectionChangedEventArgs(
                                                                NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void NotifyPropertyChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
                this.CollectionChanged(this, args);
        }

        private void AddDirectoryToFileName(ObservableCollection<Media> mediaList)
        {
            string appDirectory = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string newFilePathForImages = appDirectory + "\\Images\\";
            foreach (Media m in mediaList)
            {
                m.FileName = string.Format("{0}{1}", newFilePathForImages, m.FileName);
            }
        }
    }
}
