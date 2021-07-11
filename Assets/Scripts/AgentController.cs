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
            // No queue: wait until agent is ready, then perform actions
            //StartCoroutine(AgentActions());

            // Queue
            queue = new AgentTaskManager();
            agent.SetQueue(queue);
            //Debug.Log(queue.RequestNextTask());
            AgentTaskManager.AgentMovementTask task1 = new AgentTaskManager.AgentMovementTask(object2);
            AgentTaskManager.AgentMovementTask task2 = new AgentTaskManager.AgentMovementTask(gameObject);
            AgentTaskManager.AgentMovementTask task3 = new AgentTaskManager.AgentMovementTask(object2);
            queue.AddTask(task1);
            queue.AddTask(task2);
            queue.AddTask(task3);
            queue.AddTask(task2);
            //Debug.Log(queue.RequestNextTask());
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
