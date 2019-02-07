namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlightUpdate4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PriceList", "FlightID", "dbo.Flight");
            DropIndex("dbo.PriceList", new[] { "FlightID" });
            DropTable("dbo.PriceList");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PriceList",
                c => new
                    {
                        PriceListID = c.Guid(nullable: false),
                        FlightID = c.Guid(nullable: false),
                        FlightClass = c.String(maxLength: 20),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.PriceListID);
            
            CreateIndex("dbo.PriceList", "FlightID");
            AddForeignKey("dbo.PriceList", "FlightID", "dbo.Flight", "FlightID", cascadeDelete: true);
        }
    }
}
