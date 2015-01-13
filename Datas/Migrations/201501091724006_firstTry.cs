namespace Datas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstTry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Email", c => c.String());
            AddColumn("dbo.AspNetUsers", "Money", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Money");
            DropColumn("dbo.AspNetUsers", "Email");
        }
    }
}
