using System.Web;
using System.Web.Optimization;

namespace photoEditSystem
{
    public class BundleConfig
    {
        // 如需統合的詳細資訊，請瀏覽 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
            // 準備好可進行生產時，請使用 https://modernizr.com 的建置工具，只挑選您需要的測試。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            bundles.Add(new ScriptBundle("~/bundles/jquery3.5").Include(
                        "~/Scripts/jquery-3.5.1.slim.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap4").Include(
                        "~/Scripts/js/popper.min.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/js/bootstrap4.min.js",
                        "~/Scripts/js/bootstrap-select.js",
                        "~/Scripts/js/bootstrap.bundle.min.js",
                        "~/Scripts/bootstrap-datetimepicker.min.js"));
            bundles.Add(new StyleBundle("~/Content/bcss4").Include(
                     "~/Content/bootstrap4.min.css",
                     "~/Content/site.css",
                     "~/Content/fontawesome-free-5.15.3-web/css/fontawesome.min.css",
                     "~/Content/fontawesome-free-5.15.3-web/css/all.css",
                     "~/Content/PagedList.css",
                     "~/Content/bootstrap-select.css",
                     "~/Content/bootstrap-datetimepicker.min.css"));

        }
    }
}
