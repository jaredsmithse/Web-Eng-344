using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;

namespace WebPortal.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        //
        // GET: /Chat/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getHist()
        {
            using (var db = new WebPortalContext())
            {
                var events = db.ChatMessages.ToList<ChatMessage>();

                Object[] eventsArray = new Object[25];
                int i = events.Count - 1;
               for (int j = 24; j >= 0; j--, i--)
               {
                   ChatMessage e = events.ElementAt(i);
                   eventsArray[j] = new { user = e.user, message = e.message, time = e.timestamp };
               }
                var json = Json(eventsArray, JsonRequestBehavior.AllowGet);
                return json;
            }
        }
        public ActionResult addMessage(string message)
        {
            if (message != null && !message.Trim().Equals("") )
                using (var db = new WebPortalContext())
                {
                    var e = new ChatMessage();
                    e.message = message;
                    e.timestamp = DateTime.Now;
                    e.user = User.Identity.Name;

                    db.ChatMessages.Add(e);
                    db.SaveChanges();
                }

            return View("Index");
        }
	}
}