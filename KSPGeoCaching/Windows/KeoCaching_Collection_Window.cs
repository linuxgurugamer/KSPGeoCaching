
using System.Collections;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KeoCaching
{
    partial class KeoCacheDriver : MonoBehaviour
    {
        public enum ParentWin { import, collection };
        bool saveable;
        //GUIStyle curLabelStyle, curbuttonStyle;

        void StartLoadDialog(ParentWin pWin, Rect r)
        {
            fsDialog = gameObject.AddComponent<FileSelection>();
            fsDialog.SetExtensions(FileIO.SUFFIX);
            fsDialog.parentRect = r;
            fsDialog.pWin = pWin;
            dialogEntry = null;
            Log.Info("KeoScenario.Instance.lastDirectory: " + KeoScenario.Instance.lastDirectory);
            fsDialog.SetSelectedDirectory(KeoScenario.Instance.lastDirectory, false);
            fsDialog.startDialog();
        }
        internal Rect GetParentRect(ParentWin pWin)
        {
            switch (pWin)
            {
                case ParentWin.import:
                    return importwinRect;
                case ParentWin.collection:
                    return collectionWinRect;
            }
            return new Rect();
        }
        internal void UpdateParentWin(ParentWin pWin, Rect r)
        {
            switch (pWin)
            {
                case ParentWin.import:
                    importwinRect = r;
                    break;
                case ParentWin.collection:
                    collectionWinRect = r;
                    break;
            }
        }

        void DisplayCollectionData()
        {
            saveable = true;
  
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:"); //, activeKeoCacheCollection.keocacheCollectionData.name == "" ? labelRed : labelNormal);
            GUILayout.FlexibleSpace();
            //Log.Info("DisplayCollectionData, visibleActiveCollectionsReadOnly: " + visibleActiveCollectionsReadOnly);
            saveable = saveable & !SetBackground(activeKeoCacheCollection.keocacheCollectionData.name == "");
            string s = GUILayout.TextField(activeKeoCacheCollection.keocacheCollectionData.name, GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeKeoCacheCollection.keocacheCollectionData.name = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("Title:"); //, activeKeoCacheCollection.keocacheCollectionData.title == "" ? labelRed : labelNormal);
            GUILayout.FlexibleSpace();
            saveable = saveable & ! SetBackground(activeKeoCacheCollection.keocacheCollectionData.title.Length == 0);
            s = GUILayout.TextField(activeKeoCacheCollection.keocacheCollectionData.title, GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeKeoCacheCollection.keocacheCollectionData.title = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:"); //, activeKeoCacheCollection.keocacheCollectionData.author.Length == 0 ? labelRed : labelNormal);
            GUILayout.FlexibleSpace();
            saveable = saveable & !SetBackground(activeKeoCacheCollection.keocacheCollectionData.author.Length == 0);
            s = GUILayout.TextField(activeKeoCacheCollection.keocacheCollectionData.author, GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeKeoCacheCollection.keocacheCollectionData.author = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            GUILayout.FlexibleSpace();
            saveable = saveable & !SetBackground(activeKeoCacheCollection.keocacheCollectionData.description.Length == 0);
            s = GUILayout.TextArea(activeKeoCacheCollection.keocacheCollectionData.description, GUILayout.Height(75), GUILayout.Width(300));
            SetBackground(false);
            if (!visibleActiveCollectionsReadOnly)
                activeKeoCacheCollection.keocacheCollectionData.description = s;
            GUILayout.EndHorizontal();

            if (!visibleActiveCollectionsReadOnly)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Initial Hint:");
                GUILayout.FlexibleSpace();
                saveable = saveable & !SetBackground(activeKeoCacheCollection.keocacheCollectionData.initialHint.Length == 0);
                s = GUILayout.TextArea(activeKeoCacheCollection.keocacheCollectionData.initialHint, GUILayout.Height(75), GUILayout.Width(300));
                SetBackground(false);
                if (!visibleActiveCollectionsReadOnly)
                    activeKeoCacheCollection.keocacheCollectionData.initialHint = s;
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Difficulty: ");
            GUILayout.FlexibleSpace();
            if (visibleActiveCollectionsReadOnly)
                GUI.enabled = false;
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                if (activeKeoCacheCollection.keocacheCollectionData.difficulty > 0)
                    activeKeoCacheCollection.keocacheCollectionData.difficulty--;
            }
            GUI.enabled = true;
            GUILayout.Label(activeKeoCacheCollection.keocacheCollectionData.difficulty.ToString(), GUILayout.Width(100));
            if (visibleActiveCollectionsReadOnly)
                GUI.enabled = false;
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                if (activeKeoCacheCollection.keocacheCollectionData.difficulty < Difficulty.Insane)
                    activeKeoCacheCollection.keocacheCollectionData.difficulty++;
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Required Mods:");
            GUILayout.FlexibleSpace();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(60), GUILayout.Width(300));
            foreach (var s1 in activeKeoCacheCollection.keocacheCollectionData.requiredMods)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(s1);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        void DisplayCollectionButtons()
        {
            if (!visibleActiveCollectionsReadOnly)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Load Collection", buttonWidth))
                {
                    StartLoadDialog(ParentWin.collection, collectionWinRect);
                }
                GUILayout.FlexibleSpace();
                if (!saveable)
                    GUI.enabled = false;
                if (GUILayout.Button("Save Collection", buttonWidth))
                {
                    FileIO.SaveKeoCacheFile(activeKeoCacheCollection);
                    CloseKeoCacheWindow();
                }
                GUI.enabled = true;


                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close without saving", buttonWidth))
                    CloseKeoCacheWindow();

            GUILayout.FlexibleSpace();

            //GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            if (!visibleActiveCollectionsReadOnly)
            {
                    saveable = saveable & activeKeoCacheCollection.keoCacheDataList.Count > 0;
                if (GUILayout.Button("New KeoCache", activeKeoCacheCollection.keoCacheDataList.Count == 0 ? redButton : normalButton, buttonWidth))
                {
                    visibleKeoCache = true;
                    activeKeoCacheData = new KeoCacheData();

                    newKeoCacheData = true;
                    //LaunchEvent();
                    //StartCoroutine(WaitForSpawn());
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            }
            else
            {

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", buttonWidth))
                    CloseKeoCacheWindow();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Select", buttonWidth))
                {
                    SelectKeoCacheForBug(activeKeoCacheCollection.fullPath);
                    CloseKeoCacheWindow();
                }

                GUILayout.FlexibleSpace();
            }
        }

        void DisplayCollectionCaches()
        {
            GUILayout.BeginHorizontal();
#if false
            GUIStyle labelStyle = labelNormal;
            if (activeKeoCacheCollection.keoCacheDataList.Count == 0)
                labelStyle = labelRed;
#endif
            GUILayout.Label("KeoCaches"); //, labelStyle);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));

            int indexToMove = -1, indexToMoveTo = -1;
            KeoCacheData itemToMove = null;
            KeoCacheData itemToDelete = null;
            for (int i = 0; i < activeKeoCacheCollection.keoCacheDataList.Count; i++)
            {
                KeoCacheData g = activeKeoCacheCollection.keoCacheDataList[i];
                saveable = saveable & !SetBackground(g.hints.Count == 0 || g.description == "");
                if (g.hints.Count == 0 || g.description == "")
                    saveable = false;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.keoCacheName))
                {
                    activeKeoCacheData = g;
                    newKeoCacheData = false;
                    visibleKeoCache = true;
                }
                SetBackground(false);
                GUILayout.BeginVertical(GUILayout.Width(15));
                GUILayout.Space(3);
                if (i == 0)
                    GUI.enabled = false;
                if (GUILayout.Button(upContent, arrowButtonStyle))
                {
                    indexToMove = i;
                    indexToMoveTo = i - 1;
                    itemToMove = g;
                }
                if (i < activeKeoCacheCollection.keoCacheDataList.Count - 1)
                    GUI.enabled = true;
                else
                    GUI.enabled = false;
                if (GUILayout.Button(downContent, arrowButtonStyle))
                {
                    indexToMove = i;
                    indexToMoveTo = i + 1;
                    itemToMove = g;
                }
                GUILayout.EndVertical();
                GUI.enabled = true;
                if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
                {
                    itemToDelete = g;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            if (itemToDelete != null)
            {
                activeKeoCacheCollection.keoCacheDataList.Remove(itemToDelete);
                itemToDelete.DeleteVessel();
                itemToDelete.hints.Clear();
            }
            if (indexToMove != -1)
            {
                activeKeoCacheCollection.keoCacheDataList.Remove(itemToMove);
                activeKeoCacheCollection.keoCacheDataList.Insert(indexToMoveTo, itemToMove);
            }
        }

        void KeoCaching_Collection_Window(int windowId)
        {
            saveable = true;
            if (loadDialog)
            {
                loadDialog = false;
                // following to avoid some nullrefs 
                Log.Info("Setting activeKeoCacheCollection to null 2");
                activeKeoCacheCollection = new KeoCacheCollection();
                StartLoadDialog(ParentWin.collection, collectionWinRect);
            }
            GUILayout.BeginHorizontal();


            GUILayout.BeginVertical();
            DisplayCollectionData();
            // GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            if (!visibleActiveCollectionsReadOnly)
            {
                GUILayout.BeginVertical();
                DisplayCollectionCaches();
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DisplayCollectionButtons();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private IEnumerator WaitForSpawn()
        {
            Log.Info("WaitForSpawn");
            // activeKeoCacheData = new KeoCacheData();
            while (VesselSpawn.instance.spawnedVessel == null)
                yield return null;
            activeKeoCacheData.CacheVessel = VesselSpawn.instance.spawnedVessel;
            Part part = activeKeoCacheData.CacheVessel.Parts[0];
            KeoCacheModule partmod = part.FindModuleImplementing<KeoCacheModule>();
            Log.Info("WaitForSpawn");
            Log.Info("part.craftID: " + part.craftID);
            Log.Info("activeKeoCacheData.keocacheId: " + activeKeoCacheData.keocacheId);
            Log.Info("KeoCacheModule.AssignStatus.inprogress: " + KeoCacheModule.AssignStatus.inprogress);
            Log.Info("activeKeoCacheCollection.keocacheCollectionData.collectionId: " + activeKeoCacheCollection.keocacheCollectionData.collectionId);
            partmod.UpdateData(activeKeoCacheData.keocacheId, KeoCacheModule.AssignStatus.inprogress, activeKeoCacheCollection.keocacheCollectionData.collectionId);

            //activeKeoCacheCollection.keocacheData.Add(activeKeoCacheData);
        }

    }
}
