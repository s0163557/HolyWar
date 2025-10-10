using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HolyWar.Diplomacy
{
    public class WinLoose : MonoBehaviour
    {
        [SerializeField]
        protected List<Player> players;
        [SerializeField]
        protected List<Text> playersName;
        [SerializeField]
        protected List<Text> playersScores;
        [SerializeField]
        protected string toScene;

        protected void Start()
        {
            UpdateWinLoseBoard();
        }

        protected void UpdateWinLoseBoard()
        {
            for (int i = 0; i < players.Count; i++)
            {
                playersName[i].text = players[i].PlayerName;
                playersName[i].color = players[i].TeamColor;
                playersScores[i].text = players[i].Points.ToString();
                playersScores[i].color = players[i].TeamColor;
            }
        }

        public void EndGame()
        {
            SceneManager.LoadScene(toScene);
        }
    }
}