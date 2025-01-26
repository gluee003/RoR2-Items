using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2_Items.BepinexPlugin;
using UnityEngine;
using LBoL.Core;
using LBoL.Base;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Base.Extensions;
using System.Collections;
using LBoL.EntityLib.Cards.Neutral.Blue;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.Exhibits;
using LBoL.Core.Units;
using LBoL.Core.StatusEffects;
using Mono.Cecil;
using Ruina_Mod.Exhibits;

namespace RoR2_Items.Exhibits
{
    public sealed class APRoundsDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(APRounds);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "ExhibitsEn.yaml");
            return loc;
        }

        public override ExhibitSprites LoadSprite()
        {
            var folder = "Resources.";
            var exhibitSprites = new ExhibitSprites();
            Func<string, Sprite> wrap = (s) => ResourceLoader.LoadSprite((folder + GetId() + s + ".png"), embeddedSource);

            exhibitSprites.main = wrap("");

            return exhibitSprites;
        }

        public override ExhibitConfig MakeConfig()
        {
            var exhibitConfig = new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: true,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.Anywhere,
                Owner: "",
                LosableType: ExhibitLosableType.Losable,
                Rarity: Rarity.Common,
                Value1: 25,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: null,
                BaseManaAmount: 1,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                )
            {

            };
            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(APRoundsDef))]
    public sealed class APRounds : RoR2Item
    {
        private float Ratio
        {
            get
            {
                return ((float)this.Value1 + 100f) / 100f;
            }
        }
        protected override void OnEnterBattle()
        {
            foreach (EnemyUnit enemyUnit in Battle.AllAliveEnemies)
            {
                base.HandleBattleEvent<DamageEventArgs>(enemyUnit.DamageReceiving, new GameEventHandler<DamageEventArgs>(this.OnEnemyDamageReceiving));
            }
            base.HandleBattleEvent<UnitEventArgs>(base.Battle.EnemySpawned, new GameEventHandler<UnitEventArgs>(this.OnEnemySpawned));
        }
        private void OnEnemySpawned(UnitEventArgs args)
        {
            base.HandleBattleEvent<DamageEventArgs>(args.Unit.DamageReceiving, new GameEventHandler<DamageEventArgs>(this.OnEnemyDamageReceiving));
        }
        private void OnEnemyDamageReceiving(DamageEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack && args.Target is EnemyUnit enemyUnit
                && (enemyUnit.Config.Type == EnemyType.Elite || enemyUnit.Config.Type == EnemyType.Boss))
            {
                args.DamageInfo = args.DamageInfo.MultiplyBy(this.Ratio);
                args.AddModifier(this);
                if (args.Cause != ActionCause.OnlyCalculate)
                {
                    base.NotifyActivating();
                }
            }
        }
    }
}
