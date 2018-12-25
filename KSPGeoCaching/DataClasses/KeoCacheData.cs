using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeoCaching
{
    public class KeoCacheData
    {
        public string keocacheId;

        public string keoCacheName;
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
        public bool spawned;

        double dfc;
        public double distanceFromCache
        {
            get { return dfc; }
            set { dfc = value; SetAbsoluteDistance(); }
        }
        Scale sc;
        public KeoCaching.Scale scale {
            get { return sc; }
            set { sc = value; SetAbsoluteDistance(); }
        }

        internal double absoluteDistance;
        void SetAbsoluteDistance()
        {
            absoluteDistance = distanceFromCache;
            if (scale == Scale.km)
                absoluteDistance *= 1000;
        }



        public int lastHintFound;

        public KeoCacheData()
        {
            keocacheId = System.Guid.NewGuid().ToString("N");
            keoCacheName = "";
            scienceNodeRequired = "";
            found = false;
            vessel = null;
            spawned = false;
            distanceFromCache = 1f;
            absoluteDistance = 1000f;
            scale = Scale.km;
            if (FlightGlobals.ActiveVessel != null)
            {
                latitude = FlightGlobals.ActiveVessel.latitude;
                longitude = FlightGlobals.ActiveVessel.longitude;
            }
            description = "";
            hints = new List<Hint>();
            //protoVessel =new ProtoVessel(new Vessel());
            //nextKeocacheId = "";
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
