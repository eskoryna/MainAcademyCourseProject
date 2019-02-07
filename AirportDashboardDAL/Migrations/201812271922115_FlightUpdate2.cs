namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlightUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flight", "DepartureDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Flight", "ArrivalDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Flight", "FlightStatus", c => c.String(maxLength: 20));
            AddColumn("dbo.Flight", "Terminal", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flight", "Terminal");
            DropColumn("dbo.Flight", "FlightStatus");
            DropColumn("dbo.Flight", "ArrivalDateTime");
            DropColumn("dbo.Flight", "DepartureDateTime");
        }
    }
}
