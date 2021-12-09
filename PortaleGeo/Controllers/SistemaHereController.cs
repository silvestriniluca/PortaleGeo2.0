using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortaleGeoWeb.Models;
using PortaleGeoWeb.reader.csv;
using PortaleGeoWeb.Helpers;
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
using PortaleGeoWeb.ViewModels;

namespace PortaleGeoWeb.Controllers
{
    public class SistemaHereController : Controller
    {

        // GET: SistemaHere
        public ActionResult Index()
        {
            return View();
        }

        // TEST GEOCODE

        public ActionResult Test(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {


                //Stream stream = upload.InputStream; //Un InputStream è il metodo grezzo per ottenere informazioni da una risorsa.
                //Cattura i dati byte per byte senza eseguire alcun tipo di traduzione. Se stai leggendo dati di immagine o
                //qualsiasi file binario, questo è il flusso da usare.
                string _FileName = Path.GetFileName(file.FileName);
                //
                string _tempFileName = Path.GetFileName(Path.GetTempFileName());
                string path = Path.Combine(Server.MapPath("~/CsvModello"), _tempFileName) + ".csv";
                //
                // save file
                file.SaveAs(path);
                CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                conf.BadDataFound = null;
                conf.Delimiter = ";";
                conf.HasHeaderRecord = true;
                MyCsvReader reader = new MyCsvReader();

                if (path != null)
                {
                    //
                    DataDescriptor data = reader.LoadData(path, conf);
                    //
                    // System.IO.File.Delete(path);
                    //
                    InputParam par = new InputParam();
                    par.posComune = Array.IndexOf(data.Header, "Comune");
                    par.posIndirizzo = Array.IndexOf(data.Header, "Indirizzo");
                    par.posProvincia = Array.IndexOf(data.Header, "Provincia");
                    //
                    //
                    par.posStartOutput = data.Header.Length;
                    //
                    data.i = true;
                    DataDescriptor data_computed = GeocodeProcessor.EsecuteGecoding(data, par, path, conf);
                    //
                    string tempFile = Path.Combine(Server.MapPath("~/CsvModello"), _tempFileName) + ".csv";
                    //
                    // write csv elaborated
                    //
                    MyCsvWriter.SaveCsvFile(tempFile, conf, data_computed);

                    HerePageModel model = new HerePageModel()
                    {
                        Data = data_computed,
                        UrlPathDownload = "/CsvModello/" + _tempFileName + ".csv"
                    };

                    return Json(new { Status = 1, Message = model.Data.Header, Message2 = model.Data.Rows, Message1 = model.UrlPathDownload });
                    //return View(model);

                }

            }
            return Json(new { message = "File caricato con successo" });
        }



        //AVVIA GEOCODE

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {


                //Stream stream = upload.InputStream; //Un InputStream è il metodo grezzo per ottenere informazioni da una risorsa.
                //Cattura i dati byte per byte senza eseguire alcun tipo di traduzione. Se stai leggendo dati di immagine o
                //qualsiasi file binario, questo è il flusso da usare.
                string _FileName = Path.GetFileName(file.FileName);  
                //senza le ultime 4 caratteri
                string newName = _FileName.Remove(_FileName.Length - 4); //elimino .csv e mantengo solamente il nome del file
                string path_newfile = Path.Combine(Server.MapPath("~/Here/CsvModello"), newName) + "GeocodingbyHere.csv";
         
                //string _tempFileName = Path.GetFileName(Path.GetTempFileName());
             //string path = Path.Combine(Server.MapPath("~/CsvModello"), _tempFileName) + ".csv";
             // string PATHVERO = Path.Combine(Server.MapPath("~/CsvModello"), _FileName);
                              //
                // save file
                file.SaveAs(path_newfile);
                CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                conf.BadDataFound = null;
                conf.Delimiter = ";";
                conf.HasHeaderRecord = true;
                MyCsvReader reader = new MyCsvReader();


                if (path_newfile != null)
                {
                    // inizializza la copia del file passato (righe, colonne e dati) per poi aggiungere colonne geocode Here
                    DataDescriptor data = reader.LoadData(path_newfile, conf);

                    // System.IO.File.Delete(path);

                    //stabilisce le posizioni delle colonne che ci servono
                    InputParam par = new InputParam();
                    par.posComune = Array.IndexOf(data.Header, "Comune");
                    par.posIndirizzo = Array.IndexOf(data.Header, "Indirizzo");
                    par.posProvincia = Array.IndexOf(data.Header, "Provincia");

                    //conta righe totali file iniziale
                    par.posStartOutput = data.Header.Length;

                    //file data + colonne Here (nuovo file georefernziato)
                    DataDescriptor data_computed = GeocodeProcessor.EsecuteGecoding(data, par, path_newfile, conf);
                    //
                    //   string tempFile = Path.Combine(Server.MapPath("~/CsvModello"), _tempFileName) + ".csv";
                    //
                    // write csv elaborated
                    //
                    MyCsvWriter.SaveCsvFile(path_newfile, conf, data_computed);

                   

                    HerePageModel model = new HerePageModel()
                    {
                        Data = data_computed,
                        UrlPathDownload = "/Here/CsvModello/" + newName+ "GeocodingbyHere.csv"
                    };

                    return Json(new { Status = 1, Message = model.Data.Header, Message2 = model.Data.Rows, Message1 = model.UrlPathDownload, Message3=_FileName });


                }

            }
            return Json(new { message = "File caricato con successo" });
        }




        //MAPPA

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
    
    }
  }
            
            
        
        
    
   
 


        
            
 



