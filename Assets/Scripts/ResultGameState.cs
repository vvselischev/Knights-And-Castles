using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ResultGameState : MonoBehaviour, IGameState
    {
        private DataService dataService = new DataService("record_database.db");
        [SerializeField] private ResultMenu resultMenu;
        [SerializeField] private UserResultType resultType;
        [SerializeField] private StateType playStateType;
        [SerializeField] private ExitListener exitListener;
        
        private MenuActivator menuActivator = MenuActivator.Instance;
        
        public void Initialize(UserResultType resultType, StateType playStateType)
        {
            this.resultType = resultType;
            this.playStateType = playStateType;
        }

        public void InvokeState()
        {
            exitListener.Enable();
            exitListener.OnExitClicked += OnExit;
            
            menuActivator.OpenMenu(resultMenu);

            var records = dataService.GetRecords().ToList();
            Record record;
            if (records.Count > 0)
            {
                record = records[0];
            }
            else
            {
                record = new Record();
            }

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

            dataService.UpdateRecord(record);

            resultMenu.DisplayStatistics(record, resultType);
        }

        private void OnExit()
        {
            var stateManager = StateManager.Instance;
            stateManager.ChangeState(StateType.START_GAME_STATE);
        }

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