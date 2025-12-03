using DialogueSystem.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueExample : MonoBehaviour
{
    [SerializeField] private DialogueTree tree;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject answerPrefab;
    [SerializeField] private Transform answersParent;
    private DialogueRunner runner;

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
            Debug.Log("Диалог завершён или выбран неверный индекс.");
        }
    }

    private void ShowCurrentNode()
    {
        var node = runner.CurrentNode;
        if (node == null)
        {
            Debug.Log("Диалог завершён.");
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