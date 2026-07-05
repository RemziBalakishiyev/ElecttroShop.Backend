using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElectroShop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS "AppLogs" (
                    "Id" uuid NOT NULL,
                    "TimestampUtc" timestamp with time zone NOT NULL,
                    "Level" character varying(20) NOT NULL,
                    "Message" text NOT NULL,
                    "Exception" text,
                    "SourceContext" character varying(500),
                    "EventType" character varying(100),
                    "CorrelationId" character varying(64),
                    "UserId" uuid,
                    "UserEmail" character varying(256),
                    "RequestPath" character varying(2048),
                    "RequestMethod" character varying(16),
                    "QueryString" character varying(4096),
                    "RequestBody" text,
                    "ResponseStatusCode" integer,
                    "ElapsedMilliseconds" bigint,
                    "ClientIp" character varying(64),
                    "UserAgent" character varying(1024),
                    "MachineName" character varying(256),
                    "PropertiesJson" jsonb,
                    CONSTRAINT "PK_AppLogs" PRIMARY KEY ("Id")
                );
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_AppLogs_TimestampUtc" ON "AppLogs" ("TimestampUtc");
                CREATE INDEX IF NOT EXISTS "IX_AppLogs_Level" ON "AppLogs" ("Level");
                CREATE INDEX IF NOT EXISTS "IX_AppLogs_CorrelationId" ON "AppLogs" ("CorrelationId");
                CREATE INDEX IF NOT EXISTS "IX_AppLogs_EventType" ON "AppLogs" ("EventType");
                CREATE INDEX IF NOT EXISTS "IX_AppLogs_UserId" ON "AppLogs" ("UserId");
                CREATE INDEX IF NOT EXISTS "IX_AppLogs_TimestampUtc_Level" ON "AppLogs" ("TimestampUtc", "Level");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLogs");
        }
    }
}
