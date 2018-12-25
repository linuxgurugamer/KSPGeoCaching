using System;
using UnityEngine;
using System.Linq;


namespace KeoCaching
{
    partial class KeoCacheDriver : MonoBehaviour
    {
        internal Hint activeHint;
        internal Hint origHint;
        bool editHint;
        bool km = true;
        bool m = false;
        float max;
        bool oldKm, oldM;

        void ShowGetProximity(string obj, ref double distance, ref double absoluteDistance, ref KeoCaching.Scale scale )
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(obj + "proximity distance:");

            double.TryParse(GUILayout.TextField(distance.ToString(), GUILayout.Width(100)), out distance);

            oldKm = scale == Scale.km;
            oldM = scale == Scale.m;
            GUILayout.FlexibleSpace();
            km = GUILayout.Toggle(oldKm, "km");
            GUILayout.FlexibleSpace();
            m = GUILayout.Toggle(oldM, "m");
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            if (km && !oldKm)
            {
                scale = Scale.km;
            }
            if (m && !oldM)
            {
                scale = Scale.m;
            }
            if (km)
                max = 1000;
            if (m)
                max = 10000;

            GUILayout.BeginHorizontal();
            distance = (float)Math.Floor(GUILayout.HorizontalSlider((float)distance, 1f, max));
            
            GUILayout.EndHorizontal();
            absoluteDistance = distance;
            if (scale == Scale.km)
                absoluteDistance *= 1000;
        }

        internal void Hint_Window(int i)
        {
            //distance = 0;
            //hint = "";
            //hintTitle = "";

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            SetBackground(activeHint.hintTitle.Length == 0);
            GUILayout.Label("Title:");  //, activeHint.hintTitle.Length == 0? labelRed:labelNormal);
            activeHint.hintTitle = GUILayout.TextField(activeHint.hintTitle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hint: "); //, activeHint.hint.Length == 0?labelRed:labelNormal);
            SetBackground(activeHint.hint.Length == 0);
            activeHint.hint = GUILayout.TextArea(activeHint.hint, GUILayout.Height(75), GUILayout.Width(300));
            SetBackground(false);
            GUILayout.EndHorizontal();
#if false
                public enum Situations
        {
            LANDED = 1,
            SPLASHED = 2,
            PRELAUNCH = 4,
            FLYING = 8,
            SUB_ORBITAL = 16,
            ORBITING = 32,
            ESCAPING = 64,
            DOCKED = 128
        }
#endif
            bool landed = ((int)(Vessel.Situations.LANDED & activeHint.situations))>0;
            bool splashed = ((int)(Vessel.Situations.SPLASHED & activeHint.situations)) > 0; 
            bool flying = ((int)(Vessel.Situations.FLYING & activeHint.situations)) > 0; 

            GUILayout.BeginHorizontal();
            landed = GUILayout.Toggle(landed, "Landed");
            GUILayout.FlexibleSpace();
            splashed = GUILayout.Toggle(splashed, "Splashed");
            GUILayout.FlexibleSpace();
            flying = GUILayout.Toggle(flying, "Flying");

            activeHint.situations = 0;
            if (landed)
                activeHint.situations += (int)Vessel.Situations.LANDED;
            if (splashed)
                activeHint.situations += (int)Vessel.Situations.SPLASHED;
            if (flying)
                activeHint.situations += (int)Vessel.Situations.FLYING;

            GUILayout.EndHorizontal();
            double d = 0;
            Scale sc = 0;
            ShowGetProximity("Hint", ref d, ref activeHint.absoluteDistance, ref sc);
            activeHint.hintDistance = d;
            activeHint.scale = sc;

            GUILayout.BeginHorizontal();
            if (activeHint.hintTitle.Length == 0 || activeHint.hint.Length == 0)
                GUI.enabled = false;
            if (GUILayout.Button("Save"))
            {
                visibleHint = false;
                activeHint.absoluteDistance = activeHint.hintDistance;
                if (activeHint.scale == Scale.km)
                    activeHint.absoluteDistance *= 1000;
                if (!editHint)
                    activeKeoCacheData.hints.Add(activeHint);
                else
                {
                    origHint.Copy(activeHint);
                }
                activeKeoCacheData.hints = activeKeoCacheData.hints.OrderByDescending(ad => ad.absoluteDistance).ToList();
                activeHint = null;
                origHint = null;
            }
            GUI.enabled = true;
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
