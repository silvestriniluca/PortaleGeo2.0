using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using NuovoPortaleGeo.reader.csv;
using NuovoPortaleGeo.service;
using NuovoPortaleGeo.Models;
using System.Data;
using NuovoPortaleGeo.Controllers;
using PortaleGeoWeb.Models;

namespace NuovoPortaleGeo
{

    public class GeocodeProcessor
    {

        public static GeoCode EsecuteGecoding(GeoCode data, int GeoNorighe)
        {

            int counter = 0;

            //creao oggetto con id, codice e url per richiamare il servizio
            HereGeoCoder service = new HereGeoCoder();

            //crea lista di oggetti response json + stringa json
            List<GeocoderReply> outList = new List<GeocoderReply>();

            //crea oggetto nuovo DatDescriptor
            DataDescriptor data_computed = new DataDescriptor();

            //per me questa riga è inutile, perchè Data Descriptor lo fa già di definizione
        //    data_computed.Rows = new List<string[]>();
            //numera le nuove intestazioni
            /*
            int K_LATITUDE = data.Header.Length;
            int K_LONGITUDE = data.Header.Length + 1;
            int K_MATCH_LEVEL = data.Header.Length + 2;
            int K_MATCH_TYPE = data.Header.Length + 3;
            int K_MATCH_RELEVANCE = data.Header.Length + 4;
            int K_MATCH_ERROR = data.Header.Length + 5;
            //
            data_computed.Header = new string[data.Header.Length + 6];
            Array.Copy(data.Header, data_computed.Header, data.Header.Length);
            //
            // Array.Resize(ref data.Header, data.Header.Length + 6);
            //

            data_computed.Header[K_LATITUDE] = "Here_Latitude";
            data_computed.Header[K_LONGITUDE] = "Here_Longitude";
            data_computed.Header[K_MATCH_LEVEL] = "Here_MatchLevel";
            data_computed.Header[K_MATCH_TYPE] = "Here_MatchType";
            data_computed.Header[K_MATCH_RELEVANCE] = "Here_Relevance";
            data_computed.Header[K_MATCH_ERROR] = "Here_Error";

            */



            //AVVIA GEOCODE


           
            
                service._county = data.Provincia;
                service._city = data.Comune;
                service._country = "ITALIA";
                service._state = "MARCHE";
                GeocoderReply geocoderReply = service.executeRequest(data.Indirizzo);
                outList.Add(geocoderReply);
                //
             
                //
                if (geocoderReply.ReplyException == null)
                {
                    //se l'indirizzo non è stato trovato  --> oggetto NULLO
                    if (geocoderReply.ReplyObject.Response.View.Length > 0)
                    {
                        //se COMUNE passato non corrisponde con quello del risultato geocode dà errore --> oggetto NULLO
                        if (geocoderReply.ReplyObject != null
                                && geocoderReply.ReplyObject.Response.View[0].Result[0].Location.Address.City.ToString().ToLower().Trim() == service._city.ToLower().Trim())
                        {

                            data.Lat = geocoderReply.ReplyObject.Response.View[0].Result[0].Location
                                .DisplayPosition.Latitude;
                            data.Lon = geocoderReply.ReplyObject.Response.View[0].Result[0].Location
                                .DisplayPosition.Longitude;
                            data.Here_MatchLevel = geocoderReply.ReplyObject.Response.View[0].Result[0].MatchLevel;
                            data.Here_MatchType = geocoderReply.ReplyObject.Response.View[0].Result[0].MatchType;
                            data.Here_Relevance=
                                geocoderReply.ReplyObject.Response.View[0].Result[0].Relevance.ToString();
                            data.Here_Error = null;

                        }
                        else
                        {
                            data.Here_Error = "REPLY-CITY-NOT-CORRESPOND";
                            GeoNorighe++;
                        }
                    }
                    else
                    {
                       data.Here_Error = "REPLY-OBJECT-NULL";
                        GeoNorighe++;
                    }
                    
                }
                else
                {
                    data.Here_Error = geocoderReply.ReplyException.ToString();
                    GeoNorighe++;
                }

 
            

            return data;
        }


    }
}
