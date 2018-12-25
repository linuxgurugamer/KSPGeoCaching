using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KeoCaching
{
    partial class KeoCacheDriver
    {

        Hint currentHint;
        //int hintIndex;
        //int cacheIndex;
        //        activeKeoCacheCollection
        //        activeKeoCacheData

        static internal bool updateActive = false;
            
        void InitiatePlay()
        {
            if (!updateActive)
            {
                StartCoroutine(KeoCacheUpdate());
                //hintIndex = 0;
                //cacheIndex = 0;
            }
#if false
            if (!allCollectionsLoaded)
                ReadAllCaches();
#endif
        }
        IEnumerator KeoCacheUpdate()
        {
            updateActive = true;
            while (true)
            {
#if false
                for (int i = KeoScenario.ActiveCollections - 1; i >0; i--)
                {

                }
#endif
               // Log.Info("KeoCacheUpdate");
                yield return new WaitForSeconds(1f);
            }
        }

    }
}
