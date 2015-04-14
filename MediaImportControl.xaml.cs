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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;

using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace RAPPTest
{
    /// <summary>
    /// This class defines the Drag and Drop panel control that will hold the media items with drag and drop
    /// functionality.
    /// </summary>
    public partial class MediaImportControl : UserControl
    {
       
        public MediaImportControl()
        {
            InitializeComponent();

        }

        

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }


        private void OnDropQuery(object sender, Telerik.Windows.Controls.DragDrop.DragDropQueryEventArgs e)
        {
            e.QueryResult = e.Options.DropDataObject != null && e.Options.DropDataObject.ContainsFileDropList();
        }

        private void OnDropInfo(object sender, Telerik.Windows.Controls.DragDrop.DragDropEventArgs e)
        {

        }

        private void theGrid_DragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void theGrid_DragOver(object sender, DragEventArgs e)
        {
            Grid grid = sender as Grid;
            MediaItemControl mediaItemUC = new MediaItemControl();
            if (grid != null)
            {
                if (e.Data.GetDataPresent(mediaItemUC.GetType()))
                {
                    mediaItemUC = e.Data.GetData(mediaItemUC.GetType()) as MediaItemControl;
                }

                //if (draggedItemAdornerLayer != null)
                //{
                //    Point p = e.GetPosition(this.dndSrollbar);
                //    draggedItemAdorner.UpdatePosition(p.X, p.Y);
                //    if ((p.Y + draggedItemAdorner.ActualHeight) >= this.dndSrollbar.ActualHeight)
                //    {
                //        this.dndSrollbar.ScrollToVerticalOffset(this.dndSrollbar.VerticalOffset + 5);
                //    }
                //    if (p.Y <= 10)
                //    {
                //        this.dndSrollbar.ScrollToVerticalOffset(this.dndSrollbar.VerticalOffset - 5);
                //    }
                //}
            }

        }

        /// <summary>
        /// Handler to capture drag enter event on this panel which is called when a mediaitem is started to drag on this panel.
        /// </summary>
        private void theGrid_DragEnter(object sender, DragEventArgs e)
        {
           
        }

        /// <summary>
        /// Handler for drop event on this panel which is called when a mediaitem is dragged and dropped on this panel.
        /// </summary>
        private void theGrid_Drop(object sender, DragEventArgs e)
        {
           
            
        }
    }
}
