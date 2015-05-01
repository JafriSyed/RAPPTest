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

namespace RAPPTest
{
    public class PlayView : View
    {
        private Media elementModel;
        private List<Media> elementModelList;

        private TabItem playTabItem;
        private Viewbox playViewbox;
        private WPFWindow fullScreenWindow;

        public PlayView(TabItem playTabItem, Viewbox playViewbox)
        {
            this.playTabItem = playTabItem;
            this.playViewbox = playViewbox;
            this.fullScreenWindow = new WPFWindow(this);
            base.ChangeRequestEvents = new ChangeRequestEvents(this);

            InitializeActions();
        }

        public PlayView()
        {
            base.ChangeRequestEvents = new ChangeRequestEvents(this);            
            this.fullScreenWindow = new WPFWindow(this);
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
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.getWPFWindow.Hide();
                this.getWPFWindow.Visibility = System.Windows.Visibility.Hidden;
                this.getWPFWindow.setIsOpen = false;
            }

            if (e.Key == System.Windows.Input.Key.Space)
            {
                this.getWPFWindow.SetViewboxContent(this.getElementModel);
            }

            else if (e.Key == System.Windows.Input.Key.Left)
            {
                this.PerformPrevButtonClick();
            }

            else if (e.Key == System.Windows.Input.Key.Right)
            {
                this.PerformNextButtonClick();
            }
        }

        private void showcaseKeyUp(object sender, KeyEventArgs e)
        {

        }

        private void KeyUp0_9(object sender, KeyEventArgs e)
        {
            if (e.Key >= System.Windows.Input.Key.D0 && e.Key <= System.Windows.Input.Key.D9)
            {
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
                string keyName = e.Key.ToString();
                this.PerformKeyChange(keyName);
                this.getWPFWindow.SetKeyName(keyName);
                this.getWPFWindow.KeyUp += new KeyEventHandler(WindowKeyUpAfterA_Z);
            }
        }

        private void WindowKeyUpAfter0_9(object sender, KeyEventArgs e)
        {
            if (e.Key >= System.Windows.Input.Key.A && e.Key <= System.Windows.Input.Key.Z)
            {
                string keyName = e.Key.ToString();
                this.getWPFWindow.SetViewboxContent(this.getWPFWindow.getFolderName() + keyName);
                this.PerformKeyChange(keyName);
                this.getWPFWindow.SetKeyName(keyName);
                this.getWPFWindow.KeyUp -= new KeyEventHandler(WindowKeyUpAfter0_9);
                this.getWPFWindow.KeyUp += new KeyEventHandler(KeyUpA_Z);
            }
            else if (e.Key == System.Windows.Input.Key.Enter)
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                this.getWPFWindow.KeyUp -= new KeyEventHandler(WindowKeyUpAfterA_Z);
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

        public int getElementModelListCount()
        {
            return elementModelList.Count();
        }

        public void PerformPrevButtonClick()
        {
            // TODO
            //ChangeRequestEvents.Fire<ElementModel>(EventProperties.PrevElementModel, elementModel);
            if (elementModel != null)
            {
                int index = elementModelList.FindIndex(e => e.MediaId == elementModel.MediaId);
                int prevIndex = index - 1;
                if (prevIndex >= 0)
                {
                    ElementModel = elementModelList[prevIndex];
                }
                else
                {
                    ElementModel = elementModelList.LastOrDefault();
                }
            }
        }

        public void PerformNextButtonClick()
        {
            // TODO
            //ChangeRequestEvents.Fire<ElementModel>(EventProperties.NextElementModel, elementModel);

            if (elementModel != null)
            {
                int index = elementModelList.FindIndex(e => e.MediaId == elementModel.MediaId);
                int nextIndex = index + 1;
                if (nextIndex < elementModelList.Count)
                //this.Dispatcher.Invoke(new Action(() => dynamicViewbox.Child = uiElement));
                {
                    ElementModel = elementModelList[nextIndex];
                }
                else
                {
                    ElementModel = elementModelList.FirstOrDefault();
                }
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
            get { return fullScreenWindow.IsOpen; }
        }

        
        public void EnterFullScreen()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            //Label lblMediaFolderId = (Label)window.lblMediaFolderId.Content;
            RappTestEntities entity = new RappTestEntities();
            var query = from m in entity.Media
                        select new Media
                        {
                            MediaId = (Guid)m.MediaId,
                            FileName = m.FileName,
                            Sequence = (Int32)m.Sequence,
                            Title = m.Title,
                            Description = m.Description
                        };

            this.ElementModelList = new List<Media>(query);
            this.ElementModel = ElementModelList[0];
            fullScreenWindow.EnterFullScreen(ElementModel);
        }

        public void EnterFullScreen(Media e)
        {
            fullScreenWindow.EnterFullScreen(e);
        }

        public Media getElementModel
        {
            get { return elementModel; }
        }

        //
        public WPFWindow getWPFWindow
        {
            get { return fullScreenWindow; }
        }

        public class WPFWindow : Window
        {
            private bool isOpen;
            private PlayView parent;
            private Viewbox dynamicViewbox;

            private string folderName;
            private string keyName;

            private bool isAnimated;

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
                //this.KeyUp += new KeyEventHandler(WindowKeyUp);
                this.MouseMove += new MouseEventHandler(WindowMouseMove);

                dynamicViewbox = new Viewbox();
                dynamicViewbox.StretchDirection = StretchDirection.Both;
                dynamicViewbox.Stretch = Stretch.Uniform;

                this.Content = dynamicViewbox;

                this.isAnimated = false;
            }

            private void WindowMouseMove(Object sender, MouseEventArgs e)
            {
                if (isAnimated)
                {
                    isAnimated = false;

                    // this.Dispatcher.Invoke(new Action(() => this.Hide()));
                    this.Hide();
                }
            }

            public void EnterFullScreenNormal(Media elementModel, bool animate)
            {
                this.Show();
                isOpen = true;
                if (elementModel != null)
                {
                    this.SetViewboxContent(elementModel, 1);
                }

                if (animate)
                {
                    isAnimated = true;
                    while (isAnimated)
                    {
                        parent.PerformNextButtonClick();
                        Thread.Sleep(INTERVAL);
                    }
                }
            }

            public void EnterFullScreen(Media elementModel)
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                EnterFullScreenNormal(elementModel, false);
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

            public void SetViewboxContent(Media elementModel, int a)
            {
                System.Windows.Controls.Grid myGrid = new Grid();
                System.Windows.Controls.TextBlock titleTextBlock = new TextBlock();
                titleTextBlock.TextAlignment = TextAlignment.Center;
                titleTextBlock.Foreground = Brushes.White;
                titleTextBlock.FontSize = 16;
                int count = parent.getElementModelListCount();
                int index = parent.elementModelList.FindIndex(e => e.MediaId == elementModel.MediaId) + 1;
                titleTextBlock.Text = index.ToString() + " / " + count.ToString();

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

                myGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(delegate(Object state)
                {
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
                    myGrid.Children.Add(elementModel.GetUIElement());
                    myGrid.Children.Add(indexCanvas);
                }

                dynamicViewbox.Child = myGrid;
            }

            public void SetViewboxContent(Media elementModel)
            {
                System.Windows.Controls.TextBlock indexTextBlock = new TextBlock();
                indexTextBlock.TextAlignment = TextAlignment.Center;
                indexTextBlock.Foreground = Brushes.White;
                indexTextBlock.FontSize = 16;
                int count = parent.getElementModelListCount();
                int index = parent.elementModelList.FindIndex(e => e.MediaId == elementModel.MediaId) + 1;
                indexTextBlock.Text = index.ToString() + " / " + count.ToString();

                System.Windows.Controls.Grid myGrid = new Grid();
                System.Windows.Controls.TextBlock titleTextBlock = new TextBlock();
                titleTextBlock.FontSize = 36;
                titleTextBlock.Foreground = Brushes.White;
                titleTextBlock.Text = string.IsNullOrEmpty(elementModel.Title) ? string.Empty : elementModel.Title;

                System.Windows.Controls.TextBlock descTextBlock = new TextBlock();
                descTextBlock.Foreground = Brushes.Red;
                descTextBlock.FontStyle = FontStyles.Italic;
                descTextBlock.FontSize = 20;
                descTextBlock.TextWrapping = TextWrapping.Wrap;
                descTextBlock.Text = string.IsNullOrEmpty(elementModel.Description) ? string.Empty : elementModel.Description;

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

                    titleDescCanvas.Width = indexCanvas.ActualWidth / scale.ScaleX;
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
                    indexCanvas.Children.Add(indexTextBlock);
                    titleDescCanvas.Children.Add(titleTextBlock);
                    titleDescCanvas.Children.Add(descTextBlock);
                    myGrid.Children.Add(elementModel.GetUIElement());
                    myGrid.Children.Add(indexCanvas);
                    myGrid.Children.Add(titleDescCanvas);
                }
                dynamicViewbox.Child = myGrid;
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
            get { return playTabItem; }
        }

        public Viewbox PlayViewbox
        {
            get { return playViewbox; }
        }

        public Media ElementModel
        {
            get
            {
                return elementModel;
            }
            set
            {
                elementModel = value;
                if (elementModel != null)
                {
                    if (fullScreenWindow.IsVisible)
                    {
                        fullScreenWindow.SetViewboxContent(elementModel, 1);
                    }
                }

            }
        }

        // TODO: Replace with actual caching

        public List<Media> ElementModelList
        {
            get
            {
                return elementModelList;
            }
            set
            {
                if (elementModelList != value)
                {
                    elementModelList = value;

                    if (elementModelList.Count() > 0)
                    {
                        ElementModel = elementModelList[0];
                    }
                }
            }
        }
    }
}
