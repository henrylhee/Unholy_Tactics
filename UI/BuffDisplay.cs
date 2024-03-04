using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BuffDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Tooltip tooltip;
    private Image icon;
    public Sprite Icon
    {
        set
        {
            icon.sprite = value;
        }
    }
    private TextMeshProUGUI turnText;

    public int RemainingTurns
    {
        set
        {
            turnText.text = value.ToString();
            tooltip.Description = tooltip.BaseDescription.Replace("_Duration_", value.ToString());
        }
    }

    public string Description
    {
        set => tooltip.Description = value;
        get => tooltip.Description;
    }

    public void Start()
    {
        if (icon == null)
            Setup();
    }

    public void Setup()
    {
        turnText = GetComponentInChildren<TextMeshProUGUI>();
        icon = GetComponent<Image>();
        if (tooltip == null)
            tooltip = GetComponentInChildren<Tooltip>();
        tooltip.IsActive = false;
        tooltip.Setup();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.IsActive = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.IsActive = false;
    }
}
