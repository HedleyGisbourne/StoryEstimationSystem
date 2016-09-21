using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web.Optimization;

namespace HG.SoftwareEstimationService.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            StyleTransformer cssTransformer = new StyleTransformer();
            ScriptTransformer jsTransformer = new ScriptTransformer();
            NullOrderer nullOrderer = new NullOrderer();

            StyleBundle cssBundle = new StyleBundle("~/bundles/css");
            cssBundle.Include(
                "~/Content/Site.less",
                "~/Content/bootstrap/bootstrap.less");
            cssBundle.Transforms.Add(cssTransformer);
            cssBundle.Orderer = nullOrderer;
            bundles.Add(cssBundle);

            StyleBundle applicationCssBundle = new StyleBundle("~/bundles/applicationCss");
            cssBundle.Include("~/Content/CSS/*.css");
            cssBundle.Transforms.Add(cssTransformer);
            cssBundle.Orderer = nullOrderer;
            bundles.Add(applicationCssBundle);

            ScriptBundle jqueryBundle = new ScriptBundle("~/bundles/jquery");
            jqueryBundle.Include("~/Scripts/jquery-{version}.js");
            jqueryBundle.Include("~/Scripts/jquery.validate.js");
            jqueryBundle.Include("~/Scripts/linq.min.js");
            jqueryBundle.Transforms.Add(jsTransformer);
            jqueryBundle.Orderer = nullOrderer;
            bundles.Add(jqueryBundle);

            ScriptBundle jqueryvalBundle = new ScriptBundle("~/bundles/jqueryval");
            jqueryvalBundle.Include("~/Scripts/jquery.validate*");
            jqueryvalBundle.Transforms.Add(jsTransformer);
            jqueryvalBundle.Orderer = nullOrderer;
            bundles.Add(jqueryvalBundle);

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            ScriptBundle modernizrBundle = new ScriptBundle("~/bundles/modernizr");
            modernizrBundle.Include("~/Scripts/modernizr-*");
            modernizrBundle.Transforms.Add(jsTransformer);
            modernizrBundle.Orderer = nullOrderer;
            bundles.Add(modernizrBundle);

            ScriptBundle bootstrapBundle = new ScriptBundle("~/bundles/bootstrap");
            bootstrapBundle.Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js");
            bootstrapBundle.Transforms.Add(jsTransformer);
            bootstrapBundle.Orderer = nullOrderer;
            bundles.Add(bootstrapBundle);

            ScriptBundle applicationBundle = new ScriptBundle("~/bundles/application");
            bootstrapBundle.Include("~/Scripts/application/namespace.js");
            bootstrapBundle.Include("~/Scripts/application/*.js");
            bootstrapBundle.Include("~/Scripts/ui/grids/*.js");
            bootstrapBundle.Include("~/Scripts/ui/*.js");
            bootstrapBundle.Include("~/Scripts/Service/*.js");
            bootstrapBundle.Transforms.Add(jsTransformer);
            bootstrapBundle.Orderer = nullOrderer;
            bundles.Add(applicationBundle);
        }
    }
}