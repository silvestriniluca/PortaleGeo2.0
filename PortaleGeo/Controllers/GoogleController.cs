using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuovoPortaleGeo.Controllers
{
    [Authorize(Roles = "Amministratore,Utente,Consultatore")]
    public class GoogleController : Controller
    {
        // GET: Google
        public ActionResult Index()
        {
            return View();
        }
    }
}