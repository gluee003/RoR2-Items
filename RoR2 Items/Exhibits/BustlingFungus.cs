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
    public sealed class BustlingFungusDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(BustlingFungus);
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
                Value1: 15,
                Value2: 1,
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
                );

            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(BustlingFungusDef))]
    public sealed class BustlingFungus : Item
    {
        protected override Type VoidItemType()
        {
            return typeof(WeepingFungus);
        }
        public int Value
        {
            get { return this.Stack * this.Value1; }
        }
        protected override void OnEnterBattle()
        {
            base.Counter = this.Value2;
            base.HandleBattleEvent<UnitEventArgs>(base.Battle.Player.TurnStarted, new GameEventHandler<UnitEventArgs>(OnPlayerTurnStarted));
            base.ReactBattleEvent<UnitEventArgs>(base.Battle.Player.TurnEnded, new EventSequencedReactor<UnitEventArgs>(OnPlayerTurnEnded));
            base.HandleBattleEvent<CardUsingEventArgs>(base.Battle.CardUsed, new GameEventHandler<CardUsingEventArgs>(OnCardUsed));
        }
        private void OnPlayerTurnStarted(UnitEventArgs args)
        {
            if (Counter > 0)
            {
                base.Active = true;
            }
        }
        private IEnumerable<BattleAction> OnPlayerTurnEnded(UnitEventArgs args)
        {
            if (base.Active && base.Counter > 0)
            {
                yield return new HealAction(base.Owner, base.Owner, this.Value, HealType.Normal, 0.2f);
                base.Active = false;
                base.Blackout = true;
                base.Counter--;
            }
        }
        private void OnCardUsed(CardUsingEventArgs args)
        {
            base.Active = false;
        }
        protected override void OnLeaveBattle()
        {
            base.Blackout = false;
        }
    }
}
