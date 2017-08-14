namespace expenseTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedId : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Expenses", new[] { "ExpenseId" });
            DropColumn("dbo.Expenses", "ExpenseId");
            AddColumn("dbo.Expenses", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Expenses", "Id");
        }  
        public override void Down()
        {
            AddColumn("dbo.Expenses", "ExpenseId", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Expenses");
            DropColumn("dbo.Expenses", "Id");
            AddPrimaryKey("dbo.Expenses", "ExpenseId");
        }
    }
}
