namespace expenseTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changeddbcontext : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Expenses", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Expenses", "ApplicationUser_Id");
            AddForeignKey("dbo.Expenses", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Expenses", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Expenses", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Expenses", "ApplicationUser_Id");
        }
    }
}
