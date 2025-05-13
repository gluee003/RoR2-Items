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
using LBoL.Presentation;
using static UnityEngine.TouchScreenKeyboard;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace RoR2_Items.Exhibits
{
    public sealed class DelicateWatchDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(DelicateWatch);
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
                Value1: 20,
                Value2: 25,
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
                );

            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(DelicateWatchDef))]
    public sealed class DelicateWatch : Item
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
        private float HPThreshold
        {
            get
            {
                return (float)this.Value2 / 100f;
            }
        }
        private float HPPercentage
        {
            get
            {
                return base.Owner.Hp / (float)base.Owner.MaxHp;
            }
        }
        protected override void OnAdded(PlayerUnit player)
        {
            base.HandleGameRunEvent<DamageEventArgs>(player.DamageReceived, new GameEventHandler<DamageEventArgs>(this.OnDamageReceived), GameEventPriority.Lowest);
        }
        protected override void OnEnterBattle()
        {
            base.HandleBattleEvent<DamageDealingEventArgs>(base.Owner.DamageDealing, new GameEventHandler<DamageDealingEventArgs>(this.OnDamageDealing));
        }
        private void OnDamageDealing(DamageDealingEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack)
            {
                args.DamageInfo = args.DamageInfo.MultiplyBy(this.Ratio);
                args.AddModifier(this);
                if (args.Cause != ActionCause.OnlyCalculate)
                {
                    base.NotifyActivating();
                }
            }
        }
        private void OnDamageReceived(DamageEventArgs args)
        {
            if (HPPercentage < HPThreshold)
            {
                args.CancelBy(this);
                int stacks = this.Stack;
                base.GameRun.LoseExhibit(this, true, true);
                Utils.GainExhibits(typeof(DelicateWatchBroken), stacks);
            }
        }
    }
}
