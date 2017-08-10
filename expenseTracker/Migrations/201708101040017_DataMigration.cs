namespace expenseTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Expenses",
                c => new
                    {
                        ExpenseId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Comment = c.String(),
                        Amount = c.Int(nullable: false),
                        DateAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ExpenseId);
            
            AddColumn("dbo.AspNetUsers", "Age", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Age");
            DropTable("dbo.Expenses");
        }
    }
}
