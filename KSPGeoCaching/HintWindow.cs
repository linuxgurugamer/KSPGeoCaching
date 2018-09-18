using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using ToolbarControl_NS;
using ClickThroughFix;


namespace KSPGeoCaching
{
    partial class GeoCacheDriver : MonoBehaviour
    {
        internal Hint activeHint;
        internal Hint origHint;
        bool editHint;
        bool km = true;
        bool m = false;
        float max;
        bool oldKm, oldM;

        internal void Hint_Window(int i)
        {
            //distance = 0;
            //hint = "";
            //hintTitle = "";

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Title:");
            activeHint.hintTitle = GUILayout.TextField(activeHint.hintTitle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hint: ");
            activeHint.hint = GUILayout.TextArea(activeHint.hint, GUILayout.Height(75), GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Proximity distance:");

            try
            {
                activeHint.distance = Int32.Parse(GUILayout.TextField(activeHint.distance.ToString(), GUILayout.Width(100)));
            }
            catch { }

            oldKm = activeHint.scale == Scale.Km;
            oldM = activeHint.scale == Scale.m;
            GUILayout.FlexibleSpace();
            km = GUILayout.Toggle(oldKm, "Km");
            GUILayout.FlexibleSpace();
            m = GUILayout.Toggle(oldM, "M");
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();



            if (km && !oldKm)
            {
                activeHint.scale = Scale.Km;
            }
            if (m && !oldM)
            {
                activeHint.scale = Scale.m;
            }
            if (km)
                max = 1000;
            if (m)
                max = 10000;
            GUILayout.BeginHorizontal();

            activeHint.distance = (float)Math.Floor(GUILayout.HorizontalSlider((float)activeHint.distance, 1f, max));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save"))
            {
                visibleHint = false;
                activeHint.sort = activeHint.distance;
                if (activeHint.scale == Scale.Km)
                    activeHint.sort *= 1000;
                if (!editHint)
                    activeGeoCacheData.hints.Add(activeHint);
                else
                {
                    origHint.Copy(activeHint);
                }
                activeGeoCacheData.hints = activeGeoCacheData.hints.OrderBy(d => d.sort).ToList();
                activeHint = null;
                origHint = null;
            }
            if (GUILayout.Button("Cancel"))
            {
                visibleHint = false;
                activeHint = null;
                origHint = null;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
