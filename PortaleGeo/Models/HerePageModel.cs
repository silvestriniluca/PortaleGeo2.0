using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortaleGeoWeb.Models
{
    public class DataDescriptor
    {
        public bool i;
        public string[] Header { get; set; }
        public List<string[]> Rows { get; set; }

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
