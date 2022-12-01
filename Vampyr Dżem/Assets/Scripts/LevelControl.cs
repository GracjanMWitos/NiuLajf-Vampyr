using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

namespace PatataStudio
{
    public class LevelControl : MonoBehaviour
    {

        public int index;
        public string levelName;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(index);
              //  SceneManager.LoadScene(levelName);
            }
        }

    }
}
