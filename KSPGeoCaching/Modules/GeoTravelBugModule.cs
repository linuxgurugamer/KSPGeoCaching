using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPGeoCaching
{
    public class GeoTravelBugModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public System.Guid travelbugId;

        [KSPEvent(name = "SelectCollection", guiActive = false, guiActiveEditor = true, guiName = "Select collection")]
        public void SelectCollection()
        {
            Log.Info("GeoTravelBugModule.SelectCollection");
        }

        void OnDestroy()
        {
        }
    }
}
