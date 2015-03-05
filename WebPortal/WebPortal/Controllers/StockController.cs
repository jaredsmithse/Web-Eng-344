using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;

namespace WebPortal.Controllers
{
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
	}
}