using System;

namespace stock_csduck
{
    internal class RequestCountUtil
    {
        static DateTime startTime;
        static int cnt = 0;
        static int MAX_CNT = 60;
        static int PEIROD = 15000;
        static int DEFAULT_DELAY = 200;

        internal static void add()
        {
            if (cnt == 0 ) startTime = DateTime.Now;
            cnt++;

            Delay(DEFAULT_DELAY);

            if (cnt >= MAX_CNT) {
                DateTime now  = DateTime.Now;
                TimeSpan ts = now - startTime;
                int watiMilli = PEIROD - ts.Milliseconds;

                if (watiMilli > 0) {
                    Delay(watiMilli);
                }

                cnt = 0;
                startTime = DateTime.Now;
            }
        }

        private static DateTime Delay(int MS)
        {
          

            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
            while (AfterWards >= ThisMoment)
            {
                //System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;
        }
    }
}