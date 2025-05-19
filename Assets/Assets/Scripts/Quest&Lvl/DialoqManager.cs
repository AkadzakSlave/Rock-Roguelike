using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("UI Elements")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public Button continueButton;

    private Dialog currentDialog;
    private int currentLine;
    private UnityAction onDialogComplete;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        dialogPanel.SetActive(false);
        continueButton.onClick.AddListener(ContinueDialog);
    }

    public IEnumerator ShowDialog(Dialog dialog, UnityAction callback = null)
    {
        currentDialog = dialog;
        currentLine = 0;
        onDialogComplete = callback;

        dialogPanel.SetActive(true);
        yield return StartCoroutine(TypeText(dialog.lines[currentLine]));
    }

    IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void ContinueDialog()
    {
        currentLine++;
        if (currentLine < currentDialog.lines.Count)
        {
            StartCoroutine(TypeText(currentDialog.lines[currentLine]));
        }
        else
        {
            dialogPanel.SetActive(false);
            onDialogComplete?.Invoke();
        }
    }
}

[System.Serializable]
public class Dialog
{
    public List<string> lines;
    public string[] Lines { get; set; }
}
