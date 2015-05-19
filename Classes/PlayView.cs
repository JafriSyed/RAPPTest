using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Reflection;

namespace RAPPTest
{
    public class PlayView : View
    {
        private Media _media;
        private List<Media> _mediaList;

        private TabItem _playTabItem;
        private Viewbox _playViewbox;
        private WPFWindow _fullScreenWindow;
        private showcaseList _showcase = null;

        public PlayView(TabItem playTabItem, Viewbox playViewbox)
        {
            //intializing all the components withing playbox tab
            
            this._playTabItem = playTabItem;
            this._playViewbox = playViewbox;
            this._fullScreenWindow = new WPFWindow(this);
            base.ChangeRequestEvents = new ChangeRequestEvents(this);

            InitializeActions();
        }

        public PlayView()
        {
            base.ChangeRequestEvents = new ChangeRequestEvents(this);
            this._fullScreenWindow = new WPFWindow(this);
            InitializeActions();
        }

        private void InitializeActions()
        {
            this.getWPFWindow.KeyUp += new KeyEventHandler(WindowKeyUp);
            this.getWPFWindow.KeyUp += new KeyEventHandler(KeyUp0_9);
            this.getWPFWindow.KeyUp += new KeyEventHandler(KeyUpA_Z);
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            // handling all the key events 

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                //exiting full screen
                this.getWPFWindow.Hide();
                this.getWPFWindow.Visibility = System.Windows.Visibility.Hidden;
                this.getWPFWindow.setIsOpen = false;
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                window.tabControl.SelectedIndex = 2;
                StopVideo();
                ScreenSaver._screenSaverWindow.isOpen = false;
            }

            if (e.Key == System.Windows.Input.Key.Up)
            {
                PauseVideo();
            }

            if (e.Key == System.Windows.Input.Key.Down)
            {
                PlayVideo();
            }

            if (e.Key == System.Windows.Input.Key.Space)
            {
                //show description
                System.Windows.Controls.Grid myGrid = (Grid)this.getWPFWindow.dynamicViewbox.Tag;
                if (myGrid != null && myGrid.Children.Count > 0)
                {
                    if (myGrid.Children[0] is MediaElement)
                    {
                        MediaElement me = (MediaElement)myGrid.Children[0];
                        me.UnloadedBehavior = MediaState.Manual;
                        me.LoadedBehavior = MediaState.Manual;
                        FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
                        object helperObject = hlp.GetValue(me);
                        FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
                        MediaState state = (MediaState)stateField.GetValue(helperObject);
                        if (state == MediaState.Pause) 
                        {
                            me.Play();
                        }
                        else if (state == MediaState.Play)
                        {
                            me.Pause();
                        }
                    }
                }
                //this.getWPFWindow.SetViewboxContent(this.getMedia);
            }

           
            if (e.Key == System.Windows.Input.Key.Left)
            {
                StopVideo();
                this.PerformPrevButtonClick();
          
            }

            if (e.Key == System.Windows.Input.Key.Right)
            {
                StopVideo();
                this.PerformNextButtonClick();
                
            }
        }

        private void KeyUp0_9(object sender, KeyEventArgs e)
        {
            if (e.Key >= System.Windows.Input.Key.D1 && e.Key <= System.Windows.Input.Key.D9)
            {
                //checking if any key between 0 and 9 is pressed
                StopVideo();
                string folderName = e.Key.ToString().Substring(1, 1);
                this.PerformFolderChange(folderName);
                this.getWPFWindow.SetFolderName(folderName);
                this.getWPFWindow.SetViewboxContent(folderName + " ");
                this.getWPFWindow.KeyUp += new KeyEventHandler(WindowKeyUpAfter0_9);
                this.getWPFWindow.KeyUp -= new KeyEventHandler(KeyUpA_Z);
            }
        }

        private void KeyUpA_Z(object sender, KeyEventArgs e)
        {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z)
            {
                StopVideo();
                //checking if any key between a-z is pressed
                string keyName = e.Key.ToString();
                this.PerformKeyChange(keyName);
                this.getWPFWindow.SetKeyName(keyName);
                this.getWPFWindow.KeyUp += new KeyEventHandler(WindowKeyUpAfterA_Z);
            }
        }

        private void WindowKeyUpAfter0_9(object sender, KeyEventArgs e)
        {
            string keyName = e.Key.ToString();
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z)
            {
                //showing the folder number and name is full screen after a key is pressed
                this.getWPFWindow.SetViewboxContent(this.getWPFWindow.getFolderName() + keyName);
                this.PerformKeyChange(keyName);
                this.getWPFWindow.SetKeyName(keyName);
                this.getWPFWindow.KeyUp -= new KeyEventHandler(WindowKeyUpAfter0_9);
                this.getWPFWindow.KeyUp += new KeyEventHandler(KeyUpA_Z);
                BindMediaList(this.getWPFWindow.getFolderName(), keyName);
            
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                this._showcase = new showcaseList(this.getWPFWindow.getFolderName());
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                this._showcase.Width = screenWidth * 0.95;
                this._showcase.Height = screenHeight * 0.9;
                double windowWidth = this._showcase.Width;
                double windowHeight = this._showcase.Height;
                this._showcase.Left = (screenWidth / 2) - (windowWidth / 2);
                this._showcase.Top = 0;
                this._showcase.WindowStyle = WindowStyle.None;
                this._showcase.Show();
                this._showcase.ShowActivated = true;
                this.getWPFWindow.KeyUp -= new KeyEventHandler(WindowKeyUpAfterA_Z);
                this._showcase.KeyUp += new KeyEventHandler(showcaseKeyUp);
            }
        }

        private void BindMediaList(string folderName, string keyName)
        {
            //Binding media list again when the folder is changed via key press for e.g user presses 2A 
            //then all the images against 2A will be fetched.

            MainWindow window = (MainWindow)Application.Current.MainWindow;
            Label lblMediaFolderId = (Label)window.lblMediaFolderId;
            IEnumerable<Folder> folder = MediaView.GetFolderId(Convert.ToInt32(folderName), keyName);
            RappTestEntities rappEntity = new RappTestEntities();
            foreach (var f in folder)
            {
                lblMediaFolderId.Content = f.MediaFolderId;
            }

            var query = from m in rappEntity.Media
                        where m.MediaFolderId == (Guid)lblMediaFolderId.Content
                        select new Media
                        {
                            MediaId = (Guid)m.MediaId,
                            FileName = m.FileName,
                            Sequence = (Int32)m.Sequence,
                            Title = m.Title,
                            Description = m.Description
                        };

            if (query.Count() > 0)
            {
                this.MediaModelList = new List<Media>(query);
                this.MediaModel = MediaModelList[0];
                _fullScreenWindow.EnterFullScreen(MediaModel);
            }
        }

        private void WindowKeyUpAfterA_Z(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                this.getWPFWindow.KeyUp -= new KeyEventHandler(WindowKeyUpAfterA_Z);
            }
        }

        public int getMediaModelListCount()
        {
            return _mediaList.Count();
        }

        public void PerformPrevButtonClick()
        {
            // TODO
            //ChangeRequestEvents.Fire<ElementModel>(EventProperties.PrevElementModel, elementModel);
            if (_media != null)
            {
                int index = _mediaList.FindIndex(e => e.MediaId == _media.MediaId);
                int prevIndex = index - 1;
                if (prevIndex >= 0)
                {
                    MediaModel = _mediaList[prevIndex];
                }
                else
                {
                    MediaModel = _mediaList.LastOrDefault();
                }
            }
        }

        public void PerformNextButtonClick()
        {
            if (_media != null)
            {
                int index = _mediaList.FindIndex(e => e.MediaId == _media.MediaId);
                int nextIndex = index + 1;
                if (nextIndex < _mediaList.Count)
                {
                    MediaModel = _mediaList[nextIndex];
                }
                else
                {
                    MediaModel = _mediaList.FirstOrDefault();
                }
            }
        }

        private void showcaseKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this._showcase.Hide();
            }
        }

        public void PerformFolderChange(string folderName)
        {
            ChangeRequestEvents.Fire<string>("FolderName", folderName);
        }

        public void PerformKeyChange(string keyName)
        {
            ChangeRequestEvents.Fire<string>("KeyName", keyName);
        }

        public bool IsFullScreen
        {
            get { return _fullScreenWindow.IsOpen; }
        }

        private void PlayVideo()
        {
            System.Windows.Controls.Grid myGrid = (Grid)this.getWPFWindow.dynamicViewbox.Tag;
            if (myGrid.Children.Count > 0)
            {
                if (myGrid.Children[0] is MediaElement)
                {
                    MediaElement me = (MediaElement)myGrid.Children[0];
                    me.UnloadedBehavior = MediaState.Manual;
                    me.LoadedBehavior = MediaState.Manual;
                    me.Play();
                }
            }
        }

        private void PauseVideo()
        {
            System.Windows.Controls.Grid myGrid = (Grid)this.getWPFWindow.dynamicViewbox.Tag;
            if (myGrid.Children.Count > 0)
            {
                if (myGrid.Children[0] is MediaElement)
                {
                    MediaElement me = (MediaElement)myGrid.Children[0];
                    me.UnloadedBehavior = MediaState.Manual;
                    me.LoadedBehavior = MediaState.Manual;
                    me.Pause();
                }
            }       
        }
        
        private void StopVideo()
        {
            System.Windows.Controls.Grid myGrid = (Grid)this.getWPFWindow.dynamicViewbox.Tag;
            if (myGrid != null && myGrid.Children.Count > 0)
            {
                if (myGrid.Children[0] is MediaElement)
                {
                    MediaElement me = (MediaElement)myGrid.Children[0];
                    me.UnloadedBehavior = MediaState.Manual;
                    me.LoadedBehavior = MediaState.Manual;
                    me.Stop();
                }
            }
        }

        public void EnterFullScreen()
        {
            //getting the images and showing as fullscreen
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            Label lblMediaFolderId = (Label)window.lblMediaFolderId;
            Guid mediaFolderId = (Guid)lblMediaFolderId.Content;
            RappTestEntities entity = new RappTestEntities();
            var query = from m in entity.Media
                        where m.MediaFolderId == mediaFolderId
                        select new Media
                        {
                            MediaId = (Guid)m.MediaId,
                            FileName = m.FileName,
                            Sequence = (Int32)m.Sequence,
                            Title = m.Title,
                            Description = m.Description
                        };

            this.MediaModelList = new List<Media>(query);
            if (MediaModelList.Count > 0)
            {
                this.MediaModel = MediaModelList[0];
                _fullScreenWindow.EnterFullScreen(MediaModel);
            }
        }

        public void EnterFullScreen(Media e)
        {
            _fullScreenWindow.EnterFullScreen(e);
        }

        public Media getMedia
        {
            get { return _media; }
        }

        //
        public WPFWindow getWPFWindow
        {
            get { return _fullScreenWindow; }
        }

        public class WPFWindow : Window
        {
            private bool isOpen;
            private PlayView parent;
            public Viewbox dynamicViewbox;

            private string folderName;
            private string keyName;

            private ScaleTransform scale;

            private static int INTERVAL = 2000;

            public WPFWindow(PlayView parent)
            {
                this.parent = parent;

                //SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                SolidColorBrush brush = new SolidColorBrush(Colors.Black);

                //this.AllowsTransparency = true;
                //this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.Background = brush;
                this.Topmost = false;

                this.Width = 300;
                this.Height = 200;

                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;

                this.isOpen = false;

                dynamicViewbox = new Viewbox();
                dynamicViewbox.StretchDirection = StretchDirection.Both;
                dynamicViewbox.Stretch = Stretch.Uniform;

                this.Content = dynamicViewbox;
            }


            public void EnterFullScreen(Media media)
            {
                if (ScreenSaver._screenSaverWindow == null)
                    ScreenSaver.InitializeActions();

                ScreenSaver._screenSaverWindow.isOpen = true;

                MainWindow window = (MainWindow)Application.Current.MainWindow;
                this.Show();
                isOpen = true;
                if (media != null)
                {
                    this.SetViewboxContent(media, 1);
                }
            }

            public bool IsOpen
            {
                get { return isOpen; }
            }

            public bool setIsOpen
            {
                set { isOpen = value; }
            }

            public void SetViewboxContentEmpty()
            {
                this.Dispatcher.Invoke(new Action(() => dynamicViewbox.Child = null));
            }

            public void SetViewboxContent(Media media, int a)
            {
                //styling the controls
                
                System.Windows.Controls.Grid myGrid = new Grid();
                System.Windows.Controls.TextBlock titleTextBlock = new TextBlock();
                titleTextBlock.TextAlignment = TextAlignment.Center;
                titleTextBlock.Foreground = Brushes.White;
                titleTextBlock.FontSize = 16;

                //showing current item's index and total count
                int count = parent.getMediaModelListCount();
                int index = parent._mediaList.FindIndex(e => e.MediaId == media.MediaId) + 1;
                titleTextBlock.Text = index.ToString() + " / " + count.ToString();

                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double Width = screenWidth * 0.3;
                double Height = screenHeight * 0.5;

                //formatting index text block
                Canvas indexCanvas = new Canvas();
                indexCanvas.HorizontalAlignment = HorizontalAlignment.Left;
                indexCanvas.VerticalAlignment = VerticalAlignment.Bottom;
                indexCanvas.Width = Width;
                indexCanvas.Height = 40;
                indexCanvas.Background = Brushes.Black;
                indexCanvas.Opacity = 0.6;

                myGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
                {
                    //setting height and width for the index text block
                    var child = VisualTreeHelper.GetChild(dynamicViewbox, 0) as ContainerVisual;
                    var scale = child.Transform as ScaleTransform;
                    this.scale = scale;

                    indexCanvas.Width = indexCanvas.ActualWidth / scale.ScaleX;
                    indexCanvas.Height = indexCanvas.ActualHeight / scale.ScaleY;

                    titleTextBlock.FontSize = titleTextBlock.FontSize / scale.ScaleX;
                    titleTextBlock.Height = titleTextBlock.ActualHeight / scale.ScaleY;
                    return null;
                }), null);
                if (this.isOpen)
                {
                    indexCanvas.Children.Add(titleTextBlock);
                    myGrid.Children.Add(media.GetUIElement());
                    myGrid.Children.Add(indexCanvas);
                }

                this.dynamicViewbox.Tag = myGrid;
                dynamicViewbox.Child = myGrid;
            }

            public void SetViewboxContent(Media media)
            {
                //styling the controls

                System.Windows.Controls.TextBlock indexTextBlock = new TextBlock();
                indexTextBlock.TextAlignment = TextAlignment.Center;
                indexTextBlock.Foreground = Brushes.White;
                indexTextBlock.FontSize = 16;
                int count = parent.getMediaModelListCount();
                int index = parent._mediaList.FindIndex(e => e.MediaId == media.MediaId) + 1;
                indexTextBlock.Text = index.ToString() + " / " + count.ToString();

                System.Windows.Controls.Grid myGrid = new Grid();
                System.Windows.Controls.TextBlock titleTextBlock = new TextBlock();
                titleTextBlock.FontSize = 36;

                //showing title, description and index and formatting the controls

                titleTextBlock.Foreground = Brushes.White;
                titleTextBlock.Text = string.IsNullOrEmpty(media.Title) ? string.Empty : media.Title;

                System.Windows.Controls.TextBlock descTextBlock = new TextBlock();
                descTextBlock.Foreground = Brushes.Red;
                descTextBlock.FontStyle = FontStyles.Italic;
                descTextBlock.FontSize = 20;
                descTextBlock.TextWrapping = TextWrapping.Wrap;
                descTextBlock.Text = string.IsNullOrEmpty(media.Description) ? string.Empty : media.Description;

                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double Width = screenWidth * 0.3;
                double Height = screenHeight * 0.5;

                Canvas indexCanvas = new Canvas();
                indexCanvas.HorizontalAlignment = HorizontalAlignment.Left;
                indexCanvas.VerticalAlignment = VerticalAlignment.Bottom;
                indexCanvas.Width = Width;
                indexCanvas.Height = 40;
                indexCanvas.Background = Brushes.Black;
                indexCanvas.Opacity = 0.6;

                Canvas titleDescCanvas = new Canvas();
                titleDescCanvas.HorizontalAlignment = HorizontalAlignment.Left;
                titleDescCanvas.VerticalAlignment = VerticalAlignment.Top;
                titleDescCanvas.Width = Width;
                titleDescCanvas.Height = 100;
                titleDescCanvas.Background = Brushes.Black;
                titleDescCanvas.Opacity = 0.3;

                myGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
                {
                    indexCanvas.Width = indexCanvas.ActualWidth / scale.ScaleX;
                    indexCanvas.Height = indexCanvas.ActualHeight / scale.ScaleY;
                    titleTextBlock.Padding = new Thickness(0, 0, 0, 0); //titleTextBlock is title

                    titleDescCanvas.Width = (indexCanvas.ActualWidth * 3);// (scale.ScaleX + scale.ScaleY);
                    titleDescCanvas.Height = indexCanvas.ActualHeight / scale.ScaleY;

                    descTextBlock.Padding = new Thickness(5, indexCanvas.Height, 0, 0); //descTextBlock is description

                    indexTextBlock.FontSize = indexTextBlock.FontSize / scale.ScaleX;// indexTextBlock is index
                    titleTextBlock.FontSize = titleTextBlock.FontSize / scale.ScaleX;
                    descTextBlock.FontSize = descTextBlock.FontSize / scale.ScaleX;
                    titleTextBlock.Height = titleTextBlock.ActualHeight / scale.ScaleY;

                    return null;
                }), null);
                if (this.isOpen)
                {

                    //adding title, descrption and index on the canvas

                    indexCanvas.Children.Add(indexTextBlock);
                    titleDescCanvas.Children.Add(titleTextBlock);
                    titleDescCanvas.Children.Add(descTextBlock);
                    myGrid.Children.Add(media.GetUIElement());
                    myGrid.Children.Add(indexCanvas);
                    myGrid.Children.Add(titleDescCanvas);
                }
                dynamicViewbox.Child = myGrid;
                this.dynamicViewbox.Tag = myGrid;
            }



            public void SetViewboxContent(String s)
            {
                System.Windows.Controls.Grid myGrid = new Grid();
                System.Windows.Controls.TextBlock titleTextBlock = new TextBlock();
                titleTextBlock.Foreground = Brushes.White;
                titleTextBlock.Text = s;
                myGrid.Children.Add(titleTextBlock);
                dynamicViewbox.Child = myGrid;
            }

            public String getFolderName()
            {
                return this.folderName;
            }

            public void SetFolderName(string folderName)
            {
                this.folderName = folderName;
            }

            public String getKeyName()
            {
                return this.keyName;
            }

            public void SetKeyName(string keyName)
            {
                this.keyName = keyName;
            }
        }

        public TabItem PlayTabItem
        {
            get { return _playTabItem; }
        }

        public Viewbox PlayViewbox
        {
            get { return _playViewbox; }
        }

        public Media MediaModel
        {
            get
            {
                return _media;
            }
            set
            {
                _media = value;
                if (_media != null)
                {
                    if (_fullScreenWindow.IsVisible)
                    {
                        _fullScreenWindow.SetViewboxContent(_media, 1);
                    }
                }

            }
        }

        // TODO: Replace with actual caching

        public List<Media> MediaModelList
        {
            get
            {
                return _mediaList;
            }
            set
            {
                if (_mediaList != value)
                {
                    _mediaList = value;

                    if (_mediaList.Count() > 0)
                    {
                        MediaModel = _mediaList[0];
                    }
                }
            }
        }
    }
}
