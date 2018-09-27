using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
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

        public int lastCacheFound;

        internal GeoCacheCollectionData()
        {
            collectionId = System.Guid.NewGuid();
            name = "";
            title = "";
            author = "";
            description = "";
            difficulty = Difficulty.Normal;
            requiredMods = new List<string>();

            lastCacheFound = -1;
        }

        public override string ToString()
        {
            string s = "GeoCacheCollectionData: " + collectionId + "\n" + name + "\n" + author + "\n" + description + "\n";
            return s;
        }
    }
}
