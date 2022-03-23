using CsvHelper;
using CsvHelper.Configuration;
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

namespace NuovoPortaleGeo.Controllers
{

    public class ProvaController : Controller
    {
        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult Datatable()
        {
            return View();
        }

        [Authorize(Roles = "Amministratore,Utente,Consultatore")]
        public ActionResult DatiJSON()
        {
            if (Request.Files.Count > 0)
            {
                //string delimiter = detectDelimiter(Request.Files[0].InputStream);

                CsvConfiguration conf = new CsvConfiguration(CultureInfo.InvariantCulture);
                conf.BadDataFound = null;
                //conf.Delimiter = ";";
                conf.DetectDelimiter = true;
                conf.HasHeaderRecord = true;
                var reader = new StreamReader(Request.Files[0].InputStream);
                var csv = new CsvHelper.CsvReader(reader, conf);


                //  using (var streamReader = File.
                // using (var csvReader = new CsvReader(streamreade, CultureInfo.CurrentCulture)) ;
                var dr = new CsvDataReader(csv);
                DataTable tablerisultati = new DataTable();

                var dati =
                "[" +
                "   {" +
                "       \"nome\": \"Tina Mukherjee\"," +
                "       \"indirizzo\": \"BPO member\"," +
                "       \"città\": \"Pune\"," +
                "       \"prova\": \"Pune\"" +
                "   }," +
                "   {" +
                "       \"nome\": \"Gaurav\"," +
                "       \"indirizzo\": \"Teacher\"," +
                "       \"città\": \"Pune\"," +
                "       \"prova\": \"Pune\"" +
                "   }" +
                "]";

                return Content(dati, "application/json");
            }
            else
                return HttpNotFound();
        }

        /*
        private string detectDelimter(Stream stream)
        {
            string delimiter = ";";
            using (StreamReader reader = new StreamReader(stream))
            {
                while (string line = reader.ReadLine()) {
                     line.Split(";")
                        line.Split(";")
                        line.Split(";")
                }
            }

            return delimiter;
        }*/
    }
}