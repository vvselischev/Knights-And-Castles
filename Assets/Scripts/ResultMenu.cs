using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ResultMenu : MonoBehaviour, IMenu
    {
        public Text gamesWithBotText;
        public Text winsBotText;
        public Text gamesNetworkText;
        public Text winsNetworkText;
        public Text winsBotPercentageText;
        public Text winsNetworkPercentageText;
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void DisplayStatistics(Record record)
        {
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