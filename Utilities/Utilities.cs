using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using DexterLib;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;


namespace RAPPTest.Utilities
{
    public static class Utilities
    {
        private static string[] _imageFormats = new string[] { ".bmp", ".gif", ".jpg", ".jpeg", ".tif" };
        private static string[] _videoFormats = new string[] { ".avi", ".flv", ".mov", ".mp4", ".mpg", ".wmv" };
        /// <summary>
        /// populates screen saver drop down control
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<int> GetTimerList()
        {
            ObservableCollection<int> timerList = new ObservableCollection<int>();
            for (int i = 1; i <= 10; i++)
            {
                timerList.Add(30 * i);
            }
            return timerList;
        }

        /// <summary>
        /// renaming and moving file to the images folder
        /// </summary>
        /// <param name="oldFileName"></param>
        /// <returns></returns>
        public static string RenameAndMoveFile(string oldFileName)
        {
            string newFileName = string.Empty;
            string ext = Path.GetExtension(oldFileName);
            string oldFilePath = Path.GetFileNameWithoutExtension(oldFileName);
            string appDirectory = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string newFilePath = appDirectory + "\\Media\\";
            string newFileThumbnailPath = appDirectory + "\\Media\\Thumbnails\\";
            newFileName = DateTime.Now.Ticks.ToString() + ext;
            File.Copy(oldFileName, newFilePath + newFileName);
            if(IsVideo(newFileName))
                ExtractVideoFromImage(newFilePath, newFileName, newFileThumbnailPath);
            else
                GenerateThumbnail(newFileName, newFilePath, newFileThumbnailPath, false);

            return newFileName;
        }

        private static void ExtractVideoFromImage(string filePath, string fileName, string thumbnailFilePath)
        {
            MediaDet md = new MediaDet();
            float interval = 1.0f;
            md.Filename = filePath + fileName;
            md.CurrentStream = 0;
            int len = (int)md.StreamLength;
            decimal length = len / 2;
            int midLen = Convert.ToInt32(Math.Round(Convert.ToDecimal(len / 2)));
            for (float i = 0; i < len; i = i + interval)
            {
                if (i == midLen)
                {
                    string fBitmapName = filePath + Path.GetFileNameWithoutExtension(fileName);
                    string bitmapFileName = fBitmapName + ".bmp";                  
                    md.WriteBitmapBits(i, 320, 240, bitmapFileName);               
                    GenerateThumbnail(bitmapFileName, filePath, thumbnailFilePath, true);
                }
            }
        }

        public static bool IsImage(string fileName)
        {
            return _imageFormats.Contains(Path.GetExtension(fileName).ToLower());
        }

        public static bool IsVideo(string fileName)
        {
            return _videoFormats.Contains(Path.GetExtension(fileName).ToLower());
        }

        public static string ReplaceVideoExtension(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return fileName + ".jpg";
        }

        private static void GenerateThumbnail(string fileName, string filePath, string thumbnailPath, bool isVideo)
        {
            try
            {

                // Load image.
                Image image;

                if (isVideo)
                    image = Image.FromFile(fileName);
                else
                    image = Image.FromFile(filePath + fileName);

                // Compute thumbnail size.
                Size thumbnailSize = GetThumbnailSize(image);

                // Get thumbnail.
                Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);

                // Save thumbnail.
                if (!isVideo)
                    thumbnail.Save(thumbnailPath + fileName);
                else
                {
                    string jpegFileName = Path.GetFileNameWithoutExtension(fileName) + ".jpg";
                    thumbnail.Save(thumbnailPath + jpegFileName, ImageFormat.Jpeg);
                    thumbnail.Dispose();
                    //System.IO.File.Delete(fileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static Size GetThumbnailSize(Image original)
        {
            // Width and height.
            int originalWidth = original.Width;
            int originalHeight = original.Height;
            int width = 400;
            int height = 350;
            decimal ratio;
            int newWidth = 0;
            int newHeight = 0;
               
            if (originalWidth < width && originalHeight < height)
                return new Size(originalWidth, originalHeight);

            if (originalWidth > originalHeight)
            {
                ratio = (decimal)width /originalWidth;
                newWidth = width;
                decimal temp = originalHeight * ratio;
                newHeight = (int)temp;
            }
            else
            {
                ratio = (decimal)height / originalHeight;
                newHeight = height;
                decimal temp = originalWidth * ratio;
                newWidth = (int)temp;
            }

            return new Size((int)(newWidth), (int)(newHeight));
        }

   
    }
}