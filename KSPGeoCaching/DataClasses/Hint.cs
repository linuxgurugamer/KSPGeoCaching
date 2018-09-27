using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
    public enum Scale { m, Km };
    public class Hint
    {
        System.Guid hintId;
        public string hintTitle;
        public string hint;
        public double distance;
        public Scale scale;
        public double absoluteDistance;
        public bool spawn;
        public bool found;

        internal Hint()
        {

            hintId = new Guid();
            distance = 1;
            hint = "";
            hintTitle = "";
            scale = Scale.Km;
            spawn = false; // if true, spawn geocache vessel when within this distance
            found = false;
        }
        internal Hint Copy()
        {
            Hint newHint = new Hint();

            newHint.distance = distance;
            newHint.hint = hint;
            newHint.hintTitle = hintTitle;
            newHint.scale = scale;
            newHint.spawn = spawn;
            return newHint;
        }
        internal void Copy(Hint oldHint)
        {
            distance = oldHint.distance;
            hint = oldHint.hint;
            hintTitle = oldHint.hintTitle;
            scale = oldHint.scale;
            spawn = oldHint.spawn;
        }
    }
}
