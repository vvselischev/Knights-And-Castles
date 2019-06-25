using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Receives game results from PlayGameState, updates the database and displays on canvas.
    /// </summary>
    public class ResultGameState : MonoBehaviour, IGameState
    {
        private DataService dataService = new DataService("record_database.db");
        [SerializeField] private ResultMenu resultMenu;
        [SerializeField] private UserResultType resultType;
        [SerializeField] private StateType playStateType;
        [SerializeField] private ExitListener exitListener;
        
        private MenuActivator menuActivator = MenuActivator.Instance;
        
        /// <summary>
        /// Must be called by PlayGameState.
        /// </summary>
        public void Initialize(UserResultType resultType, StateType playStateType)
        {
            this.resultType = resultType;
            this.playStateType = playStateType;
        }

        /// <inheritdoc />
        /// <summary>
        /// Updates the user record in the database according to the previously set result type.
        /// </summary>
        public void InvokeState()
        {
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
            menuActivator.OpenMenu(resultMenu);

            //Get the existing record or create a new one.
            var record = GetUserRecord();

            //Update record with the current result.
            UpdateRecord(record);
            //Save it to the database.
            dataService.UpdateRecord(record);

            resultMenu.DisplayStatistics(record, resultType);
        }

        /// <summary>
        /// Returns the user record from the database or creates a new one if does not exist.
        /// </summary>
        private Record GetUserRecord()
        {
            Record record;
            var records = dataService.GetRecords().ToList();
            if (records.Count > 0)
            {
                record = records[0];
            }
            else
            {
                record = new Record();
            }
            return record;
        }

        /// <summary>
        /// Updates the given record according to the game mode and result.
        /// </summary>
        private void UpdateRecord(Record record)
        {
            if (playStateType == StateType.AI_GAME_STATE)
            {
                record.GamesWithBot++;
                if (resultType == UserResultType.WIN)
                {
                    record.WinsBot++;
                }
            }
            else if (playStateType == StateType.NETWORK_GAME_STATE)
            {
                record.GamesNetwork++;
                if (resultType == UserResultType.WIN)
                {
                    record.WinsNetwork++;
                }
            }
        }

        /// <summary>
        /// Method is called by the exit listener when exit button is pressed.
        /// </summary>
        private void OnExit()
        {
            var stateManager = StateManager.Instance;
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void CloseState()
        {
            exitListener.OnExitClicked -= OnExit;
            exitListener.Disable();
            menuActivator.CloseMenu();
            playStateType = StateType.START_GAME_STATE;
            resultType = UserResultType.NONE;
        }
    }
}