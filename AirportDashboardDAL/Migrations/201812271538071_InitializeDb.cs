namespace AirportDashboardDAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Flight",
                c => new
                    {
                        FlightID = c.Guid(nullable: false),
                        Destination = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.FlightID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Flight");
        }
    }
}
