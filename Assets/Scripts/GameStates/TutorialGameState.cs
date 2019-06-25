using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TutorialGameState : MonoBehaviour, IGameState
    {
        [SerializeField] private List<Canvas> pages = new List<Canvas>();
        [SerializeField] private ExitListener exitListener;
        private int currentPage;

        private void Awake()
        {
            exitListener.OnExitClicked += () =>
            {
                StateManager.Instance.ChangeState(StateType.START_GAME_STATE);
            };
        }

        public void InvokeState()
        {
            if (pages.Count == 0)
            {
                return;
            }
            currentPage = 0;
            exitListener.Enable();
            ActivatePage(currentPage);
        }

        public void MoveToRightPage()
        {
            if (currentPage == pages.Count - 1)
            {
                //It is the last page.
                return;
            }

            DeactivatePage(currentPage);
            currentPage++;
            ActivatePage(currentPage);
        }
        
        public void MoveToLeftPage()
        {
            if (currentPage == 0)
            {
                //It is the first page.
                return;
            }

            DeactivatePage(currentPage);
            currentPage--;
            ActivatePage(currentPage);
        }

        public void CloseState()
        {
            exitListener.Disable();
            DeactivatePage(currentPage);
        }

        private void DeactivatePage(int pageIndex)
        {
            pages[pageIndex].gameObject.SetActive(false);
        }
        
        private void ActivatePage(int pageIndex)
        {
            pages[pageIndex].gameObject.SetActive(true);
        }
    }
}