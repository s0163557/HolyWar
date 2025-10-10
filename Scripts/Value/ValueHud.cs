using HolyWar.Diplomacy;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ValueHud : MonoBehaviour
{
    [SerializeField]
    private Player targetPlayer;
    [SerializeField]
    TextMeshProUGUI valueLabel;

    void Start()
    {
        targetPlayer.SubscribeToWalletChanges(OnBalanceChanged);
        OnBalanceChanged(targetPlayer.GetCurrentBalance());
    }

    private void OnBalanceChanged(int newBalance)
    {
        valueLabel.text = $"Золото: {newBalance}";
    }

    private void OnDestroy()
    {
        targetPlayer.UnsubscribeToWalletChanges(OnBalanceChanged);
    }

}
