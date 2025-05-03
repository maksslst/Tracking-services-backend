using FluentMigrator;
using System.Data;

namespace Infrastructure.Database.Migrations;

[Migration(20250321)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        #region Companies
        Create.Table("companies")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("company_name").AsString(100).NotNullable();
        #endregion

        #region Users
        Create.Table("users")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("username").AsString(100).NotNullable().Unique()
            .WithColumn("first_name").AsString(100).NotNullable()
            .WithColumn("last_name").AsString(100).NotNullable()
            .WithColumn("patronymic").AsString(100).Nullable()
            .WithColumn("email").AsString(100).NotNullable().Unique()
            .WithColumn("company_id").AsInt32().Nullable()
            .ForeignKey("companies", "id")
            .OnDeleteOrUpdate(Rule.Cascade);
        #endregion

        #region Resources
        Create.Table("resources")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("type").AsString(100).NotNullable()
            .WithColumn("source").AsString(100).NotNullable()
            .WithColumn("company_id").AsInt32().Nullable()
            .ForeignKey("companies", "id")
            .OnDelete(Rule.SetNull)
            .WithColumn("status").AsInt32().NotNullable();
        #endregion

        #region Metrics
        Create.Table("metrics")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable()
            .WithColumn("resource_id").AsInt32().NotNullable()
            .ForeignKey("resources", "id")
            .OnDeleteOrUpdate(Rule.Cascade)
            .WithColumn("unit").AsString(10).NotNullable()
            .WithColumn("created").AsDateTime().NotNullable();
        #endregion

        #region MetricValues
        Create.Table("metric_values")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("metric_id").AsInt32().NotNullable()
            .ForeignKey("metrics", "id")
            .OnDeleteOrUpdate(Rule.Cascade)
            .WithColumn("value").AsDouble().NotNullable();
        #endregion

        #region MonitoringSettings
        Create.Table("monitoring_settings")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("resource_id").AsInt32().NotNullable()
            .ForeignKey("resources", "id")
            .OnDeleteOrUpdate(Rule.Cascade)
            .WithColumn("check_interval").AsString(50).NotNullable()
            .WithColumn("mode").AsBoolean().NotNullable();
        #endregion

        #region ServiceTasks
        Create.Table("service_tasks")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("resource_id").AsInt32().NotNullable()
            .ForeignKey("resources", "id")
            .OnDeleteOrUpdate(Rule.Cascade)
            .WithColumn("description").AsString(100).NotNullable()
            .WithColumn("assigned_user_id").AsInt32().NotNullable()
            .ForeignKey("fk_serviceTasks_assigned_user_id", "users", "id")
            .OnDeleteOrUpdate(Rule.Cascade)
            .WithColumn("created_by_id").AsInt32().NotNullable()
            .ForeignKey("fk_serviceTasks_created_by_id", "users", "id")
            .OnDeleteOrUpdate(Rule.Cascade)
            .WithColumn("start_time").AsDateTime().NotNullable()
            .WithColumn("completion_time").AsDateTime().Nullable()
            .WithColumn("status").AsInt32().NotNullable();
        #endregion

        #region TestDataCompanies
        Insert.IntoTable("companies")
            .Row(new { company_name = "Kub Group" })
            .Row(new { company_name = "Schoen, Lang and Reichert" });
        #endregion

        #region TestDataUsers
        Insert.IntoTable("users")
            .Row(new
            {
                username = "Patrick1",
                first_name = "Patrick",
                last_name = "Bednar",
                patronymic = "Grimes",
                email = "Patrick_Bednar@gmail.com",
                company_id = 1
            })
            .Row(new
            {
                username = "Patrick2",
                first_name = "Patrick",
                last_name = "Bednar",
                patronymic = "Marks",
                email = "Patrick_Bednar1@gmail.com",
                company_id = 2
            })
            .Row(new
            {
                username = "Patrick3",
                first_name = "Patrick",
                last_name = "Jones",
                patronymic = "Marks",
                email = "Patrick_Bednar3@gmail.com",
                company_id = 1
            });
        #endregion

        #region TestDataResources
        Insert.IntoTable("resources")
            .Row(new
            {
                company_id = 1,
                name = "tanner.biz",
                type = "website",
                source = "240.220.247.221",
                status = 1
            })
            .Row(new
            {
                company_id = 2,
                name = "anahi.biz",
                type = "website",
                source = "205.0.224.240",
                status = 1
            });
        #endregion

        #region TestDataMetrics
        Insert.IntoTable("metrics")
            .Row(new
            {
                name = "Проверка доступности ping",
                resource_id = 1,
                created = "2025-03-20",
                unit = "мс"
            })
            .Row(new
            {
                name = "Проверка доступности ping",
                resource_id = 2,
                created = "2025-03-20",
                unit = "мс"
            });
        #endregion

        #region TestDataMetricValues
        Insert.IntoTable("metric_values")
            .Row(new { metric_id = 1, value = 0.73 })
            .Row(new { metric_id = 1, value = 0.19 })
            .Row(new { metric_id = 2, value = 0.5 })
            .Row(new { metric_id = 2, value = 0.29 });
        #endregion

        #region TestDateMonitoringSettings
        Insert.IntoTable("monitoring_settings")
            .Row(new
            {
                resource_id = 1,
                check_interval = "0 0/5 * * * ?",
                mode = true
            })
            .Row(new
            {
                resource_id = 2,
                check_interval = "*/2 * * * *",
                mode = true
            });
        #endregion

        #region TestDataServiceTasks
        Insert.IntoTable("service_tasks")
            .Row(new
            {
                resource_id = 1,
                description = "synergize",
                assigned_user_id = 1,
                created_by_id = 3,
                start_time = "2025-03-20",
                completion_time = "2025-03-20",
                status = 3
            })
            .Row(new
            {
                resource_id = 1,
                description = "text",
                assigned_user_id = 1,
                created_by_id = 3,
                start_time = "2025-03-20",
                completion_time = "2025-03-20",
                status = 1
            });
        #endregion
    }

    public override void Down()
    {
        Delete.Table("service_tasks");
        Delete.Table("monitoring_settings");
        Delete.Table("metric_values");
        Delete.Table("metrics");
        Delete.Table("resources");
        Delete.Table("users");
        Delete.Table("companies");
    }
}