using CloudManager.Control;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace CloudManager
{
    public abstract class PageBase : Page
    {
        private LoadingAdorner PageLoading;
        private Grid BlankPage;
        private bool Initialization;

        protected bool HideBlankPage;
        protected bool Refreshed;
        

        public PageBase()
        {
            this.Style = (Style)App.Current.Resources["PageBaseStyle"];
            this.Loaded += delegate
            {
                if (!Initialization)
                {
                    Initialization = true;

                    var root = (Grid)this.Template.FindName("RootGrid", this);
                    var layer = AdornerLayer.GetAdornerLayer(root);
                    PageLoading = new LoadingAdorner(root) { Visibility = Visibility.Collapsed };
                    layer.Add(PageLoading);
                    //content.Effect = new BlurEffect { Radius = 2 };

                    BlankPage = (Grid)this.Template.FindName("BlankGrid", this);
                    if (HideBlankPage)
                    {
                        BlankPage.Visibility = Visibility.Hidden;
                    }

                    Button fresh = (Button)this.Template.FindName("Refresh", this);
                    fresh.Click += delegate
                    {
                        RefreshPage();
                    };
                    Button link = (Button)this.Template.FindName("AliyunLink", this);
                    link.Click += delegate
                    {
                        System.Diagnostics.Process.Start("https://www.aliyun.com/");
                    };
                }
            };
        }

        protected abstract void RefreshPage();

        public void DoLoadingWork(Action<PageBase> doWhat, Action<Exception> doError)
        {
            PageLoading?.SetValue(VisibilityProperty, Visibility.Visible);
            Task.Run(() =>
            {
                try
                {
                    doWhat?.Invoke(this);
                }
                catch (Exception ex)
                {
                    try { doError?.Invoke(ex); } catch { }
                }
                Dispatcher.Invoke(() =>
                {
                    PageLoading?.SetValue(VisibilityProperty, Visibility.Hidden);
                });
            });
        }

        public void DoLoadingWork(Action<PageBase> doWhat, Action<PageBase> doNext, Action<Exception> doError)
        {
            PageLoading?.SetValue(VisibilityProperty, Visibility.Visible);
            Task.Run(() =>
            {
                try
                {
                    doWhat?.Invoke(this);
                }
                catch (Exception ex)
                {
                    try { doError?.Invoke(ex); } catch { }
                }
                Dispatcher.Invoke(() =>
                {
                    PageLoading?.SetValue(VisibilityProperty, Visibility.Hidden);
                });

                doNext?.Invoke(this);
            });
        }

        protected void ProcessGotResults<T>(ObservableCollection<T> results) where T : INotifyPropertyChanged
        {
            if (BlankPage == null || HideBlankPage)
            {
                return;
            }

            if (results.Count > 0)
            {
                if (BlankPage.Visibility != Visibility.Hidden)
                {
                    BlankPage.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (BlankPage.Visibility != Visibility.Visible)
                {
                    BlankPage.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
