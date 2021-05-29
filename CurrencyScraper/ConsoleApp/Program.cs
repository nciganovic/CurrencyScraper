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

            StreamReader getAllCurrenciesStream = CreateStreamFromUrl(baseUrl);
            ScrapeCurrencyNamesInStream(getAllCurrenciesStream);
            getAllCurrenciesStream.Close();

            Console.WriteLine($"{currencyNames.Count} currencies found!");

            DateTime startDate = DateTime.Now.TwoDaysAgo();
            DateTime endDate = DateTime.Now;

            foreach (string currencyName in currencyNames)
            {
                int index = currencyNames.FindIndex(x => x == currencyName) + 1;

                string searchUrl = baseUrl + $"?erectDate={startDate.ToString("yyyy-MM-dd")}&nothing={endDate.ToString("yyyy-MM-dd")}&pjname=" + currencyName;

                StreamReader readTotalRowCountStream = CreateStreamFromUrl(searchUrl);
                int totalRows = ScrapeTotalRowsNumber(readTotalRowCountStream);
                int totalPages = GetTotalPagesNumber(totalRows);
                readTotalRowCountStream.Close();

                for (int page = 1; page <= totalPages; page++) 
                {
                    Console.WriteLine($"=========== {currencyName} {index}/{currencyNames.Count} start page {page}/{totalPages}   rows {totalRows} ===========");

                    string searchEachPageUrl = searchUrl + $"&page={page}";

                    StreamReader readCurrencyValueStream = CreateStreamFromUrl(searchEachPageUrl);
                    ScrapeCurrencyDataFromStream(readCurrencyValueStream);
                    readCurrencyValueStream.Close();
                }

                //Write values to CSV
                string fileName = CreateFileName(startDate, endDate, currencyName);
                CsvWriter.WriteToCsv(currencyObjList, fileName);

                currencyObjList.Clear();
            }
        }

        private static void ScrapeCurrencyNamesInStream(StreamReader readStream) {

            while (true)
            {
                string line = readStream.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.Contains("<option value=\"")) {
                    int startIndex = line.IndexOf("\"");
                    string currency = line.Substring(startIndex + 1, 3);
                    currencyNames.Add(currency);
                }
            }

            //Remove first element that is 'Select the currency'
            currencyNames.RemoveAt(0);
        }

        private static void ScrapeCurrencyDataFromStream(StreamReader readStream)
        {
            int i = 0;
            while (true)
            {
                string line = readStream.ReadLine();

                if (line == null)
                    break;
  
                if (line.Contains("class=\"hui12_20\""))
                {
                    int startIndex = line.IndexOf(">");
                    int endIndex = line.GetNthIndex('<', 2);
                    string value = line.Substring(startIndex + 1, endIndex - startIndex - 1);

                    AddCurrencyProperty(ref currencyObj, value, i);
                    i++;

                    if (i > 6) i = 0;
                }
            }
        }

        private static int ScrapeTotalRowsNumber(StreamReader readStream) 
        {
            int i = 0;

            while (true)
            {
                string line = readStream.ReadLine();

                if (line == null)
                    break;

                if (line.Contains("m_nRecordCount"))
                {
                    int startIndex = line.IndexOf("=");
                    int endIndex = line.IndexOf(";");
                    string value = line.Substring(startIndex + 2, endIndex - startIndex - 2);
                    i = Convert.ToInt32(value);
                    break;
                }
            }

            return i;
        }

        private static void AddCurrencyProperty(ref Currency currencyObj, string value, int i)
        {
            switch (i)
            {
                case 0:
                    currencyObj.CurrencyName = value;
                    break;
                case 1:
                    currencyObj.BuyingRate = value;
                    break;
                case 2:
                    currencyObj.CashBuyingRate = value;
                    break;
                case 3:
                    currencyObj.SellingRate = value;
                    break;
                case 4:
                    currencyObj.CashSellingRate = value;
                    break;
                case 5:
                    currencyObj.MiddleRate = value;
                    break;
                case 6:
                    currencyObj.PubTime = value;
                    currencyObjList.Add(new Currency(currencyObj));
                    PrintCurrency(currencyObj);
                    break;
            }
        }

        private static StreamReader CreateStreamFromUrl(string url) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);
            return readStream;
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

        private static void PrintCurrency(Currency currency) {
            Console.WriteLine("============================================================");
            Console.Write(currency.CurrencyName + ", ");
            Console.Write(currency.BuyingRate + ", ");
            Console.Write(currency.CashBuyingRate + ", ");
            Console.Write(currency.SellingRate + ", ");
            Console.Write(currency.CashSellingRate + ", ");
            Console.Write(currency.MiddleRate + ", ");
            Console.Write(currency.PubTime + "\n");
            Console.WriteLine("============================================================");
        }
    }
}
