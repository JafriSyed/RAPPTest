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
    public class ElementModel : Model
    {
        private static string ROOT = "";
        private static string thumbROOT = "";

        private static string[] IMAGE_FORMATS = new string[] { ".bmp", ".gif", ".jpg", ".jpeg", ".tif" };
        private static string[] VIDEO_FORMATS = new string[] { ".avi", ".flv", ".mov", ".mp4", ".mpg", ".wmv" };

        private Media element;
        //private CellModel cellModel;

        private string source;

        //Deserialization constructor.
        public ElementModel(SerializationInfo info, StreamingContext ctxt)
		{
			//Get the values from info and assign them to the appropriate properties
            element = (Media)info.GetValue("element", typeof(Media));
            //cellModel = (CellModel)info.GetValue("cellModel", typeof(CellModel));
		}
		

        private static string GetName(string source, string targetWithoutExt)
        {
            string ext = Path.GetExtension(source);
            return targetWithoutExt + ext;
        }


        public bool IsImage()
        {
            return IMAGE_FORMATS.Contains(Path.GetExtension(element.FileName).ToLower());
        }

        public bool IsVideo()
        {
            return VIDEO_FORMATS.Contains(Path.GetExtension(element.FileName).ToLower());
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

      
        public Media Element
        {
            get { return element; }
        }

        public Guid Id
        {
            get { return element.MediaId; }
        }

        // source can either refer to absolute path, in case its the original
        // or to relative path, in case its transferred into our file folder
        public string Source
        {
            get
            {
                if (IsSourceInternal)
                {
                    return Path.GetFullPath(Path.Combine(ROOT, source));
                }
                else
                {
                    return source;
                }
            }
        }

        // Siwei added 23 Aug 2013
        // defines thumbnail source
        public string ThumbSource
        {
            get
            {
                if (IsSourceInternal)
                {
                    return Path.GetFullPath(Path.Combine(thumbROOT, source));
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

    }


    public class ClientIdleHandler : IDisposable
    {
        public bool IsActive { get; set; }

        int _hHookKbd;
        int _hHookMouse;

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public event HookProc MouseHookProcedure;
        public event HookProc KbdHookProcedure;

        //Use this function to install thread-specific hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
            IntPtr hInstance, int threadId);

        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //Use this function to pass the hook information to next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode,
            IntPtr wParam, IntPtr lParam);

        //Use this hook to get the module handle, needed for WPF environment
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public enum HookType : int
        {
            GlobalKeyboard = 13,
            GlobalMouse = 14
        }

        public int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //user is active, at least with the mouse
            IsActive = true;

            //just return the next hook
            return CallNextHookEx(_hHookMouse, nCode, wParam, lParam);
        }

        public int KbdHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //user is active, at least with the keyboard
            IsActive = true;

            //just return the next hook
            return CallNextHookEx(_hHookKbd, nCode, wParam, lParam);
        }

        public void Start()
        {
            using (var currentProcess = Process.GetCurrentProcess())
            using (var mainModule = currentProcess.MainModule)
            {

                if (_hHookMouse == 0)
                {
                    // Create an instance of HookProc.
                    MouseHookProcedure = new HookProc(MouseHookProc);
                    // Create an instance of HookProc.
                    KbdHookProcedure = new HookProc(KbdHookProc);

                    //register a global hook
                    _hHookMouse = SetWindowsHookEx((int)HookType.GlobalMouse,
                                                  MouseHookProcedure,
                                                  GetModuleHandle(mainModule.ModuleName),
                                                  0);
                    if (_hHookMouse == 0)
                    {
                        Close();
                        throw new ApplicationException("SetWindowsHookEx() failed for the mouse");
                    }
                }

                if (_hHookKbd == 0)
                {
                    //register a global hook
                    _hHookKbd = SetWindowsHookEx((int)HookType.GlobalKeyboard,
                                                KbdHookProcedure,
                                                GetModuleHandle(mainModule.ModuleName),
                                                0);
                    if (_hHookKbd == 0)
                    {
                        Close();
                        throw new ApplicationException("SetWindowsHookEx() failed for the keyboard");
                    }
                }
            }
        }

        public void Close()
        {
            if (_hHookMouse != 0)
            {
                bool ret = UnhookWindowsHookEx(_hHookMouse);
                if (ret == false)
                {
                    throw new ApplicationException("UnhookWindowsHookEx() failed for the mouse");
                }
                _hHookMouse = 0;
            }

            if (_hHookKbd != 0)
            {
                bool ret = UnhookWindowsHookEx(_hHookKbd);
                if (ret == false)
                {
                    throw new ApplicationException("UnhookWindowsHookEx() failed for the keyboard");
                }
                _hHookKbd = 0;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_hHookMouse != 0 || _hHookKbd != 0)
                Close();
        }

        #endregion
    }
}