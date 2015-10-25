using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Umbraco.Core.Persistence;
using EventCalendar.Core.Models;
using System.Web.Hosting;
using System.Xml;
using System.Web.Configuration;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Logging;
using System.Configuration;
using System.IO;
using umbraco.BusinessLogic;
using EventCalendar.Core.Dto;

namespace EventCalendarBelle
{
    public partial class installer : Umbraco.Web.UmbracoUserControl
    {
        private UmbracoDatabase _db = null;
        protected Version newVersion = new Version("2.2.0");
        private Version oldVersion = new Version("2.0.0");

        protected void Page_Load(object sender, EventArgs e)
        {
            this._db = ApplicationContext.DatabaseContext.Database;

            //Get the current version from appsettings if its there
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);

            if (config.AppSettings.Settings.AllKeys.Contains("EventCalendarVersion"))//WebConfigurationManager.AppSettings.AllKeys.Contains("EventCalendarVersion"))
            {
                oldVersion = new Version(WebConfigurationManager.AppSettings["EventCalendarVersion"]);
                if (newVersion != oldVersion)
                {
                    config.AppSettings.Settings.Remove("EventCalendarVersion");
                    config.Save();
                    config.AppSettings.Settings.Add("EventCalendarVersion", newVersion.ToString());
                    config.Save();
                }
            }
            else
            {                
                config.AppSettings.Settings.Add("EventCalendarVersion", newVersion.ToString());
                config.Save();
            }

            //Create section and language code
            this.CreateSection();
            this.addLanguagekey();
            this.AddSectionDashboard();

            //Create database tables
            this.CreateTable();

            this.RunMigrations();
        }

        private void RunMigrations()
        {
            this.BulletedList1.Items.Add(new ListItem("Running Migrations."));
            try
            {
                var runner = new MigrationRunner(this.oldVersion, this.newVersion, "UpdateEventCalendarTables");
                var upgraded = runner.Execute(this._db, true);
                this.BulletedList1.Items.Add(new ListItem("Done doing migration for version " + this.newVersion.ToString()));
                LogHelper.Info<installer>("Done doing migration for version " + this.newVersion.ToString());
            }
            catch (Exception ex) {
                this.BulletedList1.Items.Add(new ListItem("Failed to do the migration for a version."));
                LogHelper.Error<installer>("Failed to do the migration for a version", ex);
            }

            this.BulletedList1.Items.Add(new ListItem("Done running Migrations."));
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
                this.BulletedList1.Items.Add(new ListItem("Done creating the section."));

                //Add the section to the allowed once for the admin
                var admin = new User(0);
                if (!admin.Applications.Any(x => x.alias == "eventCalendar"))
                {
                    admin.AddApplication("eventCalendar");
                    admin.Save();
                }
                this.BulletedList1.Items.Add(new ListItem("Added Admin to the section."));
            }            
        }

        private void addLanguagekey()
        {
            this.BulletedList1.Items.Add(new ListItem("Adding language keys."));

            bool saveFile = false;

            //Open up language file
            //umbraco/config/lang/en.xml
            var langPath = "~/umbraco/config/lang/"; //en.xml";

            //Path to the file resolved
            var langFilePath = HostingEnvironment.MapPath(langPath);

            var folder = new DirectoryInfo(langFilePath);

            foreach (var file in folder.GetFiles("*.xml"))
            {
                //Load settings.config XML file
                XmlDocument langXml = new XmlDocument();
                langXml.Load(file.FullName);

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
                    langXml.Save(file.FullName);
                }
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
                        this._db.CreateTable<CalendarDto>(false);
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
                        this._db.CreateTable<EventDto>(false);
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
                        this._db.CreateTable<RecurringEventDto>(false);
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

                //Creating UserSettings table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating user settings table."));
                    if (!this._db.TableExist("ec_usettings"))
                    {
                        this._db.CreateTable<UserSettings>(false);
                        //Auto install the settings for superadmin
                        this._db.Save(new UserSettings() { UserId = 0, CanCreateCalendar = true, CanCreateEvents = true, CanCreateLocations = true, CanDeleteCalendar = true, CanDeleteEvents = true, CanDeleteLocations = true });
                        this.BulletedList1.Items.Add(new ListItem("Successfully created table."));
                    }
                }
                catch (Exception ex) { this.BulletedList1.Items.Add(new ListItem(ex.Message)); }

                //Creating date exceptions table
                try
                {
                    this.BulletedList1.Items.Add(new ListItem("Creating date exceptions table."));
                    if (!this._db.TableExist("ec_dateexceptions"))
                    {
                        this._db.CreateTable<DateExceptionDto>(false);
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

        /// <summary>
        /// 
        /// </summary>
        public void AddSectionDashboard()
        {
            bool saveFile = false;

            //Open up language file
            var dashboardPath = "~/config/dashboard.config";

            //Path to the file resolved
            var dashboardFilePath = HostingEnvironment.MapPath(dashboardPath);

            //Load settings.config XML file
            XmlDocument dashboardXml = new XmlDocument();

            try
            {
                dashboardXml.Load(dashboardFilePath);

                // Section Node
                XmlNode findSection = dashboardXml.SelectSingleNode("//section [@alias='EventCalendarDashboardSection']");

                //Couldn't find it
                if (findSection == null)
                {
                    //Let's add the xml
                    var xmlToAdd = "<section alias='EventCalendarDashboardSection'>" +
                                        "<areas>" +
                                            "<area>eventCalendar</area>" +
                                        "</areas>" +
                                        "<tab caption='EventCalendar'>" +
                                            "<control addPanel='true' panelCaption=''>/App_Plugins/EventCalendar/backoffice/ecTree/dashboard.html</control>" +
                                        "</tab>" +
                                        "<tab caption='Import'>" +
                                            "<control addPanel='true' panelCaption=''>/App_Plugins/EventCalendar/backoffice/ecTree/importDashboard.html</control>" +
                                        "</tab>" +
                                   "</section>";

                    //Get the main root <dashboard> node
                    XmlNode dashboardNode = dashboardXml.SelectSingleNode("//dashBoard");

                    if (dashboardNode != null)
                    {
                        //Load in the XML string above
                        XmlDocument xmlNodeToAdd = new XmlDocument();
                        xmlNodeToAdd.LoadXml(xmlToAdd);

                        //Append the xml above to the dashboard node
                        try
                        {
                            var copiedNode = dashboardXml.ImportNode(xmlNodeToAdd.DocumentElement, true);
                            dashboardNode.AppendChild(copiedNode);
                            //Save the file flag to true
                            saveFile = true;
                        }
                        catch (Exception ex) { LogHelper.Error<installer>("Couldn't add dashboard section", ex); }
                    }
                }
                else
                {
                    //Check if the import dashboard is there
                    XmlNode importTab = findSection.SelectSingleNode("//tab [@caption='Import']");

                    if (importTab == null)
                    {
                        var xmlToAdd = "<tab caption='Import'>" +
                                            "<control addPanel='true' panelCaption=''>/App_Plugins/EventCalendar/backoffice/ecTree/importDashboard.html</control>" +
                                        "</tab>";

                        //Load in the XML string above
                        XmlDocument xmlNodeToAdd = new XmlDocument();
                        xmlNodeToAdd.LoadXml(xmlToAdd);

                        //Append the xml above to the dashboard node
                        try
                        {
                            var copiedNode = dashboardXml.ImportNode(xmlNodeToAdd.DocumentElement, true);
                            findSection.AppendChild(copiedNode);
                            //Save the file flag to true
                            saveFile = true;
                        }
                        catch (Exception ex) { LogHelper.Error<installer>("Couldn't add dashboard section", ex); }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<installer>("Couldn't add dashboard section", ex);
            }

            //If saveFile flag is true then save the file
            if (saveFile)
            {
                //Save the XML file
                dashboardXml.Save(dashboardFilePath);
            }
        }
    }
}