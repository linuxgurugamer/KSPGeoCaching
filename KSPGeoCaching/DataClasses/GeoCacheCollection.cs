using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
    // All Id's will be GUIDs for uniqueness

    public class GeoCacheCollection
    {
        public GeoCacheCollectionData geocacheCollectionData;
        public List<GeoCacheData> geocacheData;

        internal GeoCacheCollection()
        {
            geocacheCollectionData = new GeoCacheCollectionData();
            geocacheData = new List<GeoCacheData>();
        }
    }

 
}
