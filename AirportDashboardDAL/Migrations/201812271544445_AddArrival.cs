namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArrival : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Flight", "Arrival", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flight", "Arrival");
        }
    }
}
