using System;
using System.Collections.Generic;
using HolyWar.Diplomacy;
using HolyWar.Units;
using UnityEngine;
using UnityEngine.UIElements;
using static HolyWar.Units.Enums;

public class UnitIconGridController : MonoBehaviour
{
    [System.Serializable]
    public class UnitDefinition
    {
        public string unitName;
        public SpriteRenderer unitSprite;
        public GameObject unitPrefab;
        public BaseUnit unitBase;
    }

    private UIDocument UIDocument;
    public VisualTreeAsset unitIconElementAsset;
    public VisualTreeAsset unitIconRowAsset;
    public FactionBase Faction;

    [SerializeField]
    public Player bindPlayer;

    private Dictionary<Tier, List<UnitDefinition>> _tierUnits = new Dictionary<Tier, List<UnitDefinition>>();
    public void LoadUnits()
    {
        //Очистим текущих юнитов
        _tierUnits = new Dictionary<Tier, List<UnitDefinition>>();
        foreach (var tier in Enum.GetValues(typeof(Tier)))
            _tierUnits.Add((Tier)tier, new List<UnitDefinition>());

        //Распилим префабы на все интересующие нас части и рассортируем юнитов по тирам
        foreach (var unit in Faction.FactionUnitPrefabs)
        {
            UnitDefinition currentUnit = new UnitDefinition()
            {
                unitName = unit.name,
                unitSprite = unit.GetComponent<SpriteRenderer>(),
                unitPrefab = unit,
                unitBase = unit.GetComponent<BaseUnit>(),
            };
            _tierUnits[currentUnit.unitBase.Stats.Tier].Add(currentUnit);
        }
    }

    private void OnUnitIconClicked(UnitDefinition def)
    {
        EventBus.UnitSelection.RaiseStartEvent(def.unitPrefab);
    }

    public void GenerateUI()
    {
        var grid = UIDocument.rootVisualElement.Q<ScrollView>("unit-icon-grid");
        foreach (var key in _tierUnits.Keys)
        {
            var rowTemplateContainer = unitIconRowAsset.CloneTree();
            var rowContainer = rowTemplateContainer.Q<VisualElement>("unit-icon-row-container");

            var rowElement = rowContainer.Q<VisualElement>("unit-icon-row");

            var rowLabel = rowContainer.Q<Label>("unit-icon-row-label");
            rowLabel.text = $"Тир {key}";

            var currentTierUnits = _tierUnits[key];

            foreach (var unitDefintion in _tierUnits[key])
            {
                try
                {
                    var unitIconTemplateContainer = unitIconElementAsset.CloneTree();
                    var unitIconElement = unitIconTemplateContainer.Q<UnitIconElement>();
                    rowElement.Add(unitIconElement);
                    unitIconElement.Init(unitDefintion.unitSprite.sprite, unitDefintion.unitBase.UnitName, unitDefintion.unitBase.Stats.Cost);
                    unitIconElement.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        OnUnitIconClicked(unitDefintion);
                        evt.StopImmediatePropagation();
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            grid.Add(rowContainer);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIDocument = GetComponent<UIDocument>();
        LoadUnits();
        GenerateUI();
    }

    public void ChangeFaction(FactionBase newFaction)
    {
        Faction = newFaction;
        LoadUnits();
        GenerateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
