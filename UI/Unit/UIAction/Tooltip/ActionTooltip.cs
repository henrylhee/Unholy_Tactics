using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class ActionTooltip : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI name_;
    [SerializeField]
    private TextMeshProUGUI description;

    private RectTransform rect;
    [SerializeField]
    private RectTransform statRect;

    private Camera cam;
    private Vector3 min, max;

    [SerializeField]
    private Image unusableBanner;
    [SerializeField]
    private TooltipCosts costs;
    [SerializeField]
    private Image icon;
    
    [Header("Stats")]

    [SerializeField]
    public ActionStat damageStat;
    [SerializeField]
    public ActionStat manaCost;
    [SerializeField]
    public ActionStat soulCost;
    [SerializeField]
    public ActionStat healthCost;
    [SerializeField]
    private ActionStat hitChance;
    [SerializeField]
    private ActionStat healStat;
    [SerializeField] 
    private ActionStat cooldownStat;

    private void Awake()
    {
        cam = Camera.main;
        min = new Vector3(0, 0, 0);
        max = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);
    }

    public void SetUp(Action action, Unit playerUnit, UnitUI ui)
    {
        rect = GetComponent<RectTransform>();
        name_.text = action.name;
        icon.sprite = action.icon;
        description.text = action.description;

        transform.SetParent(ui.transform);
        UpdateStats(action, playerUnit);
        costs.SetUp(action);
    }

    public void UpdateStats(Action action, Unit caster)
    {
        foreach(Transform t in statRect)
        {
            Destroy(t.gameObject, 0);
        }

        if(action.GetMaxDamage(caster) > 0)
        {
            SpawnStat(damageStat, Mathf.RoundToInt(action.GetMinDamage(caster)), Mathf.RoundToInt(action.GetMaxDamage(caster)));
        }
        SpawnStat(hitChance, action.GetHitChance(caster), 0, "%");
       // SpawnStat(manaCost, action.GetManaCost());
        //SpawnStat(soulCost, action.GetSoulCost());
        SpawnStat(healStat, action.GetHealing(caster));
       // SpawnStat(healthCost, action.GetHealthCost());
        SpawnStat(cooldownStat, action.GetCooldown());
        
        statRect.gameObject.SetActive(statRect.childCount > 0);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowUnusable(Action action, Party p, Unit caster)
    {
        unusableBanner.gameObject.SetActive(true);
        costs.UpdateVisibleUsability(action,p,caster);
    }

    public void ShowUsable(Action action, Party p, Unit caster)
    {
        unusableBanner.gameObject.SetActive(false);
        costs.UpdateVisibleUsability(action, p, caster);
    }

    private void SpawnStat(ActionStat stat, float value, float valueTwo = 0, string extraSign = "")
    {
        if(value > 0)
        {
            ActionStat instance = Instantiate(stat);
            instance.SetUp(value, valueTwo, extraSign);
            instance.transform.SetParent(statRect);
        }

        statRect.ForceUpdateRectTransforms();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            Vector3 corner = new Vector3(rect.rect.width / 2, rect.rect.height / 2);
            transform.position = corner + Input.mousePosition;

            var position = rect.position;
            transform.position = new Vector3(Mathf.Clamp(position.x, min.x + rect.rect.width / 2, max.x - rect.rect.width / 2), Mathf.Clamp(position.y, min.y + rect.rect.height / 2, max.y - rect.rect.height / 2), transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}
