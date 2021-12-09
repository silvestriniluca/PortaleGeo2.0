using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using PortaleGeoWeb.Models;

namespace PortaleGeoWeb.reader.csv
{
    public class MyCsvReader
    {

        public DataDescriptor LoadData(string fileName, CsvConfiguration conf)
        {

            if (!File.Exists(fileName))
                throw new ArgumentException(String.Format("Path al file errato. {0}", fileName));

            //nuovo oggetto DatDescriptor
            DataDescriptor retData = new DataDescriptor();
            
                retData.Rows = new List<string[]>();

            var reader = new StreamReader(fileName);
            using (var csv = new CsvReader(reader,  conf))
            {
                csv.Read();
                csv.ReadHeader();
              
                   retData.Header = csv.HeaderRecord;
                
               // int colCount = csv.ColumnCount;
                while (csv.Read())
                {
                    string[] row = new string[csv.HeaderRecord.Length];

                    //carica tutti i dati del file passato
                    for (int j = 0; j < csv.HeaderRecord.Length; j++)
                    {
                        row[j] = csv.GetField<string>(j);
                    }
                    
                    retData.Rows.Add(row);
                }
            }
            //
            return retData;
        }
    }
}
