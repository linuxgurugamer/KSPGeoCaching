using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace KSPGeoCaching
{

    public class GeoCacheOptions : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Default Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "KSP GeoCaching"; } }
        public override string DisplaySection { get { return "KSP GeoCaching"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Use KSP skin")]
        public bool useKSPskin = true;

        [GameParameters.CustomParameterUI("Use Degrees & Decimal Minutes only",
           toolTip = "Only use Degrees and decimal minutes.  If not set, uses Degrees/Minutes/Seconds")]
        public bool useDecimalMinutes = true;

        [GameParameters.CustomParameterUI("Hide UI when paused")]
        public bool hideWhenPaused = false;

        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }
}
