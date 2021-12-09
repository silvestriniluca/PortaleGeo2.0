using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PortaleGeoWeb.message;

namespace PortaleGeoWeb.service
{
    public class HereGeoCoder
    {
        //private string _app_id = "rVcIzsUMhtWKeviqczPq";
        // rivate string _app_code = "mih3dbnA0d-9gbvIr_ywhg"; 


        private string _app_id = "cekVYYpERe7h38ELgWSS";
        private string _app_code = "ihMFm1IVDRZpCZ8Pv3hj9Q";

        public string _state = "Marche";
        public string _country = "ITA";
        public string _city = "";


        private string _urlPattern =
            "https://geocoder.api.here.com/6.2/geocode.json?app_id={0}&app_code={1}&gen=9&state={2}&Country={3}&city={4}&street=";


        public GeocoderReply executeRequest(string specie, string denominazioneIndirizzo, string civico, string lettera)
        {
            GeocoderReply reply =
                CallRestService(buildUrlPattern() + WebUtility.UrlEncode(buildAddress(specie, denominazioneIndirizzo, civico, lettera)),
                    "GET", String.Empty);
            return reply;
        }

        public GeocoderReply executeRequest(string indirizzo)
        {
            GeocoderReply reply =
                CallRestService(buildUrlPattern() + WebUtility.UrlEncode(indirizzo),
                    "GET", String.Empty);
            return reply;
        }

        private GeocoderReply CallRestService(string uri, string method, dynamic parms)
        {
            //json + stringa json + exception
            GeocoderReply gr = new GeocoderReply();

            var req = HttpWebRequest.Create(uri);
            req.Method = method;
            req.ContentType = "application/json";

            try
            {
                using (var resp = req.GetResponse())
                {
                    dynamic result_json = new StreamReader(resp.GetResponseStream()).ReadToEnd();

                    //TROVA IL RISULTATO DEL GEOCODE in formato stringa
                    gr.ReplyJson = result_json;

                    HereGeocodeResponse hereReply = HereGeocodeResponse.FromJson(result_json);

                    gr.ReplyObject = hereReply;
                    
                }
            }
            catch (Exception ex)
            {
                gr.ReplyException = ex;
            }

            return gr;
        }

        private string buildUrlPattern()
        {
            string kom = String.Format(_urlPattern, _app_id, _app_code, _state, _country, _city);
            //
            return kom;
        }

        /// <summary>
        /// Costruisce un indirizzo valido.
        /// </summary>
        /// <param name="specie"></param>
        /// <param name="denominazioneIndirizzo"></param>
        /// <param name="civico"></param>
        /// <param name="lettera"></param>
        /// <returns></returns>
        private string buildAddress(string specie, string denominazioneIndirizzo, string civico, string lettera)
        {
            string rightPattern = String.Empty;

            if (!String.IsNullOrWhiteSpace(civico))
            {
                if (!String.IsNullOrWhiteSpace(lettera))
                {
                    rightPattern = $"{civico}/{lettera}";
                }
                else
                {
                    rightPattern = $"{civico}";
                }
            }

            string retAddr = String.Empty;
            if (!string.IsNullOrWhiteSpace(rightPattern))
            {
                retAddr = $"{specie} {denominazioneIndirizzo}, {rightPattern}";

            }
            else
            {
                retAddr = $"{specie} {denominazioneIndirizzo}";
            }

            return retAddr;
        }

    }

    public class GeocoderReply
    {
        public HereGeocodeResponse ReplyObject { get; set; }
        public string ReplyJson { get; set; }
        public Exception ReplyException { get; set; }
    }
}
