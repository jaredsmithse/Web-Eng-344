using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebPortal.Models
{
    public class WebPortalContext : DbContext
    {
        public WebPortalContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<CalEvent> CalEvents { get; set; }
        public virtual DbSet<Holding> Holdings { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
    }
}