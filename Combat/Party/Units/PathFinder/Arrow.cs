using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    public List<Vector3> validPathPoints;
    public List<Vector3> invalidPathPoints;
    public float lastMousePosition;
    private float validPathPercentage;
    private float totalDistance;

    private NavMeshPath navMeshPath;
    private LineRenderer lineRenderer;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float heightOffset;

    [Header("Path Display")]
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;
    private Gradient colorGradient;

    private Material material;

    [SerializeField] private float lineWidth;
    [SerializeField] private float segmentDensity;
    // range : [0,1]

    [SerializeField] private float speed;

    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private float opacity;
    [SerializeField] private float targetRadius;
    [SerializeField] private float thickness;


    private void Awake()
    {
        colorGradient = new Gradient();
        navMeshPath = new NavMeshPath();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.widthMultiplier = lineWidth;

        lineRenderer.material.SetFloat("_lineWidth", lineWidth);
        lineRenderer.material.SetFloat("_speed", speed);
        lineRenderer.material.SetFloat("_width", width);
        lineRenderer.material.SetFloat("_height", height);
        lineRenderer.material.SetFloat("_opacity", opacity);
        lineRenderer.material.SetFloat("_targetRadius", targetRadius);
        lineRenderer.material.SetFloat("_thicknessCircle", thickness);
    }

    public void CalculatePath(Vector3 startPoint, Vector3 center, Vector3 endPoint, float maxDistance)
    {
        if(IsPathInRange(maxDistance, center, endPoint))
        {
            if (NavMesh.CalculatePath(startPoint, endPoint, NavMesh.AllAreas, navMeshPath) == false)
                return;

            totalDistance = 0f;
            float distanceStep = 0f;

            ClearPath();

            validPathPoints.Add(navMeshPath.corners[0]);
            validPathPercentage = 1f;
            lineRenderer.material.SetColor("_color", validColor);

            for (int i = 1; i < navMeshPath.corners.Length; i++)
            {
                distanceStep = Vector3.Distance(navMeshPath.corners[i - 1], navMeshPath.corners[i]);
                totalDistance += distanceStep;
                validPathPoints.Add(navMeshPath.corners[i]);

                //if (Vector3.Distance(navMeshPath.corners[0], navMeshPath.corners[i]) <= maxDistance)
                //{
                //    totalDistance += distanceStep;
                //    validPathPoints.Add(navMeshPath.corners[i]);
                //}
                //else
                //{
                //    distanceStep = maxDistance - totalDistance;

                //    validPathPoints.Add((navMeshPath.corners[i] - navMeshPath.corners[i - 1]).normalized * distanceStep + navMeshPath.corners[i - 1]);
                //    invalidPathPoints.Add((navMeshPath.corners[i] - navMeshPath.corners[i - 1]).normalized * distanceStep + navMeshPath.corners[i - 1]);
                //    invalidPathPoints.Add(navMeshPath.corners[i]);

                //    totalDistance += Vector3.Distance(navMeshPath.corners[i - 1], navMeshPath.corners[i]);

                //    if (i + 1 < navMeshPath.corners.Length)
                //    {
                //        for (int j = i + 1; j < navMeshPath.corners.Length; j++)
                //        {
                //            totalDistance += Vector3.Distance(navMeshPath.corners[j - 1], navMeshPath.corners[j]);
                //            invalidPathPoints.Add(navMeshPath.corners[j]);
                //        }
                //    }
                //    validPathPercentage = maxDistance / totalDistance;
                //    lineRenderer.material.SetColor("_color", invalidColor);
                //    break;
                //}
            }
        }
        else
        {
            ClearPath();
        }
    }
    public bool IsPathInRange(float range, Vector3 center, Vector3 endPoint)
    {
        return Vector3.Distance(center, endPoint) <= range;
    }

    public void RenderPath(float range) 
    {
        if(validPathPoints.Count > 0)
        {
            lineRenderer.enabled = true;

            lineRenderer.material.SetFloat("_segmentDensity", segmentDensity * totalDistance * -1f);
            lineRenderer.material.SetFloat("_endSegment", lineWidth / (totalDistance + 0.5f * lineWidth));

            lineRenderer.colorGradient = colorGradient;

            if (totalDistance < lineWidth / 2)
            {
                lineRenderer.enabled = false;
            }
            else
            {
                lineRenderer.positionCount = validPathPoints.Count + 1;
                for (int i = 0; i < validPathPoints.Count; i++)
                {
                    lineRenderer.SetPosition(i, new Vector3(validPathPoints[i].x, validPathPoints[i].y + heightOffset, validPathPoints[i].z));
                }
                Vector3 lastPosition = (validPathPoints[validPathPoints.Count - 1] - validPathPoints[validPathPoints.Count - 2]).normalized * (lineWidth / 2) + validPathPoints[validPathPoints.Count - 1];
                lineRenderer.SetPosition(validPathPoints.Count, new Vector3(lastPosition.x, lastPosition.y + heightOffset, lastPosition.z));

                colorGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(validColor, 0.0f), new GradientColorKey(validColor, 1f) },
                                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
        
        //AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0.4f)
        //    , new Keyframe(arrowStartLength, 0.4f)
        //    , new Keyframe(arrowStartLength + 0.01f, 1f)
        //    , new Keyframe(1, 0f));
        //lineRenderer.widthCurve = curve;

        //lineRenderer.positionCount = validPathPoints.Count + invalidPathPoints.Count - 1;
        //for (int i = 0; i < validPathPoints.Count; i++)
        //{
        //    lineRenderer.SetPosition(i, new Vector3(validPathPoints[i].x, validPathPoints[i].y + heightOffset, validPathPoints[i].z));
        //}
        //for (int i = validPathPoints.Count; i < validPathPoints.Count + invalidPathPoints.Count - 1; i++)
        //{
        //    lineRenderer.SetPosition(i, new Vector3(invalidPathPoints[i-validPathPoints.Count+1].x, invalidPathPoints[i-validPathPoints.Count+1].y + heightOffset, 
        //                                            invalidPathPoints[i-validPathPoints.Count+1].z));
        //}
        //colorGradient.SetKeys(new GradientColorKey[] { new GradientColorKey(validColor, 0.0f), new GradientColorKey(validColor, validPathPercentage), 
        //                                                new GradientColorKey(invalidColor, validPathPercentage + 0.02f), new GradientColorKey(invalidColor, 1f) },
        //                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, validPathPercentage), 
        //                                                new GradientAlphaKey(1f, validPathPercentage + 0.02f), new GradientAlphaKey(1f, 1f) });
    }

    // todo: improve performance by partial clear
    public void ClearPath() 
    {
        lineRenderer.enabled = false;
        validPathPoints.Clear();
        invalidPathPoints.Clear();
    }

    public void EvaluateMousePosition(float mosuePosition) { }

    private void SetShader(float pathLength)
    {
    }


    public void Update()
    {

    }
}
