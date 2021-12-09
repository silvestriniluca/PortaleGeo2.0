using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PortaleGeoWeb.Models;
using PortaleGeoWeb.ViewModels;

namespace PortaleGeoWeb.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class FornitoreController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();

        // GET: Fornitore
        public ActionResult Index()
        {
            return View(db.Geo_Fornitore.ToList());
        }

        // GET: Fornitore/Details/5
        public ActionResult Dettagli(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Fornitore geO_Fornitore = db.Geo_Fornitore.Find(id);
            if (geO_Fornitore == null)
            {
                return HttpNotFound();
            }

            var vm = new VmFornitore(geO_Fornitore);
            return View(vm);

        }

        // GET: Fornitore/Create
        public ActionResult Aggiungi()
        {
            return View();
        }

        // POST: Fornitore/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "Id,RagioneSociale,PartitaIva,ContattoTecnico,EmailTecnico,TelefonoTecnico,ContattoTecnico1,EmailTecnico1,TelefonoTecnico1")] Geo_Fornitore geO_Fornitore)
        {
            if (ModelState.IsValid)
            {
                db.Geo_Fornitore.Add(geO_Fornitore);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var vm = new VmFornitore(geO_Fornitore);
            return View(vm);
        }

        // GET: Fornitore/Edit/5
        public ActionResult Modifica(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Fornitore geO_Fornitore = db.Geo_Fornitore.Find(id);
            if (geO_Fornitore == null)
            {
                return HttpNotFound();
            }

            var vm = new VmFornitore(geO_Fornitore);
            return View(vm);
        }

        // POST: Fornitore/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifica([Bind(Include = "Id,RagioneSociale,PartitaIva,ContattoTecnico,EmailTecnico,TelefonoTecnico,ContattoTecnico1,EmailTecnico1,TelefonoTecnico1")] Geo_Fornitore geO_Fornitore)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geO_Fornitore).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var vm = new VmFornitore(geO_Fornitore);
            return View(vm);
        }

        // GET: Fornitore/Delete/5
        public ActionResult Cancella(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Fornitore geO_Fornitore = db.Geo_Fornitore.Find(id);
            if (geO_Fornitore == null)
            {
                return HttpNotFound();
            }

            var vm = new VmFornitore(geO_Fornitore);
            return View(vm);
        }

        // POST: Fornitore/Delete/5
        [HttpPost, ActionName("Cancella")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Geo_Fornitore Geo_Fornitore = db.Geo_Fornitore.Find(id);
            db.Geo_Fornitore.Remove(Geo_Fornitore);
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
