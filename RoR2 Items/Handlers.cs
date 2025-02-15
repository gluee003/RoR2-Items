using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Presentation;
using LBoLEntitySideloader.CustomHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HarmonyLib;
using LBoL.Presentation.UI.Widgets;
using LBoL.Core.Cards;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using LBoL.Base.Extensions;
using System.Reflection.Emit;
using LBoLEntitySideloader.CustomKeywords;
using System.Runtime.CompilerServices;
using LBoL.Base;
using LBoL.EntityLib.EnemyUnits.Character;
using LBoL.ConfigData;
using RoR2_Items.Exhibits;

namespace RoR2_Items
{
    internal static class Handlers
    {
        public static void RegisterHandlers()
        {
            CHandlerManager.RegisterGameEventHandler(gr => gr.StationEntering, OnEnterStation, GameEventPriority.ConfigDefault);
        }
        private static void OnEnterStation(StationEventArgs args)
        {
            GameRunController gr = GameMaster.Instance?.CurrentGameRun;

            foreach ((Type type, ExhibitConfig config) in Library.EnumerateExhibitTypes())
            {
                if (type.IsSubclassOf(typeof(RoR2Item)) && !(gr.ExhibitPool.Contains(type)))
                {
                    gr.ExhibitPool.Add(type);
                }
            }
        }
    }
}