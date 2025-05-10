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
    public sealed class FuelCellDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FuelCell);
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
                Value1: 1,
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
                );

            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(FuelCellDef))]
    public sealed class FuelCell : Item
    {
        protected override Type VoidItemType()
        {
            return typeof(LysateCell);
        }
        public int Value
        {
            get { return this.Stack * this.Value1; }
        }
        protected override void OnGain(PlayerUnit player)
        {
            foreach (Card card in base.GameRun.BaseDeck)
            {
                if (card.DeckCounter != null)
                {
                    card.DeckCounter += 1;
                }
            }
            base.OnGain(player);
        }
        protected override void OnAdded(PlayerUnit player)
        {
            base.HandleGameRunEvent<CardsEventArgs>(base.GameRun.DeckCardsAdded, new GameEventHandler<CardsEventArgs>(this.OnAddCard));
        }
        protected override void OnEnterBattle()
        {
            base.HandleBattleEvent<CardsEventArgs>(base.Battle.CardsAddedToHand, new GameEventHandler<CardsEventArgs>(this.OnAddCard));
            base.HandleBattleEvent<CardsEventArgs>(base.Battle.CardsAddingToDiscard, new GameEventHandler<CardsEventArgs>(this.OnAddCard));
            base.HandleBattleEvent<CardsEventArgs>(base.Battle.CardsAddedToExile, new GameEventHandler<CardsEventArgs>(this.OnAddCard));
            base.HandleBattleEvent<CardsAddingToDrawZoneEventArgs>(base.Battle.CardsAddedToDrawZone, new GameEventHandler<CardsAddingToDrawZoneEventArgs>(this.OnCardsAddedToDrawZone));
        }
        private void OnAddCard(CardsEventArgs args)
        {
            foreach (Card card in args.Cards)
            {
                if (card.DeckCounter != null)
                {
                    base.NotifyActivating();
                    card.DeckCounter += Value;
                }
            }
        }
        private void OnCardsAddedToDrawZone(CardsAddingToDrawZoneEventArgs args)
        {
            foreach (Card card in args.Cards)
            {
                if (card.DeckCounter != null)
                {
                    base.NotifyActivating();
                    card.DeckCounter += Value;
                }
            }
        }
    }
}
