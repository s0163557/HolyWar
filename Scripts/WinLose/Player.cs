using UnityEngine;
using System;

namespace HolyWar.Diplomacy
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        protected string playerName;
        [SerializeField]
        protected int points;
        [SerializeField]
        protected Color teamColor;
        [SerializeField]
        protected EconomyConfigSO economyConfig;

        [System.NonSerialized] private Wallet _wallet;

        public Wallet Wallet
        {
            get { return _wallet ??= new Wallet(); }
        }

        public static int maxPoints = 3;

        public int CurrentWinReward;

        public string PlayerName { get { return playerName; } set { playerName = value; } }
        public int Points { get { return points; } set { points = value; } }
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

        public void AddPoint(int points)
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