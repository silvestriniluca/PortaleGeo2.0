using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace PortaleGeoWeb.ViewModels
{
    public class VmHere
    {
        
        [Name("geo_Latitude")]
         public string geo_Latitude { get; set; }
        [Name("geo_Longitude")]
        public string geo_Longitude { get; set; }
        [Name("Indirizzo")]
        public string Indirizzo { get; set; }
        [Name("DENOMINAZIONE")]
        public string DENOMINAZIONE { get; set; }



    }
}