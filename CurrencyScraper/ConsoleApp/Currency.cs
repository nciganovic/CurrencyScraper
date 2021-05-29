﻿using System;
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

        public string CurrencyName { get; set; }
        public string BuyingRate { get; set; }
        public string CashBuyingRate { get; set; }
        public string SellingRate { get; set; }
        public string CashSellingRate { get; set; }
        public string MiddleRate { get; set; }
        public string PubTime { get; set; }
    }
}
