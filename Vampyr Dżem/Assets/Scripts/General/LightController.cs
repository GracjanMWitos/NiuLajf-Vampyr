using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatataStudio.General
{
    public class LightController : MonoBehaviour
    {

        public GameObject lightToActive;
        public GameObject lightToDisctive;
        public GameObject BloomEffect;
        public void ChangeLight()
        {
            if (lightToDisctive != null && lightToActive != null)
            {
                lightToActive.SetActive(true);
                lightToDisctive.SetActive(false);
            }
        }
        public void BloomEffectControl(bool activeEffect)
        {
            BloomEffect.SetActive(activeEffect);
        }
    }
}
