namespace FoodDeliveryDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterOrdetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropColumn("dbo.OrderDetails", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderDetails", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.OrderDetails", "ProductId");
            AddForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
    }
}
