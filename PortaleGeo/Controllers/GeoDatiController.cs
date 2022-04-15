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
using Newtonsoft.Json;
using NuovoPortaleGeo.Helpers;
using NuovoPortaleGeo.Models;
using PagedList;

namespace NuovoPortaleGeo.Controllers
{
    [Authorize(Roles = "Amministratore,Utente,Consultatore")]
    public class GeoDatiController : Controller
    {
        private GeoCodeEntities1 db = new GeoCodeEntities1();

        //da verificare alternativa con Mirco

        public static string _File;
        public static string Name_File_Export;

        // GET: GeoDati
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
            var cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
                  
            ViewBag.DescrizioneFile = new SelectList(db.Geo_Dati.Where(s => s.IdUtente == Geo_Utente.Id).GroupBy(p => new { p.DescrizioneFile })
                                                     .Select(g => g.FirstOrDefault()), "DescrizioneFile", "DescrizioneFile");
           
            if(_File!=null)
            {
                DescrizioneFile = _File;
            }
            else if (DescrizioneFile != null)
            {
                ViewBag.CurrentDescrizioneFile = DescrizioneFile;
            }
       
            if (Comune != null)
            {
                page = 1;
            }
            else
            {
                
            }

            var geodati = db.Geo_Dati.Where(s => s.IdUtente == Geo_Utente.Id).Select(s => s);
                         

            if (!String.IsNullOrEmpty(Comune))
            {
                geodati = geodati.Where(s => s.Comune.Contains(Comune));
                               
            }
            if(!String.IsNullOrEmpty(Provincia))
            {
                geodati = geodati.Where(s => s.Provincia.Contains(Provincia));
            }
             if (!String.IsNullOrEmpty(Indirizzo))
            {
                geodati = geodati.Where(s => s.Indirizzo.Contains(Indirizzo));
            }
             if (!String.IsNullOrEmpty(DescrizioneFile))
            {
                geodati = geodati.Where(s => s.DescrizioneFile.Contains(DescrizioneFile));
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

            int pageSize = 100;

            int pageNumber = (page ?? 1);
            _File = null;
            
          
            return View(geodati.ToPagedList(pageNumber,pageSize));
        }

        // GET: geo_Dati/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Dati geo_Dati = db.Geo_Dati.Find(id);
            if (geo_Dati == null)
            {
                return HttpNotFound();
            }
            return View(geo_Dati);
        }

        // GET: geo_Dati/Create
        public ActionResult Create()
        {
            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email");
            return View();
        }

        // POST: geo_Dati/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdUtente,DescrizioneFile,Provincia,Comune,Indirizzo,Descrizione")] Geo_Dati geo_Dati)
        {
            if (ModelState.IsValid)
            {
                db.Geo_Dati.Add(geo_Dati);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email", geo_Dati.IdUtente);
            return View(geo_Dati);
        }

        // GET: geo_Dati/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Dati geo_Dati = db.Geo_Dati.Find(id);
            if (geo_Dati == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email", geo_Dati.IdUtente);
            return View(geo_Dati);
        }

        // POST: GeoDati/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdUtente,DescrizioneFile,Provincia,Comune,Indirizzo,Descrizione")] Geo_Dati geo_Dati)
        {
            if (ModelState.IsValid)
            {
                db.Entry(geo_Dati).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUtente = new SelectList(db.Geo_Utente, "Id", "Email", geo_Dati.IdUtente);
            return View(geo_Dati);
        }

        // GET: GeoDati/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Geo_Dati geo_Dati = db.Geo_Dati.Find(id);
            if (geo_Dati == null)
            {
                return HttpNotFound();
            }
            return View(geo_Dati);
        }

        // POST: GeoDati/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Geo_Dati geo_Dati = db.Geo_Dati.Find(id);
            db.Geo_Dati.Remove(geo_Dati);
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

        [HttpPost]
        public ActionResult ValoreExport(string Name_File)
        {

            if (Name_File != "Seleziona un file")
            {
                Name_File_Export = Name_File;
                return Json(Name_File);
            }
            else
            {
                return Json(new { code = 2 });
            }

        }
        //Estrai
        
        public ActionResult Estrai()
        {

            string errore = null;           
                var GeoDati = db.Geo_Dati.Where(s => s.DescrizioneFile == Name_File_Export).Select(s => s);
                var GeOProva = GeoDati;
                var lista_geocode = GeOProva.ToList();

                try
                {
                    var contenuto =
                        lista_geocode.Where(s => s.DescrizioneFile == Name_File_Export)
                        .Select(x => new
                        {
                            Id = x.Id,
                            Indirizzo = x.Indirizzo,
                                //   N_Civico = x.N_Civico,
                            Comune = x.Comune,
                            Provincia = x.Provincia,
                              //   Regione = x.Regione,
                              // Note1 = x.Note1,
                              //  Note2 = x.Note2,
                            Descrizione=x.Descrizione,
                            Lat = x.Lat,
                            Lon = x.Lon,
                            Approx01 = x.Approx01,
                            Approx02 = x.Approx02,      
                            Cap = x.Cap,
                            AltroIndirizzo = x.AltroIndirizzo,
                            Here_MatchLevel=x.Here_MatchLevel,
                            Here_MatchType = x.Here_MatchType,
                            Here_Relevance = x.Here_Relevance,
                            Here_Error = x.Here_Error,                           
                            //  APIGoogle = x.APIGoogle
                        })
                        .ToList();

                    var columns = new List<string>
                        {
                            "Id",
                            "Indirizzo",
                            "Comune",
                            "Provincia",
                            "Descrizione",
                            "Lat",
                            "Lon",
                            "Approx01",
                            "Approx02",
                            "Cap",                            
                            "Here_MatchLevel",
                            "Here_MatchType",
                            "Here_Relevance",
                            "Here_Error",
                        };


                    byte[] filecontent = ExcelExportHelper.ExportExcel(contenuto, "Estrazione GeoCode CSV", false, columns.ToArray());

               

                return (File(
                            filecontent,
                            ExcelExportHelper.ExcelContentType,
                            String.Format("{0} - "+  Name_File_Export+ " report-geocode-csv.xlsx", DateTime.Now.ToString("yyyy-MM-dd"))

                        ));
               
                    

                }

                catch (Exception exc)
                {
                    errore = exc.Message;
                    return null;
                }

        }
        [HttpPost]
        public ActionResult JsonRisultati()
        {

            string errore = null;
            var GeoDati = db.Geo_Dati.Where(s => s.DescrizioneFile == Name_File_Export).Select(s => s);
            var lista_geocode = GeoDati.ToList();

            try
            {
                var contenuto =
                    lista_geocode
                    .Select(x => new
                    {
                        Id = x.Id,
                        Indirizzo = x.Indirizzo,                      
                        Comune = x.Comune,
                        Provincia = x.Provincia,                                         
                        Lat = x.Lat,
                        Lon = x.Lon,
                        Descrizione=x.Descrizione
                    })
                    .ToList();

                var json_map = Json(contenuto, JsonRequestBehavior.AllowGet);
                return json_map;
            }
            catch (Exception exc)
            {
                errore = exc.Message;
                return null;
            }

           


            
            //return Json(new { risposta = lista_geocode });
        }

        public async Task<IActionResult> ordina(string sortOrder)
        {
            
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "DESCfile_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Prov_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Comune_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Indirizzo_desc" : "";
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "DESCR_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            var cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
            var geodati = db.Geo_Dati.Where(s => s.IdUtente == Geo_Utente.Id).Select(s => s);
                          
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
