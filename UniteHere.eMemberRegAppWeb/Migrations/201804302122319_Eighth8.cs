namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Eighth8 : DbMigration
    {
        public override void Up()
        {
          //  AddColumn("dbo.EMemberRegistrationViewModel", "IsNewMember", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
          //  DropColumn("dbo.EMemberRegistrationViewModel", "IsNewMember");
        }
    }
}
