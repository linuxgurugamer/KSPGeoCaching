using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeoCaching
{
    public enum Scale { m, km };
    public class Hint
    {
        System.Guid hintId;
        public string hintTitle;
        public string hint;
        public Vessel.Situations situations;

        public bool found;

        double dfc;
        public double hintDistance
        {
            get { return dfc; }
            set { dfc = value; SetAbsoluteDistance(); }
        }
        Scale sc;
        public KeoCaching.Scale scale
        {
            get { return sc; }
            set { sc = value; SetAbsoluteDistance(); }
        }

        internal double absoluteDistance;
        void SetAbsoluteDistance()
        {
            absoluteDistance = hintDistance;
            if (scale == Scale.km)
                absoluteDistance *= 1000;
        }


        internal Hint()
        {

            hintId = Guid.NewGuid();
            hintDistance = 1;
            absoluteDistance = 1000;
            hint = "";
            hintTitle = "";
            scale = Scale.km;
            situations = Vessel.Situations.LANDED;
//spawn = false; // if true, spawn keocache vessel when within this distance
            found = false;
        }
#if true
        internal Hint Copy()
        {
            Hint newHint = new Hint();

            newHint.hintDistance = hintDistance;
            newHint.hint = hint;
            newHint.hintTitle = hintTitle;
            newHint.scale = scale;
            newHint.situations = situations;
            newHint.absoluteDistance = absoluteDistance;
           // newHint.spawn = spawn;
            return newHint;
        }
        internal void Copy(Hint oldHint)
        {
            hintDistance = oldHint.hintDistance;
            hint = oldHint.hint;
            hintTitle = oldHint.hintTitle;
            scale = oldHint.scale;
            situations = oldHint.situations;
            absoluteDistance = oldHint.absoluteDistance;

            //  spawn = oldHint.spawn;
        }
#endif
    }
}
