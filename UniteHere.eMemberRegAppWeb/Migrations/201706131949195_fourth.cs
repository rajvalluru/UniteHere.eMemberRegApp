namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fourth : DbMigration
    {
        public override void Up()
        {
          //  AlterColumn("dbo.EMemberRegistration", "Date_Of_Hire", c => c.DateTime());
           // AlterColumn("dbo.EMemberRegistrationViewModel", "Date_Of_Hire", c => c.DateTime());
        }
        
        public override void Down()
        {
         //   AlterColumn("dbo.EMemberRegistrationViewModel", "Date_Of_Hire", c => c.DateTime(nullable: false));
         //   AlterColumn("dbo.EMemberRegistration", "Date_Of_Hire", c => c.DateTime(nullable: false));
        }
    }
}
