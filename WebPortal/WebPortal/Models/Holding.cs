using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebPortal.Models
{
    public class Holding
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string User { get; set; }
        public decimal AmountBought { get; set; }
        public decimal AmountSold { get; set; }
        public int AmountOwned { get; set; }
        public string Note { get; set; }
    }
}