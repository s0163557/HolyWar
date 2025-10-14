using UnityEngine;
using System;
using NUnit.Framework;
using HolyWar.Units;
using System.Collections.Generic;

namespace HolyWar.Diplomacy
{
    public enum PlayerType
    {
        Human = 0,
        AI = 1,
    }

    public class Player : MonoBehaviour
    {
        [SerializeField]
        protected string playerName;
        [SerializeField]
        protected byte points;
        [SerializeField]
        protected Color teamColor;

        [SerializeField]
        protected EconomyConfigSO economyConfig;

        //Каждый игрок сам хранит информацию о своих юнитах
        public List<BaseUnit> MeleeUnits = new List<BaseUnit>();
        public List<BaseUnit> RangedUnits = new List<BaseUnit>();
        public List<BaseUnit> ArtilleryUnits = new List<BaseUnit>();

        public PlayerType PlayerType;

        public byte PlayerNumber;

        [System.NonSerialized] private Wallet _wallet;

        public Wallet Wallet
        {
            get { return _wallet ??= new Wallet(); }
        }

        public static int maxPoints = 3;

        public int CurrentWinReward;

        public string PlayerName { get { return playerName; } set { playerName = value; } }
        public byte Points { get { return points; } set { points = value; } }
        public Color TeamColor { get { return teamColor; } set { teamColor = value; } }

        protected void Start()
        {
            points = 0;
        }

        protected void Awake()
        {
            //Узнаем какие настройки у матча
            CurrentWinReward = economyConfig.BaseWinReward;

            //Подпишемся на присвоение инкома
            EventBus.Subscribe(EventBus.EventsEnum.BattleEnd, () => { GetIncome(); });
        }

        public int GetCurrentBalance()
        {
            return Wallet.Balance;
        }

        public void GetIncome()
        {
            Wallet.Add(CurrentWinReward);
        }

        public void SubscribeToWalletChanges(Action<int> action)
        {
            Wallet.OnBalanceChanged += action;
        }

        public void UnsubscribeToWalletChanges(Action<int> action)
        {
            Wallet.OnBalanceChanged -= action;
        }

        public bool TrySpend(int cost)
        {
            return Wallet.TrySpend(cost);
        }

        public void AddPoint(byte points)
        {
            this.points += points;
            EventBus.RaiseEvent(EventBus.EventsEnum.PlayerGetsPoint);
            if (this.points >= maxPoints)
            {
                EventBus.RaiseEvent(EventBus.EventsEnum.GameEnd);
            }
        }

        [ContextMenu("AddPoint")]
        public void AddPointTest()
        {
            points += 1;
            EventBus.RaiseEvent(EventBus.EventsEnum.PlayerGetsPoint);
        }
    }
}