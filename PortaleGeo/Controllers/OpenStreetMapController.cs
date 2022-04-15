using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuovoPortaleGeo.ViewModels;
using NuovoPortaleGeo;
using NuovoPortaleGeo.Helpers;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExcelDataReader;
using Newtonsoft.Json;
using NuovoPortaleGeo.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;
using PortaleGeoWeb.Models;

namespace NuovoPortaleGeo.Controllers
{

    // GET: SistemaOpenStreetMap
    [Authorize(Roles = "Amministratore,Utente,Consultatore")]
    public class OpenStreetMapController : Controller
    {


        private GeoCodeEntities1 db = new GeoCodeEntities1();

        public static string Percorso { get; set; }
        public static string Nomepercorso { get; set; }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        //GeoCodeRow

        public static GeoCode GeoCodeObject(GeoCode geo_input)
        {




            //Inizialmente calcolo il Baricentro Comune
            GeoCode geox = GeoCode("", geo_input.Comune, geo_input.Provincia, "Baricentro Comune", geo_input.Cap);

            //Provo la ricerca DIRETTA
            GeoCode geo = GeoCode(geo_input.Indirizzo, geo_input.Comune, geo_input.Provincia, "Diretta", geo_input.Cap);
            //Se con la ricerca Diretta trovo il Baricentro Comune, continuo la ricerca
            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

            //Provo la ricerca con quanto presente in Altro Indirizzo
            if ((geo.Lat == 0 || geo.Lon == 0) && geo_input.AltroIndirizzo.ToString() != "") geo = GeoCode(geo_input.AltroIndirizzo, geo_input.Comune, geo_input.Provincia, "Altro Indirizzo", geo_input.Cap);
            //Se con la ricerca AltroIndirizzo trovo il Baricentro Comune, continuo la ricerca
            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

            //Provo la ricerca con Toponomastica AltroIndirizzo
            if (geo.AltroIndirizzo != null)
            {
                string AltroIndirizzo = geo_input.AltroIndirizzo.ToString().Trim();
                string Toponomastica_AltroIndirizzo = AltroIndirizzo.Remove(0, AltroIndirizzo.IndexOf(' ') + 1);

                if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica_AltroIndirizzo.Trim() != "") geo = GeoCode(Toponomastica_AltroIndirizzo, geo_input.Comune, geo_input.Provincia, "Altro Indirizzo", geo_input.Cap);
            }
            //Se con la ricerca Toponomastica_AltroIndirizzo trovo il Baricentro Comune, continuo la ricerca
            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

            //Provo con la ricerca DIRETTA_TOPONOMASTICA, tolgo la prima parte dell'indirizzo (qualunque cosa essa sia) così da prendere solo la toponomastica 
            if ((geo.Lat == 0 || geo.Lon == 0))
            {
                string Indirizzo = geo_input.Indirizzo.ToString().Trim();
                string Toponomastica = Indirizzo.Remove(0, Indirizzo.IndexOf(' ') + 1);
                //Toponomastica = Toponomastica.Substring(0, Toponomastica.LastIndexOf(' '));

                geo = GeoCode(Toponomastica, geo_input.Comune, geo_input.Provincia, "Diretta_Toponomastica", geo_input.Cap);
                //Se con la ricerca Diretta_Toponomastica trovo il Baricentro Comune, continuo la ricerca
                if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                //Provo togliendo l'ultima parte della toponomastica
                if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica.Contains(" "))
                {
                    Toponomastica = Toponomastica.Substring(0, Toponomastica.LastIndexOf(' '));
                    geo = GeoCode(Toponomastica, geo_input.Comune, geo_input.Provincia, "Senza Numero Civico", geo_input.Cap);
                    //Se con la ricerca Senza Numero Civico trovo il Baricentro Comune, continuo la ricerca
                    if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                    //Provo togliendo un'ulterore prima parte dalla toponomastica
                    if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica.Contains(" "))
                    {
                        Toponomastica = Toponomastica.Remove(0, Toponomastica.IndexOf(' ') + 1);
                        geo = GeoCode(Toponomastica, geo_input.Comune, geo_input.Provincia, "Successiva_Toponomastica", geo_input.Cap);
                    }
                }
            }
            //Se con la ricerce effettuate trovo il Baricentro Comune, continuo con la ricerca Successiva
            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

            Dictionary<string, string> correzioni = new Dictionary<string, string>();
            correzioni.Add("DELL' ", "DELL'");
            correzioni.Add(" I ", " PRIMO ");
            correzioni.Add(" 8 ", " OTTO ");


            correzioni.Add("A.", "");
            correzioni.Add("C.", "");
            correzioni.Add("E.", "");
            correzioni.Add("D.", "");

            correzioni.Add("F.LLI", "FRATELLI");
            correzioni.Add("NAZ.LE", "NAZIONALE");

            foreach (var correzione in correzioni)
                if ((geo.Lat == 0 || geo.Lon == 0) && geo_input.Indirizzo.ToString().Contains(correzione.Key)) geo = GeoCodeReplace(geo_input.Indirizzo, geo_input.Comune, geo_input.Provincia, correzione.Key, correzione.Value, geo_input.Cap);


            //Assegno i valori da Baricentro Comune ad Approx01 se coordinate uguali (se con la ricerca Successiva trovo il Baricentro Comune)
            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo = geox;

            //Se non viene trovato niente in nessun modo assegno i valori da Baricentro Comune
            if ((geo.Lat == 0 || geo.Lon == 0)) geo = geox;
            geo.Provincia = geo_input.Provincia;
            geo.Comune = geo_input.Comune;
            geo.Indirizzo = geo_input.Indirizzo;
            geo.Denominazione = geo_input.Denominazione;
            geo.Cap = geo_input.Cap;
            //    stopWatch.Stop();
            //    TimeSpan ts = stopWatch.Elapsed;
            //    HomeController.GetAttività(Geo_Utente.Id, Geo_Utente.UserName, FileName, Path, true, false, dataTable.Rows.Count, GeoRef, ts);

            return geo;
        }

        public static DataTable GeoCodeRow(string path, string name, string cf, DataTable dataTable, DataTable tablerisultati, string FileName, string Path)
        {

            //devo mettere la tabella

            using (GeoCodeEntities1 db = new GeoCodeEntities1())

            {
                DataColumnCollection columns = dataTable.Columns;

                if (columns.Contains("APIGoogle"))
                {


                }
                else
                {
                    dataTable.Columns.Add(new DataColumn("APIGoogle"));
                }
                if (columns.Contains("DENOMINAZIONE"))
                {

                }
                else
                {
                    dataTable.Columns.Add(new DataColumn("DENOMINAZIONE"));
                }
                if (columns.Contains("AltroIndirizzo"))
                {

                }
                else
                {
                    dataTable.Columns.Add(new DataColumn("AltroIndirizzo"));
                }


                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                var Geo_Utente = db.Geo_Utente
                        .Where(x => x.CodiceFiscale == cf).FirstOrDefault();


                //        tablerisultati = dataTable.Copy();
                tablerisultati.Columns.Add(new DataColumn("Provincia"));
                tablerisultati.Columns.Add(new DataColumn("Comune"));
                tablerisultati.Columns.Add(new DataColumn("Indirizzo"));
                tablerisultati.Columns.Add(new DataColumn("DENOMINAZIONE"));
                tablerisultati.Columns.Add(new DataColumn("Lat"));
                tablerisultati.Columns.Add(new DataColumn("Lon"));
                tablerisultati.Columns.Add(new DataColumn("Approx01"));
                tablerisultati.Columns.Add(new DataColumn("Approx02"));
                int GeoRighe = 0;
                int GeoNoRighe = 0;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                foreach (DataRow row in dataTable.Rows)
                {

                    if (row["APIGoogle"].ToString() == "S")
                    {

                        GeoCode geo = GeoCode_Google(row["Indirizzo"], row["Comune"], row["Provincia"], "Diretta", row["Cap"], "API_Google");

                        tablerisultati.Rows.Add(row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], geo.Lat, geo.Lon, geo.Approx01, geo.Approx02);

                    }
                    else
                    {
                        //Inizialmente calcolo il Baricentro Comune
                        GeoCode geox = GeoCode("", row["Comune"], row["Provincia"], "Baricentro Comune", row["Cap"]);

                        //Provo la ricerca DIRETTA
                        GeoCode geo = GeoCode(row["Indirizzo"], row["Comune"], row["Provincia"], "Diretta", row["Cap"]);
                        //Se con la ricerca Diretta trovo il Baricentro Comune, continuo la ricerca
                        if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                        //Provo la ricerca con quanto presente in Altro Indirizzo
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["AltroIndirizzo"].ToString() != "") geo = GeoCode(row["AltroIndirizzo"], row["Comune"], row["Provincia"], "Altro Indirizzo", row["Cap"]);
                        //Se con la ricerca AltroIndirizzo trovo il Baricentro Comune, continuo la ricerca
                        if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                        //Provo la ricerca con Toponomastica AltroIndirizzo

                        string AltroIndirizzo = row["AltroIndirizzo"].ToString().Trim();
                        string Toponomastica_AltroIndirizzo = AltroIndirizzo.Remove(0, AltroIndirizzo.IndexOf(' ') + 1);
                        if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica_AltroIndirizzo.Trim() != "") geo = GeoCode(Toponomastica_AltroIndirizzo, row["Comune"], row["Provincia"], "Altro Indirizzo", row["Cap"]);
                        //Se con la ricerca Toponomastica_AltroIndirizzo trovo il Baricentro Comune, continuo la ricerca
                        if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                        //Provo con la ricerca DIRETTA_TOPONOMASTICA, tolgo la prima parte dell'indirizzo (qualunque cosa essa sia) così da prendere solo la toponomastica 
                        if ((geo.Lat == 0 || geo.Lon == 0))
                        {
                            string Indirizzo = row["Indirizzo"].ToString().Trim();
                            string Toponomastica = Indirizzo.Remove(0, Indirizzo.IndexOf(' ') + 1);
                            //Toponomastica = Toponomastica.Substring(0, Toponomastica.LastIndexOf(' '));

                            geo = GeoCode(Toponomastica, row["Comune"], row["Provincia"], "Diretta_Toponomastica", row["Cap"]);
                            //Se con la ricerca Diretta_Toponomastica trovo il Baricentro Comune, continuo la ricerca
                            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                            //Provo togliendo l'ultima parte della toponomastica
                            if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica.Contains(" "))
                            {
                                Toponomastica = Toponomastica.Substring(0, Toponomastica.LastIndexOf(' '));
                                geo = GeoCode(Toponomastica, row["Comune"], row["Provincia"], "Senza Numero Civico", row["Cap"]);
                                //Se con la ricerca Senza Numero Civico trovo il Baricentro Comune, continuo la ricerca
                                if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                                //Provo togliendo un'ulterore prima parte dalla toponomastica
                                if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica.Contains(" "))
                                {
                                    Toponomastica = Toponomastica.Remove(0, Toponomastica.IndexOf(' ') + 1);
                                    geo = GeoCode(Toponomastica, row["Comune"], row["Provincia"], "Successiva_Toponomastica", row["Cap"]);
                                }
                            }
                        }
                        //Se con la ricerce effettuate trovo il Baricentro Comune, continuo con la ricerca Successiva
                        if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("DELL' ")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "DELL' ", "DELL'", row["Cap"]);
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains(" I ")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], " I ", " PRIMO ", row["Cap"]);
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains(" 8 ")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], " 8 ", " OTTO ", row["Cap"]);


                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("A.")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "A.", "", row["Cap"]);
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("C.")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "C.", "", row["Cap"]);
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("E.")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "E.", "", row["Cap"]);
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("D.")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "D.", "", row["Cap"]);

                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("F.LLI")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "F.LLI", "FRATELLI", row["Cap"]);
                        if ((geo.Lat == 0 || geo.Lon == 0) && row["Indirizzo"].ToString().Contains("NAZ.LE")) geo = GeoCodeReplace(row["Indirizzo"], row["Comune"], row["Provincia"], "NAZ.LE", "NAZIONALE", row["Cap"]);

                        //Assegno i valori da Baricentro Comune ad Approx01 se coordinate uguali (se con la ricerca Successiva trovo il Baricentro Comune)
                        if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo = geox;

                        //Se non viene trovato niente in nessun modo assegno i valori da Baricentro Comune
                        if ((geo.Lat == 0 || geo.Lon == 0)) geo = geox;
                        if (geo.Lat == 0 && geo.Lon == 0)
                        {

                            GeoNoRighe++;
                        }
                        GeoRighe++;
                        tablerisultati.Rows.Add(row["Provincia"], row["Comune"], row["Indirizzo"], row["DENOMINAZIONE"], geo.Lat, geo.Lon, geo.Approx01, geo.Approx02);
                    }




                }
                int GeoRef = GeoRighe - GeoNoRighe;
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
            //    HomeController.GetAttività(Geo_Utente.Id, Geo_Utente.UserName, FileName, Path, true, false, dataTable.Rows.Count, GeoRef, ts);
                return tablerisultati;
            }







        }


        public static GeoCode GeoCodeReplace(object Indirizzo, object Comune, object Provincia, string original, string replace, object Cap)
        {

            GeoCode geo = GeoCode(Indirizzo.ToString().Replace(original, replace), Comune, Provincia, "Successiva", Cap);

            return geo;
        }


        public static GeoCode GeoCode(object Indirizzo, object Comune, object Provincia, object Approx01, object Cap)
        {
            dynamic result_json;
            string ind = Indirizzo.ToString();
            string Com = Comune.ToString();
            var Indirizzo_Full = ind.Trim() + ", " + Com.Trim() + ", " + Provincia + ", ITALY";

            string uri = $"http://nominatim.openstreetmap.org/?format=json&addressdetails=1&q=" + Indirizzo_Full + "&limit=1";

            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.DefaultConnectionLimit = 9999;
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Sismapp GeoCodeCSV");

            var result_json_string = webClient.DownloadString(uri);
            result_json = Newtonsoft.Json.JsonConvert.DeserializeObject(result_json_string);


            var lat = (double)0;
            var lon = (double)0;
            var Approx02 = "";
            var Postcode = "";
            var Village = "";
            var municipality = "";
            var town = "";
            var city = "";
            var display_name = "";

            foreach (dynamic it in result_json)
            {
                lat = (double)it.lat;
                lon = (double)it.lon;
                Approx02 = it.type;
                display_name = it.display_name == null ? "" : it.display_name;
                Postcode = it.address.postcode == null ? "" : it.address.postcode;
                Village = it.address.village == null ? "" : it.address.village;
                municipality = it.address.municipality == null ? "" : it.address.municipality;
                town = it.address.town == null ? "" : it.address.town;
                city = it.address.city == null ? "" : it.address.city;

                if (Approx01.ToString() != "Baricentro Comune")
                {
                    if (Postcode != "" & Postcode == Cap.ToString()) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    if (municipality.ToUpper() != "" & municipality.ToUpper() == Comune.ToString().ToUpper()) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    if (town.ToUpper() != "" & town.ToUpper() == Comune.ToString().ToUpper()) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    if (Village.ToUpper() != "" & Village.ToUpper() == Comune.ToString().ToUpper()) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    if (Village.ToUpper() != "" & Village.ToUpper().Contains(Comune.ToString().ToUpper())) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    if (Village.ToUpper() != "" & Village.ToUpper() == Indirizzo.ToString().ToUpper()) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    if (city.ToUpper() != "" & city.ToUpper() == Comune.ToString().ToUpper()) return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
                    //if (display_name != "" & display_name.ToUpper().Contains(Comune.ToUpper())) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);

                    // se non è verificata nessuna delle condizioni precedenti annullo lat/lon
                    lat = 0;
                    lon = 0;

                }

            }

            return new GeoCode(lat, lon, Approx01, Approx02, Cap, Village);
        }


        public static GeoCode GeoCode_Google(object Indirizzo, object Comune, object Provincia, object Approx01, object Cap, object apiKey)
        {
            string Ind = Indirizzo.ToString();
            string Com = Comune.ToString();

            var Indirizzo_Full = Ind.Trim() + ", " + Com.Trim() + ", " + Provincia + ", ITALY";
            string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(Indirizzo_Full), apiKey);

            WebRequest request = WebRequest.Create(requestUri);
            WebResponse response = request.GetResponse();
            XDocument xdoc = XDocument.Load(response.GetResponseStream());
            //XDocument xdoc = XDocument.Load("C:/app_db/Response_Google_API_2.xml");


            try
            {
                XElement result = xdoc.Element("GeocodeResponse").Element("result");
                XElement locationElement = result.Element("geometry").Element("location");
                XElement lat = locationElement.Element("lat");
                XElement lon = locationElement.Element("lng");
                XElement location_type = result.Element("geometry").Element("location_type");

                var comune_google = "";

                var address_components = from d in result.Elements("address_component")
                                         select new
                                         {
                                             type = (string)d.Element("type"),
                                             short_name = (string)d.Element("short_name"),
                                             long_name = (string)d.Element("long_name"),
                                         };

                foreach (var address_component in address_components)
                {
                    if (address_component.type == "administrative_area_level_3")
                        comune_google = address_component.long_name;

                }

                if (comune_google.Trim().ToUpper() == Com.Trim().ToUpper())
                {
                    double lat_d = (double)lat;
                    double lon_d = (double)lon;
                    string location_type_s = (string)location_type.Value;

                    return new GeoCode(lat_d, lon_d, Approx01, location_type_s, Cap, "");
                }
                else
                {
                    return new GeoCode(0, 0, Approx01, "NOT FOUND", Cap, "");

                }

            }
            catch (Exception ex)
            {
                return new GeoCode(0, 0, Approx01, "NOT FOUND", Cap, "");

            }

        }


    }
}

    
