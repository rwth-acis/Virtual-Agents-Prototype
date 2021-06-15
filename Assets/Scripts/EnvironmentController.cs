//﻿using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;

public class EnvironmentController : MonoBehaviour
{
    public GameObject obstacle;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(obstacle, new Vector3(-3.5f, 0.5f, 16.3f), Quaternion.identity);
        NavMeshBuilder.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
