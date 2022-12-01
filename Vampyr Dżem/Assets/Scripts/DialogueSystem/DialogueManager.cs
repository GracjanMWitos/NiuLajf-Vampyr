using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PatataStudio.Inputs;
using PatataStudio.DataPersitence;

namespace PatataStudio.DialogueSystem
{
	public class DialogueManager : MonoBehaviour, IDataPersistence
	{	
		public static DialogueManager instance { get; private set; }

		[Header("Params")]
		[SerializeField] private float typingSpeed = 0.04f;

		[Header("Load Globals JSON")]
		[SerializeField] private TextAsset loadGlobalsJSON;

		[Header("Dialogue UI")]
		[SerializeField] private GameObject dialoguePanel;
		[SerializeField] private GameObject continueIcon;
		[SerializeField] private TextMeshProUGUI dialogueText;
		[SerializeField] private TextMeshProUGUI displayNameText;
		[SerializeField] private Animator portraitAnimator;
		private Animator layoutAnimator;

		[Header("Choices UI")]
		[SerializeField] private GameObject[] choices;
		private TextMeshProUGUI[] choicesText;

		private Story currentStory;
		public bool dialogueIsPlaying { get; private set; }
		private bool canContinueToNextLine = false;
		private Coroutine displayLineCoroutine;

		private const string SPEAKER_TAG = "speaker";
		private const string PORTRAIT_TAG = "portrait";
		private const string LAYOUT_TAG = "layout";

		private DialogueVariables dialogueVariables;
		public ContinueDialogue continueDialogue;

		private void Awake()
		{
			if (instance != null)
			{
				Debug.Log("More than one Dialogue Manager in scene.");
				return;
			}
			instance = this;
		}

		private void Start()
		{
			dialogueIsPlaying = false;
			dialoguePanel.SetActive(false);

			layoutAnimator = dialoguePanel.GetComponent<Animator>();

			choicesText = new TextMeshProUGUI[choices.Length];
			int index = 0;
			foreach (GameObject choice in choices)
			{
				choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
				index++;
			}

		}

		private void Update()
		{
			if (!dialogueIsPlaying)
			{
				return;
			}

			if (canContinueToNextLine && currentStory.currentChoices.Count == 0 && InputManager.instance.GetSubmitPressed())
			{
				ContinueStory();
			}

		}

		public void EnterDialogueMode(TextAsset inkJSON)
		{
			currentStory = new Story(inkJSON.text);
			dialogueIsPlaying = true;
			dialoguePanel.SetActive(true);

			dialogueVariables.StartListening(currentStory);

			displayNameText.text = "???";
			portraitAnimator.Play("default");
			layoutAnimator.Play("Right");

			ContinueStory();
		}

		private void ExitDialogueMode()
		{
			dialogueVariables.StopListening(currentStory);
			dialogueIsPlaying = false;
			dialoguePanel.SetActive(false);
			dialogueText.text = "";
			if (continueDialogue != null)
				continueDialogue.Fadeout();
			continueDialogue = null;
			
		}

		private void ContinueStory()
		{
			if (currentStory.canContinue)
			{
				if (displayLineCoroutine != null)
				{
					StopCoroutine(displayLineCoroutine);
				}
				
				NarratorManager.instance.UpdateCombination("_");
				displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
				HandleTags(currentStory.currentTags);
			}
			else
			{
				ExitDialogueMode();
			}
		}

		private IEnumerator DisplayLine(string line)
		{
			dialogueText.text = line;
			dialogueText.maxVisibleCharacters = 0;
			continueIcon.SetActive(false);
			HideChoices();
			canContinueToNextLine = false;

			bool isAddingRichTextTag = false;

			foreach (char letter in line.ToCharArray())
			{
				if (InputManager.instance.GetSubmitPressed())
				{
					dialogueText.maxVisibleCharacters = line.Length;
					break;
				}

				if (letter == '<' || isAddingRichTextTag)
				{
					isAddingRichTextTag = true;
					if (letter == '>')
					{
						isAddingRichTextTag = false;
					}
				}
				else
				{
					dialogueText.maxVisibleCharacters++;
					yield return new WaitForSeconds(typingSpeed);
				}
			}

			continueIcon.SetActive(true);
			DisplayChoices();
			canContinueToNextLine = true;
		}

		private void HideChoices()
		{
			foreach (GameObject choiceButton in choices)
			{
				choiceButton.SetActive(false);
			}
		}

		private void HandleTags(List<string> currentTags)
		{
			foreach (string tag in currentTags)
			{
				string[] splitTag = tag.Split(':');
				if (splitTag.Length != 2)
				{
					Debug.LogError("Tag could not be appropriately parsed: " + tag);
				}

				string tagKey = splitTag[0].Trim();
				string tagValue = splitTag[1].Trim();

				switch (tagKey)
				{
					case SPEAKER_TAG:
						displayNameText.text = tagValue;
						break;
					case PORTRAIT_TAG:
						portraitAnimator.Play(tagValue);
						break;
					case LAYOUT_TAG:
						layoutAnimator.Play(tagValue);
						break;
					default:
						Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
						break;
				}
			}
		}

		private void DisplayChoices()
		{
			List<Choice> currentChoices = currentStory.currentChoices;

			if (currentChoices.Count > choices.Length)
			{
				Debug.LogError("More choices were given than UI can support. Number of choices given: " + currentChoices.Count);
			}

			int index = 0;
			foreach (Choice choice in currentChoices)
			{
				choices[index].gameObject.SetActive(true);
				choicesText[index].text = choice.text;
				index++;
			}

			for (int i = index; i < choices.Length; i++)
			{
				choices[i].gameObject.SetActive(false);
			}
		}

		public void MakeChoice(int choiceIndex)
		{
			if (canContinueToNextLine)
			{
				currentStory.ChooseChoiceIndex(choiceIndex);
				ContinueStory();
			}
		}

		public Ink.Runtime.Object GetVariableState(string variableName)
		{
			Ink.Runtime.Object variableValue = null;
			dialogueVariables.variables.TryGetValue(variableName, out variableValue);
			if (variableValue == null)
			{
				Debug.LogWarning("Ink Variable was found to be null: " + variableName);
			}
			return variableValue;
		}

		public void SetVariableState(string variableName, Ink.Runtime.Object variableValue)
		{
			if (dialogueVariables.variables.ContainsKey(variableName))
			{
				dialogueVariables.variables.Remove(variableName);
				dialogueVariables.variables.Add(variableName, variableValue);
				Debug.Log("Set variable state: " + variableName + " to " + variableValue);
			}
			else
			{
				Debug.LogWarning("Tried to update variable that wasn't initialized by globals.ink: " + variableName);
			}
		}

		public void LoadData(GameData data)
		{
			dialogueVariables = new DialogueVariables(loadGlobalsJSON, data.globalVariablesStateJson);
		}

		public void SaveData(GameData data)
		{
			string globalStateJson = dialogueVariables.GetGlobalVariablesStateJson();
			data.globalVariablesStateJson = globalStateJson;
		}
		
	}
}