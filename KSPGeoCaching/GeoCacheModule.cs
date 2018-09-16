using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSPGeoCaching
{
    public class GeoCacheModule : PartModule
    {
        [KSPField]
        public System.Guid geocachId = System.Guid.NewGuid();

        [KSPField]
        public System.Guid collectionId;

#if DEBUG
        [KSPEvent(name = "LogPosition", guiActive = true, guiActiveEditor = false, guiName = "GeoCache Log Position", guiActiveUncommand = true, requireFullControl =false)]
        public void LogPosition()
        {
            Log.Info("GeoCache Logged Latitude: " + vessel.latitude + ",   Longitude: " + vessel.longitude);
        }
#endif

        [KSPEvent(name = "Edit", guiActive = true, guiActiveEditor = false, guiName = "Edit/Display Info", guiActiveUncommand = true, requireFullControl = false)]
        public void Edit()
        {
            Log.Info("GeoCacheModule.Edit");
            
        }
        [KSPEvent(name = "Add", guiActive = true, guiActiveEditor = false, guiName = "Add to current collection", guiActiveUncommand = true, requireFullControl = false)]
        public void Add()
        {
            Log.Info("GeoCacheModule.Add");

        }

        [KSPEvent(name = "PrintUid", guiActive = true, guiActiveEditor = false, guiName = "PrintUid", guiActiveUncommand = true, requireFullControl = false)]
        public void PrintUid()
        {
            Log.Info("GeoCacheModule.Add");
            Log.Info("geocachId: " + geocachId);
            Log.Info("collectionId: " + collectionId);
        }

#if false
        public void Start()
        {

        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
                return;
        }
#endif

    }
}
