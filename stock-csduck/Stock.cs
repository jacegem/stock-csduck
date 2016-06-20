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
                stockPrice.setAvg120(avgUtil.get120());
                stockPrice.setAvg60(avgUtil.get60());
                stockPrice.setAvg20(avgUtil.get20());
                stockPrice.setAvg5(avgUtil.get5());
            }
        }

        internal SortedList getStockPriceList()
        {
            return stockPriceList;
        }

        internal void evaluation()
        {
            int cnt = stockPriceList.Count;
            // 5일선이 20일 선보다 아래에 있는가. 
            // 순차적으로 되어 있는가.
            StockPrice last = (StockPrice)stockPriceList.GetByIndex(cnt-1);
            if (last.avg5 < last.avg20
                && last.avg20 < last.avg60
                && last.avg60 < last.avg120) {
                // 사야 한다. 
            }
        }
    }
}