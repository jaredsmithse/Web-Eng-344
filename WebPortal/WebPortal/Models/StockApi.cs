using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace WebPortal.Models
{
    public class Stock
    {
        public string Status { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public decimal LastPrice { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public string Timestamp { get; set; }
        public decimal MSDate { get; set; }
        public long MarketCap { get; set; }
        public int Volume { get; set; }
        public decimal ChangeYTD { get; set; }
        public decimal ChangePercentYTD { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }

        // Holding-specific fields
        public int AmountOwned { get; set; }
        public decimal Profit { get; set; }
    }

    public class StockApi
    {
        private static readonly string uri = "http://dev.markitondemand.com/Api/v2/Quote/json?symbol=";


        public async Task<Stock> GetStockAsync(string symbol)
        {
            using (HttpClient client = new HttpClient())
            {
                var res = await client.GetStringAsync(uri + symbol);
                Stock stock = JsonConvert.DeserializeObject<Stock>(res);
                stock.Change = decimal.Round(stock.Change, 2, MidpointRounding.AwayFromZero);
                return stock;
            }
        }

        public async Task<Stock> GetStockFromHolding(Holding holding)
        {
            Stock stock = await GetStockAsync(holding.Symbol);
            stock.AmountOwned = holding.AmountOwned;
            stock.Profit = holding.AmountSold - holding.AmountBought
                + holding.AmountOwned * stock.LastPrice;
            return stock;
        }

        public async Task<IEnumerable<Stock>> GetStocksFromHoldings(IEnumerable<Holding> holdings)
        {
            IEnumerable<Task<Stock>> allTasks = holdings.Select(holding => GetStockFromHolding(holding));
            Stock[] allResults = await Task.WhenAll(allTasks);
            IEnumerable<Stock> allStocks = allResults.Select(stock => stock);
            return allStocks;
        }
    }
}