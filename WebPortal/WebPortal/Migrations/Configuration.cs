namespace WebPortal.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebPortal.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebPortal.Models.WebPortalContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WebPortal.Models.WebPortalContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            new List<CalEvent>
            {
                new CalEvent() 
                { 
                    id=1,
                    title = "Event 1",
                    description = "My First Event",
                    start = DateTime.Now,
                    end = DateTime.Now.AddHours(1.0)

                },
                new CalEvent() 
                { 
                    id=2,
                    title = "Event 2",
                    description = "My Second Event",
                    start = DateTime.Now,
                    end = DateTime.Now.AddHours(2.0)

                }

            }.ForEach(calevent => context.CalEvents.AddOrUpdate(calevent));
            context.SaveChanges();
        }
    }
}
