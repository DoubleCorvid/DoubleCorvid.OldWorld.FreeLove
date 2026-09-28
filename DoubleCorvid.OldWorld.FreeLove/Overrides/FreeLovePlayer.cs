using System;
using System.Collections.Generic;
using Mohawk.SystemCore;
using TenCrowns.GameCore;
using TenCrowns.GameCore.Text;

namespace DoubleCorvid.OldWorld.FreeLove.Overrides {
    public class FreeLovePlayer : Player {
        #region Unmodified reference code
        public override bool doEventStory(EventStoryType eEventStory, ulong ulSeed, bool bModal, List<object> lTriggerSubjects)
        {
            using (var profileScope = new UnityProfileScope("Player.doEventStory"))
            using (var subjectListScoped = CollectionCache.GetListScoped<object>())
            {
                EventLinkData pEventLink = null;
                List<object> lSubjects = subjectListScoped.Value;
                if (findEventStoryBestSubjects(lSubjects, eEventStory, ulSeed, ref pEventLink, lTriggerSubjects))
                {
                    EventTriggerType eEventTrigger = infos().eventStory(eEventStory).meTrigger;

                    if (pEventLink != null)
                    {
                        removeEventLink(pEventLink);
                    }

                    using (var traitScope = CollectionCache.GetHashSetScoped<TraitType>())
                    using (var nameScope = CollectionCache.GetHashSetScoped<string>())
                    {
                        HashSet<TraitType> seTraitAdded = traitScope.Value;
                        HashSet<string> szNamesAdded = nameScope.Value;
                        
                        int iSortOrder = infos().eventStory(eEventStory).miSortOrder;

                        if (eEventTrigger != EventTriggerType.NONE)
                        {
                            iSortOrder = Math.Max(iSortOrder, infos().eventTrigger(eEventTrigger).miSortOrder);
                        }

                        EventStoryDecision pDecisionData = new EventStoryDecision(nextDecisionID(), infos(), eEventStory, iSortOrder, (bModal || infos().Helpers.isEventModal(eEventStory)));

                        for (int iI = 0; iI < lSubjects.Count; iI++)
                        {
                            object pLoopSubject = lSubjects[iI];

                            if (pLoopSubject != null)
                            {
                                string subjectString = subjectToString(pLoopSubject);
                                if (lTriggerSubjects != null)
                                {
                                    for (int triggerIndex = 0; triggerIndex < lTriggerSubjects.Count; triggerIndex++)
                                    {
                                        if (triggerIndex < lSubjects.Count && pLoopSubject.Equals(lTriggerSubjects[triggerIndex]))
                                        {
                                            pDecisionData.setTriggerSubjectIndex(triggerIndex, iI);
                                        }
                                    }
                                }
                                if (pLoopSubject is CharacterType)
                                {
                                    CharacterType eCharacter = infos().subject(infos().eventStory(eEventStory).maeSubjects[iI]).meCharacter;
                                    if (eCharacter == CharacterType.NONE)
                                    {
                                        MohawkAssert.Assert(false, "No character found for " + infos().eventStory(eEventStory).mzType);
                                    }

                                    PlayerType ePlayer = PlayerType.NONE;
                                    NationType eNation = NationType.NONE;
                                    FamilyType eFamily = FamilyType.NONE;
                                    TribeType eTribe = TribeType.NONE;
                                    ReligionType eReligion = ReligionType.NONE;

                                    foreach ((int First, SubjectRelationType Second, int Third) pLoopSubjectRelation in infos().eventStory(eEventStory).mltSubjectRelations)
                                    {
                                        if (pLoopSubjectRelation.First > pLoopSubjectRelation.Third)
                                        {
                                            if (pLoopSubjectRelation.First == iI)
                                            {
                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbArchetypeDiff)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.Third];

                                                    if (pSubjectOther is Character)
                                                    {
                                                        Character pOtherCharacter = (Character)pSubjectOther;

                                                        if (pOtherCharacter.hasArchetype())
                                                        {
                                                            seTraitAdded.Add(pOtherCharacter.getArchetype());
                                                        }
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbPlayerSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.Third];

                                                    if (pSubjectOther is PlayerType)
                                                    {
                                                        ePlayer = (PlayerType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        ePlayer = ((Character)pSubjectOther).getPlayer();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        ePlayer = ((City)pSubjectOther).getPlayer();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        ePlayer = ((Unit)pSubjectOther).getPlayer();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        ePlayer = ((Tile)pSubjectOther).getOwner();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbNationSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.Third];

                                                    if (pSubjectOther is PlayerType)
                                                    {
                                                        eNation = game().player((PlayerType)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eNation = ((Character)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        eNation = ((City)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        eNation = ((Unit)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        eNation = ((Tile)pSubjectOther).getNation();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbTribeSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.Third];

                                                    if (pSubjectOther is TribeType)
                                                    {
                                                        eTribe = (TribeType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eTribe = ((Character)pSubjectOther).getTribe();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        eTribe = ((City)pSubjectOther).getTribe();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        eTribe = ((Unit)pSubjectOther).getTribe();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        eTribe = ((Tile)pSubjectOther).getImprovementTribeSite();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbFamilySame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.Third];

                                                    if (pSubjectOther is FamilyType)
                                                    {
                                                        eFamily = (FamilyType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eFamily = ((Character)pSubjectOther).getFamily();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        eFamily = ((City)pSubjectOther).getFamily();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        eFamily = ((Unit)pSubjectOther).getFamily();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        eFamily = ((Tile)pSubjectOther).getFamily();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbReligionSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.Third];

                                                    if (pSubjectOther is PlayerType)
                                                    {
                                                        eReligion = game().player((PlayerType)pSubjectOther).getStateReligion();
                                                    }
                                                    else if (pSubjectOther is TribeType)
                                                    {
                                                        eReligion = game().getTribeReligion((TribeType)pSubjectOther);
                                                    }
                                                    else if (pSubjectOther is ReligionType)
                                                    {
                                                        eReligion = (ReligionType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is FamilyType)
                                                    {
                                                        eReligion = getFamilyReligion((FamilyType)pSubjectOther);
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eReligion = ((Character)pSubjectOther).getReligion();
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (pLoopSubjectRelation.Third == iI)
                                            {
                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbArchetypeDiff)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.First];

                                                    if (pSubjectOther is Character)
                                                    {
                                                        Character pOtherCharacter = (Character)pSubjectOther;

                                                        if (pOtherCharacter.hasArchetype())
                                                        {
                                                            seTraitAdded.Add(pOtherCharacter.getArchetype());
                                                        }
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbPlayerSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.First];

                                                    if (pSubjectOther is PlayerType)
                                                    {
                                                        ePlayer = (PlayerType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        ePlayer = ((Character)pSubjectOther).getPlayer();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        ePlayer = ((City)pSubjectOther).getPlayer();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        ePlayer = ((Unit)pSubjectOther).getPlayer();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        ePlayer = ((Tile)pSubjectOther).getOwner();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbNationSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.First];

                                                    if (pSubjectOther is PlayerType)
                                                    {
                                                        eNation = game().player((PlayerType)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eNation = ((Character)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        eNation = ((City)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        eNation = ((Unit)pSubjectOther).getNation();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        eNation = ((Tile)pSubjectOther).getNation();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbTribeSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.First];

                                                    if (pSubjectOther is TribeType)
                                                    {
                                                        eTribe = (TribeType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eTribe = ((Character)pSubjectOther).getTribe();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        eTribe = ((City)pSubjectOther).getTribe();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        eTribe = ((Unit)pSubjectOther).getTribe();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        eTribe = ((Tile)pSubjectOther).getImprovementTribeSite();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbFamilySame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.First];

                                                    if (pSubjectOther is FamilyType)
                                                    {
                                                        eFamily = (FamilyType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eFamily = ((Character)pSubjectOther).getFamily();
                                                    }
                                                    else if (pSubjectOther is City)
                                                    {
                                                        eFamily = ((City)pSubjectOther).getFamily();
                                                    }
                                                    else if (pSubjectOther is Unit)
                                                    {
                                                        eFamily = ((Unit)pSubjectOther).getFamily();
                                                    }
                                                    else if (pSubjectOther is Tile)
                                                    {
                                                        eFamily = ((Tile)pSubjectOther).getFamily();
                                                    }
                                                }

                                                if (infos().subjectRelation(pLoopSubjectRelation.Second).mbReligionSame)
                                                {
                                                    object pSubjectOther = lSubjects[pLoopSubjectRelation.First];

                                                    if (pSubjectOther is PlayerType)
                                                    {
                                                        eReligion = game().player((PlayerType)pSubjectOther).getStateReligion();
                                                    }
                                                    else if (pSubjectOther is TribeType)
                                                    {
                                                        eReligion = game().getTribeReligion((TribeType)pSubjectOther);
                                                    }
                                                    else if (pSubjectOther is ReligionType)
                                                    {
                                                        eReligion = (ReligionType)pSubjectOther;
                                                    }
                                                    else if (pSubjectOther is FamilyType)
                                                    {
                                                        eReligion = getFamilyReligion((FamilyType)pSubjectOther);
                                                    }
                                                    else if (pSubjectOther is Character)
                                                    {
                                                        eReligion = ((Character)pSubjectOther).getReligion();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (eTribe == TribeType.NONE)
                                    {
                                        if (ePlayer == PlayerType.NONE)
                                        {
                                            ePlayer = getPlayer();
                                        }

                                        if (eNation == NationType.NONE)
                                        {
                                            eNation = game().player(ePlayer).getNation();
                                        }
                                    }

                                    if (eFamily == FamilyType.NONE)
                                    {
                                        if (infos().character(eCharacter).meFamily != FamilyType.NONE)
                                        {
                                            eFamily = infos().character(eCharacter).meFamily;
                                        }
                                    }

                                    if (eReligion == ReligionType.NONE)
                                    {
                                        if (infos().character(eCharacter).meReligion != ReligionType.NONE)
                                        {
                                            eReligion = infos().character(eCharacter).meReligion;
                                        }
                                    }

                                    Character pCharacter = null;

                                    if (infos().character(eCharacter).mbSuitorTemp)
                                    {
                                        int iMarrySubject = infos().eventStory(eEventStory).miMarryTempSubject;
                                        if ((iMarrySubject >= 0) && (iMarrySubject < lSubjects.Count))
                                        {
                                            object pSubject = lSubjects[iMarrySubject];
                                            if (pSubject is Character)
                                            {
                                                #region Modified Code
                                                FreeLoveCharacter pMarryCharacter = ((FreeLoveCharacter)pSubject);

                                                GenderType eGender = pMarryCharacter.GetSuitorGender ();
                                                #endregion

                                                int iAge = pMarryCharacter.randomSuitorAge(true);
                                                if (iAge >= 0)
                                                {
                                                    pCharacter = game().createNewCharacter(iAge, ePlayer, eGender, NameType.NONE, eFamily, eTribe, eNation, szNamesAdded);

                                                    if (pCharacter != null)
                                                    {
                                                        if (eReligion != ReligionType.NONE)
                                                        {
                                                            pCharacter.setReligion(eReligion);
                                                        }

                                                        game().addCharacterTraits(eCharacter, pCharacter);

                                                        #region Added Code
                                                        var orientation = pMarryCharacter.GetRequiredSuitorOrientation ();

                                                        if (orientation != TraitType.NONE) {
                                                            pCharacter.addTrait (orientation);
                                                            seTraitAdded.Add (orientation);
                                                        }
                                                        #endregion

                                                        pCharacter.fillValues(infos().character(eCharacter).miRating, TraitType.NONE, seTraitAdded);

                                                        pCharacter.setTemporary(true);

                                                        szNamesAdded.Add(pCharacter.getFirstName());
                                                        foreach (TraitType eLoopTrait in pCharacter.getTraits())
                                                        {
                                                            seTraitAdded.Add(eLoopTrait);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                        
                                    else if (infos().character(eCharacter).meCourtier != CourtierType.NONE)
                                    {
                                        CourtierType eCourtier = infos().character(eCharacter).meCourtier;
                                        GenderType eGender = infos().character(eCharacter).meGender;
                                        int iAge = infos().Globals.COURTIER_AGE + game().randomNext(8);

                                        pCharacter = game().createNewCharacter(iAge, ePlayer, eGender, infos().character(eCharacter).meFirstName, eFamily, eTribe, eNation, szNamesAdded);

                                        if (pCharacter != null)
                                        {
                                            if (eReligion != ReligionType.NONE)
                                            {
                                                pCharacter.setReligion(eReligion);
                                            }

                                            game().addCharacterTraits(eCharacter, pCharacter);
                                            pCharacter.generateRatingsCourtier(eCourtier);

                                            pCharacter.setTemporary(true);

                                            szNamesAdded.Add(pCharacter.getFirstName());
                                            foreach (TraitType eLoopTrait in pCharacter.getTraits())
                                            {
                                                seTraitAdded.Add(eLoopTrait);
                                            }
                                        }
                                    }

                                    if (pCharacter == null)
                                    {
                                        pCharacter = game().createPresetCharacter(eCharacter, ePlayer, eFamily, eTribe, eNation, eReligion);
                                        pCharacter.setTemporary(true);
                                    }

                                    lSubjects[iI] = pCharacter;

                                    subjectString = subjectToString(pCharacter);
                                }

                                if (subjectString != "")
                                {
                                    pDecisionData.setSubject(iI, subjectString);
                                }
                                else
                                {
                                    MohawkAssert.Assert(false, "unexpected subject type: " + pLoopSubject.GetType());
                                }
                            }
                        }

                        using (TextBuilder textBuilder = TextBuilder.GetTextBuilder(TextManager))
                        using (new TextManager.LanguageSwitchScoped(TextManager, this))
                        {
                            for (int iI = 0; iI < infos().eventStory(eEventStory).maeSubjects.Count; iI++)
                            {
                                if (iI < infos().eventStory(eEventStory).maeBonuses.Count)
                                {
                                    BonusType eBonus = infos().eventStory(eEventStory).maeBonuses[iI];

                                    if (eBonus != BonusType.NONE)
                                    {
                                        SubjectClassType eLoopSubjectClass = infos().subject(infos().eventStory(eEventStory).maeSubjects[iI]).meClass;

                                        if (eLoopSubjectClass == infos().Globals.PLAYER_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eOtherPlayer: (PlayerType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.TRIBE_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eTribe: (TribeType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.RELIGION_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eReligion: (ReligionType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.FAMILY_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eFamily: (FamilyType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.TECH_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eTech: (TechType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.LAW_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eLaw: (LawType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.THEOLOGY_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eTheology: (TheologyType) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.RESOURCE_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eResource: (ResourceType)lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.GOAL_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eGoal: (GoalType)lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.OCCURRENCE_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eOccurrence: (OccurrenceType)lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.TRAIT_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, eTrait: (TraitType)lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.CHARACTER_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, pCharacter: (Character) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.CITY_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, pCity: (City) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.UNIT_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, pUnit: (Unit) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                        else if (eLoopSubjectClass == infos().Globals.TILE_SUBJECTCLASS)
                                        {
                                            doBonus(eBonus, textBuilder, pTile: (Tile) lSubjects[iI], lSubjectsPrevious: ((iI > 0) ? lSubjects.GetRange(0, iI) : null));
                                        }
                                    }
                                }
                            }

                            pDecisionData.setBonus(textBuilder.ProfiledToString());

                            if (textBuilder.HasContent)
                            {
                                TextVariable eventName = HelpText.fillEventStringVariable(infos().eventStory(eEventStory).meName, pDecisionData, game(), this);
                                Tile pTile = getEventLookAtTile(eEventStory, pDecisionData);

                                if (infos().eventStory(eEventStory).meName != TextType.NONE)
                                {
                                    pushLogData(() => TextManager.TEXT("TEXT_HELPTEXT_CONCAT_SPACE_TWO", HelpText.TEXTVAR_TYPE("TEXT_GAME_DO_BONUS_LOG_DATA", eventName), textBuilder.ToTextVariable()), GameLogType.DO_BONUS, ((pTile != null) ? pTile.getID() : -1));
                                }
                                else
                                {
                                    pushLogData(() => textBuilder.ProfiledToString(), GameLogType.DO_BONUS, ((pTile != null) ? pTile.getID() : -1));
                                }
                            }
                        }

                        using (var optionHashScoped = CollectionCache.GetHashSetScoped<EventOptionType>())
                        {
                            HashSet<EventOptionType> seOptionsAdded = optionHashScoped.Value;
                            int iIndex = 0;

                            foreach (EventOptionType eLoopEventOption in infos().eventStory(eEventStory).maeOptions)
                            {
                                EventOptionType eFinalEventOption = EventOptionType.NONE;

                                if (infos().eventOption(eLoopEventOption).maiEventOptionProb.Sum() > 0)
                                {
                                    eFinalEventOption = findBestEventOptionProb(eEventStory, eLoopEventOption, ulSeed, lSubjects, seOptionsAdded);
                                }
                                else
                                {
                                    if ((infos().eventOption(eLoopEventOption).mbHideInvalid) ? isValidEventOptionSubjects(eEventStory, eLoopEventOption, lSubjects, true, false) : true)
                                    {
                                        eFinalEventOption = eLoopEventOption;
                                    }
                                }

                                if (eFinalEventOption != EventOptionType.NONE)
                                {
                                    pDecisionData.setEventOption(iIndex, eFinalEventOption);
                                    iIndex++;
                                }
                            }
                        }

                        if (!(infos().eventStory(eEventStory).mbHidden))
                        {
                            pushDecisionDataNext(pDecisionData);

                            if (!isAIAutoPlay())
                            {
                                for (int i = 0; i < pDecisionData.getNumEventOptions(); i++)
                                {
                                    InfoEventOption optionInfo = infos().eventOption(pDecisionData.getEventOption(i));
                                    foreach (BonusType eLoopBonus in optionInfo.maeBonuses)
                                    {
                                        if (eLoopBonus == BonusType.NONE)
                                        {
                                            continue;
                                        }
                                        GoalType eAmbition = infos().bonus(eLoopBonus).meAmbition;
                                        if (eAmbition == GoalType.NONE)
                                        {
                                            eAmbition = infos().bonus(eLoopBonus).meAmbitionFamily;
                                        }
                                        if (eAmbition == GoalType.NONE)
                                        {
                                            eAmbition = infos().bonus(eLoopBonus).meAmbitionReligion;
                                        }

                                        if (eAmbition != GoalType.NONE)
                                        {
                                            Dictionary<string, object> dEventData = new Dictionary<string, object>();
                                            dEventData.Add("ambition", infos().goal(eAmbition).mzType);
                                            dEventData.Add("source", pDecisionData.getEventStoryType());
                                            dEventData.Add("ambition_subject", false);
                                            game().sendAnalytics(AnalyticsEventType.GOAL_AMBITION_OFFERED, dEventData, this);
                                        }

                                        if (infos().bonus(eLoopBonus).mbAddAmbition)
                                        {
                                            int subjectIndex = optionInfo.maeBonuses.IndexOf(eLoopBonus);
                                            eAmbition = infos().getType<GoalType>(pDecisionData.getSubject(subjectIndex));
                                            if (eAmbition != GoalType.NONE)
                                            {
                                                Dictionary<string, object> dEventData = new Dictionary<string, object>();
                                                dEventData.Add("ambition", infos().goal(eAmbition).mzType);
                                                dEventData.Add("source", pDecisionData.getEventStoryType());
                                                dEventData.Add("ambition_subject", true);
                                                game().sendAnalytics(AnalyticsEventType.GOAL_AMBITION_OFFERED, dEventData, this);
                                            }
                                        }
                                    }
                                }
                            }

                            if (bModal && isAIAutoPlay())
                            {
                                if (AI.doDecision(pDecisionData, false))
                                {
                                    AI.updateImprovementValues();
                                    AI.updateTechValues(); // in case techs were gained by the decision
                                }
                            }

                            pushLogData(() => HelpText.fillEventStringVariable(infos().eventStory(eEventStory).meName, pDecisionData, game(), this, false).ToString(TextManager), GameLogType.EVENT_TRIGGER);
                        }
                        else
                        {
                            if (!isAIAutoPlay())
                            {
                                game().sendAnalytics(AnalyticsEventType.HIDDEN_EVENT, "event", infos().eventStory(eEventStory).mzType);
                            }
                        }

                        setAllEventStoryTurn(eEventStory, game().getTurn());
                        clearEventStoryTested(eEventStory);
                        if (infos().eventStory(eEventStory).miRepeatTurns == -1)
                        {
                            mzTriggerValidEventStories.removeEventStory(infos().eventStory(eEventStory).meTrigger, eEventStory);
                        }

                        {
                            EventClassType eEventClass = infos().eventStory(eEventStory).meClass;

                            if (eEventClass != EventClassType.NONE)
                            {
                                setEventClassTurn(eEventClass, game().getTurn());
                            }
                        }

                        {
                            foreach (object pLoopSubject in lSubjects)
                            {
                                if (pLoopSubject != null)
                                {
                                    if (pLoopSubject is PlayerType)
                                    {
                                        PlayerType ePlayer = (PlayerType)pLoopSubject;

                                        setPlayerEventStoryTurn(ePlayer, eEventStory, game().getTurn());
                                    }
                                    else if (pLoopSubject is TribeType)
                                    {
                                        TribeType eTribe = (TribeType)pLoopSubject;

                                        setTribeEventStoryTurn(eTribe, eEventStory, game().getTurn());
                                    }
                                    else if (pLoopSubject is ReligionType)
                                    {
                                        ReligionType eReligion = (ReligionType)pLoopSubject;

                                        setReligionEventStoryTurn(eReligion, eEventStory, game().getTurn());
                                    }
                                    else if (pLoopSubject is FamilyType)
                                    {
                                        FamilyType eFamily = (FamilyType)pLoopSubject;

                                        setFamilyEventStoryTurn(eFamily, eEventStory, game().getTurn());
                                    }
                                    else if (pLoopSubject is TechType)
                                    {
                                        // not needed
                                    }
                                    else if (pLoopSubject is LawType)
                                    {
                                        // not needed
                                    }
                                    else if (pLoopSubject is TheologyType)
                                    {
                                        // not needed
                                    }
                                    else if (pLoopSubject is ResourceType)
                                    {
                                        // not needed
                                    }
                                    else if (pLoopSubject is GoalType)
                                    {
                                        // not needed
                                    }
                                    else if (pLoopSubject is OccurrenceType)
                                    {
                                        // not needed
                                    }
                                    else if (pLoopSubject is Character)
                                    {
                                        Character pCharacter = (Character)pLoopSubject;

                                        pCharacter.setEventStoryTurn(eEventStory, game().getTurn());
                                        if (infos().eventStory(eEventStory).meName != TextType.NONE)
                                        {
                                            if (pCharacter.hasPlayer())
                                            {
                                                using (new TextManager.LanguageSwitchScoped(TextManager, pCharacter.player()))
                                                using (var builderScoped = CollectionCache.GetStringBuilderScoped())
                                                {
                                                    HelpText.fillEventStringVariable(builderScoped.Value, infos().eventStory(eEventStory).meName, pDecisionData, game(), this);
                                                    pCharacter.addEventStoryText(game().getTurn(), builderScoped.Value.ToString());
                                                }
                                            }
                                        }
                                    }
                                    else if (pLoopSubject is City)
                                    {
                                        City pCity = (City)pLoopSubject;

                                        pCity.setEventStoryTurn(eEventStory, game().getTurn());
                                    }
                                    else if (pLoopSubject is Unit)
                                    {
                                        Unit pUnit = (Unit)pLoopSubject;

                                        pUnit.setEventStoryTurn(eEventStory, game().getTurn());
                                    }
                                    else if (pLoopSubject is Tile)
                                    {
                                        // not needed
                                    }
                                }
                            }
                        }

                        if ((eEventTrigger == infos().Globals.TRIBE_PEACE_OFFER_EVENTTRIGGER) ||
                            (eEventTrigger == infos().Globals.TRIBE_TRUCE_OFFER_EVENTTRIGGER) ||
                            (eEventTrigger == infos().Globals.TRIBE_WAR_OFFER_EVENTTRIGGER))
                        {
                            int iIndex = infos().eventStory(eEventStory).maiTriggerSubjects.Count > 0 ? infos().eventStory(eEventStory).maiTriggerSubjects[0] : -1;
                            if (iIndex < lSubjects.Count)
                            {
                                MohawkAssert.Assert(iIndex != -1, "Tribe diplomacy event ignores tribe trigger subject");
                                object pSubject = lSubjects[iIndex];

                                if (pSubject is TribeType)
                                {
                                    addMemory(infos().Globals.TRIBE_OFFER_MEMORY, eTribe: ((TribeType)pSubject));
                                    addMemoryTribeOfferAll();
                                }
                            }
                        }
                        else if ((eEventTrigger == infos().Globals.PLAYER_PEACE_OFFER_EVENTTRIGGER) ||
                                 (eEventTrigger == infos().Globals.PLAYER_TRUCE_OFFER_EVENTTRIGGER) ||
                                 (eEventTrigger == infos().Globals.PLAYER_WAR_OFFER_EVENTTRIGGER) ||
                                 (eEventTrigger == infos().Globals.PLAYER_WAR_DECLARE_EVENTTRIGGER) ||
                                 (eEventTrigger == infos().Globals.PLAYER_DIPLOMACY_OFFER_EVENTTRIGGER))
                        {
                            int iIndex = infos().eventStory(eEventStory).maiTriggerSubjects.Count > 0 ? infos().eventStory(eEventStory).maiTriggerSubjects[0] : -1;
                            if (iIndex < lSubjects.Count)
                            {
                                MohawkAssert.Assert(iIndex != -1, "Player diplomacy event ignores player trigger subject");
                                object pSubject = lSubjects[iIndex];

                                if (pSubject is PlayerType)
                                {
                                    addMemoryPlayer(infos().Globals.PLAYER_OFFER_MEMORY, ((PlayerType)pSubject));
                                    addMemoryPlayerOfferAll();
                                }
                            }
                        }
                    }

                    return true;
                }
                else
                {
                    MohawkAssert.Assert(false, eventStorySubjectValidation(eEventStory, lSubjects).ToString());
                    return false;
                }
            }
        }

        private StringBuilder eventStorySubjectValidation(EventStoryType eEventStory, List<object> lSubjects)
        {
            using (var scope = CollectionCache.GetStringBuilderScoped())
            {
                StringBuilder subjectError = scope.Value;
                InfoEventStory eventStory = infos().eventStory(eEventStory);
                subjectError.Append("EVENT FAIL\nEvent Story: ").Append(eventStory.mzType);

                //SUBJECTS
                subjectError.Append("\nSubjects Found :\n");
                if (lSubjects.Count == 0)
                {
                    subjectError.Append("NONE\n");
                }
                else
                {
                    int index = 0;
                    foreach (object pSubject in lSubjects)
                    {
                        subjectError.Append(index).Append(": ").Append(getValidationSubjectName(pSubject)).Append("\n");
                        index++;
                    }

                    if (index < eventStory.maeSubjects.Count)
                    {
                        subjectError.Append(index).Append(": ").Append("NOT FOUND").Append("\n");
                    }
                }

                subjectError.Append("Subjects Required :\n");
                if (eventStory.maeSubjects.Count == 0)
                {
                    subjectError.Append("NONE\n");
                }
                else
                {
                    int index = 0;
                    foreach (SubjectType sub in eventStory.maeSubjects)
                    {
                        InfoSubject infoSub = infos().subject(sub);
                        if (infoSub != null)
                            subjectError.Append(index).Append(":").Append(infoSub.mzType).Append("\n");
                        index++;
                    }
                }

                //SUBJECT EXTRAS
                subjectError.Append("Subject Extras :\n");
                if (eventStory.mlpSubjectExtras.Count == 0)
                {
                    subjectError.Append("NONE\n");
                }
                else
                {
                    foreach ((int, SubjectType) pairStruct in eventStory.mlpSubjectExtras)
                    {
                        subjectError.Append(pairStruct.Item1).Append(":");
                        InfoSubject infoSub = infos().subject(pairStruct.Item2);
                            if (infoSub != null)
                                subjectError.Append(infoSub.mzType).Append("\n");
                        }
                    }

                //SUBJECT NOT EXTRAS
                subjectError.Append("Subject Not Extras :\n");
                if (eventStory.mlpSubjectNotExtras.Count == 0)
                {
                    subjectError.Append("NONE\n");
                }
                else
                {
                    foreach ((int, SubjectType) pairStruct in eventStory.mlpSubjectNotExtras)
                    {
                            subjectError.Append(pairStruct.Item1).Append(":");
                            InfoSubject infoSub = infos().subject(pairStruct.Item2);
                            if (infoSub != null)
                                subjectError.Append(infoSub.mzType).Append("\n");
                        }
                    }

                //SUBJECT ANYS
                subjectError.Append("Subject Any :\n");
                if (eventStory.mlpSubjectAny.Count == 0)
                {
                    subjectError.Append("NONE\n");
                }
                else
                {
                    foreach ((int, SubjectType) pairStruct in eventStory.mlpSubjectAny)
                    {
                        subjectError.Append(pairStruct.Item1).Append(":");
                        InfoSubject infoSub = infos().subject(pairStruct.Item2);
                            if (infoSub != null)
                                subjectError.Append(infoSub.mzType).Append("\n");
                        }
                    }

                //Repeat TURNS
                subjectError.Append("Repeat Turns: ").Append(eventStory.miRepeatTurns);
                subjectError.Append("\nis EventStory Turn Valid : ").Append(isAllEventStoryTurnValid(eEventStory));

                //Law PreReq
                subjectError.Append("\nLaw Pre: ").Append(eventStory.meLawPrereq);

                //Trigger
                EventTriggerType trigger = eventStory.meTrigger;
                subjectError.Append("\nTrigger: ").Append(trigger != EventTriggerType.NONE ? infos().eventTrigger(trigger).mzType : "NONE");
                return subjectError;
            }
        }
    }
    #endregion
}
