using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace HolyWar.FloatText
{
    public class FloatingTextSystem:MonoBehaviour
    {
        protected List<FloatingText> pooling = new List<FloatingText>();
        public static FloatingTextSystem activeSystem;

        [SerializeField]
        protected FloatingText floatingRedTextPrefab;
        protected FloatingText floatingGreenTextPrefab;

        public enum TextColors
        { 
            Red = 1, Green = 2,
        };

        private Dictionary<TextColors, FloatingText> _colorTextDictioanry;

        public void Start()
        {
            activeSystem = this;

            _colorTextDictioanry = new Dictionary<TextColors, FloatingText>
            {
                {TextColors.Red, floatingRedTextPrefab },
                {TextColors.Green, floatingGreenTextPrefab },
            };
        }

        private void InstFloatingText(Vector3 position, string value, TextColors color)
        {
            var floatingTextPrefab = _colorTextDictioanry[color];
            GameObject instFloatText = Instantiate(floatingTextPrefab.gameObject);
            FloatingText instFloatComp = instFloatText.GetComponent<FloatingText>();    

            pooling.Add(instFloatComp);
            instFloatComp.Call(position, value);
        }

        public static void CreateFloatingText(Vector3 position, string value, TextColors color)
        {
            FloatingText tagetedText = null;
            for(int i = 0; i < activeSystem.pooling.Count; i++)
            {
                if (activeSystem.pooling[i].IsFree)
                {
                    tagetedText = activeSystem.pooling[i];
                    break;
                }
            }

            if (tagetedText == null)
            {
                activeSystem.InstFloatingText(position, value, color);
                Debug.Log("Pooling size: " + activeSystem.pooling.Count);
            }
            else
            {
                tagetedText.Call(position, value);
            }
        }
    }
}