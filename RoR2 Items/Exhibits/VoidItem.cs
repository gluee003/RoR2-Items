﻿using LBoL.ConfigData;
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
using System.Linq;

namespace RoR2_Items.Exhibits
{
    [ExhibitInfo(WeighterType = typeof(VoidItem.VoidWeighter))]
    public abstract class VoidItem : RoR2Item
    {
        protected abstract Type[] OriginalItemTypes();
        protected override void OnGain(PlayerUnit player)
        {
            base.OnGain(player);

            int stacks = 0;

            foreach (Type originalItemType in this.OriginalItemTypes())
            {
                foreach (Exhibit exhibit in player.Exhibits)
                {
                    if (exhibit is Item item && item.GetType() == originalItemType)
                    {
                        stacks += item.Stack;
                        base.GameRun.LoseExhibit(item, true, true);
                        break;
                    }
                }
            }
            Utils.GainExhibits(this.GetType(), stacks);
        }
        private class VoidWeighter : IExhibitWeighter
        {
            public float WeightFor(Type type, GameRunController gameRun)
            {
                return 0f;
            }
        }
    }
}

