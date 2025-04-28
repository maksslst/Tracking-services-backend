using FluentMigrator;

namespace Infrastructure.Database.Migrations;

[Migration(20250428)]
public class AddUserRoleAndPassword : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE TYPE user_role AS ENUM ('Admin', 'Moderator', 'User')");

        Alter.Table("users")
            .AddColumn("role").AsCustom("user_role").WithDefaultValue("User")
            .AddColumn("password_hash").AsString(255).Nullable();
    }

    public override void Down()
    {
        Delete.Column("role")
            .Column("password_hash")
            .FromTable("users");
        
        Execute.Sql("DROP TYPE user_role");
    }
}