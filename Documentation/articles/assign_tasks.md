# How to assign tasks to a virtual agent

## Preparation

1. Create an AgentController GameObject in the scene.
2. Create an AgentController script and assign it to the AgentController GameObject.
3. Create a SerializeField Agent attribute in the AgentController script and assign the agent you created (the proxy object) to it in the Inspector.

## Task management using shortcut functions

The Agent class provides numerous methods for assigning tasks to the agent. For instance, you can use the following shortcut functions:

### Movement

- WalkTo(GameObject destinationObject, bool asap = false)
- WalkTo(Vector3 destinationCoordinates, bool asap = false)
- RunTo(GameObject destinationObject, bool asap = false)
- RunTo(Vector3 destinationCoordinates, bool asap = false)

### Animation

PlayAnimation(string animationName, bool asap = false)

Make sure that the respective animation is available and correctly set up in your agent's Animator.

### Waiting

WaitForSeconds(float secondsWaiting, bool asap = false)

## Task management using generic functions

Alternatively, you can create IAgentTask tasks using the constructors in the agent task classes provided in the VirtualAgentsFramework.AgentTasks namespace. After this, you can use the methods ScheduleTask(IAgentTask task) and ExecuteTaskASAP(IAgentTask task) from the Agent class in order to assign the tasks you created to the agent.