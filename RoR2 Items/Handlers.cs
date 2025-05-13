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
using RoR2_Items.Status;
using LBoL.Core.Stations;
using LBoL.Core.Randoms;

namespace RoR2_Items
{
    internal static class Handlers
    {
        public static float VoidEncounterProbability { get; set; }
        private static float InitialProbability = 0.15f;
        private static float IncrementAmount = 0.1f;
        private static void ResetProbability()
        {
            VoidEncounterProbability = InitialProbability;
        }
        private static void IncreaseProbability()
        {
            VoidEncounterProbability = Math.Min(VoidEncounterProbability + IncrementAmount, 1f);
        }
        public static void RegisterHandlers()
        {
            CHandlerManager.RegisterGameEventHandler(gr => gr.StationEntering, OnEnterStation, GameEventPriority.ConfigDefault);
            CHandlerManager.RegisterBattleEventHandler(battle => battle.BattleStarting, OnBattleStarting,
                battle => battle.EnemyGroup.EnemyType != EnemyType.Elite && battle.EnemyGroup.EnemyType != EnemyType.Boss, GameEventPriority.ConfigDefault);
            CHandlerManager.RegisterGameEventHandler(gr => gr.StageEntered, OnEnterStage, GameEventPriority.ConfigDefault);
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
        private static void OnBattleStarting(GameEventArgs args)
        {
            GameRunController gr = GameMaster.Instance?.CurrentGameRun;

            // if void encounter roll fails, increase probability and do nothing
            float roll = gr.AdventureRng.NextFloat();
            // Debug.Log($"probability: {VoidEncounterProbability}, roll: {roll}");
            if (roll > VoidEncounterProbability)
            {
                IncreaseProbability();
                return;
            } else
            {
                ResetProbability();
            }

            BattleController battle = GameMaster.Instance?.CurrentGameRun.Battle;
            battle.React(new ApplyStatusEffectAction<VoidtouchedStatus>(battle.RandomAliveEnemy), null, ActionCause.None);

            IEnumerable<Type> voidTypes = Library.EnumerateExhibitTypes()
                .Select(tuple => tuple.exhibitType)
                .Where(type => type.IsSubclassOf(typeof(VoidItem)));

            RarityWeightTable rarityTable = new RarityWeightTable(0.5f, 0.33f, 0.17f, 0f);
            RepeatableRandomPool<Type> repeatableRandomPool = new RepeatableRandomPool<Type>();

            foreach (Type type in voidTypes)
            {
                ExhibitConfig config = ExhibitConfig.FromId(type.Name);
                float weight = rarityTable.WeightFor(config.Rarity);
                repeatableRandomPool.Add(type, weight);
            }

            Type voidType = repeatableRandomPool.SampleOrDefault(gr.StationRng);
            if (voidType != null)
            {
                gr.CurrentStation.AddReward(StationReward.CreateExhibit(Library.CreateExhibit(voidType)));
            }
        }
        private static void OnEnterStage(GameEventArgs args)
        {
            // base probability of 15%
            GameRunController gr = GameMaster.Instance?.CurrentGameRun;
            if (gr.CurrentStage.Level == 1)
            {
                ResetProbability();
            }
        }
    }
}