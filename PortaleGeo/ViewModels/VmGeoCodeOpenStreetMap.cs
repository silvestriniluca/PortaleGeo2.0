using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PortaleGeoWeb.Models;

namespace PortaleGeoWeb.ViewModels
{
    
        public class VmGeoCodeOpenStreetMap
        {
            public int Id { get; set; }
            [Required] [Display(Name = "Indirizzo")] public string Indirizzo { get; set; }
            [Required] [Display(Name = "N. Civico")] public string N_Civico { get; set; }
            [Required] [Display(Name = "Comune")] public string Comune { get; set; }

            [Required] [Display(Name = "Sigla Provincia")] public string Provincia { get; set; }
            [Display(Name = "Regione")] public string Regione { get; set; }
            [Display(Name = "Nota 1")] public string Note1 { get; set; }
            [Display(Name = "Nota 2")] public string Note2 { get; set; }

            [Display(Name = "Latitudine")] public string Lat { get; set; }
            [Display(Name = "Longitudine")] public string Lon { get; set; }

            [Display(Name = "Approx 1")] public string Approx01 { get; set; }
            [Display(Name = "Approx 2")] public string Approx02 { get; set; }

            [Display(Name = "Cap")] public string Cap { get; set; }
            [Display(Name = "Altro Indirizzo")] public string AltroIndirizzo { get; set; }

            [Display(Name = "Usa API Google (S/N)")] public string APIGoogle { get; set; }

            public VmGeoCodeOpenStreetMap()
            {

            }


            public VmGeoCodeOpenStreetMap(OpenStreetMapCSV model)
            {
                Id = model.Id;
                Indirizzo = model.Indirizzo;
                N_Civico = model.N_Civico;
                Comune = model.Comune;

                Provincia = model.Provincia;

                Regione = model.Regione;

                Note1 = model.Note1;
                Note2 = model.Note2;

                Lat = model.Lat;
                Lon = model.Lon;
                Approx01 = model.Approx01;
                Approx02 = model.Approx02;

                Cap = model.Cap;
                AltroIndirizzo = model.AltroIndirizzo;

                APIGoogle = model.APIGoogle;
            }

        
    }
}