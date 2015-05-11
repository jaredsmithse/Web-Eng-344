using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPortal.Models
{
    public class ChatMessage
    {
        public int id { get; set; }
        public string user { get; set; }
        public string message { get; set; }
        public DateTime? timestamp { get; set; }
    }
}