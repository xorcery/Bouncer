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
        private static string excludedExtensions = System.Web.Configuration.WebConfigurationManager.AppSettings["bouncer:excludedExtensions"];
        private static bool impersonateNonAdmin = System.Web.Configuration.WebConfigurationManager.AppSettings["bouncer:impersonateNonAdmin"] != null ? Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["bouncer:impersonateNonAdmin"]) : false;

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
            context.ResolveRequestCache += new EventHandler(Start);
            context.PreSendRequestContent += new EventHandler(Finish);
        }

        void Start(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;
            var path = context.Request.Url.AbsolutePath;
            var extension = (path.Contains('.')) ? Path.GetExtension(path).Substring(1) : Path.GetExtension(path);

            /*
             * Checking for excluded file extensions b/c it appears something is interfering with the mime/types
             * 
             * This allows the offline.html to have images/css/js come thru
             */

            if (!String.IsNullOrEmpty(extension) && !String.IsNullOrEmpty(excludedExtensions) && excludedExtensions.Split(',').Contains(extension))
                return;

            if (context.Response.ContentType == bannerMimeType)
            {
                //test for file presense which means the site is offline
                if (File.Exists(context.Server.MapPath(offlineFilePath)))
                {
                    var userIp = Utility.GetUserIp(application);

                    //whitelist check
                    if (!Utility.IsUserIpWhiteListed(userIp, ipAcl) || impersonateNonAdmin)
                    {
                        //non-admin
                        context.RewritePath(offlineFilePath);
                    }
                }
            }
        }

        void Finish(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;

            if (context.Response.ContentType != bannerMimeType)
                return;

            if (File.Exists(context.Server.MapPath(offlineFilePath)))
            {
                var userIp = Utility.GetUserIp(application);

                //whitelist check
                if (Utility.IsUserIpWhiteListed(userIp, ipAcl) && !impersonateNonAdmin)
                {
                    var excludedBannerPaths = ignoreBannerPaths.ToLower().Split(',');

                    //test for html/text and add message if not excluded
                    if (!excludedBannerPaths.Any(x => context.Request.Path.ToLower().StartsWith(x)))
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
