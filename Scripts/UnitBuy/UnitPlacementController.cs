using HolyWar.Diplomacy;
using HolyWar.Fields;
using HolyWar.Units;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class UnitPlacementController : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Camera _sceneCamera;
    [SerializeField] private LayerMask _fieldLayerMask;
    [SerializeField] private BattleManager _battleManager;

    [Header("Ghost Visuals")]
    [SerializeField] private Color _validColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color _invalidColor = new Color(1f, 0.4f, 0.4f, 0.6f);

    private GameObject _selectedPrefab;
    private GameObject _ghost;
    private Field _hoveredField;

    private void Start()
    {
        EventBus.UnitSelection.SubscribeToEvent((GameObject selectedUnitPrefab) => { BeginPlacement(selectedUnitPrefab); },
                                                (GameObject selectedUnitPrefab) => { EndPlacement(); });
    }

    private void BeginPlacement(GameObject unitPrefab)
    {
        //Если вдруг юнит перевыбран - удалим старые данные, чтоб не оставалась "мёртвых" призраков
        EndPlacement();
        _selectedPrefab = unitPrefab;
        CreateGhost(unitPrefab);
    }

    private void CreateGhost(GameObject unitPrefab)
    {
        _ghost = Instantiate(unitPrefab);
        _ghost.name = unitPrefab.name + "(Ghost)";
        _ghost.layer = LayerMask.NameToLayer("Ignore Raycast");

        //Выключим все ненужные компоненты нашему призраку
        foreach (var c in _ghost.GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        var baseUnitComponent = _ghost.GetComponent<BaseUnit>();
        if (baseUnitComponent)
            baseUnitComponent.enabled = false;

        SetGhostColor(_validColor);
    }

    private void SetGhostColor(Color color)
    {
        if (!_ghost)
            return;

        var sr = _ghost.GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.color = color;
    }

    private void Update()
    {
        if (_selectedPrefab == null)
            return;

        //Не нужно проверять положение юнитов, если они упёрлись в UI.
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        var mouse = _sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        if (_ghost)
            _ghost.transform.position = mouse;

        var field = GetFieldUnder(mouse);

        var baseUnit = _selectedPrefab.GetComponent<BaseUnit>();
        bool canPlace = false;
        if (baseUnit && field)
            canPlace = field.IsUnitPlacebale(baseUnit);

        SetGhostColor(canPlace ? _validColor : _invalidColor);

        if (field != _hoveredField)
        {
            if (_hoveredField)
                _hoveredField.RemoveHighlight();
            _hoveredField = field;
            if (_hoveredField)
                _hoveredField.HighlightField(canPlace);
        }

        if (Input.GetMouseButton(0) && canPlace)
        {
            PlaceOnField(_hoveredField);
        }

        if (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            //Вызываем ивент отмены
            EventBus.UnitSelection.RaiseEndEvent(_selectedPrefab);
        }
    }

    private Field GetFieldUnder(Vector3 worldPos)
    {
        var hit = Physics2D.OverlapPoint(worldPos, _fieldLayerMask);
        if (!hit)
            return null;
        return hit.GetComponent<Field>();
    }

    private void PlaceOnField(Field field)
    {
        var instantiatedUnit = Instantiate(_selectedPrefab);
        var unitComponent = instantiatedUnit.GetComponent<BaseUnit>();

        if (!unitComponent.Owner.TrySpend(unitComponent.Stats.Cost))
        {
            //Если денег не хватило - пробиваем отмену.
            Destroy(instantiatedUnit);
            return;
        }

        field.RegisterUnitToMain(unitComponent);
        field.PoseUnits();
        EventBus.UnitSelection.RaiseEndEvent(_selectedPrefab);
    }

    private void EndPlacement()
    {
        if (_hoveredField)
        {
            _hoveredField.RemoveHighlight();
            _hoveredField = null;
        }

        if (_ghost)
            Destroy(_ghost);
        _ghost = null;
        _selectedPrefab = null;
    }
}

