using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PortaleGeoWeb.ViewModels;
using PortaleGeoWeb;
using PortaleGeoWeb.Helpers;
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
using PortaleGeoWeb.Models;
using System.Web.Script.Serialization;

namespace PortaleGeoWeb.Controllers
{
    [Authorize(Roles = "Administrators,EnteLocale,Fornitore")]
    // GET: SistemaOpenStreetMap
    public class GeoCode
    {
        public int Id { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        public string Approx01 { get; set; }
        public string Approx02 { get; set; }

        public string Cap { get; set; }
        public string Village { get; set; }

        public GeoCode(int Id, double Lat, double Lon, string Approx01, string Approx02, string Cap, string Village)
        {
            this.Id = Id;
            this.Lat = Lat;
            this.Lon = Lon;
            this.Approx01 = Approx01;
            this.Approx02 = Approx02;
            this.Cap = Cap;
            this.Village = Village;
        }

    }


    public class OpenStreetMapController : Controller
    {

        private GeoCodeEntities1 db = new GeoCodeEntities1();

        // GET: GeoCodeCSV
        public ActionResult Index()
        {
            return View(db.OpenStreetMapCSV.ToList());
        }

        // GET: GeoCodeCSV/Details/5
        public ActionResult Dettaglio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenStreetMapCSV opeNStreetMap_CSV = db.OpenStreetMapCSV.Find(id);
            if (opeNStreetMap_CSV == null)
            {
                return HttpNotFound();
            }

            var vm = new VmGeoCodeOpenStreetMap(opeNStreetMap_CSV);
            return View(vm);
        }

        // GET: GeoCodeCSV/Create
        public ActionResult Aggiungi()
        {
            return View();
        }

        // POST: GeoCodeCSV/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Aggiungi([Bind(Include = "Id,Indirizzo,N_Civico,Comune,Provincia,Regione,Note1,Note2,Lat,Lon,Approx01,Approx02,Cap,AltroIndirizzo,APIGoogle")] OpenStreetMapCSV opeNStreetMap_CSV)
        {
            if (ModelState.IsValid)
            {
                db.OpenStreetMapCSV.Add(opeNStreetMap_CSV);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(opeNStreetMap_CSV);
        }

        // GET: GeoCodeCSV/Edit/5
        public ActionResult Modifica(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenStreetMapCSV opeNStreetMap_CSV = db.OpenStreetMapCSV.Find(id);
            if (opeNStreetMap_CSV == null)
            {
                return HttpNotFound();
            }


            var vm = new VmGeoCodeOpenStreetMap(opeNStreetMap_CSV);
            return View(vm);

        }

        // POST: GeoCodeCSV/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifica([Bind(Include = "Id,Indirizzo,N_Civico,Comune,Provincia,Regione,Note1,Note2,Lat,Lon,Approx01,Approx02,Cap,AltroIndirizzo,APIGoogle")] OpenStreetMapCSV opeNStreetMap_CSV)
        {
            if (ModelState.IsValid)
            {
                db.Entry(opeNStreetMap_CSV).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var vm = new VmGeoCodeOpenStreetMap(opeNStreetMap_CSV);
            return View(vm);

        }

        // GET: GeoCodeCSV/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpenStreetMapCSV opeNStreetMap_CSV = db.OpenStreetMapCSV.Find(id);
            if (opeNStreetMap_CSV == null)
            {
                return HttpNotFound();
            }

            var vm = new VmGeoCodeOpenStreetMap(opeNStreetMap_CSV);
            return View(vm);

        }

        // POST: GeoCodeCSV/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OpenStreetMapCSV opeNStreetMap_CSV = db.OpenStreetMapCSV.Find(id);
            db.OpenStreetMapCSV.Remove(opeNStreetMap_CSV);
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
        public ActionResult ImportFile(HttpPostedFileBase importFile)
        {
            if (importFile == null) return Json(new { Status = 0, Message = "Nessun file selezionato" });

            try
            {
                var fileData = GetDataFromCSVFile(importFile.InputStream);

                foreach (OpenStreetMapCSV dr in fileData)
                {
                    db.OpenStreetMapCSV.Add(dr);
                    db.SaveChanges();
                }

                return Json(new { Status = 1, Message = "File Importato con Successo " });
            }
            catch (Exception ex)
            {
                return Json(new { Status = 0, Message = ex.Message });
            }
        }

        private List<OpenStreetMapCSV> GetDataFromCSVFile(Stream stream)
        {
            var MM_GeoCodeCSV_List = new List<OpenStreetMapCSV>();
            try
            {
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true // To set First Row As Column Names  
                        }
                    });

                    if (dataSet.Tables.Count > 0)
                    {
                        var dataTable = dataSet.Tables[0];
                        foreach (DataRow objDataRow in dataTable.Rows)
                        {
                            if (objDataRow.ItemArray.All(x => string.IsNullOrEmpty(x?.ToString()))) continue;
                            //GeoCode geo = GeoCode(Convert.ToInt32(objDataRow["Id"].ToString()), objDataRow["Indirizzo"].ToString(), objDataRow["Comune"].ToString(), objDataRow["Provincia"].ToString());

                            MM_GeoCodeCSV_List.Add(new OpenStreetMapCSV()
                            {
                                Id = Convert.ToInt32(objDataRow["Id"].ToString()),
                                Indirizzo = objDataRow["Indirizzo"].ToString(),
                                N_Civico = objDataRow["N_Civico"].ToString(),
                                Comune = objDataRow["Comune"].ToString(),

                                Provincia = objDataRow["Provincia"].ToString(),

                                Regione = objDataRow["Regione"].ToString(),

                                Note1 = objDataRow["Note1"].ToString(),
                                Note2 = objDataRow["Note2"].ToString(),
                                Lat = "",
                                Lon = "",

                                Approx01 = "",
                                Approx02 = "",

                                Cap = objDataRow["Cap"].ToString(),
                                AltroIndirizzo = objDataRow["AltroIndirizzo"].ToString(),

                                APIGoogle = objDataRow["APIGoogle"].ToString(),

                            });
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return MM_GeoCodeCSV_List;
        }

        //DeleteAll
        public ActionResult DeleteAll()
        {
            foreach (var row in db.OpenStreetMapCSV)
            {
                db.OpenStreetMapCSV.Remove(row);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GeoCodeRow

        public ActionResult GeoCodeRow()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            foreach (var row in db.OpenStreetMapCSV)
            {

                if (row.APIGoogle == "S")
                {

                    GeoCode geo = GeoCode_Google(row.Id, row.Indirizzo, row.Comune, row.Provincia, "Diretta", row.Cap, "API_Google");
                    row.Lat = geo.Lat.ToString();
                    row.Lon = geo.Lon.ToString();
                    row.Approx01 = geo.Approx01;
                    row.Approx02 = geo.Approx02;


                }
                else
                {
                    //Inizialmente calcolo il Baricentro Comune
                    GeoCode geox = GeoCode(row.Id, "", row.Comune, row.Provincia, "Baricentro Comune", row.Cap);

                    //Provo la ricerca DIRETTA
                    GeoCode geo = GeoCode(row.Id, row.Indirizzo, row.Comune, row.Provincia, "Diretta", row.Cap);
                    //Se con la ricerca Diretta trovo il Baricentro Comune, continuo la ricerca
                    if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                    //Provo la ricerca con quanto presente in Altro Indirizzo
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.AltroIndirizzo != "") geo = GeoCode(row.Id, row.AltroIndirizzo, row.Comune, row.Provincia, "Altro Indirizzo", row.Cap);
                    //Se con la ricerca AltroIndirizzo trovo il Baricentro Comune, continuo la ricerca
                    if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                    //Provo la ricerca con Toponomastica AltroIndirizzo
                    string AltroIndirizzo = row.AltroIndirizzo.Trim();
                    string Toponomastica_AltroIndirizzo = AltroIndirizzo.Remove(0, AltroIndirizzo.IndexOf(' ') + 1);
                    if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica_AltroIndirizzo.Trim() != "") geo = GeoCode(row.Id, Toponomastica_AltroIndirizzo, row.Comune, row.Provincia, "Altro Indirizzo", row.Cap);
                    //Se con la ricerca Toponomastica_AltroIndirizzo trovo il Baricentro Comune, continuo la ricerca
                    if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                    //Provo con la ricerca DIRETTA_TOPONOMASTICA, tolgo la prima parte dell'indirizzo (qualunque cosa essa sia) così da prendere solo la toponomastica 
                    if ((geo.Lat == 0 || geo.Lon == 0))
                    {
                        string Indirizzo = row.Indirizzo.Trim();
                        string Toponomastica = Indirizzo.Remove(0, Indirizzo.IndexOf(' ') + 1);
                        //Toponomastica = Toponomastica.Substring(0, Toponomastica.LastIndexOf(' '));

                        geo = GeoCode(row.Id, Toponomastica, row.Comune, row.Provincia, "Diretta_Toponomastica", row.Cap);
                        //Se con la ricerca Diretta_Toponomastica trovo il Baricentro Comune, continuo la ricerca
                        if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                        //Provo togliendo l'ultima parte della toponomastica
                        if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica.Contains(" "))
                        {
                            Toponomastica = Toponomastica.Substring(0, Toponomastica.LastIndexOf(' '));
                            geo = GeoCode(row.Id, Toponomastica, row.Comune, row.Provincia, "Senza Numero Civico", row.Cap);
                            //Se con la ricerca Senza Numero Civico trovo il Baricentro Comune, continuo la ricerca
                            if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;

                            //Provo togliendo un'ulterore prima parte dalla toponomastica
                            if ((geo.Lat == 0 || geo.Lon == 0) && Toponomastica.Contains(" "))
                            {
                                Toponomastica = Toponomastica.Remove(0, Toponomastica.IndexOf(' ') + 1);
                                geo = GeoCode(row.Id, Toponomastica, row.Comune, row.Provincia, "Successiva_Toponomastica", row.Cap);
                            }
                        }
                    }
                    //Se con la ricerce effettuate trovo il Baricentro Comune, continuo con la ricerca Successiva
                    if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo.Lat = 0;
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("DELL' ")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "DELL' ", "DELL'", row.Cap);
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains(" I ")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, " I ", " PRIMO ", row.Cap);
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains(" 8 ")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, " 8 ", " OTTO ", row.Cap);


                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("A.")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "A.", "", row.Cap);
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("C.")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "C.", "", row.Cap);
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("E.")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "E.", "", row.Cap);
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("D.")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "D.", "", row.Cap);

                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("F.LLI")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "F.LLI", "FRATELLI", row.Cap);
                    if ((geo.Lat == 0 || geo.Lon == 0) && row.Indirizzo.Contains("NAZ.LE")) geo = GeoCodeReplace(row.Id, row.Indirizzo, row.Comune, row.Provincia, "NAZ.LE", "NAZIONALE", row.Cap);

                    //Assegno i valori da Baricentro Comune ad Approx01 se coordinate uguali (se con la ricerca Successiva trovo il Baricentro Comune)
                    if (geo.Lat == geox.Lat && geo.Lon == geox.Lon) geo = geox;

                    //Se non viene trovato niente in nessun modo assegno i valori da Baricentro Comune
                    if ((geo.Lat == 0 || geo.Lon == 0)) geo = geox;

                    row.Lat = geo.Lat.ToString();
                    row.Lon = geo.Lon.ToString();
                    row.Approx01 = geo.Approx01;
                    row.Approx02 = geo.Approx02;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public GeoCode GeoCodeReplace(int Id, string Indirizzo, string Comune, string Provincia, string original, string replace, string Cap)
        {

            GeoCode geo = GeoCode(Id, Indirizzo.Replace(original, replace), Comune, Provincia, "Successiva", Cap);

            return geo;
        }


        public GeoCode GeoCode(int Id, string Indirizzo, string Comune, string Provincia, string Approx01, string Cap)
        {
            dynamic result_json;
            var Indirizzo_Full = Indirizzo.Trim() + ", " + Comune.Trim() + ", " + Provincia + ", ITALY";

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

                if (Approx01 != "Baricentro Comune")
                {
                    if (Postcode != "" & Postcode == Cap) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    if (municipality.ToUpper() != "" & municipality.ToUpper() == Comune.ToUpper()) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    if (town.ToUpper() != "" & town.ToUpper() == Comune.ToUpper()) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    if (Village.ToUpper() != "" & Village.ToUpper() == Comune.ToUpper()) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    if (Village.ToUpper() != "" & Village.ToUpper().Contains(Comune.ToUpper())) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    if (Village.ToUpper() != "" & Village.ToUpper() == Indirizzo.ToUpper()) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    if (city.ToUpper() != "" & city.ToUpper() == Comune.ToUpper()) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
                    //if (display_name != "" & display_name.ToUpper().Contains(Comune.ToUpper())) return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);

                    // se non è verificata nessuna delle condizioni precedenti annullo lat/lon
                    lat = 0;
                    lon = 0;

                }

            }

            return new GeoCode(Id, lat, lon, Approx01, Approx02, Cap, Village);
        }


        public GeoCode GeoCode_Google(int Id, string Indirizzo, string Comune, string Provincia, string Approx01, string Cap, string apiKey)
        {

            var Indirizzo_Full = Indirizzo.Trim() + ", " + Comune.Trim() + ", " + Provincia + ", ITALY";
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

                if (comune_google.Trim().ToUpper() == Comune.Trim().ToUpper())
                {
                    double lat_d = (double)lat;
                    double lon_d = (double)lon;
                    string location_type_s = (string)location_type.Value;

                    return new GeoCode(Id, lat_d, lon_d, Approx01, location_type_s, Cap, "");
                }
                else
                {
                    return new GeoCode(Id, 0, 0, Approx01, "NOT FOUND", Cap, "");

                }

            }
            catch (Exception ex)
            {
                return new GeoCode(Id, 0, 0, Approx01, "NOT FOUND", Cap, "");

            }

        }


        //Estrai
        public ActionResult Estrai()
        {
            string errore = null;
            var OpenStreetMapCSV = db.OpenStreetMapCSV;

            var lista_geocode = OpenStreetMapCSV.ToList();

            try
            {
                var contenuto =
                    lista_geocode
                    .Select(x => new
                    {
                        Id = x.Id,
                        Indirizzo = x.Indirizzo,
                        N_Civico = x.N_Civico,
                        Comune = x.Comune,

                        Provincia = x.Provincia,

                        Regione = x.Regione,

                        Note1 = x.Note1,
                        Note2 = x.Note2,
                        Lat = x.Lat,
                        Lon = x.Lon,
                        Approx01 = x.Approx01,
                        Approx02 = x.Approx02,
                        Cap = x.Cap,
                        AltroIndirizzo = x.AltroIndirizzo,
                        APIGoogle = x.APIGoogle
                    })
                    .ToList();

                var columns = new List<string>
                        {
                            "Id",
                            "Indirizzo",
                            "N_Civico",
                            "Comune",

                            "Provincia",

                            "Regione",

                            "Note1",
                            "Note2",
                            "Lat",
                            "Lon",
                            "Approx01",
                            "Approx02",
                            "Cap",
                            "AltroIndirizzo",
                            "APIGoogle"
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
        
        public ActionResult Mappa()
        {

            return View();
        }

       [HttpPost]
        public ActionResult JsonRisultati()
        {

            string errore = null;
            var OpenStreetMapCSV = db.OpenStreetMapCSV;
            var lista_geocode = OpenStreetMapCSV.ToList();

            try
            {
                var contenuto =
                    lista_geocode
                    .Select(x => new
                    {
                        Id = x.Id,
                        Indirizzo = x.Indirizzo,
                        N_Civico = x.N_Civico,
                        Comune = x.Comune,

                        Provincia = x.Provincia,

                        Regione = x.Regione,

                        Note1 = x.Note1,
                        Note2 = x.Note2,
                        Lat = x.Lat,
                        Lon = x.Lon,
                        Approx01 = x.Approx01,
                        Approx02 = x.Approx02,
                        Cap = x.Cap,
                        AltroIndirizzo = x.AltroIndirizzo,
                        APIGoogle = x.APIGoogle
                    })
                    .ToList();

                var columns = new List<string>
                        {
                            "Id",
                            "Indirizzo",
                            "N_Civico",
                            "Comune",

                            "Provincia",

                            "Regione",

                            "Note1",
                            "Note2",
                            "Lat",
                            "Lon",
                            "Approx01",
                            "Approx02",
                            "Cap",
                            "AltroIndirizzo",
                            "APIGoogle"
                        };
            }
            catch (Exception exc)
            {
                errore = exc.Message;
                return null;
            }

            var json_map = Json(lista_geocode, JsonRequestBehavior.AllowGet);



            //  var jsonProductList = new JavaScriptSerializer().Serialize(lista_geocode);

            /*  Response.ClearContent();
              Response.ClearHeaders();
              Response.Buffer = true;
              Response.ContentType = "application/json";
              Response.AddHeader("Content-Length", jsonProductList.Length.ToString());
              Response.AddHeader("Content-Disposition", "allegato; filename=EsportazioneJSON.json;");
              Response.Output.Write(jsonProductList);
              Response.Flush();
              Response.End();*/

            return json_map;
        }
    }
}

    
