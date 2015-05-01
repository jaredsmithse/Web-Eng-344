namespace WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CalEvents",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user = c.String(),
                        title = c.String(),
                        description = c.String(),
                        start = c.DateTime(),
                        end = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CalEvents");
        }
    }
}
