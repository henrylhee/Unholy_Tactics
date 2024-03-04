using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Tooltip tooltip;
    [SerializeField] [TextArea] public String tooltipText;
    [SerializeField] private float showTooltipDelay = 0.34f;

    private bool TooltipQueued = false;
    private bool isHovered = false;
    
    public void Start()
    {
        if (tooltip == null)
            tooltip = GetComponentInChildren<Tooltip>();
        tooltip.IsActive = false;
        tooltip.Setup();
        tooltip.text.text = tooltipText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    { 
        Invoke(nameof(ShowTooltip), showTooltipDelay);
        isHovered = true;
    }

    private void ShowTooltip()
    {
        if (isHovered)
            tooltip.IsActive = true;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        tooltip.IsActive = false;
    }
}
