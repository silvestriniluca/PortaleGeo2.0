using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PortaleGeoWeb.Models;
using PortaleGeoWeb.ViewModels;
using Microsoft.AspNet.Identity;
using PortaleGeoWeb.Helpers;


namespace PortaleGeoWeb.Controllers

{
    [Authorize(Roles = "Administrators,EnteLocale,Fornitore")]
    public class EnteController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();
        // Instantiate random number generator.  
        private readonly Random _random = new Random();

        // GET: Ente
        [Authorize(Roles = "Administrators,EnteLocale,Fornitore")]
        public ActionResult Index()
        {


            return View();
        }

        // GET: Ente/Details/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Dettagli(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Ente geO_ENTE = db.Geo_Ente.Find(id);
            if (geO_ENTE == null)
            {
                return HttpNotFound();
            }

            var vm = new VmEnte(geO_ENTE);
            return View(vm);
        }

        // GET: Ente/Create
        [Authorize(Roles = "Administrators")]
        public ActionResult Aggiungi()
        {

            ViewBag.AderisceDigipalm = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text");

            ViewBag.OnBoardingRicevuto = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text");


            ViewBag.OnBoardingInviato = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text");

            ViewBag.PraticheUSR = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text");

            return View();
        }

        // POST: Ente/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators")]
        public ActionResult Aggiungi([Bind(Include = "Id,CodiceIstat,CodiceFisco,SiglaProv,Cap,Nome,ValidoDal,ValidoAl,AderisceDigipalm,OnBoardingRicevuto,OnBoardingInviato,CodiceFiscale,PraticheUSR,AuthIoWS")] Geo_Ente geO_ENTE)
        {
            if (ModelState.IsValid)
            {
                db.Geo_Ente.Add(geO_ENTE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(geO_ENTE);
        }

        // GET: Ente/Edit/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Modifica(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Ente geO_ENTE = db.Geo_Ente.Find(id);
            if (geO_ENTE == null)
            {
                return HttpNotFound();
            }

            ViewBag.AderisceDigipalm = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text", geO_ENTE.AderisceDigipalm);


            ViewBag.OnBoardingRicevuto = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text", geO_ENTE.OnBoardingRicevuto);


            ViewBag.OnBoardingInviato = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text", geO_ENTE.OnBoardingInviato);


            ViewBag.PraticheUSR = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Selected = false, Text = "SI", Value = "1"},
                    new SelectListItem { Selected = false, Text = "NO", Value = "0"}
                }, "Value", "Text", geO_ENTE.PraticheUSR);


            var vm = new VmEnte(geO_ENTE);
            return View(vm);
        }

        // POST: Ente/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators")]
        public ActionResult Modifica([Bind(Include = "Id,CodiceIstat,CodiceFisco,SiglaProv,Cap,Nome,ValidoDal,ValidoAl,AderisceDigipalm,OnBoardingRicevuto,OnBoardingInviato,CodiceFiscale,PraticheUSR,AuthIoWS")] Geo_Ente geO_ENTE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geO_ENTE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(geO_ENTE);
        }

        // GET: Ente/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Cancella(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Ente geO_ENTE = db.Geo_Ente.Find(id);
            if (geO_ENTE == null)
            {
                return HttpNotFound();
            }

            var vm = new VmEnte(geO_ENTE);
            return View(vm);
        }

        // POST: Ente/Delete/5
        [HttpPost, ActionName("Cancella")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrators")]
        public ActionResult DeleteConfirmed(int id)
        {
            Geo_Ente geO_ENTE = db.Geo_Ente.Find(id);
            db.Geo_Ente.Remove(geO_ENTE);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // POST: Ente/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrators")]
        public String GeneraAuthIoWS()
        {
            return RandomString(30);
        }

        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);
            

        // Unicode/ASCII Letters are divided into two blocks
        // (Letters 65–90 / 97–122):
        // The first group containing the uppercase letters and
        // the second group containing the lowercase.  

        // char is a single Unicode character  
        char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public IQueryable<Geo_Ente> ElencoEnti(IQueryable<Geo_Ente> geO_Ente)
        {
            if (this.User.IsInRole("Administrators"))
            {
                return geO_Ente;
            }


            if (this.User.IsInRole("EnteLocale"))
            {
                var Userid = HttpContext.User.Identity.GetUserId();

                var EntiAbilitati = VmEnte.Geo_Ente_List(Userid);

                geO_Ente = geO_Ente.Where(t => EntiAbilitati.Contains(t.Id));

                return geO_Ente;
            }

            if (this.User.IsInRole("Fornitore"))
            {
                var Userid = HttpContext.User.Identity.GetUserId();
                var FornitoriAbilitati = VmFornitore.Geo_Fornitore_List(Userid);


          

                
                
                return geO_Ente;

            }

            return null;
        }


        public ActionResult Estrai()
        {
           
                return null;
            }
        
    }
}

