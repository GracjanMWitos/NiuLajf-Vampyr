using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatataStudio.DialogueSystem
{
    [System.Serializable]
    public class Narration
    {
        public string name;
        public AudioClip narratorAudioClip;
        public string combinationToCallNarrator;
    }
    public class NarratorPlayer : MonoBehaviour
    {
        [SerializeField] private List<Narration> narrationList = new List<Narration>();
        [SerializeField] private AudioSource audioSource;
        private void Awake()
        {
            audioSource = this.GetComponent<AudioSource>();
        }

        public void FindNarratorState()
        {
            foreach (Narration narrator in narrationList)
            {
                if (NarratorManager.instance.currentCombination == narrator.combinationToCallNarrator)
                {
                    audioSource.clip = narrator.narratorAudioClip;
                    audioSource.Play();

                    return;
                }
            }
        }
    }
}
