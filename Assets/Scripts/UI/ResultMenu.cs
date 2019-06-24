using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    /// <summary>
    /// Displays the statistics.
    /// </summary>
    public class ResultMenu : MonoBehaviour, IMenu
    {
        [SerializeField] private Text gamesWithBotText;
        [SerializeField] private Text winsBotText;
        [SerializeField] private Text gamesNetworkText;
        [SerializeField] private Text winsNetworkText;
        [SerializeField] private Text winsBotPercentageText;
        [SerializeField] private Text winsNetworkPercentageText;
        [SerializeField] private Text resultText;
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Displays the given statistics.
        /// </summary>
        /// <param name="resultType">Result type for the title.</param>
        public void DisplayStatistics(Record record, UserResultType resultType)
        {
            UpdateResultText(resultType);
            UpdateStatisticLabels(record);
        }

        /// <summary>
        /// Updates the labels according to the given record.
        /// </summary>
        private void UpdateStatisticLabels(Record record)
        {
            gamesWithBotText.text = record.GamesWithBot.ToString();
            winsBotText.text = record.WinsBot.ToString();
            gamesNetworkText.text = record.GamesNetwork.ToString();
            winsNetworkText.text = record.WinsNetwork.ToString();
            UpdatePercentageStatisticLabels(record);
        }

        /// <summary>
        /// Updates the labels that contain percentage.
        /// </summary>
        private void UpdatePercentageStatisticLabels(Record record)
        {
            if (record.GamesWithBot > 0)
            {
                //Convert relative part to percents.
                winsBotPercentageText.text = record.WinsBot * 100 / record.GamesWithBot + "%";
            }
            else
            {
                //No games were played with the bot.
                winsBotPercentageText.text = "0%";
            }

            if (record.GamesNetwork > 0)
            {
                //Convert relative part to percents.
                winsNetworkPercentageText.text = record.WinsNetwork * 100 / record.GamesNetwork + "%";
            }
            else
            {
                //No games were played with network.
                winsNetworkPercentageText.text = "0%";
            }
        }

        /// <summary>
        /// Updates the text on the top of the menu depending on the result for the user.
        /// </summary>
        private void UpdateResultText(UserResultType resultType)
        {
            //TODO: maybe better to make colors and strings serialized to setup in the editor?
            if (resultType == UserResultType.WIN)
            {
                resultText.color = Color.green;
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
        }
    }
}