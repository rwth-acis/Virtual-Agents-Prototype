using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// NavMesh
using UnityEngine.AI;

public class CharacterAgent : MonoBehaviour
{
    public GameObject destination;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(destination.transform.position);
    }
}
