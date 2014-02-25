using EventCalendarBelle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;

namespace EventCalendarBelle
{
    public class DatabaseUp : ApplicationEventHandler
    {
        private UmbracoDatabase _db = null;

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            //try {
            //    var runner = new MigrationRunner( new System.Version("2.0"), new System.Version("2.0.1"), "UpdateEventsForCategorys");                
            //    var upgraded = runner.Execute(applicationContext.DatabaseContext.Database, true);
            //} catch (Exception ex) { LogHelper.Error<DatabaseUp>("Failed to do the migration",ex); }

            //this._db = applicationContext.DatabaseContext.Database;

            //if (this._db != null)
            //{
            //    //Create Calendar table
            //    try
            //    {
            //        if (!this._db.TableExist("ec_calendars"))
            //        {
            //            this._db.CreateTable<EventCalendarBelle.Models.ECalendar>(false);
            //            //Create first calendar
            //            ECalendar cal = new ECalendar() { Calendarname = "TestCalendar", DisplayOnSite = true, IsGCal = false, GCalFeedUrl = "", Color = "#0E0E0E" };
            //            this._db.Save(cal);
            //        }
            //    }
            //    catch (Exception ex)
            //    {}

            //    //Create Locations table
            //    try
            //    {
            //        if (!this._db.TableExist("ec_locations"))
            //        {
            //            this._db.CreateTable<EventLocation>(false);
            //        }
            //    }
            //    catch (Exception ex)
            //    {}

            //    try
            //    {
            //        if (!this._db.TableExist("ec_events"))
            //        {
            //            this._db.CreateTable<Event>(false);
            //        }
            //    }
            //    catch (Exception ex)
            //    { }

            //    if (!this._db.TableExist("ec_recevents"))
            //    {
            //        this._db.CreateTable<RecurringEvent>(false);
            //    }

            //    if (!this._db.TableExist("ec_eventdescriptions"))
            //    {
            //        this._db.CreateTable<EventDescription>(false);
            //    }
            //}
        }
    }
}
