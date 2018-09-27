using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KSPGeoCaching
{
    public class GeoCacheModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public System.Guid geocacheId;

        [KSPField(isPersistant = true, guiActive = false)]
        public System.Guid collectionId;

        public enum AssignStatus { unassigned, inprogress, assigned};

        [KSPField(isPersistant = true, guiActive = false)]
        public AssignStatus assigned = AssignStatus.unassigned;

#if false
        [KSPEvent(name = "LogPosition", guiActive = true, guiActiveEditor = false, guiName = "GeoCache Log Position", guiActiveUncommand = true, requireFullControl =false)]
        public void LogPosition()
        {
            Log.Info("GeoCache Logged Latitude: " + vessel.latitude + ",   Longitude: " + vessel.longitude);
        }

        [KSPEvent(name = "PrintUid", guiActive = true, guiActiveEditor = false, guiName = "PrintUid", guiActiveUncommand = true, requireFullControl = false)]
        public void PrintUid()
        {
            Log.Info("GeoCacheModule.Add");
            Log.Info("geocachId: " + geocachId);
            Log.Info("collectionId: " + collectionId);
        }
#endif

        [KSPEvent(name = "Edit", guiActive = true, guiActiveEditor = false, guiName = "Edit/Display Info", guiActiveUncommand = true, requireFullControl = false)]
        public void Edit()
        {
            Log.Info("GeoCacheModule.Edit");

        }
        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Add to current collection", guiActiveUnfocused = true, unfocusedRange = 20, externalToEVAOnly﻿ = false)]
        public void AddToCollection()
        {
            Log.Info("GeoCacheModule.AddToCollection");
            Events["AddToCollection"].guiActive = false;
            Events["RemoveFromCollection"].guiActive = true;

            GeoCacheData data = new GeoCacheData();
            data.geoCacheName = this.vessel.name;
            int idxLeftParen = data.geoCacheName.LastIndexOf('(');
            if (idxLeftParen > 0)
            {
                int idxRightParen = data.geoCacheName.LastIndexOf(')');
                if (idxRightParen > idxLeftParen)
                {
                    data.geoCacheName = data.geoCacheName.Substring(idxLeftParen + 1, idxRightParen - (idxLeftParen+1));
                }
            }
            data.geoCacheName = "Location of " + data.geoCacheName;
            data.CacheVessel = this.vessel;
            data.latitude = this.vessel.latitude;
            data.longitude = this.vessel.longitude;
            assigned = AssignStatus.assigned;
            this.geocacheId = data.geocacheId;

            collectionId = GeoCacheDriver.activeGeoCacheCollection.geocacheCollectionData.collectionId;

            GeoCacheDriver.activeGeoCacheCollection.geocacheData.Add(data);
            UpdateEvents();
            
        }

        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Remove from current collection", guiActiveUnfocused = true, unfocusedRange = 20, externalToEVAOnly﻿ = false)]
        public void RemoveFromCollection()
        {
            Log.Info("GeoCacheModule.RemoveFromCollection");
            Events["AddToCollection"].guiActive = true;
            Events["RemoveFromCollection"].guiActive = false;
            for (int i = 0; i < GeoCacheDriver.activeGeoCacheCollection.geocacheData.Count(); i++)
            {
                if (GeoCacheDriver.activeGeoCacheCollection.geocacheData[i].geocacheId == this.geocacheId)
                {
                    GeoCacheDriver.activeGeoCacheCollection.geocacheData.Remove(GeoCacheDriver.activeGeoCacheCollection.geocacheData[i]);
                    geocacheId = new Guid();
                    assigned = AssignStatus.unassigned;
                    collectionId = new Guid();
                    UpdateEvents();
                    if (GeoCacheDriver.Instance != null && GeoCacheDriver.Instance.visibleGeoCache)
                    {
                        GeoCacheDriver.Instance.visibleGeoCache = false;
                        GeoCacheDriver.Instance.visibleEditCollection = true;
                    }
                    return;
                }
            }

        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            GameEvents.onVesselGoOffRails.Add(onVesselGoOffRails);
            UpdateEvents();
        }
        public override void OnAwake()
        {
            base.OnAwake();
            UpdateEvents();
        }


        public void Start()
        {
            Log.Info("GeoCacheModule.Start");

        }


        public void OnDestroy()
        {
            GameEvents.onVesselGoOffRails.Remove(onVesselGoOffRails);
        }
        public void UpdateData(Guid gid, AssignStatus asgn, Guid cid)
        {
            geocacheId = gid;
            assigned = asgn;
            collectionId = cid;
            UpdateEvents();
        }
        void UpdateEvents()
        {
            Log.Info("UpdateEvents, assigned: " + assigned);
            Log.Info("geocacheId: " + geocacheId);
            Log.Info("collectionId: " + collectionId);
            Log.Info("part.craftID: "+ this.part.craftID);
            if (assigned == AssignStatus.assigned)
            {
                Events["AddToCollection"].guiActive = false;
                Events["AddToCollection"].guiActiveUnfocused = false;
                // need to see if this is in an active collection, if so, set to false
                if (GeoScenario.IdInCollection(collectionId))
                {
                    Log.Info("In collection");
                    Events["RemoveFromCollection"].guiActive = false;
                    Events["RemoveFromCollection"].guiActiveUnfocused = false;

                }
                else
                {
                    Log.Info("In collection");
                    Events["RemoveFromCollection"].guiActive = true;
                    Events["RemoveFromCollection"].guiActiveUnfocused = true;
                }
            }
            else
            {
                if (assigned == AssignStatus.unassigned)
                {
                    Events["AddToCollection"].guiActive = true;
                    Events["AddToCollection"].guiActiveUnfocused = true;
                    Events["RemoveFromCollection"].guiActive = false;
                    Events["RemoveFromCollection"].guiActiveUnfocused = false;
                }
                else
                {
                    Events["AddToCollection"].guiActive = false;
                    Events["AddToCollection"].guiActiveUnfocused = false;
                    Events["RemoveFromCollection"].guiActive = false;
                    Events["RemoveFromCollection"].guiActiveUnfocused = false;
                }
            }
        }
        void onVesselGoOffRails(Vessel p)
        {
            if (p == this.vessel)
            {
                UpdateEvents();
               
            }
        }
#if false
        public void FixedUpdate()
        {
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
                return;            
        }
#endif

    }
}
