using System;

namespace stock_csduck
{
    internal class StockPrice
    {
        public object priceEnd { get; set; }
        public object date { get; set; }
        private object priceStart;
        private object volumn;
        public int avg120 { get; set; }
        public int avg60 { get; set; }
        public int avg20 { get; set; }
        public int avg5 { get; set; }

   
        public StockPrice(object date, object priceStart, object priceEnd, object volumn)
        {
            this.date = date;
            this.priceStart = priceStart;
            this.priceEnd = priceEnd;
            this.volumn = volumn;
        }

        internal void setAvg120(int avg120)
        {
            this.avg120 = avg120;
        }

        internal void setAvg60(int avg60)
        {
            this.avg60 = avg60;
        }

        internal void setAvg20(int avg20)
        {
            this.avg20 = avg20;
        }

        internal void setAvg5(int avg5)
        {
            this.avg5 = avg5;
        }
    }
}