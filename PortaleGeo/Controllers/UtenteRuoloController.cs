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
    public class UtenteRuoloController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();

        // GET: UtenteRuolo
        public ActionResult Index()
        {
            var Geo_UtenteRuolo = db.Geo_UtenteRuolo.Include(i => i.Geo_Ente).Include(i => i.Geo_Fornitore).Include(i => i.Geo_Ruolo).Include(i => i.Geo_Utente);
            return View(Geo_UtenteRuolo.ToList());
        }

        // GET: UtenteRuolo/Details/5
        public ActionResult Dettagli(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_UtenteRuolo geO_UtenteRuolo = db.Geo_UtenteRuolo.Find(id);
            if (geO_UtenteRuolo == null)
            {
                return HttpNotFound();
            }

            var vm = new VmUtenteRuolo(geO_UtenteRuolo);
            return View(vm);
            
        }

        // GET: UtenteRuolo/Create
        public ActionResult Aggiungi(string UserId)
        {
            ViewBag.IdEnte = new SelectList(db.Geo_Ente, "Id", "Nome");
            ViewBag.IdFornitore = new SelectList(db.Geo_Fornitore, "Id", "RagioneSociale");
            ViewBag.RoleId = new SelectList(db.Geo_Ruolo, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Geo_Utente, "Id", "Email");
            return View();
        }

        // POST: UtenteRuolo/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "Id,UserId,RoleId,IdEnte,IdFornitore")] Geo_UtenteRuolo geO_UtenteRuolo)
        {
            if (ModelState.IsValid)
            {
                db.Geo_UtenteRuolo.Add(geO_UtenteRuolo);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Index", "Utente");
            }

            ViewBag.IdEnte = new SelectList(db.Geo_Ente, "Id", "Nome", geO_UtenteRuolo.IdEnte);
            ViewBag.IdFornitore = new SelectList(db.Geo_Fornitore, "Id", "RagioneSociale", geO_UtenteRuolo.IdFornitore);
            ViewBag.RoleId = new SelectList(db.Geo_Ruolo, "Id", "Name", geO_UtenteRuolo.RoleId);
            ViewBag.UserId = new SelectList(db.Geo_Utente, "Id", "Email", geO_UtenteRuolo.UserId);
            
            return View(geO_UtenteRuolo);
            

        }

        /*
        // GET: UtenteRuolo/Edit/5
        public ActionResult Modifica(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IO_UtenteRuolo iO_UtenteRuolo = db.IO_UtenteRuolo.Find(id);
            if (iO_UtenteRuolo == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEnte = new SelectList(db.IO_Ente, "Id", "CodiceIstat", iO_UtenteRuolo.IdEnte);
            ViewBag.IdFornitore = new SelectList(db.IO_Fornitore, "Id", "RagioneSociale", iO_UtenteRuolo.IdFornitore);
            ViewBag.RoleId = new SelectList(db.IO_Ruolo, "Id", "Name", iO_UtenteRuolo.RoleId);
            ViewBag.UserId = new SelectList(db.IO_Utente, "Id", "Email", iO_UtenteRuolo.UserId);
            return View(iO_UtenteRuolo);
        }

        // POST: UtenteRuolo/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifica([Bind(Include = "Id,UserId,RoleId,IdEnte,IdFornitore")] IO_UtenteRuolo iO_UtenteRuolo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(iO_UtenteRuolo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdEnte = new SelectList(db.IO_Ente, "Id", "CodiceIstat", iO_UtenteRuolo.IdEnte);
            ViewBag.IdFornitore = new SelectList(db.IO_Fornitore, "Id", "RagioneSociale", iO_UtenteRuolo.IdFornitore);
            ViewBag.RoleId = new SelectList(db.IO_Ruolo, "Id", "Name", iO_UtenteRuolo.RoleId);
            ViewBag.UserId = new SelectList(db.IO_Utente, "Id", "Email", iO_UtenteRuolo.UserId);
            return View(iO_UtenteRuolo);
        }
        */
        // GET: UtenteRuolo/Delete/5
        public ActionResult Cancella(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_UtenteRuolo geO_UtenteRuolo = db.Geo_UtenteRuolo.Find(id);
            if (geO_UtenteRuolo == null)
            {
                return HttpNotFound();
            }

            var vm = new VmUtenteRuolo(geO_UtenteRuolo);
            return View(vm);

        }

        // POST: UtenteRuolo/Delete/5
        [HttpPost, ActionName("Cancella")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Geo_UtenteRuolo geO_UtenteRuolo = db.Geo_UtenteRuolo.Find(id);
            db.Geo_UtenteRuolo.Remove(geO_UtenteRuolo);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction("Index", "Utente");
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
