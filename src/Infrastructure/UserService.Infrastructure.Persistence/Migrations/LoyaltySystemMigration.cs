using FluentMigrator;

namespace UserService.Infrastructure.Persistence.Migrations;

[Migration(version: 2026011501, description: "Loyalty system migration")]
public class LoyaltySystemMigration : Migration
{
    public override void Up()
    {
        Execute.Sql(
            """
            create type user_loyalty_level as enum ('bronze', 'silver', 'gold', 'platinum');

            create table user_loyalty_accounts 
            (
                user_id       bigint primary key references users (user_id) on delete cascade,
                
                period_total_spent   bigint                   not null default 0,
                loyalty_level        user_loyalty_level       not null default 'bronze',
                calculated_at        timestamp with time zone not null default now(),
                is_blocked           boolean                  not null default false,
                blocked_at           timestamp with time zone null
            );
            
            create table user_loyalty_periods
            (
                user_id                  bigint primary key references user_loyalty_accounts (user_id) on delete cascade,
                
                period_start_at          timestamp with time zone not null default now(),
                period_start_total_spent bigint                   not null default 0,
                period_end_total_spent   bigint                   not null default 0,
                calculated_at            timestamp with time zone not null default now()
            );
            """);
    }

    public override void Down()
    {
        Execute.Sql(
            """
            drop table user_loyalty_periods;
            drop table user_loyalty_accounts;
            drop type user_loyalty_level;
            """);
    }
}