Bouncer
=======

Bouncer is a simple .NET HttpModule that leverages for an IP based ACL to allow admins into a site while unauthorized users will get a static page.

### Config ###

#### Modify your web.config ####

    /* point to your static file */
    <add key="bouncer:offlineFilePath" value="~/offline.html"/>
    /* put in an CSV formatted ACL; can be single IP or range or both */
    <add key="bouncer:ipAcl" value="192.168.0.1,10.0.0.0/8"/>
    /* Exclude some directories from having the notification banner show */
    <add key="bouncer:ignoreBannerPaths" value="/umbraco,/App_Plugins"/>

Simply rename the `~/offline.html` manually/programmatically to bring the site back online
