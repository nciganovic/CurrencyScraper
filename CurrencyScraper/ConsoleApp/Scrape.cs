using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class Scrape
    {
        public static List<string> GetCurrencyNamesFromStream(StreamReader readStream)
        {
            List<string> currencyNames = new List<string>();

            while (true)
            {
                string line = readStream.ReadLine();

                if (line == null)
                {
                    break;
                }

                if (line.Contains("<option value=\""))
                {
                    int startIndex = line.IndexOf("\"");
                    string currency = line.Substring(startIndex + 1, 3);
                    currencyNames.Add(currency);
                }
            }

            if (currencyNames.Any()) { 
                //Remove first element that is 'Select the currency'
                currencyNames.RemoveAt(0);
            }

            return currencyNames;
        }

        public static int GetTotalRowsNumberFromStream(StreamReader readStream)
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

        public static void GetCurrencyValuesFromStream(StreamReader readStream, ref Currency currencyObj, ref List<Currency> currencyObjList)
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

                    //Since there are 7 different values for scraping i need to know to what property to assign value
                    //So for that reason I am using counter 'i' to track what property is next for value

                    AddCurrencyProperty(ref currencyObj, value, i);
                    
                    i++;

                    if (i > 6) 
                    {
                        i = 0;
                        currencyObjList.Add(new Currency(currencyObj));
                        //Currency.PrintCurrency(currencyObj);
                    }
                    
                }
            }
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
                    break;
            }
        }
    }
}
