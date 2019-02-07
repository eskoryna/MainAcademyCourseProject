namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatasNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Flight", "DepartureDateTime", c => c.DateTime());
            AlterColumn("dbo.Flight", "ArrivalDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Flight", "ArrivalDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Flight", "DepartureDateTime", c => c.DateTime(nullable: false));
        }
    }
}
