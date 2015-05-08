using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.Serialization;

namespace RAPPTest
{

    public interface IModel { }
    [Serializable()]
    public abstract class Model : IModel, INotifyPropertyChanged
    {


        private event PropertyChangedEventHandler m_propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add{ m_propertyChanged += value; }
            remove{ m_propertyChanged -= value; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (null != m_propertyChanged)
            {
                m_propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }

    }

    [Serializable()]
    public class MediaModel : Model
    {
        private static string _root = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "//Media//";

        private static string[] _imageFormats = new string[] { ".bmp", ".gif", ".jpg", ".jpeg", ".tif" };
        private static string[] _videoFormats = new string[] { ".avi", ".flv", ".mov", ".mp4", ".mpg", ".wmv" };

        private Media element;
        //private CellModel cellModel;

        private string source;

        //Deserialization constructor.
        public MediaModel(SerializationInfo info, StreamingContext ctxt)
		{
			//Get the values from info and assign them to the appropriate properties
            element = (Media)info.GetValue("element", typeof(Media));
            //cellModel = (CellModel)info.GetValue("cellModel", typeof(CellModel));
		}
		

        public bool IsImage()
        {
            //checking if the file is an image
            return _imageFormats.Contains(Path.GetExtension(element.FileName).ToLower());
        }

        public bool IsVideo()
        {
            //checking if the file is a video
            return _videoFormats.Contains(Path.GetExtension(element.FileName).ToLower());
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
                //getting image to show as a slideshow when in the full screen mode
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
                //creating the video to play when in the fullscreen mode.
                MediaElement video = new MediaElement();
                video.Name = "videoPlayer";
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
            // checking if its an image then get the image otherwise get the video
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


        // source can either refer to absolute path, in case its the original
        // or to relative path, in case its transferred into our file folder
        public string Source
        {
            get
            {
                 return Path.GetFullPath(Path.Combine(_root, source));
            }
        }

    }
}