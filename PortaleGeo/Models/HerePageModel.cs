using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuovoPortaleGeo.Models
{
    public class DataDescriptor
    {
        public bool i;
        public string[] Header { get; set; }
        public List<string[]> Rows { get; set; }

        public string Comune { get; set; }
        public string Provincia { get; set; }
        public string Indirizzo { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }

   
        public string Here_MatchLevel { get; set; }

        public string Here_MatchType { get; set; }
        public string Here_Error { get; set; }
    }
   

    public class HerePageModel
    {
        public DataDescriptor Data { get; set; }
        public string UrlPathDownload { get; set; }
    }
    public class dataMaps
    {
        public string DatiMappa { get; set; }
    }
}
