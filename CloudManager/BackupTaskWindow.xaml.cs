using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CloudManager
{
    /// <summary>
    /// BackupTaskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BackupTaskWindow : WindowBase
    {
        private BackupTask mBackupTask;

        public BackupTaskWindow()
        {
            InitializeComponent();
            mBackupTask = new BackupTask();
            this.DataContext = mBackupTask;
        }

        private void Browser_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
