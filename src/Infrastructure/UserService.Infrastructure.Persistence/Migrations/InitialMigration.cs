using FluentMigrator;

namespace UserService.Infrastructure.Persistence.Migrations;

[Migration(version: 2026011001, description: "Initial migration")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            create type user_role as enum ('user', 'admin', 'organizer');

            create table users
            (
                user_id bigint primary key generated always as identity,
                
                user_nickname      text                     not null unique,
                user_email         text                     not null,
                user_password_hash text                     not null,
                user_role          user_role                not null,
                user_created_at    timestamp with time zone not null,
                
                user_is_blocked boolean                  not null default false,
                user_blocked_at timestamp with time zone null
            );
            """);
    }

    public override void Down()
    {
        Execute.Sql(
            """
            drop table users;
            drop type user_role;
            """);
    }
}