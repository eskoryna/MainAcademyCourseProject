namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PassengerList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PassengerList",
                c => new
                    {
                        PassengerListID = c.Guid(nullable: false),
                        FlightID = c.Guid(nullable: false),
                        FlightClass = c.String(maxLength: 20),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Nationality = c.String(maxLength: 30),
                        Passport = c.String(maxLength: 20),
                        BirthDate = c.DateTime(nullable: false),
                        Sex = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.PassengerListID)
                .ForeignKey("dbo.Flight", t => t.FlightID, cascadeDelete: true)
                .Index(t => t.FlightID);
            
            AlterColumn("dbo.PriceList", "Price", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PassengerList", "FlightID", "dbo.Flight");
            DropIndex("dbo.PassengerList", new[] { "FlightID" });
            AlterColumn("dbo.PriceList", "Price", c => c.Double(nullable: false));
            DropTable("dbo.PassengerList");
        }
    }
}
