using HolyWar.Fields;
using HolyWar.Units;
using HolyWar.Diplomacy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //����� ������� �� ������������� ����� �������, ��� ����� ���������. � ������ ��������� ����� ������, � �������� ��� ����� ������ ����������.

    [SerializeField]
    protected int DefenderPlayerNumber;
    [SerializeField]
    protected int AttackerPlayerNumber;

    [SerializeField]
    protected Field DefenderMeleeFieldScript;
    [SerializeField]
    protected Field DefenderRangedFieldScript;
    [SerializeField]
    protected Field DefenderArtilleryFieldScript;

    [SerializeField]
    protected Field AttackerMeleeFieldScript;
    [SerializeField]
    protected Field AttackerRangedFieldScript;
    [SerializeField]
    protected Field AttackerArtilleryFieldScript;

    protected List<Field> FieldsList;

    private Dictionary<(int, FieldRange), Field> fieldDictionary = new Dictionary<(int, FieldRange), Field>();
    protected List<int> numberOfUnits;

    private Player[] players;
    protected Player[] Players
    {
        get { return players == null ? players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID) : players; }
    }

    //��� ������������� bm ������ ����� ����������� �� ���� �������, � ��������� ������ �� 0 �����, � ���� ����� ��������� �� ������ ����������
    //������ ������� ������ ����� ����� ��������. ���� ��������� ������ ��-����������� � ������� ������, � ���� �� ������ ��� ��� ������������ ����. 
    private byte _currentOpponentNumber;
    public byte GetOpponentNumber()
    {
        return _currentOpponentNumber;
    }

    public void ChangeOpponents()
    { 
        
    }

    public void Awake()
    {
        //�� ������ ���� ��������� ������� � ���, ��� ��������� ���� ��������� - ������� � �������� ������������ ��.
        DefenderMeleeFieldScript.FieldRange = FieldRange.Melee;
        DefenderRangedFieldScript.FieldRange = FieldRange.Ranged;
        DefenderArtilleryFieldScript.FieldRange = FieldRange.Artillery;

        AttackerMeleeFieldScript.FieldRange = FieldRange.Melee;
        AttackerRangedFieldScript.FieldRange = FieldRange.Ranged;
        AttackerArtilleryFieldScript.FieldRange = FieldRange.Artillery;

        //������� ����� ��� ���� � ����� ������
        fieldDictionary.Add((DefenderPlayerNumber, FieldRange.Melee), DefenderMeleeFieldScript);
        fieldDictionary.Add((DefenderPlayerNumber, FieldRange.Ranged), DefenderRangedFieldScript);
        fieldDictionary.Add((DefenderPlayerNumber, FieldRange.Artillery), DefenderArtilleryFieldScript);

        fieldDictionary.Add((AttackerPlayerNumber, FieldRange.Melee), AttackerMeleeFieldScript);
        fieldDictionary.Add((AttackerPlayerNumber, FieldRange.Ranged), AttackerRangedFieldScript);
        fieldDictionary.Add((AttackerPlayerNumber, FieldRange.Artillery), AttackerArtilleryFieldScript);

        numberOfUnits = new List<int>();
    }

    public bool GetPlayerNumberByField(Field field, out int playerNumber)
    {
        foreach (var keyValuePair in fieldDictionary)
        {
            if (keyValuePair.Value == field)
            {
                playerNumber = keyValuePair.Key.Item1;
                return true;
            }
        }

        playerNumber = -1;
        return false;
    }

    public bool TryGetPlayerByNumber(int number, out Player player)
    {
        //������ ����� ��������
        if (number > Players.Length || number < 0)
        {
            player = null;
            return false;
        }

        player = Players[number];
        return true;
    }

    public void AddUnitToTeam(int team)
    {
        numberOfUnits[team] += 1;
    }

    public void RemoveUnitFromTeam(int team)
    {
        numberOfUnits[team] -= 1;

        if (numberOfUnits[team] == 0)
        {
            Players[team].AddPoint(1);
            EventBus.RaiseEvent(EventBus.EventsEnum.BattleEnd);
        }
    }

    [ContextMenu("Start Battle")]
    public void StartBattle()
    {
        Debug.Log("Battle has begun");

        numberOfUnits.Clear();

        for (int i = 0; i < Players.Length; i++)
        {
            numberOfUnits.Add(0);
        }

        EventBus.RaiseEvent(EventBus.EventsEnum.BattleStart);
    }

    /// <summary>
    /// ���������� � ���� �� ���������� ����������.
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <param name="fieldToAttack"></param>
    /// <returns>������ ������ � �� ����������</returns>
    public (IEnumerable<BaseUnit>, int) GetField(int playerNumber, FieldRange fieldToInteract)
    {
        fieldDictionary.TryGetValue((playerNumber, fieldToInteract), out var field);
        if (field != null)
            return (field.MainUnits, field.MainUnits.Count);
        else
            return (Enumerable.Empty<BaseUnit>(), 0);
    }

}
