using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using Mono.Cecil;
using RoR2_Items.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2_Items.BepinexPlugin;

namespace RoR2_Items.Status
{
    public sealed class VoidtouchedEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(VoidtouchedStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("VoidtouchedStatus.png", BepinexPlugin.embeddedSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 2,
                                Type: StatusEffectType.Special,
                                IsVerbose: false,
                                IsStackable: false,
                                StackActionTriggerLevel: null,
                                HasLevel: false,
                                LevelStackType: StackType.Add,
                                HasDuration: false,
                                DurationStackType: StackType.Add,
                                DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                                HasCount: false,
                                CountStackType: StackType.Keep,
                                LimitStackType: StackType.Keep,
                                ShowPlusByLimit: false,
                                Keywords: Keyword.None,
                                RelativeEffects: new List<string>() { "Firepower" },
                                ImageId: null,
                                VFX: "Default",
                                VFXloop: "Default",
                                SFX: "YuyukoDeath"
                    );
            return statusEffectConfig;
        }
    }
    [EntityLogic(typeof(VoidtouchedEffect))]
    public sealed class VoidtouchedStatus : StatusEffect
    {
        public int FPBuff = 2;
        public int HPBuffPercent = 25;
        protected override void OnAdded(Unit unit)
        {
            if (unit is EnemyUnit enemy)
            {
                base.React(PerformAction.Effect(enemy, "ShenlingPurple"));
                int Hp = (int)Math.Round(enemy.Hp * (100 + this.HPBuffPercent) / 100f);
                int MaxHp = (int)Math.Round(enemy.MaxHp * (100 + this.HPBuffPercent) / 100f);
                base.GameRun.SetEnemyHpAndMaxHp(Hp, MaxHp, enemy, false);
                base.React(new ApplyStatusEffectAction<Firepower>(enemy, FPBuff));
                base.ReactOwnerEvent<DieEventArgs>(enemy.Died, new EventSequencedReactor<DieEventArgs>(this.OnDied));
            }
        }
        private IEnumerable<BattleAction> OnDied(DieEventArgs args)
        {
            if (base.Battle.BattleShouldEnd)
            {
                yield break;
            }
            if (base.Battle.HandIsFull)
            {
                yield return new AddCardsToDrawZoneAction(Library.CreateCards<VoidInfestor>(1, false), DrawZoneTarget.Top, AddCardsType.Normal);
            }
            else
            {
                yield return new AddCardsToHandAction(Library.CreateCards<VoidInfestor>(1, false));
            }
        }
    }
}
