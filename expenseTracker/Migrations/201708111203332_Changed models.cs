namespace expenseTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changedmodels : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Expenses", name: "ApplicationUser_Id", newName: "User_Id");
            RenameIndex(table: "dbo.Expenses", name: "IX_ApplicationUser_Id", newName: "IX_User_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Expenses", name: "IX_User_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Expenses", name: "User_Id", newName: "ApplicationUser_Id");
        }
    }
}
