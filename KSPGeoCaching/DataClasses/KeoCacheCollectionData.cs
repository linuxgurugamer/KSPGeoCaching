using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeoCaching
{
    public enum Difficulty { Easy, Normal, Hard, Insane };

    public class KeoCacheCollectionData
    {
        public string collectionId;                   // GUID
        public string name;                 // should match the file name
        public string title;
        public string author;
        public string description;          // Description of the entire Keocache file
        public string initialHint;
        public Difficulty difficulty;
        public List<string> requiredMods;   // Mods required as defined by the creator.  Will get mod names from game at creation time

        public int lastCacheFound;

        public string assignedTravelBug;

        internal KeoCacheCollectionData()
        {
            collectionId = System.Guid.NewGuid().ToString("N");
            assignedTravelBug = "";
            name = "";
            title = "";
            author = "";
            description = "";
            initialHint = "";
            difficulty = Difficulty.Normal;
            requiredMods = new List<string>();

            lastCacheFound = -1;
        }

        public override string ToString()
        {
            string s = "KeoCacheCollectionData: " + collectionId + "\n" + name + "\n" + author + "\n" + description + "\n";
            return s;
        }
    }
}
