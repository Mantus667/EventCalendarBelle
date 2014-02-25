using EventCalendarBelle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventCalendarBelle.Mapper
{
    public class EventDescriptionMapper
    {
        public Event current;
        public Event MapIt(Event a, EventDescription p)
        {
            // Terminating call.  Since we can return null from this function
            // we need to be ready for PetaPoco to callback later with null
            // parameters
            if (a == null)
                return current;

            // Is this the same author as the current one we're processing
            if (current != null && current.Id == a.Id)
            {
                // Yes, just add this post to the current author's collection of posts
                current.descriptions.Add(p);

                // Return null to indicate we're not done with this author yet
                return null;
            }

            // This is a different author to the current one, or this is the 
            // first time through and we don't have an author yet

            // Save the current author
            var prev = current;

            // Setup the new current author
            current = a;
            current.descriptions = new List<EventDescription>();
            current.descriptions.Add(p);

            // Return the now populated previous author (or null if first time through)
            return prev;
        }
    }
}
