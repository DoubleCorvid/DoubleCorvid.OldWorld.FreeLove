using System.Collections.Generic;
using HarmonyLib;

namespace DoubleCorvid.OldWorld.ExpandedMarriage {
    public static class MarriagePatcher {
        public static IEnumerable<CodeInstruction> Transpile_DoMarriage_SuitorGender_Patch (IEnumerable<CodeInstruction> instructions) {
            var codeMatcher = new CodeMatcher (instructions);

            return codeMatcher.Instructions ();
        }

        public static IEnumerable<CodeInstruction> Transpile_CanMarryOne_SuitorGender_Patch (IEnumerable<CodeInstruction> instructions) {
            var codeMatcher = new CodeMatcher (instructions);

            return codeMatcher.Instructions ();
        }

        public static IEnumerable<CodeInstruction> Transpile_CanMarryTwo_SuitorGender_Patch (IEnumerable<CodeInstruction> instructions) {
            var codeMatcher = new CodeMatcher (instructions);

            return codeMatcher.Instructions ();
        }
    }
}
