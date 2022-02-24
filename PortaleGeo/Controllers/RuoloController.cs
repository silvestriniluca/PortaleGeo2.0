using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NuovoPortaleGeo.Models;
using NuovoPortaleGeo.ViewModels;


namespace NuovoPortaleGeo.Controllers
{
    [Authorize(Roles = "Amministratore")]
    public class RuoloController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();

        // GET: Ruolo
        public ActionResult Index()
        {
            return View(db.Geo_Ruolo.ToList());
        }

        // GET: Ruolo/Details/5
        public ActionResult Dettagli(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Ruolo geO_Ruolo = db.Geo_Ruolo.Find(id);
            if (geO_Ruolo == null)
            {
                return HttpNotFound();
            }

            var vm = new VmRuolo(geO_Ruolo);
            return View(vm);
        }

        // GET: Ruolo/Create
        public ActionResult Aggiungi()
        {
            return View();
        }

        // POST: Ruolo/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "Id,Name")] Geo_Ruolo geO_Ruolo)
        {
            if (ModelState.IsValid)
            {
                db.Geo_Ruolo.Add(geO_Ruolo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(geO_Ruolo);
        }

        // GET: Ruolo/Edit/5
        public ActionResult Modifica(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Ruolo geO_Ruolo = db.Geo_Ruolo.Find(id);
            if (geO_Ruolo == null)
            {
                return HttpNotFound();
            }

            var vm = new VmRuolo(geO_Ruolo);
            return View(vm);
        }

        // POST: Ruolo/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifica([Bind(Include = "Id,Name")] Geo_Ruolo geO_Ruolo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geO_Ruolo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(geO_Ruolo);
        }

        // GET: Ruolo/Delete/5
        public ActionResult Cancella(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Ruolo geO_Ruolo = db.Geo_Ruolo.Find(id);
            if (geO_Ruolo == null)
            {
                return HttpNotFound();
            }
            var vm = new VmRuolo(geO_Ruolo);
            return View(vm);
        }

        // POST: Ruolo/Delete/5
        [HttpPost, ActionName("Cancella")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Geo_Ruolo geO_Ruolo = db.Geo_Ruolo.Find(id);
            db.Geo_Ruolo.Remove(geO_Ruolo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
