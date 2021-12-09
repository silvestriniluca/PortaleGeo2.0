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
            HereGeoCoder service = new HereGeoCoder();
            //
            List<GeocoderReply> outList = new List<GeocoderReply>();

            DataDescriptor savedList = new DataDescriptor();
            savedList.Rows = new List<string[]>();
            //
            int K_LATITUDE = data.Header.Length;
            int K_LONGITUDE = data.Header.Length + 1;
            int K_MATCH_LEVEL = data.Header.Length + 2;
            int K_MATCH_TYPE = data.Header.Length + 3;
            int K_MATCH_RELEVANCE = data.Header.Length + 4;
            int K_MATCH_ERROR = data.Header.Length + 5;
            //
            savedList.Header = new string[data.Header.Length + 6];
            Array.Copy(data.Header, savedList.Header, data.Header.Length);
            //
            // Array.Resize(ref data.Header, data.Header.Length + 6);
            //

            savedList.Header[K_LATITUDE] = "geo_Latitude";
            savedList.Header[K_LONGITUDE] = "geo_Longitude";
            savedList.Header[K_MATCH_LEVEL] = "geo_MatchLevel";
            savedList.Header[K_MATCH_TYPE] = "geo_MatchType";
            savedList.Header[K_MATCH_RELEVANCE] = "geo_Relevance";
            savedList.Header[K_MATCH_ERROR] = "geo_Error";

            //
            if (data.i == true)
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
                            if (geocoderReply.ReplyObject != null)
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
                        savedList.Rows.Add(outRow);
                        //
                        counter++;

                        //
                        if (counter > 5) goto Test;
                    
                }
            }
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
                        if (geocoderReply.ReplyObject != null)
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
                    savedList.Rows.Add(outRow);
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
            

            return savedList;
        }


    }
}
