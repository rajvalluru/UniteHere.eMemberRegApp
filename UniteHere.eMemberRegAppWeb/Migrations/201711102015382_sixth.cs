namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sixth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "TipActionCode", c => c.String(maxLength: 10));
            AlterColumn("dbo.EMemberRegistration", "Validation_Status", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EMemberRegistration", "Validation_Status", c => c.String(maxLength: 256));
            DropColumn("dbo.EMemberRegistration", "TipActionCode");
        }
    }
}
