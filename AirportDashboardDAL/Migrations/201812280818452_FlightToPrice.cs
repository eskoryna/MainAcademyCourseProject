namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlightToPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PriceList", "FlightID", c => c.Guid(nullable: false));
            AddColumn("dbo.PriceList", "Price", c => c.Double(nullable: false));
            CreateIndex("dbo.PriceList", "FlightID");
            AddForeignKey("dbo.PriceList", "FlightID", "dbo.Flight", "FlightID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PriceList", "FlightID", "dbo.Flight");
            DropIndex("dbo.PriceList", new[] { "FlightID" });
            DropColumn("dbo.PriceList", "Price");
            DropColumn("dbo.PriceList", "FlightID");
        }
    }
}
