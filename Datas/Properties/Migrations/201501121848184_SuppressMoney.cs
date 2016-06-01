namespace Datas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SuppressMoney : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Money");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Money", c => c.Int());
        }
    }
}
