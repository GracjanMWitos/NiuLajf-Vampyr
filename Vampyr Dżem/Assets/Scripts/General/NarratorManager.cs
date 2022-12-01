using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatataStudio.DialogueSystem
{
    public class NarratorManager : MonoBehaviour
    {
        public NarratorPlayer narratorPlayer;
        public string currentCombination = "";
        private void Awake()
        {
            instance = this;
        }
        public void UpdateCombination(string letter)
        {
            currentCombination = currentCombination + letter;
            narratorPlayer.FindNarratorState();
        }
        // Update is called once per frame
        void Update()
        {

        }
        public static NarratorManager instance;
    }
}