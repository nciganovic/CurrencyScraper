using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        private static List<string> currencyNames = new List<string>();
        private static Currency currencyObj = new Currency();
        private static List<Currency> currencyObjList = new List<Currency>();

        static void Main(string[] args)
        {
            string baseUrl = "https://srh.bankofchina.com/search/whpj/searchen.jsp";

            // Get values like GBP, USD, HKD etc...
            StreamReader getAllCurrencyNamesStream = CreateStream.FromUrl(baseUrl);
            currencyNames = Scrape.GetCurrencyNamesFromStream(getAllCurrencyNamesStream);
            getAllCurrencyNamesStream.Close();

            Console.WriteLine($"{currencyNames.Count} currencies found!");

            DateTime startDate = DateTime.Now.TwoDaysAgo();
            DateTime endDate = DateTime.Now;

            foreach (string currencyName in currencyNames)
            {
                int index = currencyNames.FindIndex(x => x == currencyName) + 1;

                string searchUrl = baseUrl + $"?erectDate={startDate.ToString("yyyy-MM-dd")}&nothing={endDate.ToString("yyyy-MM-dd")}&pjname=" + currencyName;

                // Get total rows current currency has in order to calculate the number of pages
                StreamReader readTotalRowCountStream = CreateStream.FromUrl(searchUrl);
                int totalRows = Scrape.GetTotalRowsNumberFromStream(readTotalRowCountStream);
                int totalPages = GetTotalPagesNumber(totalRows);
                readTotalRowCountStream.Close();

                if (totalPages == 0)
                {
                    Console.WriteLine($"Failed to find total pages for {currencyName}, skipping to next one...");
                    continue;
                }

                for (int page = 1; page <= totalPages; page++)
                {
                    Console.WriteLine($"=========== {currencyName} {index}/{currencyNames.Count} start page {page}/{totalPages} ,total rows {totalRows} ===========");

                    string searchEachPageUrl = searchUrl + $"&page={page}";

                    // Get currency values for every row of page and add them to object and list
                    StreamReader readCurrencyValueStream = CreateStream.FromUrl(searchEachPageUrl);
                    Scrape.GetCurrencyValuesFromStream(readCurrencyValueStream, ref currencyObj, ref currencyObjList);
                    readCurrencyValueStream.Close();

                    if (currencyObjList.Count > 0)
                    {
                        //Write values to CSV
                        Console.WriteLine(currencyObjList.Count + " rows found!");
                        string fileName = CreateFileName(startDate, endDate, currencyName);
                        CsvWriter.WriteToCsv(currencyObjList, fileName);
                        currencyObjList.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Nothing found.");
                    }
                }
            }
        }

        private static int GetTotalPagesNumber(int numberOfRows, int rowsPerPage = 20) 
        {
            int totalPages = 0;   
            while (numberOfRows > 0) 
            {
                numberOfRows -= rowsPerPage;
                totalPages++;
            }
            return totalPages;
        }

        private static string CreateFileName(DateTime startDate, DateTime endDate, string currencyName) {
            return startDate.ToString("yyyy-MM-dd") + "_" + endDate.ToString("yyyy-MM-dd") + "_" + currencyName + ".csv";
        }
    }
}
