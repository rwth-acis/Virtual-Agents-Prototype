using UnityEngine;
using System.Collections;
using VirtualAgentsFramework;

public class WalkToMe : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] GameObject object2;
    // Start is called before the first frame update
    void Start()
    {
        // Wait until agent is ready
        StartCoroutine(AgentActions());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator AgentActions()
    {
        yield return new WaitForSeconds(1f);
        agent.WalkTo(gameObject);
        /*agent.WalkTo(object2);
        agent.WalkTo(new Vector3(-7,0,-5));*/
        agent.PlayAnimation("Dancing");
    }
}
