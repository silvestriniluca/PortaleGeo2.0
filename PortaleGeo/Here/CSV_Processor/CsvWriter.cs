using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using PortaleGeoWeb.Models;

namespace PortaleGeoWeb.reader.csv
{
    public class MyCsvWriter
    {
        public static void SaveCsvFile(string outFileName, CsvConfiguration conf, DataDescriptor savedList)
        {
            using (var writer = new StreamWriter(outFileName))
            using (var csv = new CsvWriter(writer, conf))
            {
                //csv.WriteRecord( savedList.Header );
                //foreach (var rr in savedList.Rows)
                //{
                //    csv.WriteRecord(rr);
                //}


                foreach (var value in savedList.Header)
                {
                    csv.WriteField(value);
                }

                csv.NextRecord();
                foreach (var rr in savedList.Rows)
                {
                    foreach (var value in rr)
                    {
                        csv.WriteField(value);
                    }

                    csv.NextRecord();
                }

                writer.Flush();
            }
        }


    }
}
