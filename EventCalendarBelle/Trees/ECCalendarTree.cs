using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using EventCalendarBelle.Controller;
using EventCalendarBelle.Models;
using Umbraco.Web;
using umbraco.BusinessLogic.Actions;

namespace EventCalendarBelle.Trees
{
    [Tree("eventCalendar","ecTree","Calendar")]
    [PluginController("EventCalendar")]
    public class ECCalendarTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            var currentUser = Security.CurrentUser;
            var ctrl = new UserApiController();

            var settings = ctrl.GetById(currentUser.Id);
            
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                if (settings.CanCreateCalendar)
                {
                    menu.Items.Add(new MenuItem("createCalendar", "Create Calendar") { Icon = "add" });
                }
                if (settings.CanCreateLocations)
                {
                    menu.Items.Add(new MenuItem("createLocation", "Create Location") { Icon = "add" });
                }
            }
            else if (id == "calendarTree")
            {
                if (settings.CanCreateCalendar)
                {
                    menu.DefaultMenuAlias = "createCalendar";
                    menu.Items.Add(new MenuItem("createCalendar", "Create Calendar") { Icon = "add" });
                }
                menu.Items.Add<ActionRefresh>("Refresh");
            }
            else if (id == "locationTree")
            {
                if (settings.CanCreateLocations)
                {
                    menu.DefaultMenuAlias = "createLocation";
                    menu.Items.Add(new MenuItem("createLocation", "Create Location") { Icon = "add" });
                }
                menu.Items.Add<ActionRefresh>("Refresh");
            }
            else if (id.Contains("c-"))
            {
                if (settings.CanCreateEvents)
                {
                    menu.DefaultMenuAlias = "createEvents";
                    menu.Items.Add(new MenuItem("createEvents", "Create") { Icon = "add" });
                }
                if (settings.CanDeleteCalendar)
                {
                    menu.Items.Add(new MenuItem("deleteCalendar", "Delete Calendar") { Icon = "delete" });
                }
                menu.Items.Add<ActionRefresh>("Refresh");
            }
            else if (id.Contains("l-"))
            {
                if (settings.CanDeleteLocations)
                {
                    menu.Items.Add(new MenuItem("deleteLocation", "Delete Location") { Icon = "delete" });
                }
            }
            else if (id.Contains("re-"))
            {
                if (settings.CanDeleteEvents)
                {
                    menu.Items.Add(new MenuItem("deleteREvent", "Delete Recurring Event") { Icon = "delete" });
                }
            }
            else if (id.Contains("e-"))
            {
                if (settings.CanDeleteEvents)
                {
                    menu.Items.Add(new MenuItem("deleteEvent", "Delete Event") { Icon = "delete" });
                }
            }
            
            
            return menu;
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            //check if we're rendering the root node's children
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                var tree = new TreeNodeCollection();
                tree.Add(CreateTreeNode("calendarTree", id, queryStrings, "Calendar", "icon-calendar-alt", true, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/overviewCalendar/all"));
                tree.Add(CreateTreeNode("locationTree", id, queryStrings, "Locations", "icon-globe-alt", true, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/overviewLocation/all"));
                if (Security.CurrentUser.UserType.Alias.ToLower() == "admin")
                {
                    tree.Add(CreateTreeNode("security", id, queryStrings, "Security", "icon-combination-lock", true));
                }
                return tree;
            }

            if (id == "calendarTree")
            {
                var ctrl = new CalendarApiController();
                var uctrl = new UserApiController();
                var tree = new TreeNodeCollection();

                List<ECalendar> calendar = ctrl.GetAll().ToList();
                var user_settings = uctrl.GetById(Security.GetUserId());

                foreach (var cal in calendar)
                {
                    if (Security.CurrentUser.UserType.Alias.ToLower() == "admin" || user_settings.AllowedCalendar.Contains(cal.Id.ToString()))
                    {
                        tree.Add(CreateTreeNode("c-" + cal.Id.ToString(), id, queryStrings, cal.Calendarname, "icon-calendar", true, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editCalendar/" + cal.Id));
                    }
                }
                return tree;
            }

            if (id == "locationTree")
            {
                var ctrl = new LocationApiController();
                var uctrl = new UserApiController();
                var tree = new TreeNodeCollection();

                List<EventLocation> locations = ctrl.GetAll().ToList();
                var user_settings = uctrl.GetById(Security.GetUserId());

                foreach (var loc in locations)
                {
                    if (Security.CurrentUser.UserType.Alias.ToLower() == "admin" || user_settings.AllowedLocations.Contains(loc.Id.ToString()))
                    {
                        tree.Add(CreateTreeNode("l-" + loc.Id.ToString(), id, queryStrings, loc.LocationName, "icon-map-loaction", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editLocation/" + loc.Id.ToString()));
                    }
                }
                return tree;
            }

            if (id == "security")
            {
                var us = Services.UserService;
                var tree = new TreeNodeCollection();
                var total = 0;
                var users = us.GetAll(0, 1000, out total);
                foreach (var user in users.Where(x => x.AllowedSections.Contains("eventCalendar")))
                {
                    //Only the superadmin should change the settings for superadmin
                    if ((Security.CurrentUser.Id != 0 && user.Id != 0) || Security.CurrentUser.Id == 0)
                    {
                        tree.Add(CreateTreeNode("u-" + user.Id.ToString(), id, queryStrings, user.Name, "", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editUser/" + user.Id.ToString()));
                    }
                }
                return tree;
            }

            if (id.Contains("c-"))
            {
                var ctrl = new EventApiController();
                List<Event> events = ctrl.GetAll().Where(x => x.calendarId.ToString() == id.Replace("c-","")).ToList();
                var tree = new TreeNodeCollection();

                foreach(var e in events) {
                    tree.Add(CreateTreeNode("e-" + e.Id.ToString(), id, queryStrings, e.title, "icon-music", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editEvent/" + e.Id));
                }

                var ctrl2 = new REventApiController();
                List<RecurringEvent> revents = ctrl2.GetAll().Where(x => x.calendarId.ToString() == id.Replace("c-", "")).ToList();

                foreach (var e in revents)
                {
                    tree.Add(CreateTreeNode("re-" + e.Id.ToString(), id, queryStrings, e.title, "icon-axis-rotation", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editREvent/" + e.Id));
                }

                return tree;
            }

            //this tree doesn't suport rendering more than 1 level
            throw new NotSupportedException();
        }

        public override string RootNodeDisplayName
        {
            get
            {
                return "EventCalendar";
            }
        }

        public override string TreeAlias
        {
            get
            {
                return "ecTree";
            }
        }
    }
}
