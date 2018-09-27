using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{

    public class TravelBugEntry
    {
        public System.Guid geocachId;
        public System.Guid hintId;

        public double timeFound;
    }
    public class TravelBug
    {
        public System.Guid travelbugId;
        public string name;

        public System.Guid activeCollectionId;
        public List<TravelBugEntry> entries;

        public TravelBug()
        {
            entries = new List<TravelBugEntry>();
        }
    }
}
