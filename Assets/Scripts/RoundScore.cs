using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoundScore : MonoBehaviour
    {
        private Dictionary<PlayerType, int> score;
        private static RoundScore instance;

        public void Awake()
        {
            instance = this;
            score = new Dictionary<PlayerType, int>();
        }

        public void Reset()
        {
            score.Clear();
        }

        public static RoundScore GetInstance()
        {
            return instance;
        }

        public int GetScore(PlayerType playerType)
        {
            if (score.ContainsKey(playerType))
            {
                return score[playerType];
            }
            else
            {
                return 0;
            }
        }

        public void SetScore(PlayerType playerType, int newScore)
        {
            if (!score.ContainsKey(playerType))
            {
                score.Add(playerType, 0);
            }
            score[playerType] = newScore;
        }

        public void IncreaseScore(PlayerType playerType)
        {
            SetScore(playerType, GetScore(playerType) + 1);
        }
    }
}