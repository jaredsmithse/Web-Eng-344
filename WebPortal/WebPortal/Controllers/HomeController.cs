using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;
using WebPortal.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System.Threading.Tasks;

namespace WebPortal.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        // Commented out stuff is from Greg. Leaving it here in case it's needed again
        // Adding UserManager directly in this controller I *think* elimnated the need of AccountController -EJ
        //private AccountController ac = new AccountController();
        //private System.Threading.Tasks.Task<ActionResult> test;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        //public ActionResult Index()
        public async Task<ActionResult> Index()
        {
            //return View(this.ac.FacebookFeed()); // Original Code
            //var claimsforUser = await ac.UserManager.GetClaimsAsync(User.Identity.GetUserId()); // Code copied from User Manager
            
            // Types of posts we want to show
            var validTypes = new List<String>();
            validTypes.Add("status");
            validTypes.Add("link");
            validTypes.Add("photo");
            validTypes.Add("user");
            validTypes.Add("video");

            var claimsforUser = await UserManager.GetClaimsAsync(User.Identity.GetUserId()); // Copied UserManager stuff from account controller so we wouldn't need to use it
            var access_token = claimsforUser.FirstOrDefault(x => x.Type == "FacebookAccessToken").Value;
            var fb = new FacebookClient(access_token);
            fb.AppId = 1549521788629862 + "";
            fb.AppSecret = "3072d557ae33bd64013e58ed3dc44006";
            var parameters = new Dictionary<string, object>();
            parameters["limit"] = "100";
            try
            {
                dynamic myInfo = fb.Get("/me/home", parameters);
                var friendsList = new List<FacebookFeedModel>();
                foreach (dynamic post in myInfo.data)
                {
                    // Only show certain types of posts for simplicity
                    if (post.type == null || !validTypes.Contains(post.type)) continue;

                    // Skip posts with no messages
                    if (post.message == null) continue;
                    if (post.likes != null)
                    {
                        friendsList.Add(new FacebookFeedModel()
                        {
                            Name = post.from.name,
                            Message = post.message,
                            Link = post.link,
                            Likes = post.likes.data.Count
                        });
                    }//if
                    else
                    {
                        friendsList.Add(new FacebookFeedModel
                        {
                            Name = post.from.name,
                            Message = post.message,
                            Link = post.link,
                            Likes = 0
                        });
                    }
                }

                var HomePage = new HomePageViewModel();
                HomePage.friendsList = friendsList;
                HomePage.todaysEvents = getEventsForDay(DateTime.Now);

                return View(HomePage);
            }
            catch (FacebookOAuthException e)
            {
                //TODO display an error in place of the news feed
                return View();
            }
        }

        [HttpGet]
        public ActionResult UpdateStatus()
        {
            return View(new UpdateStatusViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStatus(String status)
        {
            var vm = new UpdateStatusViewModel();
            if (status == null || status == "")
            {
                vm.Message = "Your status cannot be empty";
            }
            else
            {
                // TODO actually update facebook status here. If it doesn't work use Message as error message, like this line does
                var claimsforUser = await UserManager.GetClaimsAsync(User.Identity.GetUserId()); // Copied UserManager stuff from account controller so we wouldn't need to use it
                var access_token = claimsforUser.FirstOrDefault(x => x.Type == "FacebookAccessToken").Value;
                var fb = new FacebookClient(access_token);
                fb.AppId = 1549521788629862 + "";
                fb.AppSecret = "3072d557ae33bd64013e58ed3dc44006";
                var parameters = new Dictionary<string, object>();
                parameters["message"] = status;
                dynamic myInfo = fb.Post("/me/feed", parameters);

                vm.Message = "Unable to update status. Please try again.";
            }
            vm.Status = status;
            return View(vm);
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
        public List<CalEvent> getEventsForDay(DateTime day)
        {
            using (var db = new WebPortalContext())
            {
                // TODO only grab events for current user
                var events = db.CalEvents.ToList<CalEvent>();

                List<CalEvent> eventsForDay = new List<CalEvent>();
                foreach(var e in events)
                {
                    if(e.start != null)
                        if (e.start.Value.Date == DateTime.Now.Date)
                            eventsForDay.Add(e);
                }

                return eventsForDay;
            }
        }

    }
}