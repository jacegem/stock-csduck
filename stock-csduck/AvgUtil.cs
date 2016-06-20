using System;
using System.Collections;

namespace stock_csduck
{
    internal class AvgUtil
    {
        private int cnt;

        Queue q120 = new Queue();
        Queue q60 = new Queue();
        Queue q20 = new Queue();
        Queue q5 = new Queue();

        public AvgUtil()
        {
            
        }

        public AvgUtil(int cnt)
        {
            this.cnt = cnt;
        }

        internal void add(StockPrice stockPrice)
        {
            int priceEnd = (int)stockPrice.priceEnd;
            q120.Enqueue(priceEnd);
            q60.Enqueue(priceEnd);
            q20.Enqueue(priceEnd);
            q5.Enqueue(priceEnd);

            if (q120.Count > 120) q120.Dequeue();
            if (q60.Count > 60) q60.Dequeue();
            if (q20.Count > 20) q20.Dequeue();
            if (q5.Count > 5) q5.Dequeue();
        }

        internal int get120()
        {
            return getAvg(q120);
        }

        internal int get60()
        {
            return getAvg(q60);
        }

        internal int get20()
        {
            return getAvg(q20);
        }

        internal int get5()
        {
            return getAvg(q5);
        }


        private int getAvg(Queue q)
        {
            int sum = 0;
            foreach (Object obj in q)
            {
                sum += (int)obj;
            }
            return sum / q.Count;
        }


    }
}