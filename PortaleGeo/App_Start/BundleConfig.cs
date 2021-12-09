using System.Web;
using System.Web.Optimization;

namespace PortaleGeoWeb
{
    public class BundleConfig
    {
        // Per altre informazioni sulla creazione di bundle, vedere https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilizzare la versione di sviluppo di Modernizr per eseguire attività di sviluppo e formazione. Successivamente, quando si è
            // pronti per passare alla produzione, usare lo strumento di compilazione disponibile all'indirizzo https://modernizr.com per selezionare solo i test necessari.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include
                ("~/Scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include
            ("~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/fontawesome.css",
                "~/Content/fontawesome.min.css",
                "~/Content/bootstrap.css",
                "~/Content/fontawesome-all.css",
                "~/Content/fontawesome-all.min.css",
                "~/Content/bootstrap.css",
                "~/Content/site.css",
                 "~/Content/bootstrap.min.css"

                ));
        }
    }
}
