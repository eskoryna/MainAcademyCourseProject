namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlightUpdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flight", "FlightNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.Flight", "Carrier", c => c.String(maxLength: 255));
            AddColumn("dbo.Flight", "ArriveDepart", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flight", "ArriveDepart");
            DropColumn("dbo.Flight", "Carrier");
            DropColumn("dbo.Flight", "FlightNumber");
        }
    }
}
