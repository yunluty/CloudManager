﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace CloudManager.Control
{
    /// <summary>
    /// LoaderPanel.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingPanel : Grid
    {
        //public Rectangle BackImage => _cRectFullPanel;
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

        /*public LoaderPanel00(UIElement baseUi):this()
        {
            if(baseUi != null) _visualBrush.Visual = baseUi;//TODO:极大影响性能
            BackImage.Effect = new BlurEffect { Radius = 2 };
        }*/

        /*public string Text
        {
            get { return _cText.Text; }
            set { _cText.Text = value; }
        }*/
    }
}
