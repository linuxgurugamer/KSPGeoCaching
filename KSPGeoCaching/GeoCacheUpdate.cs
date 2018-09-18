using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KSPGeoCaching
{
    partial class GeoCacheDriver
    {

        Hint currentHint;
        int hintIndex;
        int cacheIndex;
//        activeGeoCacheCollection
//        activeGeoCacheData

            
        void InitiatePlay()
        {
            StartCoroutine(GeoCacheUpdate());
            hintIndex = 0;
            cacheIndex = 0;

        }
        IEnumerator GeoCacheUpdate()
        {
            while (true)
            {
                Log.Info("GeoCacheUpdate");
                yield return new WaitForSeconds(1f);
            }
        }

    }
}
