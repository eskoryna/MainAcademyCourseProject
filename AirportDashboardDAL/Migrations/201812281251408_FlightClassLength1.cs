namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FlightClassLength1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PassengerList", "FlightClass", c => c.String(maxLength: 1));
            AlterColumn("dbo.PriceList", "FlightClass", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PriceList", "FlightClass", c => c.String(maxLength: 20));
            AlterColumn("dbo.PassengerList", "FlightClass", c => c.String(maxLength: 20));
        }
    }
}
