using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public abstract class BoardStorageItem
    {
        private BoardButton boardButton;
        public GameObject TestEssense { get; set; }
        public BoardButton BoardButton
        {
            get { return boardButton; }
            set
            {
                TestEssense.GetComponent<RectTransform>().localPosition = value.button.transform.localPosition;
                boardButton = value;
            }
        }

        public BoardStorageItem(BoardButton boardButton, GameObject essense)
        {
            TestEssense = essense;
            BoardButton = boardButton;
        }
    }
}