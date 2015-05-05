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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace RAPPTest
{
    /// <summary>
    /// Interaction logic for showcaseList.xaml
    /// </summary>
    public partial class showcaseList : Window
    {
       
        public showcaseList(String keyName)
        {
            InitializeComponent();
            RappTestEntities entity = new RappTestEntities();
            MediaView mv =new MediaView();
            ObservableCollection<Media> lstMedia = mv.GetImagesByFolderNumber(Convert.ToInt32(keyName));
            mediaListBox.ItemsSource = lstMedia;
            
        }

        
    }
}

