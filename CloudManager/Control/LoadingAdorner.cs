using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CloudManager.Control
{
    public class LoadingAdorner : Adorner
    {
        private VisualCollection VisualChildren;
        private LoadingPanel LoadingPanel;

        public LoadingAdorner(UIElement adornedElement) : base(adornedElement)
        {
            LoadingPanel = new LoadingPanel(adornedElement);
            //LoadingPanel = new LoadingPanel();
            VisualChildren = new VisualCollection(this);
            VisualChildren.Add(LoadingPanel);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return VisualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return VisualChildren[index];
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // where to position the customControl...this is relative to the element you are adorning  
            double x = 0;
            double y = 0;
            LoadingPanel.Arrange(new Rect(x, y, finalSize.Width, finalSize.Height)); // you need to arrange  

            // Return the final size.  
            return finalSize;
        }
    }
}
