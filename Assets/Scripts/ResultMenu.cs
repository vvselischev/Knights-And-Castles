using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ResultMenu : MonoBehaviour, IMenu
    {
        [SerializeField] private Text gamesWithBotText;
        [SerializeField] private Text winsBotText;
        [SerializeField] private Text gamesNetworkText;
        [SerializeField] private Text winsNetworkText;
        [SerializeField] private Text winsBotPercentageText;
        [SerializeField] private Text winsNetworkPercentageText;
        [SerializeField] private Text resultText;
        
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void DisplayStatistics(Record record, UserResultType resultType)
        {
            if (resultType == UserResultType.WIN)
            {
                resultText.text = "You win!";
            }
            else if (resultType == UserResultType.LOSE)
            {
                resultText.text = "You lose!";
            }
            else
            {
                resultText.text = "Draw";
            }
            
            gamesWithBotText.text = record.GamesWithBot.ToString();
            winsBotText.text = record.WinsBot.ToString();
            gamesNetworkText.text = record.GamesNetwork.ToString();
            winsNetworkText.text = record.WinsNetwork.ToString();

            if (record.GamesWithBot > 0)
            {
                winsBotPercentageText.text = record.WinsBot * 100 / record.GamesWithBot + "%";
            }
            else
            {
                winsBotPercentageText.text = "0%";
            }

            if (record.GamesNetwork > 0)
            {
                winsNetworkPercentageText.text = record.WinsNetwork * 100 / record.GamesNetwork + "%";
            }
            else
            {
                winsNetworkPercentageText.text = "0%";
            }
        }
    }
}