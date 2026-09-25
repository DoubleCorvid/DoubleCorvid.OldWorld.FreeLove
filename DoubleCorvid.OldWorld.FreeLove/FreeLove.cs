using DoubleCorvid.OldWorld.FreeLove.Overrides;
using TenCrowns.AppCore;
using TenCrowns.GameCore;

namespace DoubleCorvid.OldWorld.FreeLove {
    public class FreeLove : ModEntryPointAdapter {
        public override void Initialize (ModSettings modSettings) {
            modSettings.Factory = new FreeLoveGameFactory ();
        }
    }
}
