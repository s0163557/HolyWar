using HolyWar.Units;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace HolyWar.Fields
{
    public enum FieldType
    {
        BASE = 0,
        RIVER = 1,
        TOWN = 2
    }

    public enum FieldRange
    {
        Melee = 1,
        Ranged = 2,
        Artillery = 3,
    }

    public enum FieldPositionType
    {
        Attacker = 0,
        Defender = 1,
    }

    public class Field : MonoBehaviour
    {
        [SerializeField]
        protected GameObject mainField;
        [SerializeField]
        protected GameObject auxField;
        [SerializeField]
        protected GameObject redLine;
        [SerializeField]
        protected GameObject details;
        [SerializeField]
        protected List<Material> typeMats;
        [SerializeField]
        protected FieldType fieldType;

        [SerializeField]
        protected bool inAction;

        [SerializeField]
        protected float dX;
        [SerializeField]
        protected float maxX;
        [SerializeField]
        protected float mainY;
        [SerializeField]
        protected float auxY;


        [SerializeField]
        protected List<BaseUnit> mainUnits = new();
        [SerializeField]
        protected List<BaseUnit> auxUnits = new();

        private BattleManager _battleManager;

        private BattleManager BattleManager
        {
            get
            {
                if (_battleManager == null)
                {
                    _battleManager = FindAnyObjectByType<BattleManager>();
                }
                return _battleManager;
            }
        }


        public List<BaseUnit> MainUnits { get { return mainUnits; } }
        public List<BaseUnit> AuxUnits { get { return auxUnits; } }

        private List<BaseUnit> _killedUnits;

        //Ќе сериализуем эти типы, потому что они будут автоматически назначены в BattleManager при создании пол€.
        [NonSerialized]
        public FieldRange FieldRange;
        [NonSerialized]
        public FieldPositionType FieldPositionType;

        protected MeshRenderer mainRenderer;
        protected MeshRenderer auxRenderer;

        public void Start()
        {
            if (mainUnits == null)
                mainUnits = new List<BaseUnit>();
            if (auxUnits == null)
                auxUnits = new List<BaseUnit>();

            _killedUnits = new List<BaseUnit>();

            mainY = gameObject.transform.position.y + mainY;
            auxY = gameObject.transform.position.y + auxY;

            mainRenderer = mainField.GetComponent<MeshRenderer>();
            auxRenderer = auxField.GetComponent<MeshRenderer>();

            ChangeLocation(fieldType);
            PoseUnits();

            EventBus.Subscribe(EventBus.EventsEnum.BattleStart, BattleStartListener);
            EventBus.Subscribe(EventBus.EventsEnum.BattleEnd, BattleEndListener);
        }

        public void RecalcPoses()
        {
            if (mainUnits.Count > 0)
            {
                if (BattleManager.GetPositionOfPlayer(mainUnits[0].Owner) == FieldPositionType.Attacker)
                {
                    transform.Rotate(180f, 0f, 0f);
                    redLine.transform.position = new Vector3(redLine.transform.position.x,
                        redLine.transform.position.y, -redLine.transform.position.z);

                    float tf = mainY;
                    mainY = auxY;
                    auxY = tf;
                }
            }
        }

        public void RemoveKilledUnit(BaseUnit sender)
        {
            UnRegisterUnitToMain(sender);
            RegisterUnitToKilled(sender);
        }

        public void BattleStartListener()
        {
            redLine.SetActive(false);

            foreach (BaseUnit unit in mainUnits)
            {
                //ƒобавим прослушку на юнитов, наход€щихс€ на поле бо€.
                unit.OnKilled += RemoveKilledUnit;
            }
        }

        public void BattleEndListener()
        {
            redLine.SetActive(true);

            foreach (BaseUnit unit in mainUnits)
            {
                //”бираем прослушку с юнитов
                unit.OnKilled -= RemoveKilledUnit;
            }

            for (int i = 0; i < _killedUnits.Count; i++) //перекачиваем убитых юнитов в главный лист
            {
                if (!mainUnits.Contains(_killedUnits[i])) //костыльно разбираемс€ с зомби
                {
                    mainUnits.Add(_killedUnits[i]);
                }
            }
            _killedUnits.Clear(); //очищаем лист убитых
        }

        public void ChangeLocation(FieldType fType)
        {
            fieldType = fType;
            mainRenderer.material = typeMats[(int)fieldType];
            auxRenderer.material = typeMats[(int)fieldType];
            for (int i = 0; i < details.transform.childCount; i++)
                if (i == (int)fieldType)
                    details.transform.GetChild(i).gameObject.SetActive(true);
                else
                    details.transform.GetChild(i).gameObject.SetActive(false);
        }

        [ContextMenu("Pose Units")]
        public void PoseUnits()
        {
            //≈сли поле пустое - нам нечего пересчитывать и нужно просто закрывать метод
            if (mainUnits.Count == 0)
                return;

            mainUnits[0].XCoord = 0;
            mainUnits[0].transform.position = new Vector3(mainUnits[0].XCoord, mainY);

            int step = 1;
            int rep = 0;

            for (int i = 1; i < mainUnits.Count; i++)
            {
                if (mainUnits[i] != null)
                {
                    float cX = mainUnits[0].XCoord + step * dX;

                    if (cX == Mathf.Abs(mainUnits[i - 1].XCoord)) //дл€ реверса
                        cX -= 2 * step * dX;

                    if (Mathf.Abs(cX) == step * dX)
                    {
                        rep += 1;
                    }

                    if (rep == 2)
                    {
                        step += 1;
                        rep = 0;
                    }

                    mainUnits[i].XCoord = cX;
                    mainUnits[i].transform.position = new Vector3(mainUnits[i].XCoord, mainY);
                }
            }
        }

        public void RegisterUnitToMain(IEnumerable<BaseUnit> unit)
        {
            foreach (var u in unit)
            {
                RegisterUnitToMain(u);
            }
        }

        public void RegisterUnitToMain(BaseUnit unit)
        {
            unit.OnKilled += RemoveKilledUnit;
            mainUnits.Add(unit);

            if (mainUnits.Count == 1) //пересчет по первому внесенному в список юниту
                RecalcPoses();

            PoseUnits();
        }

        public void RegisterUnitToAux(BaseUnit unit)
        {
            auxUnits.Add(unit);
        }

        public void UnRegisterUnitToMain(BaseUnit unit)
        {
            unit.OnKilled -= RemoveKilledUnit;
            mainUnits.Remove(unit);
        }

        public void UnRegisterUnitToAux(BaseUnit unit)
        {
            auxUnits.Remove(unit);
        }

        public void RegisterUnitToKilled(BaseUnit unit)
        {
            _killedUnits.Add(unit);
        }

        private Material originalMaterial;
        [SerializeField]
        private Material acceptHighlightMaterial;
        [SerializeField]
        private Material declineHighlightMaterial;

        public void HighlightField(bool canPlace)
        {
            var renderer = mainField.GetComponent<MeshRenderer>();
            originalMaterial = renderer.material;
            renderer.material = canPlace ? acceptHighlightMaterial : declineHighlightMaterial;
        }

        public void RemoveHighlight()
        {
            var renderer = mainField.GetComponent<MeshRenderer>();
            renderer.material = originalMaterial;
        }

        public bool IsUnitPlaceable(BaseUnit baseUnit)
        {
            if (baseUnit.Stats.FieldAffilation != FieldRange)
                return false;

            return true;
        }
    }
}