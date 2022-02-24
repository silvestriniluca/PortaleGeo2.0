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

            ViewBag.IdUtente = new SelectList(db.CSVdati, "IdUtente");
            ViewBag.DescrizioneFile = new SelectList(db.CSVdati, "DescrizioneFile");
            //   ViewBag.Here = new SelectList(db.CSVdati, "Here");
            //   ViewBag.Google = new SelectList(db.CSVdati, "Google");
            //   ViewBag.OpenStreetMap = new SelectList(db.CSVdati, "OpenStreetMap");

           
/*
            ViewBag.Here = new SelectList(new List<SelectListItem>
            {

                new SelectListItem { Selected= false,  Value = "0", Text = "non attivo"},
                new SelectListItem { Selected= true, Value = "1", Text = "attivo"},
                }, "Value", "Text", "Here");

            ViewBag.OpenStreetMap = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected= false,  Value = "0", Text = "non attivo"},
                new SelectListItem { Selected= true, Value = "1", Text = "attivo"},
                }, "Value", "Text", "OpenStreetMap");

            ViewBag.Google = new SelectList(new List<SelectListItem>
            {

                new SelectListItem { Selected= false,  Value = "0", Text = "non attivo"},
                new SelectListItem { Selected= true, Value = "1", Text = "attivo"},
                }, "Value", "Text", "Google");
            */
            /*     ViewBag.SistemaDiGeoreferenzazione = new SelectList(new List<SelectListItem>
                     {
                          new SelectListItem {Text = "", Value = "0" },
                         new SelectListItem { Text = "SistemaOpenStreetMap", Value = "1"},
                         new SelectListItem { Text = "SistemaHERE", Value = "2"},
                        new SelectListItem { Text = "GoogleMaps", Value = "3"},
                     }, "Value", "Text");*/


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]

        public ActionResult Upload(HttpPostedFileBase upload, [Bind(Include = "DescrizioneFile,Here,OpenStreetMap,Google")] CSVdati dati, string SistemaAttivo)
        {


            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {


                    // if (importFile == null) return Json(new { Status = 0, Message = "Nessun file selezionato" });

                    //try
                    //{
                    
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
                        if (dati.OpenStreetMap is true)
                        {
                            OpenStreetMapController.GeoCodeRow(path, _FileName, cf, dt, tablerisultati);

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

                            for (int i = 0; i < tablerisultati.Rows.Count; i++)
                            {
                            }
                                // problema inserimento dati

                                foreach (DataRow row1 in tablerisultati.Rows)
                            {

                              
                                    foreach (DataRow row in dt.Rows)
                                    {
                                  
                                        dtdatabase.Rows.Add(dati.IdUtente, dati.DescrizioneFile, row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], dati.OpenStreetMap, dati.Here, dati.Google, row1["Lat"],
                                                row1["Lon"], row1["Approx01"], row1["Approx02"]);
                                    }
                                    
                                }
                            
                            //string connectionString = @"Data Source=SQL2016CLUST01\MSSQLSERV_C;Initial Catalog=IntraGeoRef;User Id=ut_IntraGeoRef;Password=K1-u9_b745P;";
                            SqlConnection connectionstring = new SqlConnection(@"Data Source=sql2016listen_c, 1733;Initial Catalog=IntraGeoRef;Integrated Security=True");
                            datatablehelper(connectionstring, dtdatabase);
                            //     db.CSVdati.Add(dati);
                            //     db.SaveChanges();
                        }
                       
                        VmUpload vm = new VmUpload(dati, dt);
                        //    db.CSVdati.Add(dati.DescrizioneFile);
                        return View(vm);


                    }


                    //   catch (Exception ex)
                    // {
                    //   return Json(new { Status = 0, Message = ex.Message });
                    //}
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
            return View();
        }
    
//                return Json(new { Status = 1, Message = "File Importato con Successo " });
  //          }
    //    }
            // return Json(new { Message = "File non caricato" });
            
    

       //     Json(new { Status = 1, Message = "File Importato con Successo " });

     

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



