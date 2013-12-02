using EventCalendarBelle.Controller;
using EventCalendarBelle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace EventCalendarBelle.Trees
{
    [Tree("eventCalendar", "ecLocationTree", "Locations","icon-globe","icon-globe", true,1)]
    [PluginController("EventCalendar")]
    public class ECLocationTreeController : TreeController
    {
        protected override Umbraco.Web.Models.Trees.MenuItemCollection GetMenuForNode(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                //menu.Items.Add(new MenuItem("createCalendar", "Create Calendar"));
                menu.Items.Add(new MenuItem("createLocation", "Create Location"));
                //menu.Items.Add(new MenuItem("createEvent", "Create Event"));
                return menu;
            }
            else
            {
                menu.Items.Add(new MenuItem("delete", "Delete Location"));
                return menu;
            }
        }

        protected override Umbraco.Web.Models.Trees.TreeNodeCollection GetTreeNodes(string id, System.Net.Http.Formatting.FormDataCollection queryStrings)
        {
            //check if we're rendering the root node's children
            if (id == global::Umbraco.Core.Constants.System.Root.ToInvariantString())
            {
                var ctrl = new LocationApiController();
                List<EventLocation> locations = ctrl.GetAll().ToList();
                var tree = new TreeNodeCollection();

                foreach (var loc in locations)
                {
                    tree.Add(CreateTreeNode(loc.Id.ToString(), id, queryStrings, loc.LocationName, "icon-globe"));
                }
                return tree;
            }

            throw new NotImplementedException();
        }
    }
}
