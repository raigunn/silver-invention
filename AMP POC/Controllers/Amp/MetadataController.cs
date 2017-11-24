using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMP_POC.Controllers.Amp
{
    public class MetadataController : Controller
    {
        // GET: Metadata
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AmpCssControllerRendering()
        {
            string css = System.IO.File.ReadAllText(Server.MapPath("~/styles/sample.css"));
            return View(new MvcHtmlString(css));// return View(css);
        }
    }
}