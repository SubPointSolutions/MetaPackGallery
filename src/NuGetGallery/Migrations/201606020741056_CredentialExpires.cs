namespace NuGetGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CredentialExpires : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "Created", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
            AddColumn("dbo.Credentials", "Expires", c => c.DateTime());

            Sql("UPDATE [dbo].[Credentials] SET [Created] = GETDATE(), [Expires] = DATEADD(Day, 90, GETDATE()) WHERE [Type] = 'apikey.v1'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Credentials", "Expires");
            DropColumn("dbo.Credentials", "Created");
        }
    }
}
