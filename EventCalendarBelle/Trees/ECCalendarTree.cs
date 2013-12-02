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

namespace EventCalendarBelle.Trees
{
    [Tree("eventCalendar","ecTree","Calendar")]
    [PluginController("EventCalendar")]
    public class ECCalendarTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {

            var menu = new MenuItemCollection();
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add(new MenuItem("createCalendar", "Create Calendar"));
                menu.Items.Add(new MenuItem("createLocation", "Create Location"));
            }
            else if (id == "calendarTree")
            {
                menu.Items.Add(new MenuItem("createCalendar", "Create Calendar"));
            }
            else if (id == "locationTree")
            {
                menu.Items.Add(new MenuItem("createLocation", "Create Location"));
            }
            else if (id.Contains("c-"))
            {
                menu.Items.Add(new MenuItem("createEvent", "Create Event"));
                menu.Items.Add(new MenuItem("createREvent", "Create recurring Event"));
                menu.Items.Add(new MenuItem("deleteCalendar", "Delete Calendar"));
            }
            else if (id.Contains("l-"))
            {
                menu.Items.Add(new MenuItem("deleteLocation", "Delete Location"));
            }
            else if (id.Contains("e-"))
            {
                menu.Items.Add(new MenuItem("deleteEvent", "Delete Event"));
            }
            else if (id.Contains("re-"))
            {
                menu.Items.Add(new MenuItem("deleteREvent", "Delete Event"));
            }
            
            return menu;
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            //check if we're rendering the root node's children
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                var tree = new TreeNodeCollection();
                tree.Add(CreateTreeNode("calendarTree", id, queryStrings, "Calendar", "icon-calendar-alt", true));
                tree.Add(CreateTreeNode("locationTree", id, queryStrings, "Locations", "icon-globe-alt", true));
                return tree;
            }

            if (id == "calendarTree")
            {
                var ctrl = new CalendarApiController();
                List<ECalendar> calendar = ctrl.GetAll().ToList();
                var tree = new TreeNodeCollection();

                foreach (var cal in calendar)
                {
                    tree.Add(CreateTreeNode("c-" + cal.Id.ToString(), id, queryStrings, cal.Calendarname, "icon-calendar", true, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editCalendar/" + cal.Id));
                }
                return tree;
            }

            if (id == "locationTree")
            {
                var ctrl = new LocationApiController();
                List<EventLocation> locations = ctrl.GetAll().ToList();
                var tree = new TreeNodeCollection();

                foreach (var loc in locations)
                {
                    tree.Add(CreateTreeNode("l-" + loc.Id.ToString(), id, queryStrings, loc.LocationName, "icon-globe", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editLocation/" + loc.Id.ToString()));
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
                    tree.Add(CreateTreeNode("re-" + e.Id.ToString(), id, queryStrings, e.title, "icon-music", false, FormDataCollectionExtensions.GetValue<string>(queryStrings, "application") + StringExtensions.EnsureStartsWith(this.TreeAlias, '/') + "/editREvent/" + e.Id));
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
