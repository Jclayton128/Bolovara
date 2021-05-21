using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class MapHelper : object
{
    static float minX = -20;  //TODO find a way to universalize these
    static float minY = -14;
    static float maxX = 20;
    static float maxY = 15;

    static public Vector3 CreateRandomValidStartPoint()
    {
        NavMeshSurface2d navMesh = GameObject.FindGameObjectWithTag("NavMeshUnits").GetComponent<NavMeshSurface2d>();

        float randX = Random.Range(minX, maxX);
        float randY = Random.Range(minY, maxY);

        Vector3 randPos = new Vector3(randX, randY, 0);

        Debug.Log(randPos);


        return randPos;
    }

}
