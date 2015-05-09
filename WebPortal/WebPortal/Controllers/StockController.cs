using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;
using System.Diagnostics;
using System.Net;

namespace WebPortal.Controllers
{
    [Authorize]
    public class StockController : Controller
    {
        //
        // GET: /Stock/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Stock/Quote
        public async Task<JsonResult> Quote()
        {
            return Json(
                await new StockApi().GetStocksAsync(),
                "application/json",
                JsonRequestBehavior.AllowGet);
        }

        //
        // Get: /Stock/Transactions
        public JsonResult Transactions()
        {
            using (var db = new WebPortalContext())
            {
                var transactions = db.Transactions.Where(t => t.User == User.Identity.Name).ToList<Transaction>();
                return Json(transactions, JsonRequestBehavior.AllowGet);
            }
        }

        // get my stocks
        // Get: api/Stock
        [Route("api/Stock"), HttpGet]
        public async Task<JsonResult> MyStocks()
        {
            using (var db = new WebPortalContext())
            {
                var stocks = db.Holdings.Where(h => h.User == User.Identity.Name).ToList<Holding>();
                /*
                Object[] stocksArray = new Object[stocks.Count];
                for (int i = 0; i < stocks.Count; i++)
                {
                    Holding h = stocks.ElementAt(i);
                    //TODO get current price and stuff from API and add to object below
                    stocksArray[i] = new { title = e.title, start = e.start, end = e.end };
                }*/
                Debug.WriteLine(stocks.Count);
                Debug.WriteLine(Json(stocks, JsonRequestBehavior.AllowGet));
                return Json(stocks, JsonRequestBehavior.AllowGet);
            }
        }

        // search
        // Get: api/Stock/:symbol
        [Route("api/Stock/{symbol}"), HttpGet]
        public async Task<JsonResult> GetStock(string symbol)
        {
            return Json(
                await new StockApi().GetStockAsync(symbol),
                "application/json",
                JsonRequestBehavior.AllowGet);
        }

        // buy
        // Put: api/Stock/:symbol/:amt
        [Route("api/Stock/{symbol}/{amt}"), HttpPost]//FIXME HttpPut doesn't work. (MVC 5.0 bug or something. supposedly fixed in 5.1)
        public async Task<ActionResult> PutBuy(string symbol, int amt)
        {
            // get current price from api
            var stock = await new StockApi().GetStockAsync(symbol);
            if (stock.Status != "SUCCESS")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            using (var db = new WebPortalContext())
            {
                // get current holding of this stock, to be updated.
                var holding = db.Holdings.FirstOrDefault(
                    h => h.User == User.Identity.Name
                    && h.Symbol == symbol);

                if (holding == null) // add it
                {
                    holding = new Holding();
                    holding.Symbol = symbol;
                    holding.AmountOwned = amt;
                    holding.AmountBought = amt * stock.LastPrice;
                    holding.AmountSold = 0.0M;
                    holding.User = User.Identity.Name;
                    db.Holdings.Add(holding);
                }
                else // update it
                {
                    holding.AmountOwned += amt;
                    holding.AmountBought += amt * stock.LastPrice;
                }

                // log transaction
                var t = new Transaction();
                t.Symbol = symbol;
                t.buy = true;
                t.Amount = amt;
                t.date = DateTime.Now;
                t.price = stock.LastPrice;
                t.User = User.Identity.Name;

                db.Transactions.Add(t);
                db.SaveChanges();
            }
            return View("Index");
        }

        // sell
        // Delete: /Stock/:symbol/:amt
        public async Task<ActionResult> DeleteSell(string symbol, int amt)
        {
            // get current price from api
            var stock = await new StockApi().GetStockAsync(symbol);
            if (stock.Status != "SUCCESS")
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            using (var db = new WebPortalContext())
            {
                // get current holding of this stock, to be updated.
                var holding = db.Holdings.FirstOrDefault(
                    h => h.User == User.Identity.Name
                    && h.Symbol == symbol);

                if (holding == null || holding.AmountOwned < amt) // can't sell
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else // update it
                {
                    holding.AmountOwned -= amt;
                    holding.AmountSold += amt * stock.LastPrice;
                }

                // log transaction
                var t = new Transaction();
                t.Symbol = symbol;
                t.buy = false;
                t.Amount = amt;
                t.date = DateTime.Now;
                t.price = stock.LastPrice;
                t.User = User.Identity.Name;

                db.Transactions.Add(t);
                db.SaveChanges();
            }
            return View("Index");
        }
	}
}