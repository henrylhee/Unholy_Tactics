using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AttackPath : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] private float pathWidth;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;

    [SerializeField] private float segmentDensity;
    [SerializeField] private float speed;
    [SerializeField] private float gap;



    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
        lineRenderer.widthMultiplier = pathWidth + pathWidth * gap;
        lineRenderer.material.SetFloat("_segmentDensity", segmentDensity);
        lineRenderer.material.SetFloat("_gap", gap);
        lineRenderer.material.SetFloat("_speed", speed);
    }

    public void RenderPath(Vector3 startPos, Vector3 endPos, bool targetValid)
    {
        if (targetValid)
        {
            lineRenderer.material.SetColor("_color", validColor);

        }
        else
        {
            lineRenderer.material.SetColor("_color", invalidColor);
        }
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
        lineRenderer.material.SetFloat("_lineLength", Vector3.Distance(startPos, endPos));
    }


    public void ClearPath()
    {
        lineRenderer.enabled = false;
    }
}
