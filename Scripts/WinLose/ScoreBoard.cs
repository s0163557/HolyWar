using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HolyWar.Diplomacy
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField]
        protected List<Player> players;
        [SerializeField]
        protected List<Text> playersName;
        [SerializeField]
        protected List<Text> playersScores;
        [SerializeField]
        protected GameObject winLose;

        protected void Start()
        {
            EventBus.Subscribe(EventBus.EventsEnum.PlayerGetsPoint, UpdateScoreBoard);
            EventBus.Subscribe(EventBus.EventsEnum.GameEnd, winLoseMenu);
            UpdateScoreBoard();
        }

        protected void UpdateScoreBoard()
        {
            for(int i = 0; i < players.Count; i++)
            {
                playersName[i].text = players[i].PlayerName;
                playersName[i].color = players[i].TeamColor;
                playersScores[i].text = players[i].Points.ToString();
                playersScores[i].color = players[i].TeamColor;
            }
        }

        protected void winLoseMenu()
        {
            winLose.SetActive(true);
        }
    }
}