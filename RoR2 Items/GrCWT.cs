using HarmonyLib;
using LBoL.Core;
using LBoL.Presentation;
using LBoLEntitySideloader.PersistentValues;
using RoR2_Items.Exhibits;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoR2_Items
{
    internal class GrCWT
    {

        static ConditionalWeakTable<GameRunController, GrState> cwt_grState = new ConditionalWeakTable<GameRunController, GrState>();

        static WeakReference<GameRunController> gr_ref;

        [MaybeNull]
        public static GameRunController GR
        {
            get
            {
                var rez = GameMaster.Instance?.CurrentGameRun;
                if (rez == null)
                    gr_ref.TryGetTarget(out rez);
                return rez;
            }
        }

        internal static GrState GetGrState(GameRunController gr) => cwt_grState.GetOrCreateValue(gr);


        [HarmonyPatch(typeof(GameRunController), MethodType.Constructor)]
        [HarmonyPriority(Priority.High)]
        class GameRunController_Patch
        {
            static void Prefix(GameRunController __instance)
            {
                GetGrState(__instance);
                gr_ref = new WeakReference<GameRunController>(__instance);
            }
        }
    }
    public class GrState{}
    public class GrStateContainer : CustomGameRunSaveData
    {
        public Dictionary<string, int> dict = new Dictionary<string, int>();
        public override void Restore(GameRunController gameRun)
        {
            foreach ((string type, int stack) in this.dict)
            {
                foreach (Exhibit exhibit in gameRun.Player.Exhibits)
                {
                    if (exhibit is RoR2Item ror2Item && ror2Item.GetType().ToString().Equals(type))
                    {
                        ror2Item.Stack = this.dict.GetValueOrDefault(type);
                        break;
                    }
                }
            }
        }

        public override void Save(GameRunController gameRun)
        {
            foreach (Exhibit exhibit in gameRun.Player.Exhibits)
            {
                if (exhibit is RoR2Item ror2Item)
                {
                    this.dict[ror2Item.GetType().ToString()] = ror2Item.Stack;
                }
            }
        }
    }
}