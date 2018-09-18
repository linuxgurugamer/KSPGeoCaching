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

    public enum Difficulty { Easy, Normal, Hard, Insane };

    public class GeoCacheCollectionData
    {
        public System.Guid collectionId;                   // GUID
        public string name;                 // should match the file name
        public string title;
        public string author;
        public string description;          // Description of the entire Geocache file
        public Difficulty difficulty;
        public List<string> requiredMods;   // Mods required as defined by the creator.  Will get mod names from game at creation time

        internal GeoCacheCollectionData()
        {
            collectionId = System.Guid.NewGuid();
            name = "";
            title = "";
            author = "";
            description = "";
            difficulty = Difficulty.Normal;
            requiredMods = new List<string>();
        }

        public override string ToString()
        {
            string s = "GeoCacheCollectionData: " + collectionId + "\n" + name + "\n" + author + "\n" + description + "\n";
            return s;
        }
    }

    public enum Scale { m, Km };
    public class Hint
    {
        public string hintTitle;
        public string hint;
        public double distance;
        public Scale scale;
        public double sort;// will be abs
        public bool spawn;

        internal Hint()
        {
            distance = 1;
            hint = "";
            hintTitle = "";
            scale = Scale.Km;
            spawn = false; // if true, spawn geocache vessel when within this distance
        }
        internal Hint Copy()
        {
            Hint newHint = new Hint();

            newHint.distance = distance;
            newHint.hint = hint;
            newHint.hintTitle = hintTitle;
            newHint.scale = scale;
            newHint.spawn = spawn;
            return newHint;
        }
        internal void Copy(Hint oldHint)
        {
            distance = oldHint.distance;
            hint = oldHint.hint;
            hintTitle = oldHint.hintTitle;
            scale = oldHint.scale;
            spawn = oldHint.spawn;
        }
    }

    public class GeoCacheData
    {
        public System.Guid geocachId = System.Guid.NewGuid();

        public string name;
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

        internal GeoCacheData()
        {
            geocachId = System.Guid.NewGuid();

            name = "";
            scienceNodeRequired = "";
            found = false;
            vessel = null;
            latitude = FlightGlobals.ActiveVessel.latitude;
            longitude = FlightGlobals.ActiveVessel.longitude;
            description = "";
            hints = new List<Hint>();
            //protoVessel =new ProtoVessel(new Vessel());
            nextGeocacheId = "";
        }
        public Vessel CacheVessel
        {
            get { return vessel; }
            set
            {
                vessel = value;
                body = vessel.mainBody;
                latitude = vessel.latitude;
                longitude = vessel.longitude;
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
