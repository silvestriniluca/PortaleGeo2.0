using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using NuovoPortaleGeo.Controllers;
using NuovoPortaleGeo.Models;
using NuovoPortaleGeo.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace NuovoPortaleGeo.Controllers
{
    
    public class UploadController : Controller
    {
        GeoCodeEntities1 db = new GeoCodeEntities1();
        public static string SistemaAttivo;
        public static DataTable dt;
        public static string path;
        public static string _FileName;
        public static string cf;
        public static Geo_Dati dati;
        public static CsvConfiguration conf ;
        public static string Git;
        public string Gittest;

        

        // GET: Upload

        public ActionResult ValSistemaGeo(string SistemaGeo)
        {
            Geo_Dati geo = new Geo_Dati();
            SistemaAttivo = SistemaGeo;
            SelectSystemGeo(SistemaAttivo, geo);
            //  if (SistemaGeo != null)
            Gittest = "";
            if(Gittest!="")
            {

            }
            else
            {

            }
          if(SistemaGeo!= null)
            {
                return Json(new { code = 1 });
            }
            return null;
        }

        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Upload()
        {
            ViewBag.SistemaGeo = new List<SelectListItem>()
           
         {
         new SelectListItem() { Text = "OpenStreetMap", Value = "1"},
         new SelectListItem() { Text = "Here", Value = "2" },
         new SelectListItem() {Text="Google", Value="3"}
         };
            


            return View();
        }

       
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Geocodifica()
        {

           
            DataTable tablerisultati = new DataTable();
            var dtdatabase = CreateTable(dt);

                     // List<string> listItems = new List<string>();
            /*
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

            */
            //OPENSTREETMAP
                        if (dati.OpenStreetMap is true)
                        {
                            OpenStreetMapController.GeoCodeRow(path, _FileName, cf, dt, tablerisultati, _FileName, path);

                foreach (DataRow row in tablerisultati.Rows)
                {

                    dtdatabase.Rows.Add(dati.IdUtente, dati.DescrizioneFile, row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], dati.OpenStreetMap, dati.Here, dati.Google, row["Lat"],
                            row["Lon"], row["Approx01"], row["Approx02"], null, null, null, null);

                }
                datasavedb(dtdatabase);
                 }
            //HERE
                        if (dati.Here is true)
                        {
                            DataColumnCollection colonna = dt.Columns;
                        try
                        {
                        if (colonna.Contains("Here_Latitude") || colonna.Contains("Here_Longitude"))
                        {

                        }
                        else
                        {

                            SistemaHereController.Upload(path, conf, _FileName, tablerisultati, cf);
                            DataColumnCollection colonnarisultati = tablerisultati.Columns;
                        if (colonnarisultati.Contains("DENOMINAZIONE"))
                        { }
                        else
                        {
                            tablerisultati.Columns.Add("DENOMINAZIONE");
                        }
                        foreach (DataRow row in tablerisultati.Rows)
                        {

                            dtdatabase.Rows.Add(dati.IdUtente, dati.DescrizioneFile, row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], dati.OpenStreetMap, dati.Here, dati.Google, row["Here_Latitude"],
                                    row["Here_Longitude"], null, null, row["Here_MatchLevel"], row["Here_MatchType"], row["Here_Relevance"], row["Here_Error"]);
                        }
                        datasavedb(dtdatabase);

                        }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                          
                        }
                         //    VmUpload vm = new VmUpload(dati, dt);
                         //    VmUpload vmGeo = new VmUpload(dati, tablerisultati);
                          
            
                        if (tablerisultati.Rows.Count > 0)
                        {

                             return Json(new { code = 1 }); 

                        }
                        else
                        {
                             //errore
                             return Json(new { code = 2 });
                        }
            


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



        public static void datasavedb(DataTable dtdatabase)
        {
      
           SqlConnection connectionstring = new SqlConnection(@"Data Source=sql2016listen_c, 1733;Initial Catalog=IntraGeoRef;Integrated Security=True");
            datatablehelper(connectionstring, dtdatabase);
        }



        private DataTable CreateTable(DataTable dt)
        {
            DataTable dtdatabase = new DataTable();
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
            DataColumnCollection columns = dt.Columns;

            if (columns.Contains("DENOMINAZIONE"))
            {

            }
            else
            {
                dtdatabase.Columns.Add(new DataColumn("DENOMINAZIONE"));
            }
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

                    dtdatabase.Columns.Add("DENOMINAZIONE");


                }
              
                    
                    
            }
            return dtdatabase;
        }


        public void SelectSystemGeo(string SistemaAttivo, Geo_Dati dat_i)
        {
            var cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
            ViewBag.FileEsportazione = new SelectList(db.Geo_Dati.Where(s => s.IdUtente == Geo_Utente.Id).GroupBy(p => new { p.DescrizioneFile })
                                                     .Select(g => g.FirstOrDefault()), "DescrizioneFile", "DescrizioneFile");
            //seleziona sistema geofereferenzazione  
            if (SistemaAttivo != null && SistemaAttivo!="")

            {

                switch (SistemaAttivo)
                {

                    case "1":
                        dat_i.OpenStreetMap = true;
                        dat_i.Here = false;
                        dat_i.Google = false;
                        break;
                    case "2":
                        dat_i.OpenStreetMap = false;
                        dat_i.Here = true;
                        dat_i.Google = false;
                        break;
                    case "3":
                        dat_i.OpenStreetMap = false;
                        dat_i.Here = false;
                        dat_i.Google = true;
                        break;

                }
            }
            else
            {
                dat_i.OpenStreetMap = false;
                dat_i.Here = false;
                dat_i.Google = false;
            }
            dati = dat_i;
            dati.DescrizioneFile = _FileName;
            dati.IdUtente = Geo_Utente.Id;
            GeoDatiController._File = _FileName;
        }


        // VISUALIZZAZIONE
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public  ActionResult DatiJSON(string descrizione)
        {
            // dati.DescrizioneFile  finire come fare?
            

             if (Request.Files.Count > 0)
            {
                //string delimiter = detectDelimiter(Request.Files[0].InputStream);
                
                CsvConfiguration con = new CsvConfiguration(CultureInfo.InvariantCulture);
                conf = con;
                conf.BadDataFound = null;
                //conf.Delimiter = ";";
                conf.DetectDelimiter = true;
                conf.HasHeaderRecord = true;
                var reader = new StreamReader(Request.Files[0].InputStream);
                var csv = new CsvHelper.CsvReader(reader, conf);
                string NameFile = Request.Files[0].FileName;
                _FileName = descrizione;
                
               SaveFile(NameFile);
              
                DataTable table = new DataTable();
                
                //  using (var streamReader = File.
                // using (var csvReader = new CsvReader(streamreade, CultureInfo.CurrentCulture)) ;
                var dr = new CsvDataReader(csv);           
                table.Load(dr);
                dt = table;
                var dati = JsonConvert.SerializeObject(dt);             
              
                return Content(dati, "application/json");
            }
            else
            
                return HttpNotFound();
            
            
           

        }
        public void SaveFile(string Name_File)
        {
            Geo_Dati dati = new Geo_Dati();
            // verifico l'utente loggato e gli associo il numero di file caricato
            cf = Session["CF"].ToString();
            var Geo_Utente = db.Geo_Utente
                       .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
            dati.IdUtente = Geo_Utente.Id;
            string IdUnivoco = Geo_Utente.Id;            
            string pathfolder = Path.Combine(Server.MapPath("~/Upload"), IdUnivoco);
            path = Path.Combine(Server.MapPath("~/Upload/" + IdUnivoco), Name_File);
            if (!Directory.Exists(pathfolder))
            {
                Directory.CreateDirectory(pathfolder);

            }
            
                
        }
        }
    }
