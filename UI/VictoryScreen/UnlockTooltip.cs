using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockTooltip : MonoBehaviour
{
    [SerializeField]
    private TooltipCosts costs;
    [SerializeField]
    private TextMeshProUGUI abilityDescription;
    [SerializeField]
    private TextMeshProUGUI abilityName;
    [SerializeField]
    private Image abilityIcon;


    [SerializeField]
    private TextMeshProUGUI unitName;
    [SerializeField]
    private Image unitIcon;
    [SerializeField]
    private List<Sprite> unitIcons = new List<Sprite>();

    public void SetUp(UnlockData data)
    {
        costs.SetUp(data.action);

        abilityDescription.text = data.action.description;
        abilityName.text = data.action.name;
        abilityIcon.sprite = data.action.icon;

        unitName.text = UnitTypeToName(data.unit);
        unitIcon.sprite = unitIcons[(int)data.unit];

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private string UnitTypeToName(UnitType type)
    {
        switch (type)
        {
            case UnitType.Support:
                return "Slime";
            case UnitType.Main:
                return "Queen";
            case UnitType.Mage:
                return "Lich";
            case UnitType.Melee:
                return "Ghoul";
            case UnitType.Range:
                return "Archer";
        }

        return "";
    }
}
