namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fifth : DbMigration
    {
        public override void Up()
        {
        //    AddColumn("dbo.EMemberRegistration", "Notes", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
         //   DropColumn("dbo.EMemberRegistration", "Notes");
        }
    }
}
