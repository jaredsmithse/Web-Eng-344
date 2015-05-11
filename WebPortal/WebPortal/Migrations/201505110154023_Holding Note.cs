namespace WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HoldingNote : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Holdings", "Note", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Holdings", "Note");
        }
    }
}
