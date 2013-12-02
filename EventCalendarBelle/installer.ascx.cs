using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbraco.Core.Persistence;
using EventCalendarBelle.Models;

namespace EventCalendarBelle
{
    public partial class installer : Umbraco.Web.UmbracoUserControl
    {
        private UmbracoDatabase _db = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._db = ApplicationContext.DatabaseContext.Database;

            this.CreateTable();
        }

        private void CreateTable()
        {
            if (this._db != null)
            {
                this.BulletedList1.Items.Add(new ListItem("Creating tables."));

                //Create Calendar table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating calendar tables."));
                    if (!this._db.TableExist("ec_calendars"))
                    {
                        this._db.CreateTable<EventCalendarBelle.Models.ECalendar>(false);
                        this.BulletedList1.Items.Add(new ListItem("Successfully created tables."));
                    }
                    else
                    {
                        try
                        {
                            this._db.Execute(new Sql("ALTER TABLE ec_calendars ALTER COLUMN gcalfeed nvarchar(255) NULL", null));
                        }
                        catch (Exception ex) { }
                        try
                        {
                            this._db.Execute(new Sql("ALTER TABLE ec_calendars ADD color nvarchar(255) NULL", null));
                        }
                        catch (Exception ex) { }
                        this.BulletedList1.Items.Add(new ListItem("Database already exists. Altering some fields or ignoring table"));
                    }
                }
                catch (Exception ex)
                {
                    this.BulletedList1.Items.Add(new ListItem(ex.Message));
                }

                //Create Events table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating events table."));
                    if (!this._db.TableExist("ec_events"))
                    {
                        this._db.CreateTable<Event>(false);
                        this.BulletedList1.Items.Add(new ListItem("Successfully created table."));
                    }
                    else
                    {
                        try
                        {
                            this._db.Execute(new Sql("ALTER TABLE ec_events ALTER COLUMN description ntext", null));
                        }
                        catch (Exception ex) { }
                        this.BulletedList1.Items.Add(new ListItem("Database already exists. Altering some fields or ignoring table."));
                    }
                }
                catch (Exception ex)
                {
                    this.BulletedList1.Items.Add(new ListItem(ex.Message));
                }

                //Create Recurring Events Table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating recurring events table."));
                    if (!this._db.TableExist("ec_recevents"))
                    {
                        this._db.CreateTable<RecurringEvent>(false);
                        this.BulletedList1.Items.Add(new ListItem("Successfully created table."));
                    }
                    else
                    {
                        this.BulletedList1.Items.Add(new ListItem("Database already exists. No changes have to be made or no alter table script has been added"));
                    }
                }
                catch (Exception ex) { }

                //Create Locations table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating locations table."));
                    if (!this._db.TableExist("ec_locations"))
                    {
                        this._db.CreateTable<EventLocation>(false);
                        this.BulletedList1.Items.Add(new ListItem("Successfully created table."));
                    }
                    else
                    {
                        this.BulletedList1.Items.Add(new ListItem("Database already exists. No changes have to be made or no alter table script has been added"));
                    }
                }
                catch (Exception ex)
                {
                    this.BulletedList1.Items.Add(new ListItem(ex.Message));                    
                }
                
                //Creating EventDescription table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating event description table."));
                    if (!this._db.TableExist("ec_eventdescriptions"))
                    {
                        this._db.CreateTable<EventDescription>(false);
                        this.BulletedList1.Items.Add(new ListItem("Successfully created table."));
                    }
                    else
                    {
                        try
                        {
                            this._db.Execute(new Sql("ALTER TABLE ec_eventdescriptions ALTER COLUMN content ntext", null));
                        }
                        catch (Exception ex) { }
                        this.BulletedList1.Items.Add(new ListItem("Database already exists. Altering some fields or ignoring table."));
                    }
                }
                catch (Exception ex) { this.BulletedList1.Items.Add(new ListItem(ex.Message)); }

                this.BulletedList1.Items.Add(new ListItem("Done creating tables."));
            }
            else
            {
                this.BulletedList1.Items.Add(new ListItem("Couldn't create necessary tables."));
            }
        }
    }
}