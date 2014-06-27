using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Migrations;

namespace EventCalendarBelle.Migrations
{
    [Migration("2.0.2", 0, "UpdateEventCalendarTables2.0.2")]
    public class CalendarTextColorMigration : MigrationBase
    {
        public override void Down()
        {
            Delete.Column("textcolor").FromTable("ec_calendars");
        }

        public override void Up()
        {
            Alter.Table("ec_calendars").AddColumn("textcolor").AsString(255).Nullable();
        }
    }
}
