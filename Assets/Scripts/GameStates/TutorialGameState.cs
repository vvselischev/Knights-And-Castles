using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Displays the pages with the tutorial.
    /// Switches to the left or to the right page.
    /// </summary>
    public class TutorialGameState : MonoBehaviour, IGameState
    {
        [SerializeField] private List<Canvas> pages = new List<Canvas>();
        [SerializeField] private ExitListener exitListener;
        private int currentPage;

        /// <summary>
        /// Initialization. Subscribe on exit listener.
        /// </summary>
        private void Awake()
        {
            exitListener.OnExitClicked += () =>
            {
                StateManager.Instance.ChangeState(StateType.START_GAME_STATE);
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Activates the first page of the tutorial.
        /// </summary>
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

        /// <summary>
        /// Deactivates the current page and moves to the right page if it exists.
        /// </summary>
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
        
        /// <summary>
        /// Deactivates the current page and moves to the left page if it exists.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        /// Deactivates the current page.
        /// </summary>
        public void CloseState()
        {
            exitListener.Disable();
            DeactivatePage(currentPage);
        }

        /// <summary>
        /// Deactivates the page by the given index.
        /// </summary>
        private void DeactivatePage(int pageIndex)
        {
            pages[pageIndex].gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Activates the page by the given index.
        /// </summary>
        private void ActivatePage(int pageIndex)
        {
            pages[pageIndex].gameObject.SetActive(true);
        }
    }
}