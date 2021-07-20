using UnityEngine;
using System.Collections;
using VirtualAgentsFramework.AgentTasks;

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
            // Solution wihout a queue: wait until agent is ready, then perform actions
            //StartCoroutine(AgentActions());

            // Queue
            queue = new AgentTaskManager();
            agent.SetQueue(queue);
            // Create tasks
            AgentMovementTask movementTask1 = new AgentMovementTask(object2, true);
            AgentMovementTask movementTask2 = new AgentMovementTask(gameObject);
            AgentMovementTask movementTask3 = new AgentMovementTask(object2);
            AgentAnimationTask animationTask1 = new AgentAnimationTask("Dancing");
            AgentWaitingTask waitingTask1 = new AgentWaitingTask(2f);
            // Queue tasks
            queue.AddTask(movementTask1);     // Run to object 2
            queue.AddTask(movementTask2);     // Walk to object 1
            queue.ForceTask(animationTask1);  // Dance
            queue.AddTask(movementTask3);     // Walk to object 2
            queue.AddTask(waitingTask1);      // Wait for 2 seconds
            queue.AddTask(movementTask2);     // Walk to object 1
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator AgentActions()
        {
            yield return new WaitForSeconds(1f);
            /*agent.WalkTo(gameObject);
            agent.WalkTo(object2);
            agent.WalkTo(new Vector3(-7,0,-5));*/
            agent.PlayAnimation("Dancing");
            yield return new WaitForSeconds(5f);
            agent.WalkTo(gameObject);
        }
    }
}
