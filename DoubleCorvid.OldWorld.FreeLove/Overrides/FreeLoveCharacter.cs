using System.Collections.Generic;
using Mohawk.SystemCore;
using TenCrowns.GameCore;

namespace DoubleCorvid.OldWorld.FreeLove.Overrides {
    public class FreeLoveCharacter : Character {
        public virtual bool IsStraight () => !(isBisexual () || isGay ());

        public override bool canMarry(CharacterType eCharacter, List<object> lSubjectsPrevious, bool bPolygamy = false) {
            using (new UnityProfileScope("Character.canMarry"))
            {
                if (!canMarry(bPolygamy))
                {
                    return false;
                }
                
                if (!infos ().character (eCharacter).mbSuitorTemp)
                {
                    if (!infos().Helpers.isAdult(eCharacter))
                    {
                        return false;
                    }
                    if (infos().character(eCharacter).meGender == getGender()) {
                        #region Modified code
                        var pSuitor = FindFreeLoveCharacter (eCharacter);
                        if (IsStraight () && getGender () == pSuitor.getGender ()) {
                            return false;
                        }

                        if (isGay () && getGender () != pSuitor.getGender ()) {
                            return false;
                        }
                        #endregion
                    }
                }
                return true;
            }
        }

        protected virtual FreeLoveCharacter FindFreeLoveCharacter (CharacterType eCharacter) {
            return ((FreeLoveCharacter) game ().findCharacter (eCharacter));
        }

        public override bool canMarry (Character pSuitor, bool bPolygamy = false, bool bLeavePlayer = false) {
            if (!canMarry(bPolygamy))
            {
                return false;
            }

            if (!(pSuitor.canMarry(bPolygamy)))
            {
                return false;
            }

            if (isLeader() && pSuitor.isLeader())
            {
                return false;
            }

            if (!bLeavePlayer && pSuitor.getPlayer() != getPlayer())
            {
                if (pSuitor.isLeader() || !hasPlayer())
                {
                    return false;
                }
            }

            #region Modified code
            if (IsStraight () && getGender () == pSuitor.getGender ()) {
                return false;
            }

            if (isGay () && getGender () != pSuitor.getGender ()) {
                return false;
            }
            #endregion

            if (isDescendantOf(pSuitor))
            {
                return false;
            }

            if (pSuitor.isDescendantOf(this))
            {
                return false;
            }

            if (isStepchildOf(pSuitor))
            {
                return false;
            }

            if (pSuitor.isStepchildOf(this))
            {
                return false;
            }

            if (isNephewNieceOf(pSuitor))
            {
                return false;
            }

            if (pSuitor.isNephewNieceOf(this))
            {
                return false;
            }

            if (isAnySiblingOf(pSuitor))
            {
                return false;
            }

            if (isCousinOf(pSuitor))
            {
                return false;
            }

            if (getSpouses().Contains(pSuitor.getID()))
            {
                return false;
            }

            if (hasRelationship(infos().Globals.DIVORCED_RELATIONSHIP, pSuitor.getID()))
            {
                return false;
            }

            if (pSuitor.hasRelationship(infos().Globals.DIVORCED_RELATIONSHIP, getID()))
            {
                return false;
            }

            return true;
        }

        protected override void doMarriage(bool bDowry)
        {
            Character pBestCharacter = getBestMarriageCandidate();
            if (pBestCharacter == null)
            {
                #region Modified Code
                GenderType eSuitorGender = GetSuitorGender();
                #endregion

                int iSuitorAge = randomSuitorAge(false);
                if (iSuitorAge >= 0)
                {
                    NameType eFirstName = game().randomFirstName(player().getNation(), TribeType.NONE, eSuitorGender, FamilyType.NONE, null, null, null, nextRandomSeed());
                    pBestCharacter = game().createNewCharacter(iSuitorAge, getPlayer(), eSuitorGender, eFirstName, FamilyType.NONE);

                    using (var traitScope = CollectionCache.GetHashSetScoped<TraitType>())
                    {
                        foreach (InfoTrait loopTrait in infos().traits())
                        {
                            if (loopTrait.mbNoMarry)
                            {
                                traitScope.Value.Add(loopTrait.meType);
                            }
                        }
                        pBestCharacter.fillValues(0, TraitType.NONE, traitScope.Value);
                    }

                    #region Added Code
                    pBestCharacter.addTrait (GetRequiredSuitorOrientation (), true);
                    #endregion
                }    
            }

            if (pBestCharacter != null)
            {
                marry(pBestCharacter, bDowry);

                player().pushLogData(() => TextManager.TEXT("TEXT_GAME_CHARACTER_MARRIAGE", HelpText.buildCharacterLinkVariable(this, player(), pPlayerRelation: player()), HelpText.buildCharacterLinkVariable(pBestCharacter, player(), pPlayerRelation: player())), GameLogType.CHARACTER_MARRIAGE, getID());
            }
        }

        protected virtual GenderType GetSuitorGender () {
            if (isGay ()) {
                return getGender ();
            }

            if (isBisexual ()) {
                if (UnityEngine.Random.Range (0, 1) == 0) {
                    return getGender ();
                }
            }

            return getGenderOpposite ();
        }

        protected virtual TraitType GetRequiredSuitorOrientation () {
            if (IsStraight ()) {
                return TraitType.NONE;
            }

            var roll = UnityEngine.Random.Range (0, 1);

            if (roll == 0) {
                return infos().Globals.BISEXUAL_TRAIT;
            }
            else {
                return infos().Globals.GAY_TRAIT;
            }
        }
    }
}
