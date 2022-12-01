using PatataStudio.Inputs;
using UnityEngine;

namespace PatataStudio.DialogueSystem
{
	public class DialogueTrigger : MonoBehaviour
	{
		[Header("Visual Cue")]
		[SerializeField] private GameObject visualCue;

		[Header("Ink JSON")]
		[SerializeField] private TextAsset inkJSON;
		
		private bool playerInRange;

		private void Awake()
		{
			playerInRange = false;
			visualCue.SetActive(false);
		}

		private void Update()
		{
			if (playerInRange && !DialogueManager.instance.dialogueIsPlaying)
			{
				visualCue.SetActive(true);
				if (InputManager.instance.GetInteractPressed())
				{
					NarratorManager.instance.narratorPlayer = this.GetComponent<NarratorPlayer>();
					NarratorManager.instance.currentCombination = "";
					NarratorManager.instance.UpdateCombination("");
					DialogueManager.instance.EnterDialogueMode(inkJSON);
					DialogueManager.instance.continueDialogue = this.GetComponent<ContinueDialogue>();
				}
			}
			else
			{
				visualCue.SetActive(false);
			}
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Player") playerInRange = true;
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.gameObject.tag == "Player") playerInRange = false;
		}
	}
}