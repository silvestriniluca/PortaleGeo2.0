using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuovoPortaleGeo.Models;
using NuovoPortaleGeo.reader.csv;
using NuovoPortaleGeo.Helpers;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;
using System.Globalization;
using Microsoft.AspNet.Identity;
using LanguageExt.ClassInstances.Pred;
using System.Collections;
using PagedList;
using System.Data;
using System.Web.Script.Serialization;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using NuovoPortaleGeo.ViewModels;
using System.Diagnostics;

namespace NuovoPortaleGeo.Controllers
{
    [Authorize(Roles = "Amministratore,Utente,Consultatore")]

    public class SistemaHereController : Controller
    {
        GeoCodeEntities1 db = new GeoCodeEntities1();
        static int GeoRighe;
        static int GeoNoRighe;       
        // GET: SistemaHere
        public ActionResult Index()
        {
            return View();
        }

     
        

        
        //AVVIA GEOCODE

        
        public static void Upload(string path, CsvConfiguration conf, string FileName, DataTable tablerisultati,string cf)
        {

            using (GeoCodeEntities1 db = new GeoCodeEntities1())

            {

                var Geo_Utente = db.Geo_Utente
                        .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
                MyCsvReader reader = new MyCsvReader();


                if (path != null)
                {
                    // inizializza la copia del file passato (righe, colonne e dati) per poi aggiungere colonne geocode Here
                    DataDescriptor data = reader.LoadData(path, conf);

                    // System.IO.File.Delete(path);

                    //stabilisce le posizioni delle colonne che ci servono
                    InputParam par = new InputParam();
                    par.posComune = Array.IndexOf(data.Header, "Comune");
                    par.posIndirizzo = Array.IndexOf(data.Header, "Indirizzo");
                    par.posProvincia = Array.IndexOf(data.Header, "Provincia");

                    //conta righe totali file iniziale
                    par.posStartOutput = data.Header.Length;
                    
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    //file data + colonne Here (nuovo file georefernziato)
                    DataDescriptor data_computed = GeocodeProcessor.EsecuteGecoding(data, par, path, conf,GeoRighe,GeoNoRighe);
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                   
                    //   string tempFile = Path.Combine(Server.MapPath("~/CsvModello"), _tempFileName) + ".csv";

                    // write csv elaborated
                    
                    //
                    MyCsvWriter.SaveCsvFile(path, conf, data_computed);
                    foreach (var col in data_computed.Header)
                    {
                        tablerisultati.Columns.Add(new DataColumn(col));
                    }
                    foreach (var row in data_computed.Rows)
                    {
                        tablerisultati.Rows.Add(row);
                    }
                    int GeoRef = GeoRighe - GeoNoRighe;
                    HomeController.GetAttività(Geo_Utente.Id, Geo_Utente.UserName, FileName, path, true, false, tablerisultati.Rows.Count, GeoRef, ts);
                    
          //          HerePageModel model = new HerePageModel()
            //        {
              //          Data = data_computed,
                //        UrlPathDownload = "/Here/CsvModello/" + FileName + "GeocodingbyHere.csv"
                  //  };



                }
            }
            }
            
        }




        //MAPPA
/*
        [HttpPost]
        public ActionResult JsonRisultati(string _FileName)
        {
            
            string newName = _FileName.Remove(_FileName.Length -4);

            string path = Path.Combine(Server.MapPath("~/User"));
            path = path.Remove(path.Length - 40);
            path = path + @"\Downloads\" + newName + "GeocodingbyHere.csv";
           

            if (System.IO.File.Exists(path))
                {

            using (var reader = new StreamReader(path))

            {
                CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                conf.BadDataFound = null;
                conf.Delimiter = ";";
                conf.HasHeaderRecord = true;


                using (var csvReader = new CsvReader(reader, conf))
                {
                    var records = csvReader.GetRecords<VmHere>().ToList();
                    var serializer = new JavaScriptSerializer();
                    var serializedResult = serializer.Serialize(records);
                    return Json(new { code = 1, ritorno = serializedResult });

                }

            }
            }
            return null;
        }
   */ 
    }
  
            
            
        
        
    
   
 


        
            
 



