﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        private static List<string> currencies = new List<string>();
        private static Currency currencyObj = new Currency();
        private static List<Currency> currencyObjList = new List<Currency>();

        static void Main(string[] args)
        {
            string baseUrl = "https://srh.bankofchina.com/search/whpj/searchen.jsp";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            Encoding encode = Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(receiveStream, encode);

            //ReadResponseStream(readStream);
            FindCurrenciesInStream(readStream);

            DateTime startDate = GetTwoDaysAgoDate();
            DateTime endDate = DateTime.Now;

            foreach (string c in currencies)
            {
                string searchUrl = baseUrl + $"?erectDate={startDate.ToString("yyyy-MM-dd")}&nothing={endDate.ToString("yyyy-MM-dd")}&pjname=" + c;

                HttpWebRequest requestCurrency = (HttpWebRequest)WebRequest.Create(searchUrl);
                HttpWebResponse responseCurrency = (HttpWebResponse)requestCurrency.GetResponse();

                Stream receiveCurrencyStream = responseCurrency.GetResponseStream();
                StreamReader readCurrencyStream = new StreamReader(receiveCurrencyStream, encode);

                ScrapeCurrencyDataFromStream(readCurrencyStream);

                Console.WriteLine("================= NEXT PAGE ===========");
            }

            // Releases the resources of the response.
            response.Close();
            // Releases the resources of the Stream.
            readStream.Close();
            Console.WriteLine("Hello World!");
        }

        private static DateTime GetTwoDaysAgoDate()
        {
            long currentTicks = DateTime.Now.Ticks;
            long twoDayTicks = DateTime.Now.AddDays(2).Ticks - currentTicks;
            long twoDaysBeaforeTicks = currentTicks - twoDayTicks;
            return new DateTime(twoDaysBeaforeTicks);
        }

        private static void FindCurrenciesInStream(StreamReader readStream) {

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
                    currencies.Add(currency);
                }

                Console.Write(line + "\n");

            }

            currencies.RemoveAt(0);
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
                    int endIndex = GetNthIndex(line, "<", 2);
                    string value = line.Substring(startIndex + 1, endIndex - startIndex - 1);

                    AddCurrencyProperty(ref currencyObj, value, i);
                    i++;

                    if (i > 6) i = 0;
                }
            }
        }

        private static int GetNthIndex(string s, string t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i].ToString() == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
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
