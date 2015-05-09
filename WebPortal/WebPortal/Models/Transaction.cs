using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebPortal.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string User { get; set; }
        public bool buy { get; set; }
        public int Amount { get; set; }
        public DateTime date { get; set; }
        public decimal price { get; set; }
    }
}