Bouncer
=======

Bouncer is a simple .NET HttpModule that leverages for an IP based ACL to allow admins into a site while unauthorized users will get a static page.

## web.config ##
    /* point to your static file */
    <add key="bouncer:offlineFilePath" value="~/offline.html"/>
    /* put in an CSV formatted ACL
    
     can be single IP or range or both
     i.e. 192.168.0.1,10.0.0.0/8
    
    */
    <add key="bouncer:ipAcl" value=""/>
    //Exclude some directories from having the notification banner show
    <add key="bouncer:ignoreBannerPaths" value="/umbraco,/App_Plugins"/>
