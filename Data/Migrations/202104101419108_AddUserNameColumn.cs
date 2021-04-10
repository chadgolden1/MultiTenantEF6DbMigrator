namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserNameColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UserName");
        }
    }
}
