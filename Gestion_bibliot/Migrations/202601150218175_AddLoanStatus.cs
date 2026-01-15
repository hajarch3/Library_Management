namespace Gestion_bibliot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoanStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Loans", "Status", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Loans", "Status");
        }
    }
}
