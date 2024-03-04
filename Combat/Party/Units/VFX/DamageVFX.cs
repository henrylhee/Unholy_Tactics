using TMPro;
using UnityEngine;

public enum DamageVFXType { Damage, Healing, Crit }
public class DamageVFX : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private GameObject dmgNumber;
    [SerializeField]
    private Color dmgColor;
    [SerializeField]
    private Color healColor;
    [SerializeField]
    private Color critColor;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        dmgNumber.transform.rotation = Camera.main.transform.rotation;
    }

    public void PlayAnimation(int number, DamageVFXType type)
    {
        dmgNumber.GetComponent<TextMeshPro>().text = number.ToString();

        Color color = new Color();

        switch (type)
        {
            case DamageVFXType.Damage:
                color = dmgColor;
                break;
            case DamageVFXType.Healing:
                color = healColor;
                break;
            case DamageVFXType.Crit:
                color = critColor;
                break;
        }

        dmgNumber.GetComponent<TextMeshPro>().color = color;
        animator.SetBool("InAnimation", true);
    }

    public void AnimationEnd()
    {
        animator.SetBool("InAnimation", false);
    }
}
