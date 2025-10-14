using HolyWar.Fields;
using HolyWar.Units;
using HolyWar.Diplomacy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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

    private Dictionary<(Player, FieldRange), Field> fieldDictionary = new Dictionary<(Player, FieldRange), Field>();
    protected List<int> numberOfUnits;

    private Player[] players;
    protected Player[] Players
    {
        get { return players == null ? players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID) : players; }
    }

    private Player HumanPlayer;
    private Player AIOpponent;

    public Player GetOpponent(Player requestingPlayer)
    {
        //������ ������ - � ����� ���������� ������ ���� ������� - ������� � ��. ������ ����������� �������� ����� ��, � ����������� �� ����� �������.
        //����� ����� �������� � ����������� ��������� ����� �������. ��� ���� � ����, ����� ��� �� �������� ��� �� ����, � ���������? � ������ �� ���
        //����������� �� ��� � ������ � ���������� �� �� ���� �����������. 
        return requestingPlayer == HumanPlayer? AIOpponent : HumanPlayer;
    }

    private void ChangeOpponents(Player newOpponent)
    {
        AIOpponent = newOpponent;
    }

    private void SortPlayers()
    {
        //�������� ��� � ��� ����� ���� �����
        var humans = Players.Where(p => p.PlayerType == PlayerType.Human).ToArray();

        //��� ������
        switch (humans.Length)
        {
            case 0:
                throw new System.Exception("[Game Rule] There is no human player in the scene! BattleManager supports only one human player!");

            case > 1:
                throw new System.Exception("[Game Rule] There is too much human players in the scene! BattleManager supports only one human player!");
        }

        var AIs = Players.Where(p => p.PlayerType == PlayerType.AI).ToArray();
        if (AIs.Length == 0)
        {
            throw new System.Exception("[Game Rule] There is not enough AI players in the scene! BattleManager supports at least one AI player!");
        }

        HumanPlayer = humans.First();

        //�� ��������� �������� ����������� ������ ��, ������� �������� ������
        AIOpponent = AIs.First();
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

        //��� �� ������������, ��� � ����� ���������� �������
        DefenderMeleeFieldScript.FieldPositionType = FieldPositionType.Defender;
        DefenderRangedFieldScript.FieldPositionType = FieldPositionType.Defender;
        DefenderArtilleryFieldScript.FieldPositionType = FieldPositionType.Defender;

        AttackerMeleeFieldScript.FieldPositionType = FieldPositionType.Attacker;
        AttackerRangedFieldScript.FieldPositionType = FieldPositionType.Attacker;
        AttackerArtilleryFieldScript.FieldPositionType = FieldPositionType.Attacker;

        //��������� �� �� ��������� � �������� - ������ ���� ����� ���� �����-������� � ���� �� ���� ��������� ��.
        SortPlayers();

        //������� ����� ��� ���� � ����� ������
        fieldDictionary.Add((HumanPlayer, FieldRange.Melee), DefenderMeleeFieldScript);
        fieldDictionary.Add((HumanPlayer, FieldRange.Ranged), DefenderRangedFieldScript);
        fieldDictionary.Add((HumanPlayer, FieldRange.Artillery), DefenderArtilleryFieldScript);

        fieldDictionary.Add((AIOpponent, FieldRange.Melee), AttackerMeleeFieldScript);
        fieldDictionary.Add((AIOpponent, FieldRange.Ranged), AttackerRangedFieldScript);
        fieldDictionary.Add((AIOpponent, FieldRange.Artillery), AttackerArtilleryFieldScript);

        numberOfUnits = new List<int>();

    }

    public void AddUnitToTeam(int team)
    {
        numberOfUnits[team] += 1;
    }

    public int AssignPlayerToSide(Player player, FieldPositionType fieldPositionType)
    {

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
    public (IEnumerable<BaseUnit>, int) GetField(Player player, FieldRange fieldToInteract)
    {
        fieldDictionary.TryGetValue((player, fieldToInteract), out var field);
        if (field != null)
            return (field.MainUnits, field.MainUnits.Count);
        else
            return (Enumerable.Empty<BaseUnit>(), 0);
    }

}
