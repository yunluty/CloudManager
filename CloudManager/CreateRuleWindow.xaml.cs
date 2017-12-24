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
    public partial class CreateRuleWindow : WindowBase
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

        public CreateRuleWindow(SLBListener l)
        {
            InitializeComponent();
            mListener = l;
            IClientProfile profile = DefaultProfile.GetProfile(mListener.RegionId, App.AKI, App.AKS);
            mClient = new DefaultAcsClient(profile);
            mRules = new ObservableCollection<DescribeRule>();
            Rules.ItemsSource = mRules;
            AddNew = true;
        }

        public CreateRuleWindow(SLBListener l, DescribeRule r)
        {
            InitializeComponent();
            this.Title = "编辑转发策略";
            mListener = l;
            mConfigureRule = r;
            IClientProfile profile = DefaultProfile.GetProfile(mListener.RegionId, App.AKI, App.AKS);
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

        private void CreateRules(string rules)
        {
            DoLoadingWork(win =>
            {
                CreateRulesRequest request = new CreateRulesRequest();
                request.LoadBalancerId = mListener.LoadBalancerId;
                request.ListenerPort = mListener.ListenerPort;
                request.RuleList = rules;
                CreateRulesResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(CreatedRules));
            },
            ex => {
                //TODO:
            });
        }

        private void DoneSetRule()
        {
            this.Close();
        }

        private void SetRule()
        {
            DoLoadingWork(win =>
            {
                SetRuleRequest request = new SetRuleRequest();
                request.RuleId = mConfigureRule.RuleId;
                request.VServerGroupId = mConfigureRule.VServerGroupId;
                SetRuleResponse response = mClient.GetAcsResponse(request);
                Dispatcher.Invoke(new Action(DoneSetRule));
            },
            ex =>
            {
                //TODO:
            });
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
                CreateRules(rules);
            }
            else
            {
                SetRule();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
