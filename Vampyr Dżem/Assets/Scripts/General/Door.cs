using PatataStudio.Inputs;
using UnityEngine;

namespace PatataStudio.General
{
	public class Door : MonoBehaviour
	{
		[SerializeField] private Transform doorDestination;
		private Transform player;
		[SerializeField] private int trackNumber;
		[SerializeField] private bool activeBloomEffect;
 
		private void Awake()
		{
			player = GameObject.FindGameObjectWithTag("Player").transform;
		}
		private void OnTriggerStay2D(Collider2D collision)
		{
			if (InputManager.instance.GetInteractPressed())
			{
				player.position = doorDestination.position;
				SoundtrackManager.instance.ChangeMusic(trackNumber);
				this.GetComponent<LightController>().BloomEffectControl(activeBloomEffect);
				this.GetComponent<LightController>().ChangeLight();
			}
		}
	}
}
