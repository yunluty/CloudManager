using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CloudManager
{
    public class WindowBase : Window
    {
        private Button MaxButton;
        private Button NormalButton;
        private Rect Rectnormal;
        private Visibility MaxVisibilityValue;

        public WindowBase()
        {
            this.Style = (Style)App.Current.Resources["WindowBaseStyle"];
            this.Loaded += delegate
            {
                InitializeEvent();
            };
        }

        private void InitializeEvent()
        {
            ControlTemplate template = (ControlTemplate)App.Current.Resources["WindowBaseControlTemplate"];

            Button min = (Button)template.FindName("btnMin", this);
            min.Click += delegate
            {
                this.WindowState = WindowState.Minimized;
            };

            MaxButton = (Button)template.FindName("btnMax", this);
            MaxButton.Visibility = this.MaxVisibilityValue;
            MaxButton.Click += delegate
            {
                //this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                MaxButton.Visibility = Visibility.Collapsed;
                NormalButton.Visibility = Visibility.Visible;
                Rectnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
                this.Left = 0;//设置位置
                this.Top = 0;
                Rect rc = SystemParameters.WorkArea;//获取工作区大小
                this.Width = rc.Width;
                this.Height = rc.Height;
            };

            NormalButton = (Button)template.FindName("btnNormal", this);
            NormalButton.Click += delegate
            {
                //this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                MaxButton.Visibility = Visibility.Visible;
                NormalButton.Visibility = Visibility.Collapsed;
                this.Left = Rectnormal.Left;
                this.Top = Rectnormal.Top;
                this.Width = Rectnormal.Width;
                this.Height = Rectnormal.Height;
                
            };

            Button close = (Button)template.FindName("btnClose", this);
            close.Click += delegate
            {
                this.Close();
            };

            Border title = (Border)template.FindName("borderTitle", this);
            title.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
            title.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
            {
                if (e.ClickCount >= 2)
                {
                    MaxButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            };
        }

        public static readonly DependencyProperty MaxVisibilityProperty = DependencyProperty.Register(
            "MaxVisibility", typeof(Visibility), typeof(WindowBase),
            new PropertyMetadata(new PropertyChangedCallback(OnMaxVisibilityChanged)));

        public Visibility MaxVisibility
        {
            get { return (Visibility)GetValue(MaxVisibilityProperty); }
            set { SetValue(MaxVisibilityProperty, value); }
        }

        private static void OnMaxVisibilityChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            WindowBase owner = sender as WindowBase;
            if (owner != null)
            {
                owner.MaxVisibilityValue = (Visibility)args.NewValue;
            }
        }
    }
}
