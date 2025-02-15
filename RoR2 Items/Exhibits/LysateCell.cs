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
    public sealed class LysateCellDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(LysateCell);
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
                Rarity: Rarity.Uncommon,
                Value1: null,
                Value2: null,
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

    [EntityLogic(typeof(LysateCellDef))]
    public sealed class LysateCell : VoidItem
    {
        protected override Type[] OriginalItemTypes()
        {
            return new Type[] { typeof(FuelCell) };
        }
        public float Value
        {
            get
            {
                return this.Stack;
            }
        }
        protected override void OnEnterBattle()
        {
            base.Counter = this.Stack;
            base.ReactBattleEvent<CardEventArgs>(base.Battle.CardExiled, new EventSequencedReactor<CardEventArgs>(this.OnCardExiled));
        }
        protected override void OnLeaveBattle()
        {
            base.Counter = 0;
        }
        private IEnumerable<BattleAction> OnCardExiled(CardEventArgs args)
        {
            if (args.Card.CardType != CardType.Status && args.Card.CardType != CardType.Misfortune && base.Counter > 0)
            {
                yield return new MoveCardAction(args.Card, CardZone.Discard);
                Counter--;
            }
        }
    }
}
