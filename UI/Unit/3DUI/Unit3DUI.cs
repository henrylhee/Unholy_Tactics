using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Unit3DUI : MonoBehaviour
{
    [SerializeField] 
    private HealthBar healthBar;
    [SerializeField]
    private PreviewHealthbar previewHealthbar;

    [SerializeField]
    private Image armor;
    [SerializeField]
    private UnitLevel level;

    [SerializeField]
    private GameObject head;

    private Vector3 startOffset;
    private Vector3 originalPosition;

    private BuffBar buffBar;

    private void Awake()
    {
        buffBar = healthBar.GetComponentInChildren<BuffBar>();
    }

    private void Start()
    {
        startOffset = transform.position - head.transform.position;
        originalPosition = armor.transform.localPosition;
    }

    private void Update() 
    {
        RotateObjects();
    }

    public void SetUp(Unit unit, int currentHealth, int maxHealth, string ImportID, int lifetimeLimit = 0)
    {
        unit.health.HealthChanged.AddListener(AdjustHealthBar);
        unit.stats.StatsUpdated.AddListener(UpdateArmorValue);

        unit.OnApplyBuff.AddListener(AddUIBuff);
        unit.OnRemoveBuff.AddListener(RemoveBuff);
        unit.stats.StatsUpdated.AddListener(UpdateArmorValue);

        AdjustHealthBar(currentHealth, maxHealth);
        UpdateArmorValue(unit.stats);

        if (ImportID.Contains("Enemy"))
        {
            level.SetLevel(1);
        }

        if(ImportID.Contains("2"))
        {
            level.SetLevel(2);
        }
        else if (ImportID.Contains("3"))
        {
            level.SetLevel(3);
        }
        else if (ImportID.Contains("4"))
        {
            level.SetLevel(4);
        }
        else if (ImportID.Contains("5")) 
        {
            level.SetLevel(5);
        }
    }

    public void AdjustHealthBar(int currentHP, int maxHp)
    {
        healthBar.UpdateHealth(currentHP, maxHp);
    }

    public void AdjustPreviewHealthbar(float maxhealth, float currentHealth, float minDamage, float maxDamage)
    {
        previewHealthbar.UpdateValue(maxhealth, currentHealth, minDamage, maxDamage);
    }


    public void Clear()
    {
        healthBar.gameObject.SetActive(false);
        armor.gameObject.SetActive(false);
    }

    //public void UpdateDeathCounter(int counter)
    //{
    //    lifeBar.SetSkulls(counter);
    //}

    public void UpdateArmorValue(UnitStats stats)
    {
        float percent = stats.armorFactor + stats.buffArmorFactor;

        armor.gameObject.SetActive(percent > 0);
        armor.transform.localPosition = originalPosition;
        armor.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.RoundToInt(percent).ToString();
    }

    public void AddUIBuff(Buff buff)
    {
        buffBar.AddBuff(buff);
    }

    public void RemoveBuff(Buff buff)
    {
        buffBar.RemoveBuff(buff);
    }



    private void RotateObjects()
    {
        transform.rotation = Camera.main.transform.rotation;
        //foreach(Transform t in transform)
        //{
        //    t.rotation = Camera.main.transform.rotation;
        //}
    }
}
