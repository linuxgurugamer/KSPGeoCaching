using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeoCaching
{
    public class TravelBugEntry
    {
        public string keocachId;
        public string activeOnVesselName;
        public Guid? activeOnVesselId; 

        public TravelBugEntry(string id, string name, Guid? vid)
        {
            keocachId = id;
            activeOnVesselName = name;
            if (vid != null)
                activeOnVesselId = (Guid)vid;
        }
    }

#if false
    public class TravelBug
    {
        public string travelbugId;

        public string activeCollectionId;
        //public List<TravelBugEntry> entries;

        public TravelBug()
        {
          //  entries = new List<TravelBugEntry>();
        }
    }
#endif
}
