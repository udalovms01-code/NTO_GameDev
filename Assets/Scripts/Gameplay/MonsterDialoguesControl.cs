using System;
using Audio;
using DialogueSystem.Runtime;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class DialogDayDetails
{
    public DialogueTree[] trees;
}

public class MonsterDialoguesControl : MonoBehaviour
{
    [SerializeField] private DialogDayDetails[] dialogData;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject answerPrefab;
    [SerializeField] private Transform answersParent;
    [Header("Audio")]
    [SerializeField] private SoundEffectPlayer dialogueSoundPlayer;
    [SerializeField] private SoundCollection choiceSelectSound;

    private DialogueRunner _runner;
    private GameStateService _gameStateService;

    [Inject]
    public void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
        _gameStateService.OnStateChanged += TryStartDialog;
    }

    private void Awake()
    {
        if (dialogueSoundPlayer == null)
        {
            dialogueSoundPlayer = GetComponent<SoundEffectPlayer>();
        }
    }

    private void TryStartDialog(GameState state)
    {
        if (state != GameState.Dialog) return;
        var dialogDayDetails = dialogData[_gameStateService.CurrentDay - 1];

        _runner = new DialogueRunner(dialogDayDetails.trees[
            dialogDayDetails.trees.Length == 1 ? 0 :
            Random.Range(0, dialogDayDetails.trees.Length)]);
        if (!_runner.Begin())
        {
            Debug.LogError("Диалог не может стартовать: отсутствует стартовый узел или дерево не задано.");
            return;
        }

        ShowCurrentNode();
    }

    public void OnChoiceSelected(int index)
    {
        dialogueSoundPlayer?.Play(choiceSelectSound);
        if (_runner.TryChoose(index))
        {
            ShowCurrentNode();
        }
        else
        {
            Invoke(nameof(DialogueEnd), 0.5f);
        }
    }

    private void DialogueEnd()
    {
        if (_gameStateService.CurrentState != GameState.Dialog) return;
        _gameStateService.SetState(GameState.WaitingForSleep);
        foreach (Transform child in answersParent)
        {
            Destroy(child.gameObject);
        }
        if (MonsterDoor.Instance.isOpen) MonsterDoor.Instance.ToggleDoor(PlayerMovement.Instance.transform.position);
    }

    private void ShowCurrentNode()
    {
        var node = _runner.CurrentNode;
        if (node == null)
        {
            Invoke(nameof(DialogueEnd), 0.5f);
            return;
        }

        dialogText.text = node.Text;
        if (node.OnEnter != null) node.OnEnter.Invoke();
        var choices = _runner.GetChoices();

        foreach (Transform child in answersParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < choices.Count; i++)
        {
            var answer = Instantiate(answerPrefab, answersParent);
            answer.GetComponentInChildren<TMP_Text>().text = choices[i].Text;
            var i1 = i;
            answer.GetComponentInChildren<Button>().onClick.AddListener(() => OnChoiceSelected(i1));
        }
    }
}
