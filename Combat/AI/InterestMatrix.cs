using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterestMatrix : MonoBehaviour
{
    public static InterestMatrix instance;

    [SerializeField]
    private Vector3 matrixPosition;
    [SerializeField]
    private Vector3 matrixSize;
    [SerializeField]
    private float matrixDensity;

    private List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>();

    private void Awake()
    {
        if(InterestMatrix.instance == null)
        {
            InterestMatrix.instance = this;
            return;
        }

        Destroy(this);
    }

    private void Start()
    {
        GenerateMatrix();
    }

    private void GenerateMatrix()
    {
        Vector3 startPosition = matrixPosition;
        startPosition.x -= matrixSize.x / 2;
        startPosition.y -= matrixSize.y / 2;
        startPosition.z -= matrixSize.z / 2;

        for (int x = 0; x < matrixDensity; x++)
        {
            for(int z = 0;z < matrixDensity; z++)
            {
                float PercentX = x / matrixDensity;
                float PercentZ = z / matrixDensity;

                Vector3 spawnPosition = startPosition;
                spawnPosition.x += matrixSize.x * PercentX;
                spawnPosition.z += matrixSize.z * PercentZ;

                GameObject spawn = new GameObject();
                spawn.AddComponent<PointOfInterest>();
                spawn.name = x.ToString() + " | " + z.ToString();
                spawn.transform.position = spawnPosition;

                spawn.transform.SetParent(this.transform);

                pointsOfInterest.Add(spawn.GetComponent<PointOfInterest>());
            }
        }
    }

    public InterestData GetBestSaveArea(Vector3 origin, float radius, Unit caster, List<Unit> enemies)
    {
        List<InterestData> coverPoints = new List<InterestData>();

        foreach (PointOfInterest point in pointsOfInterest)
        {
            point.ClearDebug();
            if(Vector3.Distance(origin, point.transform.position) < radius)
            {
                InterestData data = new InterestData();
                data.point = point;
                data.coverCount = point.GetCoverCount(caster, enemies);
                coverPoints.Add(data);
            }
        }

        float highestCoverCount = 0;
        foreach(InterestData data in coverPoints)
        {
            if(highestCoverCount < data.coverCount)
            {
                highestCoverCount = data.coverCount;
            }
        }

        InterestData closestData = null;
        float shortestDistance = Mathf.Infinity;

        foreach(InterestData data in coverPoints)
        {
            if (!caster.CanReach(data.point.transform.position))
            {
                continue;
            }

            foreach (Unit target in enemies)
            {
                if (data.coverCount >= highestCoverCount)
                {
                    if (closestData == null)
                    {
                        closestData = data;
                        continue;
                    }

                    float distance = caster.GetWalkDistance(data.point.transform.position, target.transform.position);

                    if (distance < shortestDistance)
                    {
                        closestData = data;
                        shortestDistance = distance;
                    }
                }
            }
        }

        return closestData;
/*
        InterestData best = null;
        float closestDistance = Mathf.Infinity;
        float highestCoverCount = 0;

        foreach (InterestData data in sortedByRisk)
        {
            if (data.coverCount >= highestCoverCount)
            {
                highestCoverCount = data.coverCount;

                foreach (Unit enemy in enemies)
                {
                    float distance = Vector3.Distance(enemy.transform.position, data.point.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        best = data;
                    }
                }
            }
        }

        return best;
*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(matrixPosition, matrixSize);
    }
}

public class InterestData
{
    public PointOfInterest point;
    public float coverCount;
}