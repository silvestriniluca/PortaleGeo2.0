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
using NuovoPortaleGeo.reader.csv;

namespace NuovoPortaleGeo
{

    public class HomeController : Controller
    {
        GeoCodeEntities1 db = new GeoCodeEntities1();
        public static string SistemaAttivo;

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
                case "":
                    return null;
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

        public ActionResult ValSistemaGeo(string SistemaGeo)
        {
            if (SistemaGeo != null)
            {
                SistemaAttivo = SistemaGeo;
            }
            return Json(new { code = 1 });
        }

        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Upload()
        {

           
            var cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
              ViewBag.FileEsportazione = new SelectList(db.Geo_Dati.Where(s => s.IdUtente == Geo_Utente.Id).GroupBy(p => new {p.DescrizioneFile })
                                                       .Select(g => g.FirstOrDefault()), "DescrizioneFile", "DescrizioneFile") ;

            ViewBag.SistemaGeo = new List<SelectListItem>()
      {
         new SelectListItem() { Text = "OpenStreetMap", Value = "1"},
         new SelectListItem() { Text = "Here", Value = "2" },
         new SelectListItem() {Text="Google", Value="3"}
     };

     

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Upload(HttpPostedFileBase upload ,[Bind(Include = "DescrizioneFile,Here,OpenStreetMap,Google")] Geo_Dati dati)
        {
            
            if(SistemaAttivo!= null)
            {
               
                        switch (SistemaAttivo)
                        {
                       
                        case "1":
                            dati.OpenStreetMap = true;
                            dati.Here = false;
                            dati.Google = false;
                        break;
                            case "2":
                        dati.OpenStreetMap = false;
                        dati.Here = true;
                        dati.Google = false;
                        break;
                             case "3":
                        dati.OpenStreetMap = false;
                        dati.Here = false;
                        dati.Google = true;
                        break;

                        }
            }
            else
            {
                dati.OpenStreetMap = false;
                dati.Here = false;
                dati.Google = false;
            }
        

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
                        string IdUnivoco = Geo_Utente.Id;
                        
                        string pathfolder = Path.Combine(Server.MapPath("~/Upload"), IdUnivoco);
                        string path = Path.Combine(Server.MapPath("~/Upload/" + IdUnivoco), _FileName);
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
                        dtdatabase.Columns.Add("Here_MatchLevel");
                        dtdatabase.Columns.Add("Here_MatchType");
                        dtdatabase.Columns.Add("Here_Relevance");
                        dtdatabase.Columns.Add("Here_Error");

                        foreach (DataColumn col in dt.Columns)
                        {

                            if (col.ColumnName == "Provincia")
                            {
                                dtdatabase.Columns.Add("Provincia");

                            }
                            else if (col.ColumnName == "Comune")
                            {
                                dtdatabase.Columns.Add("Comune");

                            }
                            else if (col.ColumnName == "Indirizzo")
                            {
                                dtdatabase.Columns.Add("Indirizzo");

                            }
                            else if (col.ColumnName == "DENOMINAZIONE")
                            {
                                dtdatabase.Columns.Add("Denominazione");

                            }
                        }
                            List<string> listItems = new List<string>();

                        foreach (DataColumn colonna in dt.Columns)
                        {
                            var colonna_nome = colonna.ColumnName;
                           
                            listItems.Add(colonna_nome);
                        }
                        ViewBag.Provincia = listItems;
                        ViewBag.Comune = listItems;
                        ViewBag.Indirizzo = listItems;
                        ViewBag.AltroIndirizzo = listItems;
                        ViewBag.N_Civico = listItems;
                        ViewBag.Cap = listItems;
                        ViewBag.DescrizioneGeo = listItems;


                        if (dati.OpenStreetMap is true)
                        {
                            OpenStreetMapController.GeoCodeRow(path, _FileName, cf, dt, tablerisultati, _FileName, path);
                         
                            foreach (DataRow row in tablerisultati.Rows)
                            {

                                dtdatabase.Rows.Add(dati.IdUtente, dati.DescrizioneFile, row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], dati.OpenStreetMap, dati.Here, dati.Google, row["Lat"],
                                        row["Lon"], row["Approx01"], row["Approx02"],null,null,null,null);
                               
                            }
                            SqlConnection connectionstring = new SqlConnection(@"Data Source=sql2016listen_c, 1733;Initial Catalog=IntraGeoRef;Integrated Security=True");
                            datatablehelper(connectionstring, dtdatabase);
                        }
                            if (dati.Here is true)
                            {
                            SistemaHereController.Upload(path, conf, _FileName,tablerisultati,cf);
                            foreach (DataRow row in tablerisultati.Rows)
                            {

                                dtdatabase.Rows.Add(dati.IdUtente, dati.DescrizioneFile, row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], dati.OpenStreetMap, dati.Here, dati.Google, row["Here_Latitude"],
                                        row["Here_Longitude"], null,null,row["Here_MatchLevel"],row["Here_MatchType"], row["Here_Relevance"],row["Here_Error"]);
                            }
                            SqlConnection connectionstring = new SqlConnection(@"Data Source=sql2016listen_c, 1733;Initial Catalog=IntraGeoRef;Integrated Security=True");
                            datatablehelper(connectionstring, dtdatabase);
                        }
                        VmUpload vm = new VmUpload(dati, dt);
                        VmUpload vmGeo = new VmUpload(dati, tablerisultati);
                     
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
        
        public static void GetAttività(string Id, string email,string NameFile, string Path, bool OpenStreetMap, bool Here,int TotRighe,int GeoRighe, TimeSpan timeSpan)
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
                geo_Attività.RigheTotali = TotRighe;
                geo_Attività.RigheGeoreferenziate = GeoRighe;
                geo_Attività.TempoImpiegato = timeSpan;
                db.Geo_Attività.Add(geo_Attività);  
                db.SaveChanges();
            }
        }
    }
}









