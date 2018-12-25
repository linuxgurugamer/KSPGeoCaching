using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace KeoCaching
{

    public class KeoCacheOptions : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Default Settings"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "KeoCaching"; } }
        public override string DisplaySection { get { return "KeoCaching"; } }
        public override int SectionOrder { get { return 1; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Use KSP skin")]
        public bool useKSPskin = false;

        [GameParameters.CustomParameterUI("Use Degrees & Decimal Minutes only",
           toolTip = "Only use Degrees and decimal minutes.  If not set, uses Degrees/Minutes/Seconds")]
        public bool useDecimalMinutes = true;

#if false
        [GameParameters.CustomParameterUI("Lose all progress when Travelbug is destroyed")]
        public bool loseAllProgress = false;
#endif
        [GameParameters.CustomParameterUI("Allow same collection to be on multiple vessels")]
        public bool allowOnMultiple = false;

        [GameParameters.CustomParameterUI("Allow multiple TraveBugs to be on the same vessel")]
        public bool allowMultipleOnVessel = false;

        [GameParameters.CustomParameterUI("Pause when showing hints")]
        public bool pauseAtNewHint = false;

        [GameParameters.CustomParameterUI("Hide UI when paused")]
        public bool hideWhenPaused = false;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    //loseAllProgress = false;
                    break;
                case GameParameters.Preset.Moderate:
                    //loseAllProgress = false;
                    break;
                case GameParameters.Preset.Normal:
                    //loseAllProgress = true;
                    break;
                 case GameParameters.Preset.Hard:
                    //loseAllProgress = true;
                    break;
            }
        }
        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }
}
