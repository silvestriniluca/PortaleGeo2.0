using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using PortaleGeoWeb.reader.csv;
using PortaleGeoWeb.service;
using PortaleGeoWeb.Models;

namespace PortaleGeoWeb
{
    public class GeocodeProcessor
    {

        public static DataDescriptor EsecuteGecoding(DataDescriptor data, InputParam par, string inputFile, CsvConfiguration conf)
        {

            int counter = 0;

            //creao oggetto con id, codice e url per richiamare il servizio
            HereGeoCoder service = new HereGeoCoder();

            //crea lista di oggetti response json + stringa json
            List<GeocoderReply> outList = new List<GeocoderReply>();

            //crea oggetto nuovo DatDescriptor
            DataDescriptor data_computed = new DataDescriptor();

            //per me questa riga è inutile, perchè Data Descriptor lo fa già di definizione
            data_computed.Rows = new List<string[]>();
            //numera le nuove intestazioni
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



            //TEST per i primi 5 elementi

            if (data.i == true)
            {
               

                foreach (var row in data.Rows)
                {

                    

                        service._city = row[par.posComune];
                        service._country = "ITALIA";
                        service._state = "MARCHE";
                        GeocoderReply geocoderReply = service.executeRequest(row[par.posIndirizzo]);
                        outList.Add(geocoderReply);
                        //row + 6??header semmai
                        string[] outRow = new string[row.Length + 6];
                        Array.Copy(row, outRow, row.Length);
                        //
                        if (geocoderReply.ReplyException == null)
                        {
                            //se COMUNE passato non corrisponde con quello del risultato geocode dà errore
                        if (geocoderReply.ReplyObject != null && geocoderReply.ReplyObject.Response.View[0].Result[0].Location.Address.City.ToString().ToLower() == service._city.ToLower())
                            {
                                //
                                if (geocoderReply.ReplyObject.Response.View.Length > 0)
                                {
                                    outRow[K_LATITUDE] = geocoderReply.ReplyObject.Response.View[0].Result[0].Location
                                        .DisplayPosition.Latitude.ToString();
                                    outRow[K_LONGITUDE] = geocoderReply.ReplyObject.Response.View[0].Result[0].Location
                                        .DisplayPosition.Longitude.ToString();
                                    outRow[K_MATCH_LEVEL] = geocoderReply.ReplyObject.Response.View[0].Result[0].MatchLevel;
                                    outRow[K_MATCH_TYPE] = geocoderReply.ReplyObject.Response.View[0].Result[0].MatchType;
                                    outRow[K_MATCH_RELEVANCE] =
                                        geocoderReply.ReplyObject.Response.View[0].Result[0].Relevance.ToString();
                                    outRow[K_MATCH_ERROR] = null;
                                }
                            }
                            else
                            {
                                outRow[K_MATCH_ERROR] = "REPLY-OBJECT-NULL";
                            }
                        }
                        else
                        {
                            outRow[K_MATCH_ERROR] = geocoderReply.ReplyException.ToString();
                        }

                    //
                    data_computed.Rows.Add(outRow);
                        //
                        counter++;

                        //
                        if (counter > 5) goto Test;
                    
                }
            }



            //AVVIA GEOCODE

            else
            {
                foreach (var row in data.Rows)
                {
                    service._city = row[par.posComune];
                    service._country = "ITALIA";
                    service._state = "MARCHE";
                    GeocoderReply geocoderReply = service.executeRequest(row[par.posIndirizzo]);
                    outList.Add(geocoderReply);
                    //
                    string[] outRow = new string[row.Length + 6];
                    Array.Copy(row, outRow, row.Length);
                    //
                    if (geocoderReply.ReplyException == null)
                    {
                        if (geocoderReply.ReplyObject.Response.View.Length > 0)
                        {
                            //se COMUNE passato non corrisponde con quello del risultato geocode dà errore
                            if (geocoderReply.ReplyObject != null
                                    && geocoderReply.ReplyObject.Response.View[0].Result[0].Location.Address.City.ToString().ToLower() == service._city.ToLower())
                            {

                                outRow[K_LATITUDE] = geocoderReply.ReplyObject.Response.View[0].Result[0].Location
                                    .DisplayPosition.Latitude.ToString();
                                outRow[K_LONGITUDE] = geocoderReply.ReplyObject.Response.View[0].Result[0].Location
                                    .DisplayPosition.Longitude.ToString();
                                outRow[K_MATCH_LEVEL] = geocoderReply.ReplyObject.Response.View[0].Result[0].MatchLevel;
                                outRow[K_MATCH_TYPE] = geocoderReply.ReplyObject.Response.View[0].Result[0].MatchType;
                                outRow[K_MATCH_RELEVANCE] =
                                    geocoderReply.ReplyObject.Response.View[0].Result[0].Relevance.ToString();
                                outRow[K_MATCH_ERROR] = null;

                            }
                        }
                        else
                        {
                            outRow[K_MATCH_ERROR] = "REPLY-OBJECT-NULL";
                        }
                    }
                    else
                    {
                        outRow[K_MATCH_ERROR] = geocoderReply.ReplyException.ToString();
                    }

                    //
                    data_computed.Rows.Add(outRow);
                    //
                    counter++;

                    //
                    //if (counter > 5) break;
                }
            }

     Test:



            //
            // change file name
            //
            string path = Path.GetDirectoryName(inputFile);
            string name = Path.GetFileNameWithoutExtension(inputFile);
            string outFileName = Path.Combine(path, name + "_elaborated.csv");
            

            return data_computed;
        }


    }
}
