using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace RAPPTest.Utilities
{
    public static class Utilities
    {
        public static ObservableCollection<int> GetTimerList()
        {
            ObservableCollection<int> timerList = new ObservableCollection<int>();
            for (int i = 1; i <= 10; i++)
            {
                timerList.Add(30 * i);
            }
            return timerList;
        }

        public static string RenameAndMoveFile(string oldFileName)
        {
            string newFileName = string.Empty;
            string ext = Path.GetExtension(oldFileName);
            string oldFilePath = Path.GetFileNameWithoutExtension(oldFileName);
            string appDirectory = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            string newFilePathForImageThumbnails =  appDirectory + "\\Images//Thumbnails\\";
            string newFilePathForImages = appDirectory + "\\Images\\";
            string newFilePathForVideos = appDirectory + "\\Videos\\";
            newFileName = DateTime.Now.Ticks.ToString() + ext;
            File.Copy(oldFileName, newFilePathForImages + newFileName);
            return newFileName;
        }

     
    }
}
