using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PatataStudio
{
    public class SoundtrackManager : MonoBehaviour
    {
        public AudioClip[] musicArrey;
        public int currentTrackNumber = 0;
        AudioSource audioSource;

        // Update is called once per frame
        private void Awake()
        {
            audioSource = this.GetComponent<AudioSource>();
            instance = this;
            audioSource.clip = musicArrey[1];
            audioSource.Play();
        }
        public void ChangeMusic(int trackNumber)
        {
            audioSource.clip = musicArrey[trackNumber];
            audioSource.Play();
        }
        public static SoundtrackManager instance;
    }
}
