namespace Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Terminal3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Flight", "Terminal", c => c.String(maxLength: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Flight", "Terminal", c => c.String(maxLength: 1));
        }
    }
}
