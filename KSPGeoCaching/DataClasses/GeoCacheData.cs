using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
    public class GeoCacheData
    {
        public System.Guid geocacheId;

        public string geoCacheName;
        public string scienceNodeRequired; // optional, if set, then will require this science node to be unlocked for this to be found
        public bool found;

        // location

        public CelestialBody body;
        public double latitude;
        public double longitude;

        public string description;
        public List<Hint> hints;
        public ConfigNode protoVessel;
        private Vessel vessel;
        public string nextGeocacheId;

        public int lastHintFound;

        internal GeoCacheData()
        {
            geocacheId = System.Guid.NewGuid();

            geoCacheName = "";
            scienceNodeRequired = "";
            found = false;
            vessel = null;
            if (FlightGlobals.ActiveVessel != null)
            {
                latitude = FlightGlobals.ActiveVessel.latitude;
                longitude = FlightGlobals.ActiveVessel.longitude;
            }
            description = "";
            hints = new List<Hint>();
            //protoVessel =new ProtoVessel(new Vessel());
            nextGeocacheId = "";
            lastHintFound = -1;
        }

        public Vessel CacheVessel
        {
            get { return vessel; }
            set
            {
                vessel = value;
                if (vessel != null)
                {
                    body = vessel.mainBody;
                    latitude = vessel.latitude;
                    longitude = vessel.longitude;
                }
            }
        }

        public void DeleteVessel()
        {
            CacheVessel.GoOnRails();

            foreach (Part p in CacheVessel.Parts)
            {
                if (p != null && p.gameObject != null)
                    p.gameObject.SetActive(false);
                else
                    continue;
            }

            CacheVessel.MakeInactive();
            CacheVessel.enabled = false;
            CacheVessel.Unload();
            CacheVessel = null;
        }
    }
}
