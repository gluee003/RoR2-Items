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
using System.Runtime.CompilerServices;
using LBoL.Presentation;

namespace RoR2_Items.Exhibits
{
    public abstract class Item : RoR2Item
    {
        protected abstract Type VoidItemType();
        protected override void OnGain(PlayerUnit player)
        {
            base.OnGain(player);
            foreach (Exhibit exhibit in player.Exhibits)
            {
                if (exhibit is VoidItem voidItem
                    && voidItem.GetType() == this.VoidItemType())
                {
                    base.GameRun.LoseExhibit(this, true, true);
                    voidItem.Stack++;
                    break;
                }
            }
        }
    }
}

