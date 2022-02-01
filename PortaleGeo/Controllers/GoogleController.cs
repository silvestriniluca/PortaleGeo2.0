using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortaleGeoWeb.Controllers
{
    [Authorize(Roles = "Administrators,EnteLocale,Fornitore")]
    public class GoogleController : Controller
    {
        // GET: Google
        public ActionResult Index()
        {
            return View();
        }
    }
}