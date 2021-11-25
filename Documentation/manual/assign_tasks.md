# How to Assign Tasks to a Virtual Agent

## Preparation

1. Create an AgentController GameObject in the scene.
2. Create an AgentController script and assign it to the AgentController GameObject.
3. Create a SerializeField Agent attribute in the AgentController script and assign the agent you created (the proxy object) to it in the Inspector.

## Task management using shortcut functions

The <xref:VirtualAgentsFramework.Agent> class provides numerous methods for assigning tasks to the agent.
For instance, you can use the following shortcut functions:

### Movement

- <xref:VirtualAgentsFramework.Agent.WalkTo(GameObject,System.Boolean)>
- <xref:VirtualAgentsFramework.Agent.WalkTo(Vector3,System.Boolean)>
- <xref:VirtualAgentsFramework.Agent.RunTo(GameObject,System.Boolean)>
- <xref:VirtualAgentsFramework.Agent.RunTo(Vector3,System.Boolean)>

### Animation

Use <xref:VirtualAgentsFramework.Agent.PlayAnimation(System.String,System.Boolean)> to play a custom animation.

Make sure that the respective animation is available and correctly set up in your agent's Animator.

### Waiting

Use <xref:VirtualAgentsFramework.Agent.WaitForSeconds(System.Single,System.Boolean)> to make the agent wait for a designated time span.

## Task Management Using Generic Functions

Alternatively, you can create <xref:VirtualAgentsFramework.AgentTasks.IAgentTask> tasks using the constructors in the agent task classes provided in the VirtualAgentsFramework.AgentTasks namespace.
After this, you can use the methods <xref:VirtualAgentsFramework.Agent.ScheduleTask(IAgentTask)> and <xref:VirtualAgentsFramework.Agent.ExecuteTaskASAP(IAgentTask)> from the <xref:VirtualAgentsFramework.Agent> class in order to assign the tasks you created to the agent.