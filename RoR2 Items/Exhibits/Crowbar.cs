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
using RoR2_Items.Exhibits;

namespace RoR2_Items.Exhibits
{
    public sealed class CrowbarDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(Crowbar);
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
                Order: 20,
                IsDebug: false,
                IsPooled: true,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.Anywhere,
                Owner: "",
                LosableType: ExhibitLosableType.Losable,
                Rarity: Rarity.Common,
                Value1: 75,
                Value2: 90,
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

    [EntityLogic(typeof(CrowbarDef))]
    public sealed class Crowbar : Item
    {
        protected override Type VoidItemType()
        {
            return null;
        }
        public int Value
        {
            get { return this.Stack * this.Value1; }
        }
        private float Ratio
        {
            get
            {
                return ((float)this.Value + 100f) / 100f;
            }
        }
        private bool AboveHPThreshold(Unit unit)
        {
            float percentHealth = (float)unit.Hp / unit.MaxHp;
            float threshold = this.Value2 / 100f;
            return percentHealth > threshold;
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
            if (args.DamageInfo.DamageType == DamageType.Attack && AboveHPThreshold(args.Target))
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
