using System.Web;
using System.Web.Optimization;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Resolvers;

namespace WebTest
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            BundleResolver.Current = new CustomBundleResolver();

            bundles.Add(new CustomScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new CustomScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new CustomScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new CustomScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new CustomScriptBundle("~/bundles/es6").Include(
                "~/Scripts/test_script.es6"
            ));

            bundles.Add(new CustomStyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css"
            ));

            bundles.Add(new CustomStyleBundle("~/Content/Lokalni").Include(
                "~/Content/MySite.less",
                "~/Content/Site.less"
            ));
        }
    }
}
