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

        public ActionResult History()
        {
            return View();
        }

        public ActionResult getHist()
        {
            using (var db = new WebPortalContext())
            {
                var events = db.ChatMessages.ToList<ChatMessage>();

                if (events.Count > 0)
                {
                    if (events.Count <= 25)
                    {
                        Object[] eventsArray = new Object[events.Count];
                        for (int i = 0; i < events.Count; i++)
                        {
                            ChatMessage e = events.ElementAt(i);
                            eventsArray[i] = new { user = e.user, message = e.message, time = e.timestamp };
                        }
                        var json = Json(eventsArray, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    else
                    {
                        // More than 25 elements but we only want the last 25
                        Object[] eventsArray = new Object[25];
                        for(int i = 25; i >= 1; i--)
                        {
                            ChatMessage e = events.ElementAt(events.Count - i);
                            eventsArray[i-1] = new { user = e.user, message = e.message, time = e.timestamp.ToString() };
                        }

                        var json = Json(eventsArray.Reverse(), JsonRequestBehavior.AllowGet);
                        return json;
                    }
                }
                return Json(new Object[0], JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult getFullHistory()
        {
            using (var db = new WebPortalContext())
            {
                var events = db.ChatMessages.ToList<ChatMessage>();

                Object[] eventsArray = new Object[events.Count];
                for (int i = 0; i < eventsArray.Length; i++)
                {
                    ChatMessage e = events.ElementAt(i);
                    eventsArray[i] = new { user = e.user, message = e.message, time = e.timestamp.ToString() };
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