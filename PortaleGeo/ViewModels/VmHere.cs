using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace NuovoPortaleGeo.ViewModels
{
    public class VmHere
    {
        
        [Name("Here_Latitude")]
         public string geo_Latitude { get; set; }
        [Name("Here_Longitude")]
        public string geo_Longitude { get; set; }
        [Name("Indirizzo")]
        public string Indirizzo { get; set; }

        [Name("DENOMINAZIONE")] [Optional]
        public string DENOMINAZIONE { get; set; }

        [Name("Note2")]
        [Optional]
        public string Note2 { get; set; }

    }
}