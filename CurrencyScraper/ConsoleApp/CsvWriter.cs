using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class CsvWriter
    {
        public static void WriteToCsv(List<Currency> records, string fileName)
        {
            //string path = GetFilePathFromConfig() + "/" + fileName + ".csv";

            string path = Path.Combine(GetFilePathFromConfig(), fileName + ".csv");

            bool fileExists = File.Exists(path);

            using (var w = new StreamWriter(path, fileExists)) // writer will either create new file or append on existing one if is found on given location
            {
                //If doesn't exist then add header values for csv
                if (!fileExists)
                {
                    w.WriteLine("CurrencyName,BuyingRate,CashBuyingRate,SellingRate,CashSellingRate,MiddleRate,PubTime");
                    w.Flush();
                }

                foreach (var r in records)
                {
                    var line = string.Format("{0},{1},{2},{3},{4},{5},{6}"
                        , r.CurrencyName, r.BuyingRate, r.CashBuyingRate, r.SellingRate, r.CashSellingRate, r.MiddleRate, r.PubTime);
                    w.WriteLine(line);
                    w.Flush();
                }
            }
        }

        private static string GetFilePathFromConfig() 
        {
            return ConfigurationManager.AppSettings["FilePath"];
        }
    }
}
