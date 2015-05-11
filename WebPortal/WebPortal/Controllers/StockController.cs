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
using System.IO;

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
                var holdings = db.Holdings.Where(h => h.User == User.Identity.Name).ToList<Holding>();
                var stocks = await new StockApi().GetStocksFromHoldings(holdings);
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

        // update note
        // Put: api/Stock/:symbol
        [Route("api/Stock/{symbol}"), HttpPut]
        public ActionResult PutNote(string symbol) //FIXME I'd like to have "[FromBody]string note" but can't seem to get it to work. found this workaround:
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string note = new StreamReader(req).ReadToEnd();

            using (var db = new WebPortalContext())
            {
                var holding = db.Holdings.FirstOrDefault(
                    h => h.User == User.Identity.Name
                    && h.Symbol == symbol);

                if (holding != null)
                {
                    holding.Note = note;
                    db.SaveChanges();
                }

                return View("Index");
            }
        }

        // buy
        // Put: api/Stock/:symbol/:amt
        [Route("api/Stock/{symbol}/{amt}"), HttpPut]
        public async Task<ActionResult> PutBuy(string symbol, int amt)
        {
            Debug.WriteLine("PutBuy " + symbol + " Amount: " + amt);
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
        // Delete: api/Stock/:symbol/:amt
        [Route("api/Stock/{symbol}/{amt}"), HttpDelete]
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