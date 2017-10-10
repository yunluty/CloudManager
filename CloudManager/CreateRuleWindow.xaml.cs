using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Slb.Model.V20140515;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
        private SLBListener mListener;
        private DefaultAcsClient mClient;
        private ObservableCollection<DescribeRule> mRules;
        private DescribeRule mConfigureRule;

        public ObservableCollection<ServerGroup> mVServerGroups { get; set; }
        public bool AddNew { get; set; }

        public CreateRuleWindow()
        {
            InitializeComponent();
        }

        public CreateRuleWindow(string aki, string aks, SLBListener l)
        {
            InitializeComponent();
            mListener = l;
            IClientProfile profile = DefaultProfile.GetProfile(mListener.RegionId, aki, aks);
            mClient = new DefaultAcsClient(profile);
            mRules = new ObservableCollection<DescribeRule>();
            Rules.ItemsSource = mRules;
            AddNew = true;
        }

        public CreateRuleWindow(string aki, string aks, SLBListener l, DescribeRule r)
        {
            InitializeComponent();
            mListener = l;
            mConfigureRule = r;
            IClientProfile profile = DefaultProfile.GetProfile(mListener.RegionId, aki, aks);
            mClient = new DefaultAcsClient(profile);
            mRules = new ObservableCollection<DescribeRule>();
            Rules.ItemsSource = mRules;
            AddNew = false;
        }

        private DescribeRule AddBlankRule()
        {
            //Add new blank rule
            DescribeRule rule = new DescribeRule();
            rule.VServerGroups = mVServerGroups;
            rule.AddNew = true;
            //Select default V server group
            rule.VServerGroupId = mVServerGroups[0].ServerGroupId;
            mRules.Add(rule);
            return rule;
        }

        private void AddRule_Click(object sender, RoutedEventArgs e)
        {
            AddBlankRule();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DescribeRule rule = (sender as Button).DataContext as DescribeRule;
            mRules.Remove(rule);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AddNew)
            {
                //Add first blank rule to edit
                AddBlankRule();
            }
            else if (mConfigureRule != null)
            {
                //Configure
                RulesView.Columns.Remove(Process);
                mConfigureRule.VServerGroups = mVServerGroups;
                mRules.Add(mConfigureRule);
            }
            
            this.DataContext = this;
        }

        private void CreatedRules()
        {
            this.Close();
        }

        private void CreateRules(object obj)
        {
            string rules = obj as string;
            CreateRulesRequest request = new CreateRulesRequest();
            request.LoadBalancerId = mListener.LoadBalancerId;
            request.ListenerPort = mListener.ListenerPort;
            request.RuleList = rules;
            try
            {
                CreateRulesResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(CreatedRules));
            }
            catch
            {
            }
        }

        private void DoneSetRule()
        {
            this.Close();
        }

        private void SetRule()
        {
            SetRuleRequest request = new SetRuleRequest();
            request.RuleId = mConfigureRule.RuleId;
            request.VServerGroupId = mConfigureRule.VServerGroupId;
            try
            {
                SetRuleResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(DoneSetRule));
            }
            catch
            {
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (AddNew)
            {
                string rules = "[";
                for (int i = 0; i < mRules.Count; i++)
                {
                    if (i != 0)
                    {
                        rules += ",";
                    }

                    DescribeRule rule = mRules[i];
                    rules += "{\"RuleName\":\"" + rule.RuleName +
                        "\",\"Domain\":\"" + rule.Domain +
                        "\",\"Url\":\"" + rule.Url +
                        "\",\"VServerGroupId\":\"" + rule.VServerGroupId +
                        "\"}";
                }
                rules += "]";
                Thread t = new Thread(new ParameterizedThreadStart(CreateRules));
                t.Start(rules);
            }
            else
            {
                Thread t = new Thread(SetRule);
                t.Start();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
