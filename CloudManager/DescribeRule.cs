using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aliyun.Acs.Slb.Model.V20140515.DescribeRulesResponse;

namespace CloudManager
{
    public class DescribeRule
    {
        public DescribeRule(DescribeRules_Rule r)
        {
            RuleId = r.RuleId;
            RuleName = r.RuleName;
            Domain = r.Domain;
            Url = r.Url;
            VServerGroupId = r.VServerGroupId;
        }

        public SLBListener Listener { get; set; }
        public string RuleId { get; set; }
        public string RuleName { get; set; }
        public string Domain { get; set; }
        public string Url { get; set; }
        public string VServerGroupId { get; set; }
        public string VServerGroupName { get; set; }
    }
}
