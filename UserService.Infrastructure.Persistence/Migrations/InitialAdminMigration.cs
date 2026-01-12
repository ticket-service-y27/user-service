using FluentMigrator;

namespace UserService.Infrastructure.Persistence.Migrations;

[Migration(version: 2026011002, description: "Initial admin")]
public class InitialAdminMigration : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            insert into users (user_nickname, user_email, user_password_hash, user_role, user_created_at)
            values ('admin', 'admin@gmail.com', '$2a$10$/AZ1rhJErTjaOXWFK6FWiuD2MSG4Xfv6slFKfVDIVa0C5m2Vw160u', 'admin', now());
            """);
    }

    public override void Down()
    {
        Execute.Sql(
            """
            delete from users
            where user_email = 'admin@gmail.com';
            """);
    }
}