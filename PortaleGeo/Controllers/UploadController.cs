using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using NuovoPortaleGeo.Controllers;
using NuovoPortaleGeo.Models;
using NuovoPortaleGeo.ViewModels;
using PortaleGeoWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Windows;

namespace NuovoPortaleGeo.Controllers
{
    
    public class UploadController : Controller
    {
        GeoCodeEntities1 db = new GeoCodeEntities1();
        public static string filepath;
        public static string cf;
        public static int righeGeo;       
        public static string user;
        public static int GeoNoRighe;
        public static TimeSpan time;
        public static string JSONcorrispondeze;
        public static DataTable tabellarisultati;


  
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Upload()
        {

            return View();
        }


        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Geocodifica(string FileName ,string SistemaGeo, string DatiJSON, string CorrispondenzeJSON, int TotRighe, string risultati)
        {
           // Geo_Attività _Attività = new Geo_Attività();
            Geo_Dati dati = new Geo_Dati();
            dati = SelectSystemGeo(SistemaGeo, FileName);
            if (CorrispondenzeJSON == null) { }
            else 
            {
               
                GeoNoRighe = 0;
                JSONcorrispondeze = CorrispondenzeJSON;
                //salvataggio dati Geo_Attività
                HomeController.GetAttività(user,dati.IdUtente, FileName, filepath, (bool)dati.OpenStreetMap, (bool)dati.Here, TotRighe,0);
            }
            
            righeGeo--;
            var stopWatch = new Stopwatch();          
            int GeoRef;     
            
            if (stopWatch.IsRunning) { }
            else stopWatch.Start();

            DataTable dtdatabase = new DataTable();
            Dictionary<string, object> dati_da_georef = null;
            
            try
            {
                dati_da_georef = JsonConvert.DeserializeObject<Dictionary<string, object>>(DatiJSON);
                
            }
            catch { }
            Dictionary<string, object> corrispondenze = new Dictionary<string, object>();
           
            try
            {
                corrispondenze = JsonConvert.DeserializeObject<Dictionary<string, object>>(JSONcorrispondeze);

            }
            catch { }

            if (dati_da_georef != null && dati_da_georef.Count > 0)
            {
                
                CreateTable(dtdatabase);
                // Tramite un altro parametro ottenere la corrispondenza tra dati_da_georefJSON e oggetto GeoCode
                GeoCode geo = GeoCode.CreateFrom(dati_da_georef, corrispondenze);
                
                if (SistemaGeo == "1")
                {
                   
                    geo = OpenStreetMapController.GeoCodeObject(geo);


                    //salvataggio database

                     
                    dati.DescrizioneFile = FileName;
                    dati.Provincia = geo.Provincia;
                    dati.Comune = geo.Comune;
                    dati.Indirizzo = geo.Indirizzo;
                    dati.AltroIndirizzo = geo.AltroIndirizzo;
                    dati.Cap = geo.Cap;
                    dati.Descrizione = geo.Denominazione;              
                    dati.Lat = geo.Lat.ToString();
                    dati.Lon = geo.Lon.ToString();
                    dati.Approx01 = geo.Approx01.ToString();
                    dati.Approx02 = geo.Approx02.ToString();
                    dati.Here_MatchLevel = null;
                    dati.Here_MatchType = null;
                    dati.Here_Relevance = null;
                    dati.Here_Error = null;
                    db.Geo_Dati.Add(dati);
                    db.SaveChanges();

        
                    if (geo.Lat == 0 || geo.Lon==0)
                    {
                        GeoNoRighe++;
                    }
                }
                if (SistemaGeo == "2")
                {
                  
                    geo = GeocodeProcessor.EsecuteGecoding(geo, GeoNoRighe);
                    dati.DescrizioneFile = FileName;
                    dati.Provincia = geo.Provincia;
                    dati.Comune = geo.Comune;
                    dati.Indirizzo = geo.Indirizzo;
                    dati.AltroIndirizzo = geo.AltroIndirizzo;
                    dati.Cap = geo.Cap;
                    dati.Descrizione = geo.Denominazione;
                    dati.Lat = geo.Lat.ToString();
                    dati.Lon = geo.Lon.ToString();
                    dati.Approx01 = "";
                    dati.Approx02 = "";
                    dati.Here_MatchLevel = geo.Here_MatchLevel;
                    dati.Here_MatchType = geo.Here_MatchType;
                    dati.Here_Relevance = geo.Here_Relevance;
                    dati.Here_Error = geo.Here_Error;
                    db.Geo_Dati.Add(dati);
                    db.SaveChanges();
                 
                }
                
          //      datasavedb(dtdatabase);
               
                
                 time = stopWatch.Elapsed + time;

                if (righeGeo==0)
                {
                    GeoRef = TotRighe - GeoNoRighe;
                    stopWatch.Stop();                  
                    uploadDBAttività(time, dati.IdUtente, GeoRef, FileName);
                    time =TimeSpan.Zero ;
                }
                
               

                return Content(JsonConvert.SerializeObject(geo), "application/json");
            }
            return null;
        }
            

/*

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
      
           SqlConnection connectionstring = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);       
            datatablehelper(connectionstring, dtdatabase);
        }
*/
        //visualizza tabella risultati
        public DataTable getrisultati()
        {
            DataTable table = new DataTable();
            return table;
               
        }
        public static void uploadDBAttività(TimeSpan time,  string idutente, int righe, string descrizionefile)
        {
            SqlConnection connectionstring = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            try
            {
                 var sql = @"UPDATE [IntraGeoRef].[dbo].Geo_Attività SET RigheGeoreferenziate=@RigheGeoreferenziate, TempoImpiegato=@TempoImpiegato WHERE DescrizioneFile=@DescrizioneFile AND Id_Utente=@Id_Utente";
                connectionstring.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, connectionstring);

                sqlCommand.Parameters.Add("@RigheGeoreferenziate", SqlDbType.Int).Value = righe;
                sqlCommand.Parameters.Add("@TempoImpiegato", SqlDbType.Time).Value = time;
                sqlCommand.Parameters.Add("@DescrizioneFile", SqlDbType.NVarChar).Value = descrizionefile;
                sqlCommand.Parameters.Add("@Id_Utente", SqlDbType.NVarChar).Value = idutente;
                sqlCommand.ExecuteNonQuery();
                connectionstring.Close();
                

            }

            catch (Exception e)
            {
                MessageBox.Show($"Failed to update. Error message: {e.Message}");
            }

        }


        private DataTable CreateTable(DataTable dtdatabase)
        {
           
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
            dtdatabase.Columns.Add(("DENOMINAZIONE"));
            dtdatabase.Columns.Add("Provincia");
            dtdatabase.Columns.Add("Comune");
            dtdatabase.Columns.Add("Indirizzo");
    
            return dtdatabase;
        }


        public Geo_Dati SelectSystemGeo(string SistemaAttivo, string FileName)
        {
            Geo_Dati dat_i = new Geo_Dati();
            var Geo_Utente = db.Geo_Utente
                   .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
            ViewBag.FileEsportazione = new SelectList(db.Geo_Dati.Where(s => s.IdUtente == Geo_Utente.Id).GroupBy(p => new { p.DescrizioneFile })
                                                     .Select(g => g.FirstOrDefault()), "DescrizioneFile", "DescrizioneFile");
            user = Geo_Utente.UserName;
            dat_i.IdUtente = Geo_Utente.Id;
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
            
            GeoDatiController._File = FileName;          
    

            return dat_i;
                

        }


        // VISUALIZZAZIONE
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public  ActionResult DatiJSON(string descrizione)
        {
            // dati.DescrizioneFile  finire come fare?


            if (Request.Files.Count > 0)
            {
                //string delimiter = detectDelimiter(Request.Files[0].InputStream);


                // Salvataggio del file nella cartella utente
                // TODO: Parametro per indicare se salvare o meno il file
                string NameFile = Request.Files[0].FileName;                
                SaveFile(NameFile, Request.Files[0].InputStream);

                // Impostazione configurazione csv
                // TODO: Pensare alla gestione anche di file non CSV
                CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                
                // TODO: Parametro per indicare la codifica se diversa da UTF7 (ANSI)
                conf.Encoding = System.Text.Encoding.UTF7;
                conf.BadDataFound = null;
                //conf.Delimiter = ";";
                conf.DetectDelimiter = true;
                conf.HasHeaderRecord = true;

                // Lettura file csv e caricamento in tabella
                DataTable table = new DataTable();
                Request.Files[0].InputStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(Request.Files[0].InputStream, System.Text.Encoding.UTF7))
                { 
                    using (var csv = new CsvHelper.CsvReader(reader, conf))
                    {
                        var dr = new CsvDataReader(csv);
                        table.Load(dr);
                    }
                }

                // Aggiunto una colonna con il numero di riga
                DataColumn Col = table.Columns.Add("#", typeof(Int32)); 
                Col.SetOrdinal(0);

                righeGeo = 0;
                foreach (DataRow row in table.Rows)
                {
                    righeGeo += 1;
                    row["#"] = righeGeo;
                }

                var dati = JsonConvert.SerializeObject(table);             

                return Content(dati, "application/json", System.Text.Encoding.UTF8);
            }
            else
                return HttpNotFound();

        }

        public bool SaveFile(string File_Name, System.IO.Stream Input_Stream)
        {
            try
            {
                Geo_Dati dati = new Geo_Dati();        
                // verifico l'utente loggato e gli associo il numero di file caricato
                cf = Session["CF"].ToString();
                var Geo_Utente = db.Geo_Utente
                    .Where(x => x.CodiceFiscale == cf).FirstOrDefault();
                dati.IdUtente = Geo_Utente.Id;
                string IdUnivoco = Geo_Utente.Id;
                string dirname = System.IO.Path.Combine(Server.MapPath("~/Upload"), IdUnivoco);
                filepath = System.IO.Path.Combine(Server.MapPath("~/Upload/" + IdUnivoco), Path.GetFileNameWithoutExtension(File_Name) + "_" + DateTime.Now.ToString("yyyyMMdd'-'HHmmss") + Path.GetExtension(File_Name));
                if (!System.IO.Directory.Exists(dirname))
                    System.IO.Directory.CreateDirectory(dirname);

                if (System.IO.Directory.Exists(dirname))
                {
                    using (var fileStream = System.IO.File.Create(filepath))
                    {
                        Input_Stream.Seek(0, SeekOrigin.Begin);
                        Input_Stream.CopyTo(fileStream);
                    }
                }
                return true;
            } 
            catch (Exception ex)
            {
                // TODO: Aggiungere una gestione degli errori con log su file
                return false;
            }
        }
    }
}
