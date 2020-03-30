namespace UniteHere.eMemberRegAppWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EMemberRegistration", "Department", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "Section", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "Craft", c => c.String(maxLength: 20));
            AddColumn("dbo.EMemberRegistration", "FullPartTime", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EMemberRegistration", "FullPartTime");
            DropColumn("dbo.EMemberRegistration", "Craft");
            DropColumn("dbo.EMemberRegistration", "Section");
            DropColumn("dbo.EMemberRegistration", "Department");
        }
    }
}
