using System.Collections;
using TMPro;
using UnityEngine;

public class BattleTimer : MonoBehaviour
{
    [SerializeField]
    private BattleManager _battleManager;

    [SerializeField]
    private float _intervalBetweenBattles;

    [SerializeField]
    private TextMeshProUGUI _timeLabel;

    private float _timeRemaining;

    public IEnumerator BattleCountdown()
    {
        while (_timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;

            if (_timeLabel != null)
                _timeLabel.text = $"Битва через {Mathf.Ceil(_timeRemaining)}";

            yield return null;
        }

        _timeLabel.text = "";
        _battleManager.StartBattle();
    }

    private void StartTimer()
    {
        _timeRemaining = _intervalBetweenBattles;
        StartCoroutine(BattleCountdown());
    }

    public void OnDestroy()
    {
        EventBus.Unsubscribe(EventBus.EventsEnum.BattleEnd, StartTimer);
    }

    public void Awake()
    {
        EventBus.Subscribe(EventBus.EventsEnum.BattleEnd, StartTimer);
        StartTimer();
    }

}
