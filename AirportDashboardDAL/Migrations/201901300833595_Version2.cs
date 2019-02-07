namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Version2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Flight",
                c => new
                    {
                        FlightID = c.Guid(nullable: false),
                        Destination = c.String(maxLength: 255),
                        Departure = c.String(maxLength: 255),
                        FlightNumber = c.String(maxLength: 20),
                        Carrier = c.String(maxLength: 255),
                        DepartureDateTime = c.DateTime(nullable: false),
                        ArrivalDateTime = c.DateTime(nullable: false),
                        FlightStatus = c.String(maxLength: 20),
                        Terminal = c.String(maxLength: 1),
                        ArriveDepart = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.FlightID);
            
            CreateTable(
                "dbo.PassengerList",
                c => new
                    {
                        PassengerID = c.Guid(nullable: false),
                        FlightID = c.Guid(nullable: false),
                        FlightClass = c.String(maxLength: 1),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Nationality = c.String(maxLength: 30),
                        Passport = c.String(maxLength: 20),
                        BirthDate = c.DateTime(nullable: false),
                        Sex = c.String(maxLength: 1),
                    })
                .PrimaryKey(t => t.PassengerID)
                .ForeignKey("dbo.Flight", t => t.FlightID, cascadeDelete: true)
                .Index(t => t.FlightID);
            
            CreateTable(
                "dbo.PriceList",
                c => new
                    {
                        PriceItemID = c.Guid(nullable: false),
                        FlightID = c.Guid(nullable: false),
                        FlightClass = c.String(maxLength: 1),
                        Price = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.PriceItemID)
                .ForeignKey("dbo.Flight", t => t.FlightID, cascadeDelete: true)
                .Index(t => t.FlightID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PriceList", "FlightID", "dbo.Flight");
            DropForeignKey("dbo.PassengerList", "FlightID", "dbo.Flight");
            DropIndex("dbo.PriceList", new[] { "FlightID" });
            DropIndex("dbo.PassengerList", new[] { "FlightID" });
            DropTable("dbo.PriceList");
            DropTable("dbo.PassengerList");
            DropTable("dbo.Flight");
        }
    }
}
