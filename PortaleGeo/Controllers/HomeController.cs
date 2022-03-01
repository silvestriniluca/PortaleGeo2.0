using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CsvHelper.Configuration;
using NuovoPortaleGeo.Models;
using System.Data.SqlClient;
using NuovoPortaleGeo.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NuovoPortaleGeo.Controllers;
using System;
using NuovoPortaleGeo.Helpers;

namespace NuovoPortaleGeo
{

    public class HomeController : Controller
    {
        GeoCodeEntities1 db = new GeoCodeEntities1();

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

            switch (Sistema)
            {
                case "1":
                    return Json(new { code = 1 });
                case "2":
                    return Json(new { code = 2 });
                case "3":
                    return Json(new { code = 3 });

            }
            return Json(new { Message = "Prova" });
        }
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
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
            // Geo_Attività geo_Attività = new Geo_Attività();
            var listattività = db.Geo_Attività.ToList();
            var cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
            var geo_attività = db.Geo_Attività.ToList();



            ViewBag.DescrizioneFile = new SelectList(db.Geo_Attività.Where(s => s.Id_Utente == Geo_Utente.Id), "DescrizioneFile", "DescrizioneFile");



            return View();
        }
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Upload()
        {

           
            var cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
              ViewBag.FileEsportazione = new SelectList(db.CSVdati.Where(s => s.IdUtente == Geo_Utente.Id).GroupBy(p => new {p.DescrizioneFile })
                                                       .Select(g => g.FirstOrDefault()), "Id", "DescrizioneFile") ;
           


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]

        public ActionResult Upload(HttpPostedFileBase upload, [Bind(Include = "DescrizioneFile,Here,OpenStreetMap,Google")] CSVdati dati)
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
                        
                        // verifico l'utente loggato e gli associo il numero di file caricato
                        var cf = Session["CF"].ToString();
                        var Geo_Utente = db.Geo_Utente
                                   .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
                        dati.IdUtente = Geo_Utente.Id;
                        string Cognome = Geo_Utente.Cognome;
                        
                        string pathfolder = Path.Combine(Server.MapPath("~/Upload"),Cognome);
                        string path = Path.Combine(Server.MapPath("~/Upload/" + Cognome), _FileName);
                        if (!Directory.Exists(pathfolder))
                        {
                            Directory.CreateDirectory(pathfolder);

                        }
                     
                        upload.SaveAs(path);
                        CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                        conf.BadDataFound = null;
                        conf.Delimiter = ";";
                        conf.HasHeaderRecord = true;
                        var reader = new StreamReader(path);
                        var csv = new CsvHelper.CsvReader(reader, conf);

                        
                        //  using (var streamReader = File.
                        // using (var csvReader = new CsvReader(streamreade, CultureInfo.CurrentCulture)) ;
                        var dr = new CsvDataReader(csv);
                        DataTable tablerisultati = new DataTable();
                        
                         var dt = new DataTable();
                        dt.Load(dr);
                        readcolumn(dt);
                        if (dati.OpenStreetMap is true)
                        {
                            OpenStreetMapController.GeoCodeRow(path, _FileName, cf, dt, tablerisultati,_FileName,path);

                            var dtdatabase = new DataTable();
                            dtdatabase.Columns.Add("IdUtente");
                            dtdatabase.Columns.Add("DescrizioneFile");
                            dtdatabase.Columns.Add("Here");
                            dtdatabase.Columns.Add("OpenStreetMap");
                            dtdatabase.Columns.Add("Google");
                            dtdatabase.Columns.Add("Lat");
                            dtdatabase.Columns.Add("Lon");
                            dtdatabase.Columns.Add("Approx01");
                            dtdatabase.Columns.Add("Approx02");
                            foreach (DataColumn col in dt.Columns)
                            {

                                if (col.ColumnName == "Provincia")
                                {
                                    dtdatabase.Columns.Add("Provincia");

                                }
                                if (col.ColumnName == "Comune")
                                {
                                    dtdatabase.Columns.Add("Comune");

                                }
                                if (col.ColumnName == "Indirizzo")
                                {
                                    dtdatabase.Columns.Add("Indirizzo");

                                }
                                if (col.ColumnName == "DENOMINAZIONE")
                                {
                                    dtdatabase.Columns.Add("Denominazione");

                                }
                                
                            }

                                // problema inserimento dati

                                foreach (DataRow row in tablerisultati.Rows)
                            {
       
                                        dtdatabase.Rows.Add(dati.IdUtente, dati.DescrizioneFile, row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], dati.OpenStreetMap, dati.Here, dati.Google, row["Lat"],
                                                row["Lon"], row["Approx01"], row["Approx02"]);
                                    
                                    
                             }
                            
                            //string connectionString = @"Data Source=SQL2016CLUST01\MSSQLSERV_C;Initial Catalog=IntraGeoRef;User Id=ut_IntraGeoRef;Password=K1-u9_b745P;";
                            SqlConnection connectionstring = new SqlConnection(@"Data Source=sql2016listen_c, 1733;Initial Catalog=IntraGeoRef;Integrated Security=True");
                            datatablehelper(connectionstring, dtdatabase);
                            //     db.CSVdati.Add(dati);
                            //     db.SaveChanges();
                        }
                       
                        VmUpload vm = new VmUpload(dati, dt);
                        VmUpload vmGeo = new VmUpload(dati, tablerisultati);
                        //    db.CSVdati.Add(dati.DescrizioneFile);
                        if (tablerisultati.Rows.Count>0 )
                        {
                            return View(vmGeo);
                           
                        }
                        else
                        {
                            return View(vm);
                        }
                       


                    }


                 
                }

                else
                {
                    ModelState.AddModelError("File", "This file format is not supported");
                  
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            return View();
        }
    


     

        public static void datatablehelper(SqlConnection connection, DataTable dataTable)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand("Get_GeoDati", connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@GeoDato", dataTable);
                sqlCommand.ExecuteNonQuery();
                connection.Close();
            }

        }
        [HttpPost]
        public ActionResult  readcolumn(DataTable dataTable)
        {
            List<string> lista = new List<string>();

                foreach (DataColumn col in dataTable.Columns)
            {
                var columnname= col.ColumnName;
                lista.Add(columnname);                
            }
            var json_dati = Json(lista, JsonRequestBehavior.AllowGet);
            return Json(new { code = 1, ritorno = json_dati });
           

        }

        //Estrai
        public ActionResult Estrai(string Name_File)
        {
            string errore = null;
            var GeoDati = db.CSVdati;
            //var GeoDati = db.CSVdati;
            var lista_geocode = GeoDati.Where(s => s.DescrizioneFile == Name_File).ToList();
            


                try
                {
                    var contenuto =
                        lista_geocode
                        .Select(x => new
                        {
                            Id = x.Id,
                            Indirizzo = x.Indirizzo,
                        //   N_Civico = x.N_Civico,
                        Comune = x.Comune,
                            Provincia = x.Provincia,
                        //   Regione = x.Regione,
                        // Note1 = x.Note1,
                        //   Note2 = x.Note2,
                        Lat = x.Lat,
                            Lon = x.Lon,
                            Approx01 = x.Approx01,
                            Approx02 = x.Approx02,
                            Cap = x.Cap,
                            AltroIndirizzo = x.AltroIndirizzo,
                        //  APIGoogle = x.APIGoogle
                    })
                        .ToList();

                    var columns = new List<string>
                        {
                            "Id",
                            "Indirizzo",
                            
                            "Comune",
                            "Provincia",
                            "Lat",
                            "Lon",
                            "Approx01",
                            "Approx02",
                            "Cap",
                            "AltroIndirizzo",

                        };


                    byte[] filecontent = ExcelExportHelper.ExportExcel(contenuto, "Estrazione GeoCode CSV", false, columns.ToArray());
                    return File(
                        filecontent,
                        ExcelExportHelper.ExcelContentType,
                        String.Format("{0} - report-geocode-csv.xlsx", DateTime.Now.ToString("yyyy-MM-dd"))
                    );

                }

                catch (Exception exc)
                {
                    errore = exc.Message;
                    return null;
                }

            

        }
        public static void GetAttività(string Id, string email,string NameFile, string Path, bool OpenStreetMap, bool Here)
        {
            using(GeoCodeEntities1 db = new GeoCodeEntities1())
            {
               
                Geo_Attività geo_Attività = new Geo_Attività();

                geo_Attività.Utente = email;
                geo_Attività.Id_Utente = Id;
                geo_Attività.Here = Here;
                geo_Attività.OpenStreetMap = OpenStreetMap;
                geo_Attività.PathFile = Path;
                geo_Attività.DescrizioneFile = NameFile;
                geo_Attività.DataAttività = DateTime.Now;
                db.Geo_Attività.Add(geo_Attività);  
                db.SaveChanges();
            }
        }
    }
}






//using (SqlConnection connection = new SqlConnection(connectionString))

//  for (int i = 0; dt.Rows.Count > i; i++)

//    into ProvaTabella values(dt.Rows[i].ItemArray.GetValue(0).ToString(), dt.Rows[i].ItemArray.GetValue(1).ToString(), dt.Rows[i].ItemArray.GetValue(2).ToString());



/*     using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
     {
         foreach (DataColumn c in dt.Columns)
             bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);

         bulkCopy.DestinationTableName = "ProvaTabella";
         try
         {
             bulkCopy.WriteToServer(db.ProvaTabella);
         }
         catch (Exception ex)
         {
             Console.WriteLine(ex.Message);
         }
     }
 }
/*
 using (SqlBulkCopy bulkCopy= new SqlBulkCopy())
 foreach (DataColumn column in dt.Columns)
 {

     provaTabella.Provincia = column.ColumnName;
     provaTabella.Comune = column.ColumnName;
     db.ProvaTabella.Add(provaTabella);
     db.SaveChanges();

 }
     */

//  using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
//  using (CsvReader csvReader =
//    new CsvReader(new StreamReader(stream), true))


//return Json(new { Message = csvTable });



