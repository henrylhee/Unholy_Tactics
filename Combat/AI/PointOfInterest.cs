using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour
{
    private List<Vector3> debugLastHits = new List<Vector3>();
    private int debugHits = -1;

    public void ClearDebug()
    {
        debugLastHits = new List<Vector3>();
        debugHits = -1;
    }

    float sampleSize = 10;
    public int GetCoverCount(Unit caster, List<Unit> enemies)
    {
        int hitUnits = 0;
        int mask = (LayerMask.GetMask("Obstacles"));
        bool breakFlag = false;

        ClearDebug();
        foreach(Unit enemy in enemies)
        {
            breakFlag = false;
            Check(enemy, transform.position);

            float radius = caster.GetWidth() * 1f;
            for(float i =0;i < sampleSize && !breakFlag; i++)
            {
                Vector3 targetPoint = new Vector3();
                targetPoint.x = transform.position.x + radius * Mathf.Sin((i / sampleSize) * 360);
                targetPoint.y = transform.position.y;
                targetPoint.z = transform.position.z + radius * Mathf.Cos((i / sampleSize) * 360);

                Check(enemy, targetPoint);
            }

            if (!breakFlag)
            {
                debugLastHits.Add(new Vector3());
            }
        }

        if(debugLastHits.Count > 0)
        {
            debugHits = debugLastHits.Count;
        }

        return enemies.Count - hitUnits;

        void Check(Unit unit, Vector3 startPoint)
        {
            if(unit is PlayerUnit)
            {
                PlayerUnit playerUnit = (PlayerUnit)unit;
                if(playerUnit.unitType == UnitType.Range || playerUnit.unitType == UnitType.Mage)
                {
                    CheckLineOfSight(startPoint, unit.transform.position);
                    return;
                }
                else
                {
                    CheckDistance(startPoint, unit.transform.position, playerUnit.GetMovementRange());
                    return;
                }
            }

            else
            {
                CheckLineOfSight(startPoint, unit.transform.position);
            }
        }

        void CheckLineOfSight(Vector3 startPoint, Vector3 targetPoint)
        {
            Ray ray = new Ray(startPoint, targetPoint);
            Vector3 direction = (targetPoint - startPoint).normalized;
            float distance = Vector3.Distance(startPoint, targetPoint);

            RaycastHit hit = new RaycastHit();

            if (!Physics.Raycast(startPoint, direction, out hit, distance, mask))
            {
                breakFlag = true;
                hitUnits++;

                debugLastHits.Add(targetPoint);
            }
        }

        void CheckDistance(Vector3 startPoint, Vector3 targetPoint, float radius)
        {
            if(Vector3.Distance(startPoint, targetPoint) < radius  + 1)
            {
                breakFlag = true;
                hitUnits++;
                debugLastHits.Add(targetPoint);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(debugHits >= 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }

        foreach(Vector3 v in debugLastHits)
        {
            if(v != new Vector3())
            {
                Debug.DrawLine(transform.position, v, Color.red);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.1f);
            }
        }
    }

    
}