using TenCrowns.GameCore;

namespace DoubleCorvid.OldWorld.FreeLove.Overrides {
    public class FreeLoveGameFactory : GameFactory {
        public override Player CreatePlayer () {
            return new FreeLovePlayer ();
        }

        public override Player.PlayerAI CreatePlayerAI () {
            return new FreeLovePlayer.PlayerAI ();
        }

        public override Character CreateCharacter () {
            return new FreeLoveCharacter ();
        }
    }
}
