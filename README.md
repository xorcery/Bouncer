Bouncer
=======

Bouncer is a simple .NET HttpModule that leverages an IP based ACL to allow admins into a site while unauthorized users will get a static page.

### Config ###

#### Modify your web.config ####

    /* point to your static file */
    <add key="bouncer:offlineFilePath" value="~/offline.html"/>
    
    /* put in an CSV formatted ACL; can be single IP or range or both */
    <add key="bouncer:ipAcl" value="192.168.0.1,10.0.0.0/8"/>
    
    /* Exclude some directories from having the notification banner show */
    <add key="bouncer:ignoreBannerPaths" value="/umbraco,/App_Plugins"/>
    
    /* configure file extensions to ignore; good for having images/css on your offline.html */
    <add key="bouncer:excludedExtensions" value="jpg,png,gif,css,js"/>
    
    /* (optional) for debugging you can force the system to impersonate a non-admin; useful if you're on localhost and need to see what the user will see */
    <add key="bouncer:impersonateNonAdmin" value="true"/>
    
### Create an offline static html document ###

To configure what the non-admin visitors will see, create a file in your website root named `offline.html`.  Fill it will all the goodness you want.

Simply rename the `~/offline.html` manually/programmatically to bring the site back online
