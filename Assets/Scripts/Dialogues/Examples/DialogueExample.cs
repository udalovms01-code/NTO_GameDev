using DialogueSystem.Runtime;
using TMPro;
using UnityEngine;

public class DialogueExample : MonoBehaviour
{
    [SerializeField] private DialogueTree tree;
    [SerializeField] private TMP_Text[] choiceTexts;
    [SerializeField] private TMP_Text dialogText;
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
        Debug.Log($"Реплика: {node.Text}");
        if (node.OnEnter != null) node.OnEnter.Invoke();
        var choices = runner.GetChoices();
        
        for (var i = 0; i < choiceTexts.Length; i++)
        {
            choiceTexts[i].gameObject.SetActive(i < choices.Count);
        }
        for (var i = 0; i < choices.Count; i++)
        {
            choiceTexts[i].text = choices[i].Text;
            Debug.Log($"{i}: {choices[i].Text}");
        }
    }
}