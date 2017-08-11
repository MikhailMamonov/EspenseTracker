namespace expenseTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Expensecorrected : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Expenses", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Expenses", "Comment", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Expenses", "Comment", c => c.String());
            AlterColumn("dbo.Expenses", "Description", c => c.String());
        }
    }
}
