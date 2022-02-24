using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using NuovoPortaleGeo.Models;
using PagedList;

namespace NuovoPortaleGeo.Controllers
{
    public class GeoDatiController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();

        // GET: CSVdatis
        public ActionResult Index(string sortOrder, string Provincia, string Comune, string Indirizzo, string Descrizione, string DescrizioneFile, int? page)
        {
            ViewData["DESCfileSortParm"] = String.IsNullOrEmpty(sortOrder) ? "DESCfile_desc" : "";
            ViewData["ProvSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Prov_desc" : "";
            ViewData["ComuneSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Comune_desc" : "";
            ViewData["IndirizzoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Indirizzo_desc" : "";
            ViewData["DESCRSortParm"] = String.IsNullOrEmpty(sortOrder) ? "DESCR_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            ViewBag.CurrentProvincia = Provincia;
            ViewBag.CurrentComune = Comune;
            ViewBag.CurrentIndirizzo = Indirizzo;
            ViewBag.CurrentDescrizione = Descrizione;
            ViewBag.CurrentDescrizioneFile = DescrizioneFile;

            if (Comune != null)
            {
                page = 1;
            }
            else
            {
                
            }
          
            var geodati = from s in db.CSVdati
                          select s;

            if (!String.IsNullOrEmpty(Comune))
            {
                geodati = geodati.Where(s => s.Comune.Contains(Comune));
                               
            }
            else if(!String.IsNullOrEmpty(Provincia))
                {
                geodati = geodati.Where(s => s.Provincia.Contains(Provincia));
            }
            else if (!String.IsNullOrEmpty(Indirizzo))
            {
                geodati = geodati.Where(s => s.Indirizzo.Contains(Indirizzo));
            }

            switch (sortOrder)
            {
                case "DESCfile_desc":
                    geodati = geodati.OrderBy(s => s.DescrizioneFile);
                    break;

                case "Comune_desc":
                    geodati = geodati.OrderByDescending(s => s.Comune);
                    break;
                case "DESCR_desc":
                    geodati = geodati.OrderBy(s => s.Descrizione);
                    break;
                case "Indirizzo_desc":
                    geodati = geodati.OrderBy(s => s.Indirizzo);
                    break;
                default:
                    geodati = geodati.OrderBy(s => s.Provincia);
                    break;
            }

            int pageSize = 20;

            int pageNumber = (page ?? 1);
          //  var cSVdati = db.CSVdati.Include(c => c.Geo_Utente);
            return View(geodati.ToPagedList(pageNumber,pageSize));
        }

        // GET: CSVdatis/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CSVdati cSVdati = db.CSVdati.Find(id);
            if (cSVdati == null)
            {
                return HttpNotFound();
            }
            return View(cSVdati);
        }

        // GET: CSVdatis/Create
        public ActionResult Create()
        {
            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email");
            return View();
        }

        // POST: CSVdatis/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdUtente,DescrizioneFile,Provincia,Comune,Indirizzo,Descrizione")] CSVdati cSVdati)
        {
            if (ModelState.IsValid)
            {
                db.CSVdati.Add(cSVdati);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email", cSVdati.IdUtente);
            return View(cSVdati);
        }

        // GET: CSVdatis/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CSVdati cSVdati = db.CSVdati.Find(id);
            if (cSVdati == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email", cSVdati.IdUtente);
            return View(cSVdati);
        }

        // POST: CSVdatis/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdUtente,DescrizioneFile,Provincia,Comune,Indirizzo,Descrizione")] CSVdati cSVdati)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cSVdati).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email", cSVdati.IdUtente);
            return View(cSVdati);
        }

        // GET: CSVdatis/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CSVdati cSVdati = db.CSVdati.Find(id);
            if (cSVdati == null)
            {
                return HttpNotFound();
            }
            return View(cSVdati);
        }

        // POST: CSVdatis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CSVdati cSVdati = db.CSVdati.Find(id);
            db.CSVdati.Remove(cSVdati);
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
        public async Task<IActionResult> ordina(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "DESCfile_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Prov_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Comune_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Indirizzo_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "DESCR_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            var geodati = from s in db.CSVdati
                           select s;
            switch (sortOrder)
            {
                case "DESCfile_desc":
                    geodati = geodati.OrderByDescending(s => s.DescrizioneFile);
                    break;
                
                case "Comune_desc":
                    geodati = geodati.OrderByDescending(s => s.Comune);
                    break;
                case "DESCR_desc":
                    geodati = geodati.OrderBy(s => s.Descrizione);
                    break;
                case "Indirizzo_desc":
                    geodati = geodati.OrderBy(s => s.Indirizzo);
                    break;
                default:
                    geodati = geodati.OrderBy(s => s.Provincia);
                    break;
            }
            return (IActionResult)View(await geodati.AsNoTracking().ToListAsync());
        }
    }
}
