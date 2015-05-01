using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPortal.Models
{
    public class HomePageViewModel
    {
        public List<FacebookFeedModel> friendsList { get; set;}
        public List<CalEvent> todaysEvents { get; set; }
    }
}