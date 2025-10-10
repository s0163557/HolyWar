using System;
using Unity.VisualScripting;
using UnityEngine;

public class Wallet
{
    public int Balance { get; protected set; } = 500;
    public System.Action<int> OnBalanceChanged;

    public bool CanAfford(int cost)
    {
        return Balance >= cost ? true : false;
    }

    public bool TrySpend(int cost)
    {
        if (!CanAfford(cost))
            return false;

        Balance -= cost;
        OnBalanceChanged.Invoke(Balance);
        return true;
    }

    public void Add(int amount)
    {
        if (OnBalanceChanged != null)
        {
            Balance += amount;
            OnBalanceChanged.Invoke(Balance);
        }
    }
}
