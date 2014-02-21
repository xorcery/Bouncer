using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using LukeSkywalker.IPNetwork;

namespace Bouncer
{
    public static class Utility
    {
        public static bool IsUserIpWhiteListed(string userIp, string acl)
        {
            var aclList = acl.Split(',').ToList();

            //try for a exact IP match
            if(aclList.Contains(userIp) || userIp == "127.0.0.1"){
                return true;
            }

            //try for a network match by CIDR notation; i.e. 10.0.0.0/8
            var networkList = aclList.Where(x => x.Contains('/'));

            return networkList.Any(x => IPNetwork.Contains(IPNetwork.Parse(x), IPAddress.Parse(userIp)));
        }

        public static string GetUserIp(HttpApplication application)
        {
            var ipList = application.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return application.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
