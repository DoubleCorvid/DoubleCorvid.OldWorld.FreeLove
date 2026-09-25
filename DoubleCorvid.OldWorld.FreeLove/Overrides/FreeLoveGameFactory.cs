using TenCrowns.GameCore;

namespace DoubleCorvid.OldWorld.FreeLove.Overrides {
    public class FreeLoveGameFactory : GameFactory {
        public override Character CreateCharacter () {
            return new FreeLoveCharacter ();
        }
    }
}
