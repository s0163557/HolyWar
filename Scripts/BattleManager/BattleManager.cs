using HolyWar.Fields;
using HolyWar.Units;
using HolyWar.Diplomacy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Tools;

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

    private Dictionary<FieldRange, Field> _attackerFieldsDictionary = new Dictionary<FieldRange, Field>();
    private Dictionary<FieldRange, Field> _defenderFieldsDictionary = new Dictionary<FieldRange, Field>();

    private Dictionary<FieldPositionType, Dictionary<FieldRange, Field>> _positionToFieldsDictionary = new Dictionary<FieldPositionType, Dictionary<FieldRange, Field>>();
    private BiDictionary<Player, FieldPositionType> _playerToPositionBiDictionary = new BiDictionary<Player, FieldPositionType>();

    protected Dictionary<Player, int> numberOfUnitsByPlayerDictionary = new Dictionary<Player, int>();

    public bool IsBattleRunning = false;

    private Player[] players;
    protected Player[] Players
    {
        get { return players == null ? players = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID) : players; }
    }

    private Player HumanPlayer;
    private Player AIOpponent;

    public Player GetHumanPlayer()
    {
        return HumanPlayer;
    }

    public Player GetOpponent(Player requestingPlayer)
    {
        //Логика проста - в битве учавствует только двое игроков - человек и ИИ. Значит противником человека будет ИИ, а противником ИИ будет человек.
        //Нужно будет подумать о возможности раширения этого подхода. Что если я хочу, чтобы мой ИИ оппонент был не один, а несколько? И каждый из них
        //симулировал бы бой с волной и развивался бы от этих результатов. 
        return requestingPlayer == HumanPlayer ? AIOpponent : HumanPlayer;
    }

    private void ChangeOpponents(Player newOpponent)
    {
        AIOpponent = newOpponent;
    }

    private void SortPlayers()
    {
        //Проверим что у нас точно один игрок
        var humans = Players.Where(p => p.PlayerType == PlayerType.Human).ToArray();

        //Для дебага
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

        //По умолчанию выставим противником любого ИИ, который попадётся первым
        AIOpponent = AIs.First();
    }

    public void Awake()
    {
        //Мы должны быть полностью уверены в том, что сортируем поля правильно - поэтому я насильно переобъявляю их.
        DefenderMeleeFieldScript.FieldRange = FieldRange.Melee;
        DefenderRangedFieldScript.FieldRange = FieldRange.Ranged;
        DefenderArtilleryFieldScript.FieldRange = FieldRange.Artillery;

        AttackerMeleeFieldScript.FieldRange = FieldRange.Melee;
        AttackerRangedFieldScript.FieldRange = FieldRange.Ranged;
        AttackerArtilleryFieldScript.FieldRange = FieldRange.Artillery;

        //Так же удостоверимся, что у полей правильные позиции
        DefenderMeleeFieldScript.FieldPositionType = FieldPositionType.Defender;
        DefenderRangedFieldScript.FieldPositionType = FieldPositionType.Defender;
        DefenderArtilleryFieldScript.FieldPositionType = FieldPositionType.Defender;

        AttackerMeleeFieldScript.FieldPositionType = FieldPositionType.Attacker;
        AttackerRangedFieldScript.FieldPositionType = FieldPositionType.Attacker;
        AttackerArtilleryFieldScript.FieldPositionType = FieldPositionType.Attacker;

        //Определим всё ли нормально с игроками - должен быть ровно один игрок-человек и хотя бы один противник ИИ.
        SortPlayers();

        //Полезно иметь все поля отсортированными по спискам
        _defenderFieldsDictionary.Add(FieldRange.Melee, DefenderMeleeFieldScript);
        _defenderFieldsDictionary.Add(FieldRange.Ranged, DefenderRangedFieldScript);
        _defenderFieldsDictionary.Add(FieldRange.Artillery, DefenderArtilleryFieldScript);

        _attackerFieldsDictionary.Add(FieldRange.Melee, AttackerMeleeFieldScript);
        _attackerFieldsDictionary.Add(FieldRange.Ranged, AttackerRangedFieldScript);
        _attackerFieldsDictionary.Add(FieldRange.Artillery, AttackerArtilleryFieldScript);

        _positionToFieldsDictionary.Add(FieldPositionType.Defender, _defenderFieldsDictionary);
        _positionToFieldsDictionary.Add(FieldPositionType.Attacker, _attackerFieldsDictionary);

        //Выставим по умолчанию игрока защищающимся а ИИшку - атакующей
        _playerToPositionBiDictionary.Add(HumanPlayer, FieldPositionType.Defender);
        _playerToPositionBiDictionary.Add(AIOpponent, FieldPositionType.Attacker);

        ProcessPlayers();
    }

    public void AddUnitToTeam(Player owner)
    {
        numberOfUnitsByPlayerDictionary[owner] += 1;
    }

    public void AssignPlayerToSide(Player newPlayer, FieldPositionType fieldPositionType)
    {
        //Найдем, за каким игроком закреплены в текущий момент поля
        if (_playerToPositionBiDictionary.TryGetKeyByValue(fieldPositionType, out Player oldPlayer))
        {
            //Если нашли старого игрока, то меняем его
            _playerToPositionBiDictionary.ChangeKey(oldPlayer, newPlayer);
        }
    }

    public void RemoveUnitFromTeam(Player owner)
    {
        numberOfUnitsByPlayerDictionary[owner] -= 1;

        if (numberOfUnitsByPlayerDictionary[owner] == 0)
        {
            owner.AddPoint(1);
            EndBattle();
        }
    }

    private void UpdateUnitsInField(Player player, FieldRange changedField, BaseUnit unitToAdd)
    {
        if (_playerToPositionBiDictionary.TryGetValueByKey(player, out var playerPosition))
        {
            _positionToFieldsDictionary[playerPosition][changedField].RegisterUnitToMain(unitToAdd);
        }
        else
        {
            Debug.LogWarning("Recieved Player without his assigment in BattleManager lists");
        }

    }

    private void ProcessPlayers()
    {
        foreach (Player player in _playerToPositionBiDictionary.GetKeys())
        {
            //Все юнитыд обавляются напрямую в плеера. Будем слушать их изменение и подстраивать отображение в полях соответствующимо бразом
            player.OnUnitChange += UpdateUnitsInField;

            //Выпишем всех юнитов, которые могли быть добавлены в игрока по умолчанию
            if (_playerToPositionBiDictionary.TryGetValueByKey(player, out var playerPosition))
            {
                _positionToFieldsDictionary[playerPosition][FieldRange.Melee].RegisterUnitToMain(player.MeleeUnits);
                _positionToFieldsDictionary[playerPosition][FieldRange.Ranged].RegisterUnitToMain(player.RangedUnits);
                _positionToFieldsDictionary[playerPosition][FieldRange.Artillery].RegisterUnitToMain(player.ArtilleryUnits);
            }
            else
            {
                Debug.LogWarning("Recieved Player without his assigment in BattleManager lists");
            }

        }
    }

    public void EndBattle()
    {
        EventBus.RaiseEvent(EventBus.EventsEnum.BattleEnd);
        IsBattleRunning = false;
    }

    [ContextMenu("Start Battle")]
    public void StartBattle()
    {
        Debug.Log("Battle has begun");
        IsBattleRunning = true;

        numberOfUnitsByPlayerDictionary.Clear();

        for (int i = 0; i < Players.Length; i++)
        {
            numberOfUnitsByPlayerDictionary.Add(Players[i], 0);
        }

        EventBus.RaiseEvent(EventBus.EventsEnum.BattleStart);
    }

    /// <summary>
    /// Обращается к полю по переданным параметрам.
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <param name="fieldToAttack"></param>
    /// <returns>Список юнитов и их количество</returns>
    public (IEnumerable<BaseUnit>, int) GetFieldUnits(Player player, FieldRange fieldToInteract)
    {
        //Сначала определим, за какую сторону играет игрок

        if (!_playerToPositionBiDictionary.TryGetValueByKey(player, out var positionType))
        {
            Debug.LogWarning($"When executing GetField merhon, couldn't find a Player {player.PlayerName}");
            return (new List<BaseUnit>(), 0);
        }

        //Теперь по позиции определим список полей
        if (!_positionToFieldsDictionary.TryGetValue(positionType, out var fieldsList))
        {
            Debug.LogWarning($"When executing GetField merhon, couldn't find a fields for a position {positionType}");
            return (new List<BaseUnit>(), 0);
        }

        //Найдем нужное поле по его типу
        if (!fieldsList.TryGetValue(fieldToInteract, out var field))
        {
            Debug.LogWarning($"When executing GetField method, couldn't find a field in a fields list {positionType}");
            return (new List<BaseUnit>(), 0);
        }

        //Вернём список юнитов и их количество
        return (field.MainUnits.Concat(field.AuxUnits), field.MainUnits.Count);
    }

    public FieldPositionType GetPositionOfPlayer(Player player)
    {
        return _playerToPositionBiDictionary.TryGetValueByKey(player, out var positionType) ? positionType : FieldPositionType.Defender;
    }

}
