using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using RoR2_Items.Status;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2_Items.BepinexPlugin;

namespace RoR2_Items.Cards
{
    public sealed class VoidInfestorDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(VoidInfestor);
        }

        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(embeddedSource);
            imgs.AutoLoad(this, extension: ".png");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn.yaml");
            return loc;
        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
              Index: BepinexPlugin.sequenceTable.Next(typeof(CardConfig)),
              Id: "",
              Order: 10,
              AutoPerform: true,
              Perform: new string[0][],
              GunName: "Simple1",
              GunNameBurst: "Simple2",
              DebugLevel: 0,
              Revealable: false,
              IsPooled: false,
              FindInBattle: false,
              HideMesuem: false,
              IsUpgradable: false,
              Rarity: Rarity.Uncommon,
              Type: CardType.Status,
              TargetType: TargetType.Nobody,
              Colors: new List<ManaColor>() { },
              IsXCost: false,
              Cost: new ManaGroup() { Any = 3 },
              UpgradedCost: null,
              Kicker: null,
              UpgradedKicker: null,
              MoneyCost: null,
              Damage: null,
              UpgradedDamage: null,
              Block: null,
              UpgradedBlock: null,
              Shield: null,
              UpgradedShield: null,
              Value1: null,
              UpgradedValue1: null,
              Value2: null,
              UpgradedValue2: null,
              Mana: null,
              UpgradedMana: null,
              Scry: null,
              UpgradedScry: null,

              ToolPlayableTimes: null,

              Loyalty: null,
              UpgradedLoyalty: null,
              PassiveCost: null,
              UpgradedPassiveCost: null,
              ActiveCost: null,
              ActiveCost2: null,
              UpgradedActiveCost: null,
              UpgradedActiveCost2: null,
              UltimateCost: null,
              UpgradedUltimateCost: null,

              Keywords: Keyword.Exile,
              UpgradedKeywords: Keyword.None,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

              RelativeEffects: new List<string>() { "VoidtouchedStatus" },
              UpgradedRelativeEffects: new List<string>() { },
              RelativeCards: new List<string>() { },
              UpgradedRelativeCards: new List<string>() { },

              Owner: "",
              ImageId: "",
              UpgradeImageId: "",

              Unfinished: false,
              Illustrator: "",
              SubIllustrator: new List<string>() { }
           );


            return cardConfig;
        }
    }
    [EntityLogic(typeof(VoidInfestorDef))]
    public sealed class VoidInfestor : Card
    {
        public override bool RemoveFromBattleAfterPlay
        {
            get { return true; }
        }
        public override IEnumerable<BattleAction> OnTurnEndingInHand()
        {
            yield return new ApplyStatusEffectAction<VoidtouchedStatus>(base.Battle.RandomAliveEnemy);
            yield return new RemoveCardAction(this);
        }
        public override IEnumerable<BattleAction> OnExile(CardZone srcZone)
        {
            yield return new RemoveCardAction(this);
        }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}
