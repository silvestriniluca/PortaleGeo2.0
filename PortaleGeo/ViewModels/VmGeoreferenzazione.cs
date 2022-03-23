using NuovoPortaleGeo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace NuovoPortaleGeo.ViewModels
{
    public partial class VmGeoreferenzazione
    {

        public VmGeoreferenzazione(Geo_Dati dati)
        {

        }

        public long Id { get; set; }
        [Required] public string IdUtente { get; set; }
        [Required] public string DescrizioneFile { get; set; }

        [Required] public string Provincia { get; set; }
        [Required] public string Comune { get; set; }
        [Required] public string Indirizzo { get; set; }
        public string Cap { get; set; }
        [Required] public string Descrizione { get; set; }
        public Nullable<bool> Here { get; set; }
        public Nullable<bool> OpenStreetMap { get; set; }
        public Nullable<bool> Google { get; set; }
        public string Here_MatchLevel { get; set; }
        public string Here_MatchType { get; set; }
        public string Here_Relevance { get; set; }
        public string Here_Error { get; set; }
        public string Approx01 { get; set; }
        public string Approx02 { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }

        public DataColumnCollection Columns { get; set; }
        public DataRowCollection Rows { get; set; }




        public VmGeoreferenzazione(Geo_Dati model, DataTable dataTable)
        {
            Id = model.Id;
            IdUtente = model.IdUtente;
            DescrizioneFile = model.DescrizioneFile;
            Provincia = model.Provincia;
            Comune = model.Comune;
            Indirizzo = model.Indirizzo;
            Cap = model.Cap;
            Descrizione = model.Descrizione;
            Here = model.Here;
            OpenStreetMap = model.OpenStreetMap;
            Google = model.Google;
            Columns = dataTable.Columns;
            Rows = dataTable.Rows;
            Approx01 = model.Approx01;
            Approx02 = model.Approx02;
            Lat = model.Lat;
            Lon = model.Lon;         
            Here_MatchLevel = model.Here_MatchLevel;
            Here_MatchType = model.Here_MatchType;
            Here_Relevance = model.Here_Relevance;
            Here_Error = model.Here_Error;
        }
    }
}