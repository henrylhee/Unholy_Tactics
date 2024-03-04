using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialInformations : MonoBehaviour
{
    public UnityEvent End;

    [SerializeField]
    private List<TutorialHints> hints;

    private int currentHint;

    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private Image image;

    [SerializeField]
    private Button previousButton;
    [SerializeField]
    private Button nextButtonText;
    [SerializeField]
    private Button finishButton;
   
    private void Start()
    {
        UpdateHintDisplay();
    }

    public void StartTutorial()
    {
        currentHint = 0;
        UpdateHintDisplay();
    }

    public void UpdateHintDisplay()
    {
        title.text = hints[currentHint].title;
        description.text = hints[currentHint].description;
        image.sprite = hints[currentHint].image;

        previousButton.interactable = (currentHint != 0);
        nextButtonText.interactable = (currentHint != hints.Count - 1);
        finishButton.interactable = (currentHint == hints.Count - 1);
    }

    public void NextHint()
    {
        currentHint++;
        UpdateHintDisplay();
    }

    public void PreviousHint()
    {
        currentHint--;
        UpdateHintDisplay();
    }

    public void FinishTutorial()
    {
        End?.Invoke();
    }
}

[System.Serializable]
public class TutorialHints
{
    public string title;
    public string description;
    public Sprite image;
}