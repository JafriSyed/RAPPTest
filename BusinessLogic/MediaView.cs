using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RAPPTest
{
    public class MediaView
    {
        public static ObservableCollection<Media> GetAllMediaData(Guid mediaFolderId)
        {
            try
            {
                RappTestEntities rappEntity = new RappTestEntities();
               
                var query = from m in rappEntity.Media
                            where m.MediaFolderId == mediaFolderId
                            select new Media
                            {
                                FileName = m.FileName,
                                Sequence = (Int32)m.Sequence
                            };

                ObservableCollection<Media> obsCol = new ObservableCollection<Media>(query);
                return obsCol;

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
    }
}
