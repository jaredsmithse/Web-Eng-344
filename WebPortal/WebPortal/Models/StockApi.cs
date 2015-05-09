using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace WebPortal.Models
{
    public class Stock
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public float Change { get; set; }
        public float ChangePercent { get; set; }
        public string Timestamp { get; set; }
        public float MSDate { get; set; }
        public long MarketCap { get; set; }
        public int Volume { get; set; }
        public float ChangeYTD { get; set; }
        public float ChangePercentYTD { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Open { get; set; }
    }

    public class StockApi
    {
        private static readonly string uri = "http://dev.markitondemand.com/Api/v2/Quote/json?symbol=";


        public async Task<Stock> GetStockAsync(string symbol)
        {
            using (HttpClient client = new HttpClient())
            {
                var res = await client.GetStringAsync(uri + symbol);
                Debug.WriteLine("Symbol: " + symbol + " -> " + res);
                return JsonConvert.DeserializeObject<Stock>(res);
            }
        }

        public async Task<IEnumerable<Stock>> GetStocksAsync()
        {
            string[] symbols = new[] { "AAPL", "MSFT", "GOOG", "AMZN", "FB" };
            IEnumerable<Task<Stock>> allTasks = symbols.Select(symbol => GetStockAsync(symbol));
            Stock[] allResults = await Task.WhenAll(allTasks);
            IEnumerable<Stock> allStocks = allResults.Select(stock => stock);
            return allStocks;
        }
    }
}