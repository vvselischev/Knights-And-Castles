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
            //TODO: maybe better to make colors serialized to setup in the editor.
            if (resultType == UserResultType.WIN)
            {
                resultText.color = Color.red;
                resultText.text = "You win!";
            }
            else if (resultType == UserResultType.LOSE)
            {
                resultText.color = Color.red;
                resultText.text = "You lose!";
            }
            else if (resultType == UserResultType.DRAW)
            {
                resultText.color = Color.black;
                resultText.text = "Draw";
            }
            else
            {
                resultText.color = Color.black;
                resultText.text = "Statistics";
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