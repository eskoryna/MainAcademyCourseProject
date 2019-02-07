namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewPriceList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PriceList",
                c => new
                    {
                        PriceListID = c.Guid(nullable: false),
                        FlightClass = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.PriceListID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PriceList");
        }
    }
}
