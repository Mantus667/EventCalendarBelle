using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbraco.Core.Persistence;
using EventCalendarBelle.Models;
using System.Web.Hosting;
using System.Xml;

namespace EventCalendarBelle
{
    public partial class installer : Umbraco.Web.UmbracoUserControl
    {
        private UmbracoDatabase _db = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._db = ApplicationContext.DatabaseContext.Database;

            //Create section and language code
            this.CreateSection();
            this.addLanguagekey();

            //Create database tables
            this.CreateTable();
        }

        private void CreateSection()
        {
            this.BulletedList1.Items.Add(new ListItem("Creating the section."));

            var sectionService = ApplicationContext.Services.SectionService;

            //Try & find a section with the alias of "nuget"
            var ecSection = sectionService.GetSections().SingleOrDefault(x => x.Alias == "eventCalendar");

            //If we can't find the section - doesn't exist
            if (ecSection == null)
            {
                //So let's create it the section
                sectionService.MakeNew("EventCalendar", "eventCalendar", "icon-calendar-alt");
            }
            this.BulletedList1.Items.Add(new ListItem("Done creating the section."));
        }

        private void addLanguagekey()
        {
            this.BulletedList1.Items.Add(new ListItem("Adding language keys."));

            bool saveFile = false;

            //Open up language file
            //umbraco/config/lang/en.xml
            var langPath = "~/umbraco/config/lang/en.xml";

            //Path to the file resolved
            var langFilePath = HostingEnvironment.MapPath(langPath);

            //Load settings.config XML file
            XmlDocument langXml = new XmlDocument();
            langXml.Load(langFilePath);

            // Section Node
            // <area alias="sections">
            XmlNode sectionNode = langXml.SelectSingleNode("//area [@alias='sections']");

            if (sectionNode != null)
            {
                XmlNode findSectionKey = sectionNode.SelectSingleNode("./key [@alias='eventCalendar']");

                if (findSectionKey == null)
                {
                    //Let's add the key
                    var attrToAdd = langXml.CreateAttribute("alias");
                    attrToAdd.Value = "eventCalendar";

                    var keyToAdd = langXml.CreateElement("key");
                    keyToAdd.InnerText = "EventCalendar";
                    keyToAdd.Attributes.Append(attrToAdd);

                    sectionNode.AppendChild(keyToAdd);

                    //Save the file flag to true
                    saveFile = true;
                }
            }

            // Section Node
            // <area alias="treeHeaders">
            XmlNode treeNode = langXml.SelectSingleNode("//area [@alias='treeHeaders']");

            if (treeNode != null)
            {
                XmlNode findTreeKey = treeNode.SelectSingleNode("./key [@alias='eventCalendar']");

                if (findTreeKey == null)
                {
                    //Let's add the key
                    var attrToAdd = langXml.CreateAttribute("alias");
                    attrToAdd.Value = "eventCalendar";

                    var keyToAdd = langXml.CreateElement("key");
                    keyToAdd.InnerText = "EventCalendar";
                    keyToAdd.Attributes.Append(attrToAdd);

                    treeNode.AppendChild(keyToAdd);

                    //Save the file flag to true
                    saveFile = true;
                }
            }


            //If saveFile flag is true then save the file
            if (saveFile)
            {
                //Save the XML file
                langXml.Save(langFilePath);
            }

            this.BulletedList1.Items.Add(new ListItem("Done adding language keys."));
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