using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Configuration;

namespace Bouncer
{
    public class BouncerModule : IHttpModule
    {
        private static string offlineFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["bouncer:offlineFilePath"];
        private static string ipAcl = System.Web.Configuration.WebConfigurationManager.AppSettings["bouncer:ipAcl"];
        private static string ignoreBannerPaths = System.Web.Configuration.WebConfigurationManager.AppSettings["bouncer:ignoreBannerPaths"];

        private const string defaultOfflineBanner = @"
                <script>
                    function closeBouncerBanner(element){
                        element.outerHTML = '';
                        delete element;
                    }
                </script>
                <div onclick='closeBouncerBanner(this)' style='position: fixed; margin-left: 30%; top: 0; width: 30%; color: #000; overflow: hidden; border: 1px solid #000; padding: 10px; background-color: #fff; opacity: 0.9;'>
                    <span>The website is in offline mode.</span>
                </div>
            ";
        private const string bouncerWatcherAlias = "Bouncer_Watcher";
        private const string bannerMimeType = "text/html";

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Start);
            context.PreSendRequestContent += new EventHandler(Finish);
        }

        void Start(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            //test for file presense which means the site is offline
            if (File.Exists(context.Server.MapPath(offlineFilePath)))
            {
                var userIp = Utility.GetUserIp(application);

                //whitelist check
                if (!Utility.IsUserIpWhiteListed(userIp, ipAcl))
                {
                    //non-admin
                    context.RewritePath(offlineFilePath);
                }
            }
        }

        void Finish(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            if (File.Exists(context.Server.MapPath(offlineFilePath)))
            {
                var userIp = Utility.GetUserIp(application);

                //whitelist check
                if (Utility.IsUserIpWhiteListed(userIp, ipAcl))
                {
                    var excludedBannerPaths = ignoreBannerPaths.ToLower().Split(',');

                    //test for html/text and add message if not excluded
                    if (context.Response.ContentType == bannerMimeType && !excludedBannerPaths.Any(x => context.Request.Path.ToLower().StartsWith(x)))
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine(defaultOfflineBanner);
                        context.Response.Write(sb.ToString());
                    }
                }
            }
        }

        public void Dispose() { }
    }
}
