using System;
using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CsvHelper.Configuration;
using PortaleGeoWeb;
using PortaleGeoWeb.reader.csv;
using PortaleGeoWeb.Models;
using PagedList;
using ExcelDataReader;
using PortaleGeoWeb.ViewModels;


namespace PortaleGeoWeb
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        
        public ActionResult SelezionaSistema()
        {

            ViewBag.SistemaDiGeoreferenzazione = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "SistemaOpenStreetMap", Value = "1"},
                    new SelectListItem { Text = "SistemaHERE", Value = "2"},
                   new SelectListItem { Text = "GoogleMaps", Value = "3"},
                }, "Value", "Text");

            return View();
        }
        [HttpPost]
        public ActionResult Conferma(string Sistema)
        {

            switch(Sistema)
            {
                    case "1":
                    return Json(new {code= 1 });
                    case "2":
                    return Json(new { code = 2 });
                    case "3":
                    return Json(new { code = 3 });

            }
            return Json(new { Message = "Prova" });
        }
   
        public ActionResult Mappa(string Origine)
        {
            ViewBag.OrigineDati = new SelectList(new List<SelectListItem>
            {
                 new SelectListItem { Text = "SistemaOpenStreetMap", Value = "1"},
             //    new SelectListItem { Text = "SistemaHERE", Value = "2"},
                 new SelectListItem { Text = "GoogleMaps", Value = "2"},
                }, "Value", "Text");


            ViewBag.Origine = new SelectList(new List<SelectListItem>
            {
            
                });

            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }



        [HttpPost]
         [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        //Stream stream = upload.InputStream; //Un InputStream è il metodo grezzo per ottenere informazioni da una risorsa.
                        //Cattura i dati byte per byte senza eseguire alcun tipo di traduzione. Se stai leggendo dati di immagine o
                        //qualsiasi file binario, questo è il flusso da usare.
                        string _FileName = Path.GetFileName(upload.FileName);
                        string path = Path.Combine(Server.MapPath("~/Here/CsvModello"), _FileName);                      
                        CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                        conf.BadDataFound = null;
                        conf.Delimiter = ";";
                        conf.HasHeaderRecord = true;
                        var reader = new StreamReader(path);
                        var csv = new CsvHelper.CsvReader(reader, conf);
                        {


                            //  using (var streamReader = File.
                            // using (var csvReader = new CsvReader(streamreade, CultureInfo.CurrentCulture)) ;
                            var dr = new CsvDataReader(csv);
                            
                                var dt = new DataTable();
                                dt.Load(dr);
                                
                               
                                //  using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
                                //  using (CsvReader csvReader =
                                //    new CsvReader(new StreamReader(stream), true))


                                //return Json(new { Message = csvTable });
                                return View(dt);
                            
                        }
                    }


                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        //    return Json(new { Message = "File non supportato correttamente" });
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            // return Json(new { Message = "File non caricato" });
            return View();
        }
        
    }
}
       



  


  

    
