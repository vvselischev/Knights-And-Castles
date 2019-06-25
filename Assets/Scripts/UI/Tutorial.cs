using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Displays the pages with the tutorial.
    /// Switches to the left or to the right page.
    /// </summary>
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private List<GameObject> pages = new List<GameObject>();
        [SerializeField] private ExitListener exitListener;
        private int currentPage;

        /// <summary>
        /// Activates the first page of the tutorial.
        /// </summary>
        public void Activate()
        {
            if (pages.Count == 0)
            {
                return;
            }
            
            currentPage = 0;
            exitListener.Enable();
            exitListener.OnExitClicked += Deactivate;
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

        /// <summary>
        /// Deactivates the current page.
        /// </summary>
        public void Deactivate()
        {
            exitListener.Disable();
            exitListener.OnExitClicked -= Deactivate;
            DeactivatePage(currentPage);
            StateManager.Instance.ChangeState(StateType.TUTORIAL_MENU_GAME_STATE);
        }

        /// <summary>
        /// Deactivates the page by the given index.
        /// </summary>
        private void DeactivatePage(int pageIndex)
        {
            pages[pageIndex].SetActive(false);
        }
        
        /// <summary>
        /// Activates the page by the given index.
        /// </summary>
        private void ActivatePage(int pageIndex)
        {
            pages[pageIndex].SetActive(true);
        }
    }
}