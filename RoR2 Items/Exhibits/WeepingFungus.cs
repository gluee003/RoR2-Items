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
using LBoLEntitySideloader.Utils;
using LBoL.EntityLib.Cards.Misfortune;

namespace RoR2_Items.Exhibits
{
    public sealed class WeepingFungusDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(WeepingFungus);
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
                Value1: 2,
                Value2: 3,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: null,
                BaseManaAmount: 1,
                HasCounter: true,
                InitialCounter: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                )
            {

            };
            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(WeepingFungusDef))]
    public sealed class WeepingFungus : VoidItem
    {
        protected override Type[] OriginalItemTypes()
        {
            return new Type[] { typeof(BustlingFungus) };
        }
        public int Value
        {
            get
            {
                return this.Value1 * this.Stack;
            }
        }
        protected override void OnEnterBattle()
        {
            base.Counter = 0;
            base.ReactBattleEvent<CardUsingEventArgs>(base.Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
        }
        protected override void OnLeaveBattle()
        {
            base.Counter = 0;
            base.Active = false;
        }
        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (base.Owner.IsInTurn && args.Card.CardType == CardType.Attack)
            {
                base.Counter = Math.Min(base.Counter + 1, this.Value2);
                if (base.Counter == this.Value2)
                {
                    base.Active = true;
                    base.NotifyActivating();
                    yield return new HealAction(base.Owner, base.Owner, this.Value, HealType.Normal, 0.2f);
                }
            }
            else
            {
                base.Counter = 0;
                base.Active = false;
                yield break;
            }
        }
    }
}
