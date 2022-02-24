using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NuovoPortaleGeo.Models;


namespace NuovoPortaleGeo
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //La riga successiva attiva o meno lo schedulatore -> non pi� usato!
            /*Models.ExecuteTaskServiceCallScheduler.StartAsync().GetAwaiter().GetResult();*/

                /*
                 * To resolve this follow the below steps:

                Open IIS server
                Go to Application Pools
                Select the Application Pool of your app
                Click on Advanced Settings on the right panel.
                Inside the Advance Settings pop up set "Idle Time-out (minutes)" to 0.
                */

        }
    }
}
