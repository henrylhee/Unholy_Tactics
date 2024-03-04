using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Rendering.Universal.Internal;

public class SurfaceHelper : MonoBehaviour
{
    class Edge
    {
        public int[] vertices = new int[2];
    }

    public Vector3[] vertices;
    private int[] indices;
    private int[] areas;
    private Vector3 startPoint;
    private int closestPointIndex;
    private Vector3 closestPoint;

    List<int> connectedTriangle = new List<int>();
    public List<Vector3> validVertices = new List<Vector3>();
    public List<int> validIndices = new List<int>();

    public List<int> connectedIndices = new List<int>();
    List<int> recursiveIndexHelper = new List<int>();

    public List<List<int[]>> edgeGroups = new List<List<int[]>>();
    public List<int[]> isolatedEdges = new List<int[]>();
    public List<int[]> outerEdges;

    private Vector3 casterPosition;
    private float cutRange;

    Time time;
    float startTime;

    public void Setup(NavMeshTriangulation data, Vector3 position, float range)
    {
        time = new Time();
        startTime = Time.time;
        UnityEngine.Debug.Log("###Start calculating mesh surface at time: "+ startTime);
        vertices = data.vertices;
        indices = data.indices;
        areas = data.areas;
        casterPosition = position;
        cutRange = range;

    }

    public void FindClosestPoint()
    {
        float distance = 1000000;
        int i = 0;
        foreach (Vector3 vertex in validVertices)
        {
            if (distance > Vector3.Distance(validVertices[i], startPoint))
            {
                closestPointIndex = i;
                closestPoint = validVertices[i];
                distance = Vector3.Distance(validVertices[i], startPoint);
            }
            i++;
        }
    }

    public void ProcessSurfaces()
    {
        //Debug.Log("###Start FindValidSurfaces(); time spent: " + (Time.time - startTime));
        UnityEngine.Debug.Log("Vertices: " + vertices.Length);
        UnityEngine.Debug.Log("Indices: " + indices.Length);
        CutOutsideTriangles(cutRange);
        UnityEngine.Debug.Log("cut vertices: " + validVertices.Count);

        FindValidVertices(0.005f);
        UnityEngine.Debug.Log("cut and cleaned Vertices: " + validVertices.Count);
        //Debug.Log("###Start FindCLosestPoint(); time spent: " + (Time.time - startTime));

        FindClosestPoint();
        //Debug.Log("###Start FindConnectedIndices(); time spent: " + (Time.time - startTime));

        UnityEngine.Debug.Log("closestpointindex: " + closestPointIndex);

        recursiveIndexHelper.Clear();
        FindConnectedIndices(closestPointIndex);
        UnityEngine.Debug.Log("Indices: " + indices.Length);

        UnityEngine.Debug.Log("valid indices: " + validIndices.Count);
        UnityEngine.Debug.Log("Connected indices: " + connectedIndices.Count);

        //Debug.Log("###Finished Calculation; time spent: " + (Time.time - startTime));
        //Debug.Log("###Start FindValidSurfaces(); time spent: " + (Time.time - startTime));

        //GetIsolatedEdges(connectedIndices);
        //FindEdgeGroups(isolatedEdges);



        //FindOuterEdges();
        //outerEdges = SortEdges(outerEdges);
        //for (int i = 0; i < edgeGroups.Count; i++)
        //{
        //    edgeGroups[i] = SortEdges(edgeGroups[i]); 
        //}

    }

    private void CutOutsideTriangles(float distance)
    {
        for (int i = 0; i < indices.Length/3; i++)
        {
            if(Vector3.Distance(casterPosition, vertices[indices[i * 3]]) <= distance || 
               Vector3.Distance(casterPosition, vertices[indices[i * 3 + 1]]) <= distance || 
               Vector3.Distance(casterPosition, vertices[indices[i * 3 + 2]]) <= distance)
            {
                validIndices.Add(indices[i * 3]);
                validIndices.Add(indices[i * 3 + 1]);
                validIndices.Add(indices[i * 3 + 2]);
            }
            else if (Collision(cutRange, new Vector2(casterPosition.x, casterPosition.z), new Vector2(vertices[indices[i * 3]].x, vertices[indices[i * 3]].z), new Vector2(vertices[indices[i * 3 + 1]].x, vertices[indices[i * 3 + 1]].z)))
            {
                validIndices.Add(indices[i * 3]);
                validIndices.Add(indices[i * 3 + 1]);
                validIndices.Add(indices[i * 3 + 2]);
            }
            else if (Collision(cutRange, new Vector2(casterPosition.x, casterPosition.z), new Vector2(vertices[indices[i * 3 + 1]].x, vertices[indices[i * 3 + 1]].z), new Vector2(vertices[indices[i * 3 + 2]].x, vertices[indices[i * 3 + 2]].z)))
            {
                validIndices.Add(indices[i * 3]);
                validIndices.Add(indices[i * 3 + 1]);
                validIndices.Add(indices[i * 3 + 2]);
            }
            else if (Collision(cutRange, new Vector2(casterPosition.x, casterPosition.z), new Vector2(vertices[indices[i * 3 + 2]].x, vertices[indices[i * 3 + 2]].z), new Vector2(vertices[indices[i * 3]].x, vertices[indices[i * 3]].z)))
            {
                validIndices.Add(indices[i * 3]);
                validIndices.Add(indices[i * 3 + 1]);
                validIndices.Add(indices[i * 3 + 2]);
            }
        }

        int[] translater = new int[vertices.Length];
        Array.Fill(translater, 0);

        for(int i = 0; i < vertices.Length; i++)
        { 
            if (!validIndices.Contains(i))
            {
                for(int j = i+1; j < vertices.Length; j++)
                {
                    translater[j] += 1;
                }
            }
            else
            {
                validVertices.Add(vertices[i]);
            }
        }

        for(int i = 0; i < validIndices.Count; i++)
        {
            validIndices[i] -= translater[validIndices[i]];
        }
    }

    private void FindValidVertices(float minDistance)
    {
        for (int i = 0; i < validVertices.Count;i++)
        {
            for (int j = i+1; j < validVertices.Count; j++)
            { 
                if (Vector3.Distance(validVertices[i], validVertices[j]) < minDistance)
                {
                    validVertices.RemoveAt(j);

                    for (int l = 0; l < validIndices.Count; l++)
                    {
                        if (validIndices[l] == j)
                        {
                            validIndices[l] = i;
                        }
                        else if (validIndices[l] > j)
                        {
                            validIndices[l]--;
                        }
                    }
                    j--;
                }
            }
            for(int k = i+1;k < vertices.Length; k++)
            {
                if (validVertices.Contains(vertices[k]))
                {
                    i += k - (i+1);
                    break;
                }
            }
        }
    }

    private void FindConnectedIndices(int index)
    {
        UnityEngine.Debug.Log("#### iteration, index: " + index + "/nvalidIndices.count: "+validIndices.Count);
        
        for(int s = 0; s < validVertices.Count; s++)
        {
            if (Vector3.Distance(validVertices[index], validVertices[s]) < 0.01 && s != index)
            {
                UnityEngine.Debug.Log("#### Distance: " + Vector3.Distance(validVertices[index], validVertices[s]) + "s: " + s);
            }
        }

        recursiveIndexHelper.Add(index);
        List<int> tempIndices = new List<int>();

        for (int i = 0; i < validIndices.Count; i++)
        {
            //UnityEngine.Debug.Log("valid index: " + validIndices[i] + "/n for i: " +i);
            if (index == validIndices[i])
            {
                UnityEngine.Debug.Log("---found index: " + validIndices[i] + "... i: " + i);

                int triangle = Mathf.FloorToInt(i / 3);
                if (!connectedTriangle.Contains(triangle))
                {
                    connectedTriangle.Add(triangle);

                    connectedIndices.Add(validIndices[3 * triangle]);
                    connectedIndices.Add(validIndices[3 * triangle + 1]);
                    connectedIndices.Add(validIndices[3 * triangle + 2]);
                }
                int[] triangleIndices = new int[3];
                triangleIndices[0] = validIndices[3 * triangle];
                triangleIndices[1] = validIndices[3 * triangle + 1];
                triangleIndices[2] = validIndices[3 * triangle + 2];

                foreach (int triangleIndex in triangleIndices)
                {
                    if (!tempIndices.Contains(triangleIndex) && !recursiveIndexHelper.Contains(triangleIndex))
                    {
                        tempIndices.Add(triangleIndex);
                        recursiveIndexHelper.Add(triangleIndex);
                        UnityEngine.Debug.Log("---added index for search: " + triangleIndex);
                        UnityEngine.Debug.Log("---index containing at : " + (1 < validIndices.FindAll(v => triangleIndex == v).Count));
                    }
                }
                i = i + 2 - (i % 3);
            }
        }

        if (tempIndices.Count == 0) { return; }

        foreach (int connectedIndex in tempIndices)
        {
            FindConnectedIndices(connectedIndex);
        }
    }

    private void GetIsolatedEdges(List<int> indices)
    {
        List<int[]> edges = new List<int[]>();

        for(int i = 0; i < indices.Count/3; i++)
        {
            edges.Add(new int[2]);
            edges[i * 3][0] = indices[i * 3];
            edges[i * 3][1] = indices[i * 3 + 1];

            edges.Add(new int[2]);
            edges[i * 3 + 1][0] = indices[i * 3 + 1];
            edges[i * 3 + 1][1] = indices[i * 3 + 2];

            edges.Add(new int[2]);
            edges[i * 3 + 2][0] = indices[i * 3 + 2];
            edges[i * 3 + 2][1] = indices[i * 3];
        }

        for(int j = 0; j < edges.Count; j++)
        {
            if (edges.FindAll(x => x == edges[j]).Count == 1)
            {
                isolatedEdges.Add(edges[j]);
            }
        }
    }

    private void FindEdgeGroups(List<int[]> remainingEdges)
    {
        UnityEngine.Debug.Log("find edge grups!");
        UnityEngine.Debug.Log("remaining edges: " + remainingEdges.Count);
        List<int[]> group = new List<int[]>();
        int searchIndex = remainingEdges[0][1];
        int foundEdgeIndex;
        group.Add(remainingEdges[0]);
        remainingEdges.RemoveAt(0);
        int length = remainingEdges.Count;
        for(int i = 0; i < length; i++)
        {
            foundEdgeIndex = remainingEdges.FindIndex(r => r[0] == searchIndex);
            if (foundEdgeIndex == -1)
            {
                edgeGroups.Add(group);
                
                FindEdgeGroups(remainingEdges);
                break;
            }
            searchIndex = remainingEdges[foundEdgeIndex][1];
            group.Add(remainingEdges[foundEdgeIndex]);
            remainingEdges.RemoveAt(foundEdgeIndex);
        }
        if (remainingEdges.Count == 0)
        {
            edgeGroups.Add(group);
        }
    }

    void FindOuterEdges()
    {
        if (edgeGroups.Count == 1)
        {
            outerEdges = edgeGroups[0];
        }
        else if (edgeGroups.Count == 0)
        {
            UnityEngine.Debug.LogError("no edgegroups!!");
        }
        else
        {
            if (CheckVertexInsidePolygon(validVertices[edgeGroups[1][0][0]], edgeGroups[0], validVertices))
            {
                outerEdges = edgeGroups[0];
                edgeGroups.RemoveAt(0);
            }
            else
            {
                for (int i = 1; i < edgeGroups.Count; i++)
                {
                    if (CheckVertexInsidePolygon(validVertices[edgeGroups[0][0][0]], edgeGroups[i], validVertices))
                    {
                        outerEdges = edgeGroups[i];
                        edgeGroups.RemoveAt(i);
                        break;
                    }
                }
            }
            UnityEngine.Debug.Log("-");
        }
    }

    bool CheckVertexInsidePolygon(Vector3 vertex, List<int[]> sortedEdgeGroup, List<Vector3> vertices)
    {
        float[] vertx = new float[sortedEdgeGroup.Count];
        float[] verty = new float[sortedEdgeGroup.Count];
        for (int i = 0; i < sortedEdgeGroup.Count; i++)
        {
            vertx[i] = vertices[sortedEdgeGroup[i][0]].x;
            verty[i] = vertices[sortedEdgeGroup[i][0]].z;
        }
        return RaycastCheckVertex(sortedEdgeGroup.Count, vertx, verty, vertex.x, vertex.z);
    }

    bool RaycastCheckVertex(int nvert, float[] vertx, float[] verty, float testx, float testy)
    {
        int i, j;
        bool c = false;
        for (i = 0, j = nvert - 1; i < nvert; j = i++)
        {
            if (((verty[i] > testy) != (verty[j] > testy)) &&
             (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
                c = !c;
        }
        return c;
    }



    //get length too
    private List<int[]> SortEdges(List<int[]> edges)
    {
        List<int[]> sortedEdges = new List<int[]>();
        sortedEdges.Add(edges[0]);

        for(int i = 1; i < edges.Count; i++)
        {
            sortedEdges.Add(edges.Find(x => x[0] == edges[i - 1][1]));
        }
        return sortedEdges;
    }

    //public TriangleNet.Meshing.IMesh Delanauy(Vector3[] vertices)
    //{
    //    var triangulator = new Dwyer();

    //    // Generate mesh.

    //    TriangleNet.Geometry.Vertex[] transformedVertices = To2DTriangleNetVertex(vertices);
    //    var mesh = triangulator.Triangulate(transformedVertices, new Configuration());
    //    return mesh;
    //}

    //public TriangleNet.Geometry.Vertex[] To2DTriangleNetVertex(Vector3[] vertices)
    //{
    //    TriangleNet.Geometry.Vertex[] result = new TriangleNet.Geometry.Vertex[vertices.Length];
    //    for(int i = 0; i < vertices.Length;i++)
    //    {
    //        result[i] = new TriangleNet.Geometry.Vertex(vertices[i].x, vertices[i].z);
    //    }
    //    return result;
    //}

    public static Mesh Get2DCircle(float radius, Vector3 center, int edges)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        float angle = (Mathf.PI * 2) / edges;

        vertices.Add(center);
        for(int i = 0; i < edges; i++)
        {
            vertices.Add(new Vector3(center.x + Mathf.Cos(angle * i) * radius, center.y, center.z + Mathf.Sin(angle * i) * radius));
        }

        for(int i = 0; i < edges - 1; i++)
        {
            indices.Add(0);
            indices.Add(i + 2);
            indices.Add(i + 1);
        }
        indices.Add(0);
        indices.Add(1);
        indices.Add(edges);

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        return mesh;
    }

    public static Mesh Get2DCircleWithUVs(float radius, Vector3 center, int edges)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        float angle = (Mathf.PI * 2) / edges;

        vertices.Add(center);
        uvs.Add(new Vector2(0,0));
        for (int i = 0; i < edges; i++)
        {
            vertices.Add(new Vector3(center.x + Mathf.Cos(angle * i) * radius, center.y, center.z + Mathf.Sin(angle * i) * radius));
            uvs.Add(new Vector2((angle * i)/ (Mathf.PI * 2), 1));
        }

        for (int i = 0; i < edges - 1; i++)
        {
            indices.Add(0);
            indices.Add(i + 2);
            indices.Add(i + 1);
        }
        indices.Add(0);
        indices.Add(1);
        indices.Add(edges);

        mesh.SetVertices(vertices);
        mesh.SetUVs(0,uvs);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        return mesh;
    }

    public static bool Collision(float radius, Vector2 center, Vector2 p, Vector2 q)
    {
        Vector2 oq = q - center;
        Vector2 op = p - center;
        Vector2 qp = p - q;
        Vector2 pq = q - p;

        float maxDist = Mathf.Max(op.magnitude,oq.magnitude);
        float minDist;

        if(Vector2.Dot(op,qp) > 0 && Vector2.Dot(oq,pq) > 0)
        {
            minDist = Mathf.Abs((op.x * oq.y - op.y * oq.x)) / pq.magnitude; 
        }
        else
        {
            minDist = Mathf.Min(op.magnitude, oq.magnitude);
        }

        if(minDist <= radius && maxDist >= radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
