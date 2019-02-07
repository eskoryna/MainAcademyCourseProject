namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Departure : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flight", "Departure", c => c.String(maxLength: 255));
            DropColumn("dbo.Flight", "Arrival");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Flight", "Arrival", c => c.String(maxLength: 255));
            DropColumn("dbo.Flight", "Departure");
        }
    }
}
