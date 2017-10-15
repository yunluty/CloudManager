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
    /// EditBackupPolicy.xaml 的交互逻辑
    /// </summary>
    public partial class EditBackupPolicyWindow : Window
    {
        DBBackupPolicy mPolicy;

        public EditBackupPolicyWindow(DBBackupPolicy policy)
        {
            InitializeComponent();
            mPolicy = new DBBackupPolicy(policy);
            this.DataContext = mPolicy;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
