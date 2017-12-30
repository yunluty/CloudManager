using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace CloudManager.Control
{
    /// <summary>
    /// LoaderPanel.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingPanel : Grid
    {
        public LoadingPanel()
        {
            InitializeComponent();
            //LoaderColor = 0XFF00C1DD;
            //this._cImage.MouseLeftButtonDown += (sender, args) => { this.Visibility = Visibility.Hidden; };
        }

        /*public uint LoaderColor
        {
            set
            {
                _cImage.Source = Properties.Resources.loading.GetColorBitmapSource(value);
                _cText.Foreground = value.ToArgbColor.ToColorBrush();
            }
        }*/

        public LoadingPanel(UIElement baseUi):this()
        {
            if (baseUi != null)
            {
                _visualBrush.Visual = baseUi;//TODO:极大影响性能
            }
            _cRectFullPanel.Effect = new BlurEffect { Radius = 2 };
        }

        public Visual Visual
        {
            get { return _visualBrush.Visual; }
            set { _visualBrush.Visual = value; }
        }

        public string Text
        {
            get { return _cText.Text; }
            set
            {
                if (value.Length > 0)
                {
                    _cText.Text = value + " . . .";
                }
                else
                {
                    _cText.Text = "";
                }
            }
        }
    }
}
