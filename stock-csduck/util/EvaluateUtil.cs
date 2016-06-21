using System;
using System.Collections;

namespace stock_csduck
{
    internal class EvaluateUtil
    {
        private SortedList stockPriceList;
        private int cnt;
        StockPrice lastStockPrice;

        public EvaluateUtil(SortedList stockPriceList)
        {
            this.stockPriceList = stockPriceList;
            this.cnt = stockPriceList.Count;
            this.lastStockPrice = (StockPrice)stockPriceList.GetByIndex(cnt - 1);
        }

        internal int isGoodToBuy()
        {
            int result = 7;

            if (isOrdered(10) == false) return 0;
            if (isIncrease(4) == false) return 0;
            if (hasVolumn(10) == false) return 0;

            if (isCross()) result += 7;
            if (isRateOrdered()) result += 7;

            return result;
        }

        private bool hasVolumn(int days)
        {
            for (int i = days; i > 0; i--)
            {
                StockPrice stockPrice = (StockPrice)stockPriceList.GetByIndex(cnt - i);
                if (Convert.ToInt64(stockPrice.volumn) == 0) return false;                
            }

            return true;
        }

        private bool isRateOrdered()
        {
            if (lastStockPrice.rate5 <= lastStockPrice.rate20
                && lastStockPrice.rate20 <= lastStockPrice.rate60) return true;
            else return false;
        }

        private bool isCross()
        {
            StockPrice stockPrice = (StockPrice)stockPriceList.GetByIndex(cnt - 1);
            if (stockPrice.rate5 > 0.97) return true;
            else return false;
        }

        private bool isIncrease(int days)
        {
            int price = 0;
            for (int i = days; i > 0; i--)
            {
                StockPrice stockPrice = (StockPrice)stockPriceList.GetByIndex(cnt - i);
                int priceEnd = (int)stockPrice.priceEnd;
                if (price <= priceEnd) {
                    price = priceEnd;
                } else {
                    return false;
                }
            }

            return true;
        }

        private bool isOrdered(int days)
        {
            for (int i = 0; i < days; i++) {
                StockPrice stockPrice = (StockPrice)stockPriceList.GetByIndex(cnt - 1 - i);
                if (stockPrice.avg5 < stockPrice.avg20
                    && stockPrice.avg20 < stockPrice.avg60
                    && stockPrice.avg60 < stockPrice.avg120)
                {
                    // 순차적
                }
                else {
                    return false;
                }
            }

            return true;
        }
    }
}