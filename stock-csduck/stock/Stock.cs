using System;
using System.Collections;
using System.Collections.Generic;

namespace stock_csduck
{
    internal class Stock
    {
        public object code { get; set; }
        public object name { get; set; }
        private object fullCode;
        public int buyPoint { get; set; }
        public int sellPoint { get; set; }

        public SortedList stockPriceList;
        

        public Stock(object code, object name, object fullCode)
        {
            this.code = code;
            this.name = name;
            this.fullCode = fullCode;

            stockPriceList = new SortedList();
        }

        internal void addPrice(StockPrice stockPrice)
        {
            object date = stockPrice.date;
            if (stockPriceList.ContainsKey(date)) stockPriceList.Remove(date);

            stockPriceList.Add(date, stockPrice);
        }

        internal void calculateAvg()
        {
            AvgUtil avgUtil = new AvgUtil();

            int cnt = stockPriceList.Count;           
                        
            for (int i = 0; i < cnt; i++) {
                StockPrice stockPrice = (StockPrice)stockPriceList.GetByIndex(i);
                avgUtil.add(stockPrice);
                stockPrice.setAvg(avgUtil.getAvg());
            }
        }

        internal SortedList getStockPriceList()
        {
            return stockPriceList;
        }

        internal void evaluateBuy()
        {
            EvaluateUtil eu = new EvaluateUtil(stockPriceList);
            buyPoint = eu.isGoodToBuy();            
        }

        internal StockPrice getLastPrice()
        {
            int cnt = stockPriceList.Count;
            return (StockPrice)stockPriceList.GetByIndex(cnt-1);
        }
    }
}