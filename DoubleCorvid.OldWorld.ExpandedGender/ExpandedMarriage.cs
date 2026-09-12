using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TenCrowns.AppCore;
using TenCrowns.GameCore;
using UnityEngine;

namespace DoubleCorvid.OldWorld.ExpandedMarriage {
    public class ExpandedMarriage : ModEntryPointAdapter {
        public const string HarmonyId = "DoubleCorvid.ExpandedMarriage.Patches";
        
        private Harmony _harmony;

        public override void Initialize (ModSettings modSettings) {
            try {
                _harmony = new Harmony (HarmonyId);

                ApplyPatches ();
            }
            catch (Exception e) {
                Debug.LogError ($"[ExpandedMarriage] Failed to apply harmony patches: {e}");
            }
        }

        private void ApplyPatches () {
            _harmony?.Patch (GetDoMarriageMethodInfo (), transpiler: new HarmonyMethod (GetDoMarriageSuitorGendorTranspiler ()));

            _harmony?.Patch (GetCanMarryCharacterMethodOneInfo (), transpiler: new HarmonyMethod (GetCanMarrySuitorGendorTranspilerOne ()));

            _harmony?.Patch (GetCanMarryCharacterMethodTwoInfo (), transpiler: new HarmonyMethod (GetCanMarrySuitorGendorTranspilerTwo ()));
        }

        private static MethodInfo GetDoMarriageSuitorGendorTranspiler () => 
            typeof (MarriagePatcher).GetMethod (nameof (MarriagePatcher.Transpile_DoMarriage_SuitorGender_Patch));

        private static MethodInfo GetCanMarrySuitorGendorTranspilerOne () => 
            typeof (MarriagePatcher).GetMethod (nameof (MarriagePatcher.Transpile_CanMarryOne_SuitorGender_Patch));

        private static MethodInfo GetCanMarrySuitorGendorTranspilerTwo () =>
            typeof (MarriagePatcher).GetMethod (nameof (MarriagePatcher.Transpile_CanMarryOne_SuitorGender_Patch));

        private MethodInfo GetDoMarriageMethodInfo () => 
            AccessTools.Method (typeof (Character), "doMarriage");

        private MethodInfo GetCanMarryCharacterMethodOneInfo () =>
            AccessTools.Method (typeof(Character), "canMarry", new Type[3] {
                typeof(Character),
                typeof(bool),
                typeof(bool)
            });

        private MethodInfo GetCanMarryCharacterMethodTwoInfo () =>
            AccessTools.Method (typeof(Character), "canMarry", new Type[3] {
                typeof(CharacterType),
                typeof(List<object>),
                typeof(bool)
            });
    }
}
