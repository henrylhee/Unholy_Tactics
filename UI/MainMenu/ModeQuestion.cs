using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModeQuestion : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<bool> modeSelected;

    [SerializeField]
    private GameObject confirmation;
    [SerializeField]
    private TextMeshProUGUI modeConfirmationText;
    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Image adventureButton;
    [SerializeField]
    private Image demoButton;
    [SerializeField]
    private Button confirmationButton;

    private enum mode { nothing, adventure, demo }
    private mode m = mode.nothing;
    private string startText;

    private void Start()
    {
        startText = modeConfirmationText.text;
        confirmationButton.interactable = false;
        confirmation.gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void AdventureMode()
    {
        adventureButton.color = selectedColor;
        demoButton.color = Color.white;
        confirmationButton.interactable = true;
        m = mode.adventure;
    }

    public void DemoMode()
    {
        demoButton.color = selectedColor;
        adventureButton.color = Color.white;
        confirmationButton.interactable = true;
        m = mode.demo;
    }

    public void Confirm()
    {
        switch (m)
        {
            case mode.nothing:
                return;
            case mode.adventure:
                modeConfirmationText.text = startText.Replace("mode name", "adventure mode");
                break;
            case mode.demo:
                modeConfirmationText.text = startText.Replace("mode name", "demo mode");
                break;
        }

        confirmation.gameObject.SetActive(true);
    }

    public void ConfirmationYes()
    {
        Hide();
        confirmation.SetActive(false);
        modeSelected.Invoke(m == mode.demo);
    }
    
    public void ConfirmationNo()
    {
        confirmation.gameObject.SetActive(false);
    }
}
