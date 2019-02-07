namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlightUpdate3 : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.PriceListID)
                .ForeignKey("dbo.Flight", t => t.FlightID, cascadeDelete: true)
                .Index(t => t.FlightID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PriceList", "FlightID", "dbo.Flight");
            DropIndex("dbo.PriceList", new[] { "FlightID" });
            DropTable("dbo.PriceList");
        }
    }
}
