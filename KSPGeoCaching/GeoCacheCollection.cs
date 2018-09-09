﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
    // All Id's will be GUIDs for uniqueness

    public class GeoCache
    {
        public GeoCacheCollectionData fileData;
        public List<GeoCacheData> geocacheData;

        internal GeoCache()
        {
            fileData = new GeoCacheCollectionData();
            geocacheData = new List<GeoCacheData>();
        }
    }

    public enum Difficulty { easy, normal, hard, insane };
    
    public class GeoCacheCollectionData
    {
        public string id;                   // GUID
        public string name;                 // should match the file name
        public string title;
        public string author;
        public string description;          // Description of the entire Geocache file
        public Difficulty difficulty;
        public List<string> requiredMods;   // Mods required as defined by the creator.  Will get mod names from game at creation time

        internal GeoCacheCollectionData()
        {
            id = "";
            name = "";
            title = "";
            author = "";
            description = "";
            difficulty = Difficulty.normal;
            requiredMods = new List<string>();
        }
    }

    public class Hints
    {
        public double distance;
        public string hint;
        public bool spawn;

        internal Hints()
        {
            distance = 0;
            hint = "";
            spawn = false; // if true, spawn geocache vessel when within this distance
        }
    }

    public class GeoCacheData
    {
        public string id;                   // GUID
        public string name;
        public string scienceNodeRequired; // optional, if set, then will require this science node to be unlocked for this to be found
        public bool found;

        // location

        public CelestialBody body;
        public double latitude;
        public double longitude;

        public string description;
        public List<Hints> hints;
        public string nextGeocacheId;

        internal GeoCacheData()
        {
            id = "";
            name = "";
            scienceNodeRequired = "";
            found = false;
            body = FlightGlobals.GetHomeBody();
            latitude = 0;
            longitude = 0;
            description = "";
            hints = new List<Hints>();
            nextGeocacheId = "";
        }
    }
}
