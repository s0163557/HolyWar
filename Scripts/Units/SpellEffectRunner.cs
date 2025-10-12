using HolyWar.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Spells.Effects
{
    //Отдельный компонент, который вешается на юнита и отслеживает все эффекты, которые на него повесились.
    [RequireComponent(typeof(BaseUnit))]
    internal class SpellEffectRunner : MonoBehaviour
    {

        private void Start()
        {
            gameObject.GetComponent<BaseUnit>().OnKilled += StopAllEffects;
        }

        internal void RegisterSpell(List<SpellEffect> spellEffects, int spellPower, BaseUnit target)
        {
            foreach (SpellEffect effect in spellEffects)
            {
                StartCoroutine(effect.Effect(target, spellPower));
            }
        }

        private void StopAllEffects(BaseUnit target)
        {
            StopAllCoroutines();
            target.OnKilled -= StopAllEffects;
        }

    }
}
