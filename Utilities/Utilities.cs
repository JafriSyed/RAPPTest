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
            newFileName = DateTime.Now.Ticks.ToString() + ext;
            File.Copy(oldFileName, newFilePath + newFileName);
            if(IsVideo(newFileName))
                ExtractVideoFromImage(newFilePath, newFileName);
            return newFileName;
        }

        private static void ExtractVideoFromImage(string filePath, string fileName)
        {
            MediaDet md = new MediaDet();
            float interval = 1.0f;
            Image img;
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
                    md.WriteBitmapBits(i, 320, 240, fBitmapName + ".bmp");
                    img = System.Drawing.Image.FromFile(fBitmapName + ".bmp");
                    img.Save(fBitmapName + ".jpg", ImageFormat.Jpeg);
                    img.Dispose();
                    System.IO.File.Delete(fBitmapName + ".bmp");
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
    }
}