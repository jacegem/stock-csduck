using System;

namespace stock_csduck
{
    internal class StockPrice
    {
        public object priceEnd { get; set; }
        public object date { get; set; }
        public object priceStart;
        public object volumn { get; set; }
        public int avg120 { get; set; }
        public int avg60 { get; set; }
        public int avg20 { get; set; }
        public int avg5 { get; set; }
        public double rate5 { get; set; }
        public double rate20 { get; set; }
        public double rate60 { get; set; }


        public StockPrice(object date, object priceStart, object priceEnd, object volumn)
        {
            this.date = date;
            this.priceStart = priceStart;
            this.priceEnd = priceEnd;
            this.volumn = volumn;
        }

        internal void setAvg(int[] avg)
        {
            int idx = 0;
            this.avg5 = avg[idx++];
            this.avg20 = avg[idx++];
            this.avg60 = avg[idx++];
            this.avg120 = avg[idx++];

            rate5 = (double)avg5 / avg20;
            rate20 = (double)avg20 / avg60;
            rate60 = (double)avg60 / avg120;
        }
    }
}