using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;

public class ActionRange : MonoBehaviour
{
    [SerializeField]
    private GameObject moveRangeTemplate;
    [SerializeField]
    private GameObject actionRangeTemplate;
    [SerializeField]
    private GameObject movementMaskTemplate;
    [SerializeField]
    private GameObject actionMaskTemplate;
    [SerializeField]
    private Vector3 rangeOffset = new Vector3(0, 0.1f, 0);

    private GameObject activeMovementTemplate;
    private GameObject activeActionTemplate;
    private GameObject activeMovementMask;
    private GameObject activeActionMask;
    
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private int iterations = 2;
    [SerializeField] private int debugStartIndex = -1;

    private Mesh moveArea;
    private Mesh actionArea;
    private NavMeshTriangulation triangulation;
    private Vector3 startPosition;

    private Mesh generatedNavmesh = null;


    public void GenerateMoveRange(float radius, Vector3 position)
    {
        if (activeMovementTemplate == null)
        {
            SetupMovementRange(radius, position);
        }
        ActivateMovementRange();
    }

    public void GenerateActionRange(float radius, Vector3 position)
    {
        if(activeActionTemplate == null)
        {
            SetupActionRange(radius, position);
        }
        ActivateActionRange();
    }

    public void UpdateActionRange(Vector3 position)
    {
        if(activeActionMask == null)
        {
            return;
        }

        activeActionMask.transform.position = position;
    }

    private void SetupMovementRange(float radius, Vector3 position)
    {
        triangulation = NavMesh.CalculateTriangulation();
        moveArea = new Mesh();
        moveArea.SetVertices(triangulation.vertices);
        moveArea.SetIndices(triangulation.indices, MeshTopology.Triangles, 0);

        activeMovementTemplate = GenerateRangeTemplate(radius, position);
        
        activeMovementMask = Instantiate(movementMaskTemplate);
        activeMovementMask.GetComponent<MeshFilter>().mesh = SurfaceHelper.Get2DCircle(radius, position + rangeOffset*2, 60);
        //actionRangeTemplate.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_EdgeIntern", actionRangeTemplate.GetComponent<MeshRenderer>().sharedMaterial.GetFloat("_EdgeSize") / radius);
    }

    private void SetupActionRange(float radius, Vector3 position)
    {
        triangulation = NavMesh.CalculateTriangulation();
        actionArea = new Mesh();
        actionArea.SetVertices(triangulation.vertices);
        actionArea.SetIndices(triangulation.indices, MeshTopology.Triangles, 0);

        activeActionTemplate = Instantiate(actionRangeTemplate);
        activeActionTemplate.GetComponent<MeshFilter>().mesh = actionArea;
        activeActionTemplate.GetComponent<MeshCollider>().sharedMesh = actionArea;

        activeActionMask = Instantiate(actionMaskTemplate);
        activeActionMask.GetComponent<MeshFilter>().mesh = SurfaceHelper.Get2DCircleWithUVs(radius, new Vector3(0, rangeOffset.y, 0), 60);
    }
    
    public void ActivateMovementRange()
    {
        //Debug.Log("activate movement range");
        if(activeMovementMask != null)
        {
            activeMovementMask.SetActive(true);

        }
        if(activeMovementTemplate != null)
        {
            activeMovementTemplate.SetActive(true);

        }
    }

    public void ActivateActionRange()
    {
        activeActionMask.SetActive(true);
        activeActionTemplate.SetActive(true);
    }

    public void ClearActionRange()
    {
        if(activeActionTemplate != null)
        {
            Destroy(activeActionTemplate.gameObject);
            activeActionTemplate = null;
        }
        if(activeActionMask != null)
        {
            Destroy(activeActionMask.gameObject);
            activeActionMask = null;
        }
        if(actionArea != null)
        {
            Destroy(actionArea);
            actionArea = null;
        }
    }

    public void ClearMoveRange()
    {
        if (moveArea != null)
        {
            Destroy(moveArea);
            moveArea = null;
        }

        if (activeMovementTemplate != null)
        {
            Destroy(activeMovementTemplate.gameObject);
            activeMovementTemplate = null;
        }
        if (activeMovementMask != null)
        {
            Destroy(activeMovementMask.gameObject);
            activeMovementMask = null;
        }
    }

    public void DeactivateMoveRange()
    {
        if(activeMovementMask != null)
        {
            activeMovementMask.SetActive(false);
        }
        if (activeMovementTemplate != null)
        {
            activeMovementTemplate.SetActive(false);
        }
    }

    public void DeactivateActionRange()
    {
        if (activeActionMask != null)
        {
            activeActionMask.SetActive(false);
        }
        if (activeActionTemplate != null)
        {
            activeActionTemplate.SetActive(false);
        }
    }

    private GameObject GenerateRangeTemplate(float radius, Vector3 position)
    {
        GameObject result = Instantiate(moveRangeTemplate);

        result.GetComponent<MeshFilter>().mesh = moveArea;
        result.GetComponent<MeshCollider>().sharedMesh = moveArea;
        result.transform.position += rangeOffset;

        return result;
    }
    
    float sign (Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    bool IsPointInTriangle (Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        d1 = sign(pt, v1, v2);
        d2 = sign(pt, v2, v3);
        d3 = sign(pt, v3, v1);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    public static bool IsLineIntersectingCircle(Vector2 point1, Vector2 point2, Vector2 circleCenter, double circleRadius)
    {
        // Check if either endpoint lies inside the circle.
        bool isEndpoint1InsideCircle = IsPointInsideCircle(point1.x, point1.y, circleCenter.x, circleCenter.y, circleRadius);
        bool isEndpoint2InsideCircle = IsPointInsideCircle(point2.x, point2.y, circleCenter.x, circleCenter.y, circleRadius);

        if (isEndpoint1InsideCircle || isEndpoint2InsideCircle)
        {
            return true;
        }

        return false;
        
    }

    // Helper function to check if a point is inside a circle.
    private static bool IsPointInsideCircle(double x, double y, double circleCenterX, double circleCenterY, double circleRadius)
    {
        double distanceSquared = (x - circleCenterX) * (x - circleCenterX) + (y - circleCenterY) * (y - circleCenterY);
        return distanceSquared <= circleRadius * circleRadius;
    }


    private Mesh GetCircleMesh(NavMeshTriangulation triangulation, Vector3 position, float radius)
    {
        Mesh circleMesh = new Mesh();
        var reachableVertices = new List<Vector3>();
        var reachableIndices = new List<int>();
        var indiciesTranslation = new Dictionary<int, int>();

        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            if (!IsPointInsideCircle(triangulation.vertices[triangulation.indices[i]].x, triangulation.vertices[triangulation.indices[i]].y, position.x, position.y, radius) &&
                 !IsPointInsideCircle(triangulation.vertices[triangulation.indices[i+1]].x, triangulation.vertices[triangulation.indices[i+1]].y, position.x, position.y, radius) && 
                  !IsPointInsideCircle(triangulation.vertices[triangulation.indices[i+2]].x, triangulation.vertices[triangulation.indices[i+2]].y, position.x, position.y, radius)
                && !SurfaceHelper.Collision(radius, new Vector2(position.x, position.z),
                    new Vector2(triangulation.vertices[triangulation.indices[i]].x,
                        triangulation.vertices[triangulation.indices[i]].z),
                    new Vector2(triangulation.vertices[triangulation.indices[i + 1]].x,
                        triangulation.vertices[triangulation.indices[i + 1]].z)) &&
                !SurfaceHelper.Collision(radius, new Vector2(position.x, position.z),
                    new Vector2(triangulation.vertices[triangulation.indices[i + 1]].x,
                        triangulation.vertices[triangulation.indices[i + 1]].z),
                    new Vector2(triangulation.vertices[triangulation.indices[i + 2]].x,
                        triangulation.vertices[triangulation.indices[i + 2]].z)) &&
                !SurfaceHelper.Collision(radius, new Vector2(position.x, position.z),
                    new Vector2(triangulation.vertices[triangulation.indices[i + 2]].x,
                        triangulation.vertices[triangulation.indices[i + 2]].z),
                    new Vector2(triangulation.vertices[triangulation.indices[i]].x,
                        triangulation.vertices[triangulation.indices[i]].z)))
                continue;
            for (int j = i; j < i + 3; j++)
            {
                if (!reachableIndices.Contains(triangulation.indices[j]))
                {
                    reachableVertices.Add(triangulation.vertices[triangulation.indices[j]]);
                    indiciesTranslation.Add(triangulation.indices[j],
                        indiciesTranslation.Values.Count > 0 ? indiciesTranslation.Values.Max() + 1 : 0);
                }
                reachableIndices.Add(triangulation.indices[j]);
            }
        }

        for (int i = 0; i < reachableIndices.Count; i++)
        {
            reachableIndices[i] = indiciesTranslation[reachableIndices[i]];
        }

        circleMesh.SetVertices(reachableVertices);
        circleMesh.SetIndices(reachableIndices, MeshTopology.Triangles, 0);
        
        return circleMesh;
    }
    
    private List<List<List<int>>> GetAreas(Mesh triangulation)
    {
        var areas = new List<List<List<int>>>();
        var tempTri = new List<int>();
        int k = -1;
        
        for (int j = 0; j < triangulation.triangles.Length; j++)
        {
            tempTri.Add(triangulation.triangles[j]);
            if ((j + 1) % 3 != 0) 
                continue;
            var testFind = areas.Count > 0 ? areas[k].Find(t => t.Contains(tempTri[0]) || t.Contains(tempTri[1]) || t.Contains(tempTri[2])) : null;
            if (testFind == null)
            {
                areas.Add(new List<List<int>>());
                k++;
            }
            areas[k].Add(new List<int>(tempTri));
            tempTri.Clear();
        }
        
        return areas;
    }

    private int CalcStartIndx(List<List<List<int>>> areas, Mesh triangulation)
    {
        int startIndx = 0;
        var parentPosition = transform.parent.position;
        var minDistance =
            areas.Min(area =>
                area.Min(triangle =>
                    triangle.Min(point =>
                        Vector3.Distance(triangulation.vertices[point], parentPosition)
                    )
                )
            );
        foreach (var area in areas)
        {
            bool containsPlayer = false;
            foreach (var triangle in area)
            {
                foreach (var point in triangle)
                {
                    if ( Math.Abs(Vector3.Distance(triangulation.vertices[point], parentPosition) - minDistance) < 0.001f)
                    {
                        containsPlayer = true;
                    }
                }
            }
            if (containsPlayer)
                break;
            startIndx++;
        }

        return startIndx;
    }

    private void CalcReachableMesh(List<List<List<int>>> areas, int startIndx, Vector3 position, float radius, Mesh triangulation)
    {
        Debug.Log("Generating Action Range Mesh at position " + position);

        generatedNavmesh = null;
        StartCoroutine(delay());
        IEnumerator delay()
        {
            var reachableVertices = new List<Vector3>();
            var reachableIndices = new List<int>();
            var indiciesTranslation = new Dictionary<int, int>();
            var addedAreasindices = new List<int>() { startIndx };
            Mesh navmesh = new Mesh();

            foreach (var triangle in areas[startIndx])
            {
                foreach (var index in triangle)
                {
                    if (!reachableIndices.Contains(index))
                    {
                        reachableVertices.Add(triangulation.vertices[index]);
                        indiciesTranslation.Add(index, indiciesTranslation.Values.Count > 0 ? indiciesTranslation.Values.Max() + 1 : 0);
                    }
                    reachableIndices.Add(index);
                }
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                yield return new WaitForEndOfFrame();
                for (int i = 0; debugStartIndex < 0 && i < areas.Count; i++)
                {
                    if (addedAreasindices.Contains(i))
                        continue;

                    bool isCloseEnough = false;
                    foreach (var triangle in areas[i])
                    {
                        // Debug only, todo: remove
                        Vector3 closeEnoughIndi;
                        var testDistances = new List<float>();
                        // Debug end

                        foreach (var idx in triangle)
                        {
                            var vertexToTest = new Vector2(triangulation.vertices[idx].x, triangulation.vertices[idx].z);

                            closeEnoughIndi = reachableVertices.Find(v =>
                            {
                                var reachableVertex = new Vector2(v.x, v.z);
                                var distanceToReachableVertex = Vector2.Distance(reachableVertex, vertexToTest);

                                testDistances.Add(distanceToReachableVertex);
                                return distanceToReachableVertex <= distanceThreshold;
                            });
                            if (closeEnoughIndi != Vector3.zero)
                            {
                                isCloseEnough = true;
                                break;
                            }
                        }
                    }

                    if (!isCloseEnough)
                        continue;

                    foreach (var triangle in areas[i])
                    {
                        foreach (var index in triangle)
                        {
                            if (!reachableIndices.Contains(index))
                            {
                                reachableVertices.Add(triangulation.vertices[index]);
                                indiciesTranslation.Add(index,
                                    indiciesTranslation.Values.Count > 0 ? indiciesTranslation.Values.Max() + 1 : 0);
                            }

                            reachableIndices.Add(index);
                        }
                    }

                    addedAreasindices.Add(i);
                }

            }

            for (int i = 0; i < reachableIndices.Count; i++)
            {
                reachableIndices[i] = indiciesTranslation[reachableIndices[i]];
            }

            navmesh.SetVertices(reachableVertices);
            navmesh.SetIndices(reachableIndices, MeshTopology.Triangles, 0);


            generatedNavmesh = navmesh;
        }
    }

    static bool intersectsCircle(int a, int b, int c,
        int x, int y, int radius)
    {
        // Finding the distance of line from center.
        double dist = (Math.Abs(a * x + b * y + c)) /
                      Math.Sqrt(a * a + b * b);

        return dist <= radius;
    }
    
    private void GenerateNavMesh(float radius, Vector3 position)
    {
        triangulation = NavMesh.CalculateTriangulation();
        var circleMesh = GetCircleMesh(triangulation, position, radius);
        var areas = GetAreas(circleMesh);
        var startIndx = CalcStartIndx(areas, circleMesh);
        CalcReachableMesh(areas, startIndx, position, radius, circleMesh);
    }
}
