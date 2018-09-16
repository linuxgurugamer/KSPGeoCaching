#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;
using System.Collections;

namespace KSPGeoCaching
{
    public static class LocationUtil
    {
        public static double TerrainHeight(double latitude, double longitude, CelestialBody body)
        {
            // Sun and Jool - bodies without terrain
            if (body.pqsController == null)
            {
                return 0;
            }

            // Figure out the terrain height
            double latRads = Math.PI / 180.0 * latitude;
            double lonRads = Math.PI / 180.0 * longitude;
            Vector3d radialVector = new Vector3d(Math.Cos(latRads) * Math.Cos(lonRads), Math.Sin(latRads), Math.Cos(latRads) * Math.Sin(lonRads));
            return Math.Max(body.pqsController.GetSurfaceHeight(radialVector) - body.pqsController.radius, 0.0);
        }
    }

    partial class GeoCacheDriver
    {
        const string VesselName = "GeoCache.craft";
#if false
        internal class KD
        {
            internal double altitude;
            public double latitude = 0.0;
            public double longitude = 0.0;
            public CelestialBody body = null;
            public Orbit orbit = null;
            public bool landed = false;
            //public float heading;
            public AvailablePart craftPart;

            public KD()
            {
                latitude = FlightGlobals.ActiveVessel.latitude;
                longitude = FlightGlobals.ActiveVessel.longitude;
                body = FlightGlobals.ActiveVessel.mainBody;
                orbit = FlightGlobals.ActiveVessel.orbit;
                landed = FlightGlobals.ActiveVessel.Landed;
                altitude = FlightGlobals.ActiveVessel.altitude; // LocationUtil.TerrainHeight(latitude, longitude, body);
                // heading = FlightGlobals.ActiveVessel.head
                Log.Info("Creation Latitude: " + latitude + ",   Longitude: " + longitude);
               
            }
        }

        internal void Spawn()
        {
            KD kd = new KD();
            Log.Info("Spawning a GeoCache" );

            // Set additional info for landed 
            if (kd.landed)
            {
                Vector3d pos = kd.body.GetWorldSurfacePosition(kd.latitude, kd.longitude, kd.altitude);

                kd.orbit = new Orbit(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, kd.body);
                kd.orbit.UpdateFromStateVectors(pos, kd.body.getRFrmVel(pos), kd.body, Planetarium.GetUniversalTime());
               
            }
            else
            {
                // Update the reference body in the orbit
                kd.orbit.referenceBody = kd.body;
            }

            uint flightId = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);


            kd.craftPart = PartLoader.getPartInfoByName("geocache");
            // Create part nodes
            ConfigNode[] partNodes = new ConfigNode[1];
            partNodes[0] = ProtoVessel.CreatePartNode(kd.craftPart.name, flightId, null);
            

            // Create additional nodes
            ConfigNode[] additionalNodes = new ConfigNode[0];
            // Create the config node representation of the ProtoVessel        
            ConfigNode protoVesselNode = ProtoVessel.CreateVesselNode(VesselName, VesselType.Probe, kd.orbit, 0, partNodes);

            // Additional seetings for a landed vessel
            if (kd.landed)
            {
                bool splashed = kd.altitude < 0.001 && kd.body.ocean;

                // Add a bit of height for landed
                if (!splashed)
                {
                    kd.altitude += 40.2;
                    Log.Info("Adding 40.2 to altitude");
                }
                //Guid vesselId = vessel.id;

                // Figure out the appropriate rotation
                Vector3d norm = kd.body.GetRelSurfaceNVector(kd.latitude, kd.longitude);
                double terrainHeight = 0.0;
                if (kd.body.pqsController != null)
                {
                    terrainHeight = kd.body.pqsController.GetSurfaceHeight(norm) - kd.body.pqsController.radius;
                }

                // Create the config node representation of the ProtoVessel

                protoVesselNode.SetValue("sit", (splashed ? Vessel.Situations.SPLASHED : Vessel.Situations.LANDED).ToString());
                protoVesselNode.SetValue("landed", (!splashed).ToString());
                protoVesselNode.SetValue("splashed", splashed.ToString());
                protoVesselNode.SetValue("lat", kd.latitude);
                protoVesselNode.SetValue("lon", kd.longitude);
                protoVesselNode.SetValue("alt", kd.altitude);                
                protoVesselNode.SetValue("landedAt", kd.body.name);

                float lowest = float.MaxValue;
                foreach (Collider collider in kd.craftPart.partPrefab.GetComponentsInChildren<Collider>())
                {
                    if (collider.gameObject.layer != 21 && collider.enabled)
                    {
                        lowest = Mathf.Min(lowest, collider.bounds.min.y);
                    }
                }
                if (Mathf.Approximately(lowest, float.MaxValue))
                {
                    lowest = 0;
                }

                float hgt = kd.craftPart.partPrefab.localRoot.attPos0.y - lowest;
                hgt += 10;
                protoVesselNode.SetValue("hgt", hgt);

                // Set the normal vector relative to the surface
                Quaternion normal = Quaternion.LookRotation(new Vector3((float)norm.x, (float)norm.y, (float)norm.z));
                Quaternion rotation = Quaternion.identity;
                rotation = rotation * Quaternion.FromToRotation(Vector3.up, Vector3.back);
                Vector3 nrm = (rotation * Vector3.forward);
                protoVesselNode.SetValue("prst", false.ToString());
                protoVesselNode.SetValue("nrm", nrm.x + "," + nrm.y + "," + nrm.z);
            }
            ProtoVessel protoVessel = HighLogic.CurrentGame.AddVessel(protoVesselNode);
            //protoVessel.Load(HighLogic.CurrentGame.flightState);

            protoVessel.vesselName = VesselName;
           
            vessel = protoVessel.vesselRef;
            if (vessel != null)
            {
                //var mode = OrbitDriver.UpdateMode.UPDATE;
                //vessel.orbitDriver.SetOrbitMode(mode);

                vessel.Load();
                launchPoint = vessel.GetWorldPos3D();
                // Offset the position by 10m in y and z to have it created away from the current vessel
                launchPoint.y += 10;
                launchPoint.z += 10;
                
                StartCoroutine("SpawnCoRoutine");
            }

        }
        Vessel vessel;
        Vector3 launchPoint;
        IEnumerator SpawnCoRoutine()
        {
            if ((vessel.Parts[0] != null) && (vessel.Parts[0].Rigidbody != null))
            {
                vessel.Parts[0].Rigidbody.isKinematic = true;
            }
            //vessel.GoOnRails();
            while ((vessel.packed) && (vessel.acceleration.magnitude == 0))
            {
                vessel.SetPosition(launchPoint);
                yield return null;
            }
            while (true)
            {
                bool partsInitialized = true;
                foreach (Part p in vessel.parts)
                {
                    if (!p.started)
                    {
                        partsInitialized = false;
                        break;
                    }
                }
                vessel.SetPosition(launchPoint, true);
                //vessel.SetWorldVelocity(Vector3d.up * 0);
                if (partsInitialized)
                {
                    break;
                }
                OrbitPhysicsManager.HoldVesselUnpack(2);
                yield return null;
            }
            vessel.GoOffRails();

            //flare.IgnoreGForces(250);
            Log.Info("Final Latitude: " + vessel.latitude + ",   Longitude: " + vessel.longitude);

        }
    }
#else

        // Following from the following:
        // https://forum.kerbalspaceprogram.com/index.php?/topic/164792-how-to-create-a-vessel/
        // https://github.com/TAz00/Mod-KSPSmelter

        public class HideConfigNode
        {
            private ConfigNode node = null;
            public HideConfigNode(ConfigNode node)
            {
                this.SetConfigNode(node);
            }
            public HideConfigNode()
            {

            }
            public void SetConfigNode(ConfigNode node)
            {
                this.node = node;
            }
            public void SetValue(string name, string value)
            {
                node.SetValue(name, value);
            }
            public ConfigNode GetConfigNode()
            {
                return this.node;
            }
        }

        /// <summary>
        /// This class is pretty much built on what KAS uses to pull parts out of containers
        /// All credit to https://github.com/KospY/KAS
        /// https://github.com/KospY/KAS/blob/master/LICENSE.md
        /// </summary>
        public class VesselSpawner
        {
            const string PluginPath = "/GameData/KSPGeoCaching/PluginData/Ships/";

            private HideConfigNode _OldVabShip;
            private string _craftFile = "";
            private Part _srcPart = null;
            private Vector3 _spawnOffset = new Vector3(0, 0, 0);
            /// <summary>
            /// 
            /// </summary>
            /// <param name="_craftFile">Craft file name as it appears in the PluginPath folder</param>
            /// <param name="_srcPart">Source part to spawn relative to</param>
            /// <param name="_spawnOffset">Offset spawn from Source part position</param>
            public VesselSpawner(string _craftFile, Part _srcPart, Vector3 _spawnOffset)
            {
                //Store paths
                this._craftFile = _craftFile;
                this._srcPart = _srcPart;
                this._spawnOffset = _spawnOffset;

                if (this._srcPart == null)
                    Debug.Log("Relative source part can't be null");
                if ((_craftFile != String.Empty))
                    Debug.Log("Source part path can't be null");

            }
            /// <summary>
            /// Spawns the vessel
            /// </summary>
            public void SpawnVessel()
            {
                Log.Info("SpawnVessel");
                //Load craft file
                ShipConstruct _ship = LoadVessel(this._craftFile);
                if (_ship != null)
                    SpawnVessel(_ship, this._srcPart, this._spawnOffset);
                else
                    Debug.Log("Failed to load the vessel");
            }
            /// <summary>
            /// Attempt vessel load
            /// </summary>
            /// <param name="_craftFile"></param>
            /// <returns></returns>
            private ShipConstruct LoadVessel(string _craftFile)
            {
                //Get path to vessel
                string BasePath = Environment.CurrentDirectory + PluginPath;
                string path = BasePath + _craftFile;

                Log.Info("LoadVessel, full path: " + path);
                //Save old ship for later, else player will see it in the VAB
                _OldVabShip = new HideConfigNode(ShipConstruction.ShipConfig);

                //Load craft file
                ShipConstruct _shipConstruct = ShipConstruction.LoadShip(path);

                //Check load
                if (_shipConstruct == null)
                {
                    // Restore ShipConstruction ship, otherwise player sees loaded craft in VAB
                    ShipConstruction.ShipConfig = _OldVabShip.GetConfigNode();
                    Log.Info("vessel not loaded");
                    return null; //Fail
                }
                return _shipConstruct;
            }
            /// <summary>
            /// Spawn ship construct
            /// https://github.com/KospY/KAS/blob/master/Plugin/KAS_Shared.cs
            /// </summary>
            /// <param name="_shipConstruct">Shipconstruct to spawn</param>
            /// <param name="_srcPart">Source part to spawn relative to</param>
            /// <param name="_spawnOffset">Offset spawn from Source part position</param>
            private void SpawnVessel(ShipConstruct _shipConstruct, Part _srcPart, Vector3 _spawnOffset)
            {
                //Store construct root
                Part _newConstructRootPart = _shipConstruct.parts[0];

                //Center rootpart
                Vector3 offset = _newConstructRootPart.transform.localPosition;
                _newConstructRootPart.transform.Translate(-offset);

                //Get launch spawn point, relative to part
                Transform t = _srcPart.transform;
                GameObject launchPos = new GameObject();
                launchPos.transform.parent = _srcPart.transform;
                launchPos.transform.position = t.position;
                launchPos.transform.position += t.TransformDirection(_spawnOffset);
                launchPos.transform.rotation = t.rotation;
                //Store our launch / spawn position
                Transform launchTransform = launchPos.transform;
                //Kill original object
                launchPos.DestroyGameObject();
                //Set rootpart origin
                _shipConstruct.Parts[0].localRoot.transform.Translate(launchPos.transform.position, Space.World);
                //Position
                float angle;
                Vector3 axis;
                //Extract ToAngleAxis data from selected spawning location
                launchTransform.rotation.ToAngleAxis(out angle, out axis);
                //TRANSFORM Rotate localRootPart in relation to root
                _shipConstruct.Parts[0].localRoot.transform.RotateAround(launchTransform.position, axis, angle);

                //Create vessel object
                Vessel _newVessel = _newConstructRootPart.localRoot.gameObject.AddComponent<Vessel>();
                //Attach vessel information
                _newVessel.id = Guid.NewGuid();
                _newVessel.vesselName = _srcPart.vessel.vesselName + " - " + _shipConstruct.shipName;
                _newVessel.landedAt = _srcPart.vessel.vesselName;

                //Store backup
                ShipConstruction.CreateBackup(_shipConstruct);

                //Init from VAB
                _newVessel.Initialize(true);
                //Set Landed
                _newVessel.Landed = true;

                //_newVessel.situation = Vessel.Situations.PRELAUNCH;
               // _newVessel.GoOffRails();
                //_newVessel.IgnoreGForces(240);


                //Set Orbit
                InitiateOrbit(launchTransform.position, _srcPart.vessel, _newVessel);

                //Set Mission info
                uint missionId = (uint)Guid.NewGuid().GetHashCode();
                string flagUrl = _srcPart.flagURL;
                uint launchId = HighLogic.CurrentGame.launchID++;

                //Set part mission info
                for (int i = 0; i < _newVessel.parts.Count; i++)
                {
                    Part part = _newVessel.parts[i];
                    part.flightID = ShipConstruction.GetUniqueFlightID(FlightDriver.FlightStateCache.flightState);
                    part.flagURL = flagUrl;
                    part.launchID = launchId;
                    part.missionID = missionId;
                }

                //Generate staging
                KSP.UI.Screens.StageManager.BeginFlight();
                _newConstructRootPart.vessel.ResumeStaging();
                KSP.UI.Screens.StageManager.GenerateStagingSequence(_newConstructRootPart.localRoot);
                KSP.UI.Screens.StageManager.RecalculateVesselStaging(_newConstructRootPart.vessel);

                //Set position, again
                _newVessel.SetPosition(launchTransform.position);
                _newVessel.SetRotation(launchTransform.rotation);



                //Save Protovessel
                ProtoVessel _newProto = new ProtoVessel(_newVessel);

                //Kill and remove spawned vessel, had some serious problems with spawn position warping/glitching
                _newVessel.Die();

                //Set the protovessels position to the relative one we found, maybe redundant
                _newProto.position = launchPos.transform.position;

                //If you check this value, you will see the height change from launch scene to resume scene, extra dafuq
                //float height = _newProto.height;

                if (FlightDriver.StartupBehaviour == FlightDriver.StartupBehaviours.RESUME_SAVED_FILE ||
                    FlightDriver.StartupBehaviour == FlightDriver.StartupBehaviours.RESUME_SAVED_CACHE)
                {
                    //Odd behaviour with positioning during different flight scenes, workaround awaaaay
                    Log.Info("Workaround of height");
                    _newProto.height = TrueAlt(launchTransform.position, _srcPart.vessel);
                }
                _newProto.altitude += 10;
                _newProto.height += 10;
                _newProto.situation = Vessel.Situations.FLYING;

                //Load Protovessel
                _newProto.Load(HighLogic.CurrentGame.flightState);

               // _newVessel.GoOnRails();

                // Restore ShipConstruction ship, otherwise player sees loaded craft in VAB
                ShipConstruction.ShipConfig = _OldVabShip.GetConfigNode();

                //Fix Control Lock
                FlightInputHandler.ResumeVesselCtrlState(FlightGlobals.ActiveVessel);
                //Fix active vessel staging
                FlightGlobals.ActiveVessel.ResumeStaging();

            }
            /// <summary>
            /// http://forum.kerbalspaceprogram.com/threads/111116-KSP-Altitude-Calculation-Inquiry
            /// </summary>
            /// <returns></returns>
            private float TrueAlt(Vector3 _LauncPos, Vessel _srcVessel)
            {
                //Vector3 pos = _srcPart.transform.position; //or this.vessel.GetWorldPos3D()
                float ASL = FlightGlobals.getAltitudeAtPos(_LauncPos);
                if (_srcVessel.mainBody.pqsController == null) { return ASL; }
                float terrainAlt = Convert.ToSingle(_srcVessel.pqsAltitude);
                _srcVessel.GetHeightFromTerrain();
                if (_srcVessel.mainBody.ocean && _srcVessel.heightFromTerrain <= 0) { return ASL; } //Checks for oceans
                return ASL - terrainAlt;
            }
            /// <summary>
            /// https://github.com/taniwha-qf/Extraplanetary-Launchpads/blob/master/Source/BuildControl.cs
            /// https://github.com/taniwha-qf/Extraplanetary-Launchpads/blob/master/License.txt
            /// </summary>
            /// <param name="_newVessel"></param>
            /// <param name="_srcVessel"></param>
            private void InitiateOrbit(Vector3 _spawnPoint, Vessel _srcVessel, Vessel _newVessel)
            {
                var mode = OrbitDriver.UpdateMode.UPDATE;
                _newVessel.orbitDriver.SetOrbitMode(mode);

                var craftCoM = GetVesselWorldCoM(_newVessel);
                var vesselCoM = _spawnPoint;
                var offset = (Vector3d.zero + craftCoM - vesselCoM).xzy;

                var corb = _newVessel.orbit;
                var orb = _srcVessel.orbit;
                var UT = Planetarium.GetUniversalTime();
                var body = orb.referenceBody;
                corb.UpdateFromStateVectors(orb.pos + offset, orb.vel, body, UT);

                Debug.Log(String.Format("[EL] {0} {1}", "orb", orb.pos));
                Debug.Log(String.Format("[EL] {0} {1}", "corb", corb.pos));

            }
            public Vector3 GetVesselWorldCoM(Vessel v)
            {
                var com = v.CoM;
                return v.rootPart.partTransform.TransformPoint(com);
            }
        }


    }
#endif
}
#endif