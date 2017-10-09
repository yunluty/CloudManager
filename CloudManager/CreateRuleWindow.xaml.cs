using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// CreateRuleWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CreateRuleWindow : Window
    {
        private string mAki, mAks;
        private ObservableCollection<DescribeRule> mRules;

        public CreateRuleWindow()
        {
            InitializeComponent();
        }

        public CreateRuleWindow(string aki, string aks)
        {
            InitializeComponent();
            mAki = aki;
            mAks = aks;
            mRules = new ObservableCollection<DescribeRule>();
            // Add first blank rule to edit
            Rules.ItemsSource = mRules;
            mRules.Add(new DescribeRule());
        }

        private SLBListener _mListener;
        public SLBListener mListener
        {
            get { return _mListener; }
            set
            {
                _mListener = value;
            }
        }

        private void AddRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
