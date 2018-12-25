using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace KeoCaching
{
    public class KeoCacheModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public string keocacheId;

        [KSPField(isPersistant = true, guiActive = false)]
        public string collectionId;

        public enum AssignStatus { unassigned, inprogress, assigned };

        [KSPField(isPersistant = true, guiActive = false)]
        public AssignStatus assigned = AssignStatus.unassigned;

        private ModuleAnimateGeneric _deployAnimation;

        public enum VaultStatus { idle, opening, closing };
        VaultStatus vaultStatus = VaultStatus.idle;

        [KSPEvent(name = "Open Vault", guiActive = true, guiActiveUnfocused = true, guiActiveEditor = false, guiName = "Open KeoVault", requireFullControl = false, externalToEVAOnly = true, unfocusedRange = 5)]
        public void OpenVault()
        {
            Log.Info("KeoCacheModule.OpenVault");
            Events["OpenVault"].active = false;
            Events["OpenVault"].guiActiveUnfocused = false;

            vaultStatus = VaultStatus.opening;
            if (_deployAnimation != null)
            {
                _deployAnimation.Toggle();
            }
        }
        [KSPEvent(name = "Close Vault", guiActive = false, guiActiveUnfocused = false, guiActiveEditor = false, guiName = "Close KeoVault", requireFullControl = false, externalToEVAOnly = true, unfocusedRange = 5)]
        public void CloseVault()
        {
            Log.Info("KeoCacheModule.CloseVault");

            Events["CloseVault"].active = false;
            Events["CloseVault"].guiActiveUnfocused = false;
            vaultStatus = VaultStatus.closing;
            if (_deployAnimation != null)
            {
                _deployAnimation.Toggle();
            }
        }
        private ModuleAnimateGeneric GetDeployAnimation()
        {
            Log.Info("GetDeployAnimation");
            ModuleAnimateGeneric myAnimation = null;

            try
            {
                myAnimation = part.FindModulesImplementing<ModuleAnimateGeneric>().SingleOrDefault();
            }
            catch (System.Exception x)
            {
                Log.Error("ERROR: " +  x.Message);
            }

            if (!myAnimation)
            {
                // this shouldn't happen under normal circumstances
                Log.Error("ERROR: Didn't find ModuleAnimateGeneric on Vault!");
            }

            return myAnimation;
        }



#if false
        [KSPEvent(name = "LogPosition", guiActive = true, guiActiveEditor = false, guiName = "KeoCache Log Position", guiActiveUncommand = true, requireFullControl =false)]
        public void LogPosition()
        {
            Log.Info("KeoCache Logged Latitude: " + vessel.latitude + ",   Longitude: " + vessel.longitude);
        }

        [KSPEvent(name = "PrintUid", guiActive = true, guiActiveEditor = false, guiName = "PrintUid", guiActiveUncommand = true, requireFullControl = false)]
        public void PrintUid()
        {
            Log.Info("KeoCacheModule.Add");
            Log.Info("keocachId: " + keocachId);
            Log.Info("collectionId: " + collectionId);
        }
#endif

        [KSPEvent(name = "Edit", guiActive = true, guiActiveEditor = false, guiName = "Edit/Display Vault Info", guiActiveUncommand = true, requireFullControl = false)]
        public void Edit()
        {
            Log.Info("KeoCacheModule.Edit");

        }
        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Add to current KeoCache", guiActiveUnfocused = true, unfocusedRange = 20, externalToEVAOnly﻿ = false)]
        public void AddToCollection()
        {
            Log.Info("KeoCacheModule.AddToCollection");
            Events["AddToCollection"].guiActive = false;
            Events["RemoveFromCollection"].guiActive = true;

            KeoCacheData data = new KeoCacheData();
            data.keoCacheName = this.vessel.name;
            int idxLeftParen = data.keoCacheName.LastIndexOf('(');
            if (idxLeftParen > 0)
            {
                int idxRightParen = data.keoCacheName.LastIndexOf(')');
                if (idxRightParen > idxLeftParen)
                {
                    data.keoCacheName = data.keoCacheName.Substring(idxLeftParen + 1, idxRightParen - (idxLeftParen + 1));
                }
            }
            data.keoCacheName = "Location of " + data.keoCacheName;
            data.CacheVessel = this.vessel;
            data.latitude = this.vessel.latitude;
            data.longitude = this.vessel.longitude;
            assigned = AssignStatus.assigned;
            this.keocacheId = data.keocacheId;

            collectionId = KeoCacheDriver.activeKeoCacheCollection.keocacheCollectionData.collectionId;

            KeoCacheDriver.activeKeoCacheCollection.keoCacheDataList.Add(data);
            UpdateEvents();

        }

        [KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Remove from current collection", guiActiveUnfocused = true, unfocusedRange = 20, externalToEVAOnly﻿ = false)]
        public void RemoveFromCollection()
        {
            Log.Info("KeoCacheModule.RemoveFromCollection");
            Events["AddToCollection"].guiActive = true;
            Events["RemoveFromCollection"].guiActive = false;
            if (KeoCacheDriver.activeKeoCacheCollection != null)
            {
                for (int i = 0; i < KeoCacheDriver.activeKeoCacheCollection.keoCacheDataList.Count(); i++)
                {
                    if (KeoCacheDriver.activeKeoCacheCollection.keoCacheDataList[i].keocacheId == this.keocacheId)
                    {
                        KeoCacheDriver.activeKeoCacheCollection.keoCacheDataList.Remove(KeoCacheDriver.activeKeoCacheCollection.keoCacheDataList[i]);
                    }
                }
            }
            keocacheId = System.Guid.NewGuid().ToString("N");
            assigned = AssignStatus.unassigned;
            collectionId = "";
            UpdateEvents();
            if (KeoCacheDriver.Instance != null && KeoCacheDriver.Instance.visibleKeoCache)
            {
                KeoCacheDriver.Instance.visibleKeoCache = false;
                KeoCacheDriver.Instance.visibleEditCollection = true;
            }
            return;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            GameEvents.onVesselGoOffRails.Add(onVesselGoOffRails);
            GameEvents.onVesselChange.Add(VesselChange);
            UpdateEvents();

            // set up the variables used by code for opening the vault
            _deployAnimation = GetDeployAnimation();
            if (_deployAnimation != null)
            {
                _deployAnimation.eventAvailableEVA = false;
                _deployAnimation.eventAvailableFlight = false;
                _deployAnimation.eventAvailableEditor = false;
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();
            UpdateEvents();
        }

        public new void Awake()
        {
            base.Awake();
            Log.Info("KeoCacheModule.Awake");
        }
        public void Start()
        {
            Log.Info("KeoCacheModule.Start");
        }

        void VesselChange(Vessel v)
        {
            UpdateEvents();
        }

        bool editInProgressFlag = false;
        void FixedUpdate()
        {
            if (KeoCacheDriver.editInProgress != editInProgressFlag)
                UpdateEvents();


            if (HighLogic.LoadedSceneIsFlight)
            {
                switch (vaultStatus)
                {
                    case VaultStatus.opening:
                        if (_deployAnimation.animTime == 0f)
                        {
                            Events["CloseVault"].active = true;
                            Events["CloseVault"].guiActiveUnfocused = true;
                            vaultStatus = VaultStatus.idle;
                        }
                        break;
                    case VaultStatus.closing:
                        if (_deployAnimation.animTime == 1f)
                        {
                            Events["OpenVault"].active = true;
                            Events["OpenVault"].guiActiveUnfocused = true;
                            vaultStatus = VaultStatus.idle;
                        }
                        break;
                }
            }
        }

        public void OnDestroy()
        {
            GameEvents.onVesselGoOffRails.Remove(onVesselGoOffRails);
            GameEvents.onVesselChange.Remove(VesselChange);
        }

        public void UpdateData(string gid, AssignStatus asgn, string cid)
        {
            keocacheId = gid;
            assigned = asgn;
            collectionId = cid;
            UpdateEvents();
        }
        

        void UpdateEvents()
        {
            Log.Info("UpdateEvents, assigned: " + assigned);
            Log.Info("keocacheId: " + keocacheId);
            Log.Info("collectionId: " + collectionId);
            Log.Info("part.craftID: "+ this.part.craftID);
            editInProgressFlag = KeoCacheDriver.editInProgress;

            if (!KeoCacheDriver.editInProgress ||(HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel == this.vessel))
            {
                Events["AddToCollection"].guiActive = false;
                Events["AddToCollection"].guiActiveUnfocused = false;
                Events["RemoveFromCollection"].guiActive = false;
                Events["RemoveFromCollection"].guiActiveUnfocused = false;


            }
            else
            {
                if (assigned == AssignStatus.assigned)
                {
                    Events["AddToCollection"].guiActive = false;
                    Events["AddToCollection"].guiActiveUnfocused = false;
                    // need to see if this is in an active collection, if so, set to false
                    if (KeoScenario.IdInCollection(collectionId))
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

        public override string GetModuleDisplayName()
        {
            return "KeoCache";
        }

        public override string GetInfo()
        {
            return "KeoCache part";
        }

    }
}
