//using LBoL.Core.Units;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using HarmonyLib;

//namespace RoR2_Items
//{
//    [HarmonyPatch]
//    class PatchDeez
//    {
//        [HarmonyPrefix]
//        [HarmonyPatch(typeof(UltimateSkill), nameof(UltimateSkill.PowerCost), MethodType.Getter)]
//        static bool Prefix(ref int __result)
//        {
//            Console.WriteLine("deez nuts");
//            __result = 69;
//            return false;
//        }
//    }
//}
