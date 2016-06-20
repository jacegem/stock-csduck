using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stock_csduck
{
    static class StockManager
    {
        //static List<Stock> stockList = new List<Stock>();
        static Dictionary<object, Stock> stockDic = new Dictionary<object, Stock>();

        static public void addStock(Object code, Object name, Object fullCode)
        {
            Stock stock = new Stock(code,  name, fullCode);
            stockDic.Add(code, stock);
        }

        internal static IEnumerable getStockList()
        {
            return stockDic.Values.ToList();
        }

        internal static void addStockPrice(object code, StockPrice stockPrice)
        {
            Stock stock = stockDic[code];
            stock.addPrice(stockPrice);
        }

        internal static void calculateAvg()
        {
            foreach (KeyValuePair<object, Stock> entry in stockDic)
            {
                // do something with entry.Value or entry.Key
                entry.Value.calculateAvg();
            }
        }

        internal static void calculateAvg(object code)
        {
            Stock stock = stockDic[code];
            stock.calculateAvg();
        }

        internal static void evaluation(object code)
        {
            Stock stock = stockDic[code];
            stock.evaluation();
        }
    }
}
