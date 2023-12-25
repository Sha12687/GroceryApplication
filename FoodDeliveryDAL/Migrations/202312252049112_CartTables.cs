namespace FoodDeliveryDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "ProductId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "ProductId");
        }
    }
}
