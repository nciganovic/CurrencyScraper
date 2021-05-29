using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Currency
    {
        public Currency()
        {

        }

        public Currency(Currency currency)
        {
            CurrencyName = currency.CurrencyName;
            BuyingRate = currency.BuyingRate;
            CashBuyingRate = currency.CashBuyingRate;
            SellingRate = currency.SellingRate;
            CashSellingRate = currency.CashSellingRate;
            MiddleRate = currency.MiddleRate;
            PubTime = currency.PubTime;
        }

        //Setting all values to string becase I will not use this values for anything except to write them on csv file.
        public string CurrencyName { get; set; }
        public string BuyingRate { get; set; }
        public string CashBuyingRate { get; set; }
        public string SellingRate { get; set; }
        public string CashSellingRate { get; set; }
        public string MiddleRate { get; set; }
        public string PubTime { get; set; }

        public static void PrintCurrency(Currency currency)
        {
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
