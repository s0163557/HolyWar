using System;
using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleTimer : MonoBehaviour
{
    [SerializeField]
    private BattleManager _battleManager;

    [SerializeField]
    private float _intervalBetweenBattles;

    [SerializeField]
    private TextMeshProUGUI _timeLabel;


    private float _timeRemaining;
    private bool _isCountingDown;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

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
