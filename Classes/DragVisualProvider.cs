using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.DragDrop.Behaviors;
using Telerik.Windows.DragDrop;
using Telerik.Windows.Controls;
using System.Linq;

namespace RAPPTest
{
    public class DragVisualProvider : DependencyObject, IDragVisualProvider
    {
        public DataTemplate DraggedItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DraggedItemTemplateProperty);
            }
            set
            {
                SetValue(DraggedItemTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty DraggedItemTemplateProperty =
        DependencyProperty.Register("DraggedItemTemplate", typeof(DataTemplate), typeof(DragVisualProvider), new PropertyMetadata(null));

        public FrameworkElement CreateDragVisual(DragVisualProviderState state)
        {
            var visual = new DragVisual();

            var theme = StyleManager.GetTheme(state.Host);
            if (theme != null)
            {
                StyleManager.SetTheme(visual, theme);
            }

            visual.Content = state.DraggedItems.OfType<object>().FirstOrDefault();
            visual.ContentTemplate = this.DraggedItemTemplate;

            return visual;
        }

        public Point GetDragVisualOffset(DragVisualProviderState state)
        {
            return state.RelativeStartPoint;
        }

        public bool UseDefaultCursors { get; set; }
    }
}
