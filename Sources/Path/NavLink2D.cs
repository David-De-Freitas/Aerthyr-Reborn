using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NavLink2D : MonoBehaviour
{
    [SerializeField] Color lineColor;  
    [SerializeField] [Range(1, 100)] int resolution;
    public Enemy.Species[] enemiesAllowed;
    [Space]
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] Transform pointBezier;
    [Space]
    public Vector3[] path;

	// Use this for initialization
	void Start ()
    {
        path = new Vector3[resolution + 2];
        path[0] = pointA.position;
        path[path.Length - 1] = pointB.position;
	}

    private void OnValidate()
    {
        path = new Vector3[resolution + 2];
        path[0] = pointA.position;
        path[path.Length - 1] = pointB.position;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Application.isEditor)
        {
            path[0] = pointA.position;
            path[path.Length - 1] = pointB.position;
        }	
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        for (int i = 1; i < path.Length - 1; i++)
        {
            float t = i / (float)resolution;

            path[i] = MathfAddon.QuadraticBezierPoint(t, pointA.position, pointBezier.position, pointB.position);
            Gizmos.DrawLine(path[i - 1], path[i]);
        }
    }
}
