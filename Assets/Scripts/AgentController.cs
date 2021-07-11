using UnityEngine;
using System.Collections;
using VirtualAgentsFramework;

namespace VirtualAgentsFramework
{
    public class AgentController : MonoBehaviour
    {
        [SerializeField] Agent agent;
        [SerializeField] GameObject object2;
        AgentTaskManager queue;

        // Start is called before the first frame update
        void Start()
        {
            // Wait until agent is ready
            StartCoroutine(AgentActions());
            // Queue
            queue = new AgentTaskManager();
            Debug.Log(queue.RequestNextTask());
            AgentTaskManager.AgentTask task = new AgentTaskManager.AgentTask();
            queue.AddTask(task);
            Debug.Log(queue.RequestNextTask());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator AgentActions()
        {
            yield return new WaitForSeconds(1f);
            //agent.WalkTo(gameObject);
            /*agent.WalkTo(object2);
            agent.WalkTo(new Vector3(-7,0,-5));*/
            agent.PlayAnimation("Dancing");
            yield return new WaitForSeconds(5f);
            agent.WalkTo(gameObject);
        }
    }
}
