using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatataStudio.DialogueSystem
{
    public class ContinueDialogue : MonoBehaviour
    {
        [Header("Visual Cue")]
        [SerializeField] private GameObject visualCue;

        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJSON;

        public bool tobBeContiuned;
        [SerializeField] private Animator fadeoutPanel;
        public void Fadeout()
        {
            if (tobBeContiuned)
                fadeoutPanel.GetComponent<Animator>().SetTrigger("Fadeout");

            visualCue.SetActive(true);
            NarratorManager.instance.narratorPlayer = this.GetComponent<NarratorPlayer>();
            NarratorManager.instance.UpdateCombination("_");
            DialogueManager.instance.EnterDialogueMode(inkJSON);
        }
    }
}