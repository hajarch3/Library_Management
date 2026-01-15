namespace Gestion_bibliot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubscriptionUserRelation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subscriptions", "UserId", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Subscriptions", "UserId");
            AddForeignKey("dbo.Subscriptions", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Subscriptions", new[] { "UserId" });
            AlterColumn("dbo.Subscriptions", "UserId", c => c.String(nullable: false));
        }
    }
}
