using DialogueSystem.Runtime;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DialogueExample : MonoBehaviour
{
    [SerializeField] private DialogueTree tree;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject answerPrefab;
    [SerializeField] private Transform answersParent;
    [SerializeField] private DoorTo2DInteract doorTo2DInteract;
    
    private DialogueRunner runner;

    private GameStateService _gameStateService;
    
    [Inject]
    public void Construct(GameStateService gameStateService)
    {
        _gameStateService = gameStateService;
    }

    private void Start()
    {
        runner = new DialogueRunner(tree);
        if (!runner.Begin())
        {
            Debug.LogError("Диалог не может стартовать: отсутствует стартовый узел или дерево не задано.");
            return;
        }

        ShowCurrentNode();
    }

    public void OnChoiceSelected(int index)
    {
        if (runner.TryChoose(index))
        {
            ShowCurrentNode();
        }
        else
        {
            DialogueEnd();
        }
    }
    
    private void DialogueEnd()
    {
        if (_gameStateService.IsDialogEnded) return;
        
        foreach (Transform child in answersParent)
        {
            Destroy(child.gameObject);
        }
        
        _gameStateService.SetDialogEnded(true);
        if(doorTo2DInteract.isOpen) doorTo2DInteract.ToggleDoor(PlayerMovement.Instance.transform.position);
    }

    private void ShowCurrentNode()
    {
        var node = runner.CurrentNode;
        if (node == null)
        {
            DialogueEnd(); 
            return;
        }

        dialogText.text = node.Text;
        if (node.OnEnter != null) node.OnEnter.Invoke();
        var choices = runner.GetChoices();
        
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