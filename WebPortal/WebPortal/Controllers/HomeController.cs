using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WebPortal.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var owinContext = HttpContext.GetOwinContext();
            var authentication = owinContext.Authentication;
            var user = authentication.User;
            var claim = (user.Identity as System.Security.Claims.ClaimsIdentity).FindFirst("urn:facebook:access_token");

            string accessToken = null;
            if (claim != null)
                accessToken = claim.Value;

            string sURL;
            sURL = "https://graph.facebook.com/";
            sURL += "" + accessToken + "/me";

            if (accessToken != null)
            {
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                System.IO.Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();

                StreamReader objReader = new StreamReader(objStream);

                string sLine = "";
                int i = 0;

                while (sLine != null)
                {
                    i++;
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        Console.WriteLine("{0}:{1}", i, sLine);
                }
                Console.ReadLine();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}