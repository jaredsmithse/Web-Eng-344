using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebPortal.Models
{
    public class CalEvent
    {
        public int id { get; set; }
        public string user { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }

    }
}