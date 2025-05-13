using LBoL.Core;
using LBoL.Presentation;
using RoR2_Items.Exhibits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RoR2_Items
{
    public class Utils
    {
        private static IEnumerator GainExhibitCoroutine(Type type, int num)
        {
            for (int i = 0; i < num; i++)
            {
                GameMaster.DebugGainExhibit(Library.CreateExhibit(type));
                yield return new WaitForSeconds(0.2f);
            }
        }
        public static void GainExhibits(Type type, int num)
        {
            GameMaster.Instance.StartCoroutine(GainExhibitCoroutine(type, num));
        }
    }
}
