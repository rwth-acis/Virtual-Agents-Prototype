using UnityEngine;
// NavMesh
using UnityEngine.AI;
// Third person animation
using UnityStandardAssets.Characters.ThirdPerson;
// IEnumerator
using System.Collections;
// Tasks
using System.Collections.Generic;
using VirtualAgentsFramework.AgentTasks;
// Action
using System;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace VirtualAgentsFramework
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class Agent : MonoBehaviour
    {
        private NavMeshAgent agent;
        Animator animator;

        // Queue
        public AgentTaskManager queue = new AgentTaskManager();
        public enum State
        {
            inactive, // e.g. task management has not been initialized yet
            idle,
            busy // i.e. executing a task
        }
        public State currentState;
        public enum Processes
        {
            Exercise, 
            Suggestion,
            Answer,
            None
        }
        public Processes currentProcess;
        public Process Process;
        public IAgentTask currentTask;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            // Disable NavMeshAgent rotation updates, since they are handled by ThirdPersonCharacter
            agent.updateRotation = false;

            // Queue
            // Agents start in the idle state. CHANGE_ME to inactive to disable task dispatching
            currentState = State.idle;
            currentProcess = Processes.None;

            string exercisejson = "[{\"Name\":\"Exercise one\",\"Place\":\"(3, 1, 4)\",\"Steps\":[{\"Message\":\"First, your have to put the filament in the extruder which is on the top of the printer. Then, you can choose your custom settings and choose the model you want to print.\",\"Actions\":[\"UsingLaserCutter\"]},{\"Message\":\"From the begining of the print, til the completed result several hours may pass.\",\"Actions\":[\"Talking1\"]},{\"Message\":\"When the print is done, you can take the build plate with your printed model out of the printer.\",\"Actions\":[\"WaitingLaserCutter\"]}],\"Completed\":false},{\"Name\":\"Exercise two\",\"Place\":\"(3, 1, 4)\",\"Steps\":[{\"Message\":\"There is a variety of programs for creating a 3d model.\",\"Actions\":[\"Talking2\"]},{\"Message\":\"3d Print, Elephant and VAD are examples of commonly used programs.\",\"Actions\":[\"Talking3\"]}],\"Completed\":false}]";
            string knowledgejson = "{\"IntroductionShown\":false,\"Introduction\":\"Hi, I am your virtual mentor. In this program you can ask lecture related questions and request suggestions regarding your learning process. I can also show and explain physical exercises if you ask me to. I will queue all your requests and help you one by one. I'd suggest we'll start right off! What would you like to know?\"}";
            //string output = JsonConvert.SerializeObject(exercise);
            string path = Path.Combine(Application.persistentDataPath, "Exercises.json");
            string jsonFilePath = path;
            File.WriteAllText(path, exercisejson);
            path = Path.Combine(Application.persistentDataPath, "Knowledge.json");
            File.WriteAllText(path, knowledgejson);
        
            GameObject Cube = GameObject.Find("Cube");
            Cube.GetComponent<Renderer>().enabled = false;
            GameObject Result = GameObject.Find("Result");
            Result.GetComponent<Renderer>().enabled = false;
            animator = GetComponent<Animator>();
            if(animator.runtimeAnimatorController.animationClips.Any(c => c.name == "UsingLaserCutter"))
            {
                Debug.Log("Called");
                var animation = animator.runtimeAnimatorController.animationClips.FirstOrDefault(c => c.name == "UsingLaserCutter");
                AddEvent(Array.IndexOf(animator.runtimeAnimatorController.animationClips, animation), 0f, "CleanUpResult", 0);
                AddEvent(Array.IndexOf(animator.runtimeAnimatorController.animationClips, animation), 0.2f, "PickUpCord", 0);
                AddEvent(Array.IndexOf(animator.runtimeAnimatorController.animationClips, animation), 3.8f, "PutInCord", 0);
            }
            if (animator.runtimeAnimatorController.animationClips.Any(c => c.name == "WaitingLaserCutter"))
            {
                var animation = animator.runtimeAnimatorController.animationClips.FirstOrDefault(c => c.name == "WaitingLaserCutter");
                AddEvent(Array.IndexOf(animator.runtimeAnimatorController.animationClips, animation), 2.3f, "TakeResult", 0);
                AddEvent(Array.IndexOf(animator.runtimeAnimatorController.animationClips, animation), 4.4f, "LayResultOnTable", 0);
            }



            this.Process = new Process(this);
        }

        // Update is called once per frame
        void Update()
        {
            // Queue
            switch(currentState)
            {
                case State.inactive:
                    break;
                case State.idle:
                    RequestNextTask();
                    break;
                case State.busy:
                    currentTask.Update();
                    break;
            }
        }
        //Method to call animator event at runtime
        void AddEvent(int Clip, float time, string functionName, float floatParameter)
        {
            animator = GetComponent<Animator>();
            AnimationEvent animationEvent = new AnimationEvent();
            animationEvent.functionName = functionName;
            animationEvent.floatParameter = floatParameter;
            animationEvent.time = time;
            AnimationClip clip = animator.runtimeAnimatorController.animationClips[Clip];
            clip.AddEvent(animationEvent);
        }

        // This method gets triggered by the Animator event
        public void ReturnToIdle()
        {
            var currentAnimationTask = currentTask as AgentAnimationTask;
            if(currentAnimationTask != null)
            {
                currentAnimationTask.ReturnToIdle();
            }
        }
        public void CleanUpResult()
        {
            GameObject Cube = GameObject.Find("Cube");
            Cube.GetComponent<Renderer>().enabled = false;
            GameObject Result = GameObject.Find("Result");
            Result.GetComponent<Renderer>().enabled = false;
        }

        public void PickUpCord()
        {
            GameObject Cord = GameObject.Find("Cord");
            Cord.GetComponent<Renderer>().enabled = true;
            GameObject Hand = GameObject.Find("RightHand");
            Cord.transform.parent = Hand.transform;
            Cord.transform.localPosition = new Vector3((float)-0.00053, (float)0.00091, (float)0.001);
            Cord.transform.localScale = new Vector3((float)0.00007, (float)0.001, (float)0.00007);
            Cord.transform.localRotation = Quaternion.Euler((float)162.93, (float)50, (float)90);

        }

        public void PutInCord()
         {
            GameObject Cord = GameObject.Find("Cord");
            GameObject cube = GameObject.Find("cube_prototype_a");
            Cord.transform.parent = cube.transform;
            Cord.transform.localPosition = new Vector3((float)0, (float)0.84, (float)0.71);
            Cord.transform.localScale = new Vector3((float)0.0158, (float)0.6623, (float)0.0213);
            Cord.transform.localRotation = Quaternion.Euler((float)39.464, (float)180, (float)90);
        }

        public void TakeResult()
        {
            GameObject Result = GameObject.Find("Cube");
            Result.GetComponent<Renderer>().enabled = true;
            GameObject Cube = GameObject.Find("Result");
            Cube.GetComponent<Renderer>().enabled = true;
            GameObject Hand = GameObject.Find("RightHand");
            Result.transform.parent = Hand.transform;
            Result.transform.localPosition = new Vector3((float)0.00012, (float)0.001, (float)0.00162);
            Result.transform.localScale = new Vector3((float)0.0023, (float)0.0001, (float)0.0025);
            Result.transform.localRotation = Quaternion.Euler((float)14.2, (float)0, (float)130.53);
        }

        public void LayResultOnTable()
        {
            GameObject Result = GameObject.Find("Cube");
            GameObject Table = GameObject.Find("Table");
            Result.transform.parent = Table.transform;
            Result.transform.localPosition = new Vector3((float)-0.75, (float)-0.072, (float)0.759);
            Result.transform.localScale = new Vector3((float)0.5, (float)0.01, (float)0.25);
            Result.transform.localRotation = Quaternion.Euler((float)90, (float)0, (float)0);
        }

        // Queue managenent functions
        public void AddTask(IAgentTask task)
        {
            queue.AddTask(task);
        }

        public void ForceTask(IAgentTask task)
        {
            queue.ForceTask(task);
        }

        public void RequestNextTask()
        {
            IAgentTask nextTask = queue.RequestNextTask();
            if(nextTask == null)
            {
                // The queue is empty, play the idle animation
                currentState = State.idle;
            }
            else
            {
                // Execute the task, depending on its type
                currentState = State.busy;
                nextTask.Execute(this);
                currentTask = nextTask;
                // Subscribe to the task's OnTaskFinished event to set the agent's state to idle upon task execution
                currentTask.OnTaskFinished += SetAgentStateToIdle;
            }
        }

        // Helper function to be called upon task execution
        void SetAgentStateToIdle()
        {
            currentState = State.idle;
            // Additionally, unsubscribe from the event
            currentTask.OnTaskFinished -= SetAgentStateToIdle;
        }

        // Shortcut queue management functions
        public void WalkTo(GameObject destinationObject, bool force)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject);
            ScheduleOrForce(movementTask, force);
        }

        public void WalkTo(Vector3 destinationCoordinates, bool force)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates);
            ScheduleOrForce(movementTask, force);
        }

        public void RunTo(GameObject destinationObject, bool force = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationObject, true);
            ScheduleOrForce(movementTask, force);
        }

        public void RunTo(Vector3 destinationCoordinates, bool force = false)
        {
            AgentMovementTask movementTask = new AgentMovementTask(destinationCoordinates, true);
            ScheduleOrForce(movementTask, force);
        }

        public void PlayAnimation(string animationName, bool force = false)
        {
            AgentAnimationTask animationTask = new AgentAnimationTask(animationName);
            ScheduleOrForce(animationTask, force);
        }

        public void WaitForSeconds(float secondsWaiting, bool force = false)
        {
            AgentWaitingTask waitingTask = new AgentWaitingTask(secondsWaiting);
            ScheduleOrForce(waitingTask, force);
        }

        public void SpeakOut(string message, bool force)
        {
            AgentSpeakingTask speakingTask = new AgentSpeakingTask(message);
            ScheduleOrForce(speakingTask, force);
        }

        public void SetProcess(Processes process, bool startingFlag, bool force)
        {
            AgentSetProcessTask setProcessTask = new AgentSetProcessTask(process, startingFlag);
            ScheduleOrForce(setProcessTask, force);
        }

        //TODO
        public void PickUp()
        {

        }

        public void ScheduleOrForce(IAgentTask task, bool force)
        {
            if(force == true)
            {
                queue.ForceTask(task);
                //Debug.Log("forced" + task);
            }
            else
            {
                queue.AddTask(task);
                //Debug.Log("added" + task);
            }
        }
    }

    namespace AgentTasks
    {
        public interface IAgentTask
        {
            void Execute(Agent agent);
            void Update();
            event Action OnTaskFinished;
        }

        public class AgentMovementTask : IAgentTask
        {
            Agent agent;
            NavMeshAgent navMeshAgent;
            ThirdPersonCharacter thirdPersonCharacter;

            private GameObject destinationObject = null;
            private bool run;
            private const float walkingSpeed = 1.8f;
            private const float runningSpeed = 4;
            private const float damping = 6;

            private Vector3 previousPosition;
            private float curSpeed;

            private bool isMoving;
            private const float destinationReachedTreshold = 1.5f;

            public event Action OnTaskFinished;

            // Constructor with gameObject
            public AgentMovementTask(GameObject destinationObject, bool run = false)
            {
                this.run = run;
                this.destinationObject = destinationObject; //TODO raise exception if null
            }

            // Constructor with coordinates
            public AgentMovementTask(Vector3 destinationCoordinates, bool run = false)
            {
                this.run = run;
                CreateDestinationObject(destinationCoordinates);
            }

            // Helper function, creates a destination gameObject using coordinates
            private void CreateDestinationObject(Vector3 destinationCoordinates)
            {
                destinationObject = new GameObject();
                destinationObject.transform.position = destinationCoordinates;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                navMeshAgent = agent.GetComponent<NavMeshAgent>();
                thirdPersonCharacter = agent.GetComponent<ThirdPersonCharacter>();

                // Set running or walking speed
                if(run == true)
                {
                    navMeshAgent.speed = runningSpeed;
                }
                else
                {
                    navMeshAgent.speed = walkingSpeed;
                }

                // Change agent's status to moving (busy)
                isMoving = true;
                //TODO destroy destination object upon execution (if one was created)
            }

            public void Update()
            {
                if(destinationObject != null)
                {
                    navMeshAgent.SetDestination(destinationObject.transform.position);
                }

                // Calculate actual speed
                Vector3 curMove = agent.transform.position - previousPosition;
                curSpeed = curMove.magnitude / Time.deltaTime;
                previousPosition = agent.transform.position;

                // Control movement
                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    thirdPersonCharacter.Move(navMeshAgent.desiredVelocity * curSpeed/damping, false, false);
                }
                else
                {
                    thirdPersonCharacter.Move(Vector3.zero, false, false);
                    // Check if the agent has really reached its destination
                    if(destinationObject != null)
                    {
                        float distanceToTarget = Vector3.Distance(agent.gameObject.transform.position, destinationObject.transform.position);
                        if(distanceToTarget <= destinationReachedTreshold)
                        {
                            if (isMoving == true)
                            {
                                isMoving = false;
                                OnTaskFinished();
                            }
                        }
                    }
                }
            }
        }

        public class AgentAnimationTask : IAgentTask
        {
            private Agent agent;
            private Animator animator;

            private string currentState;
            private const string idleAnimationName = "Grounded"; // CHANGE_ME
            private string animationName;

            public event Action OnTaskFinished;

            // Constructor
            public AgentAnimationTask(string animationName)
            {
                this.animationName = animationName;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                animator = agent.GetComponent<Animator>();
                ChangeAnimationState(animationName);
                Debug.Log("ExecuteAnimationtate");
            }

            public void ChangeAnimationState(string newState)
            {
                if(currentState == newState) return; // Same animation is already playing
                animator.Play(newState);
                animator.SetBool("CustomAnimation", true);
                currentState = newState;
                Debug.Log("ChangeAnimationtate");
            }

            // Gets called by the agent in response to the Animator's event
            public void ReturnToIdle()
            {
                animator.SetBool("CustomAnimation", false);
                currentState = idleAnimationName;
                OnTaskFinished();
            }

            public void Update() {}
        }

        public class AgentWaitingTask : IAgentTask
        {
            private Agent agent;
            private float waitingTime;

            public event Action OnTaskFinished;

            // Constructor
            public AgentWaitingTask(float waitingTime)
            {
                this.waitingTime = waitingTime;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                agent.StartCoroutine(WaitingCoroutine(waitingTime));
            }

            private IEnumerator WaitingCoroutine(float waitingTime)
            {
                yield return new WaitForSeconds(waitingTime);
                OnTaskFinished();
            }

            public void Update() {}
        }

        public class AgentSetProcessTask : IAgentTask
        {
            private Agent agent;
            private Agent.Processes process;
            bool startingFlag;

            public event Action OnTaskFinished;

            // Constructor
            public AgentSetProcessTask(Agent.Processes process, bool startingFlag)
            {
                this.process = process;
                this.startingFlag = startingFlag;
            }

            public void Execute(Agent agent)
            { 
                this.agent = agent;
                if(startingFlag)
                {
                    agent.currentProcess = process;
                }
                else
                {
                    agent.currentProcess = Agent.Processes.None;
                }
            }

            public void Update()
            {
                 OnTaskFinished();
            }
        }

        public class AgentSpeakingTask : IAgentTask
        {
            private Agent agent;
            private string message;
            private TextToSpeechLogic tts;

            public event Action OnTaskFinished;

            // Constructor
            public AgentSpeakingTask(string message)
            {
                this.message = message;
            }

            public void Execute(Agent agent)
            {
                this.agent = agent;
                tts = agent.GetComponent<TextToSpeechLogic>();
                tts.Speak(message);
                tts.Stop();
            }

            public void Update()
            {
                if (!tts.IsSpeaking())
                {
                    OnTaskFinished();
                }
            }
        }

        public class AgentTaskManager
        {
            public Queue<IAgentTask> taskQueue;

            // Constructor
            public AgentTaskManager()
            {
                taskQueue = new Queue<IAgentTask>();
                taskQueue.Clear();
            }


            // Using this method, an agent can request the next task
            public IAgentTask RequestNextTask()
            {
                if(taskQueue.Count > 0)
                {
                    return taskQueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }

            public void AddTask(IAgentTask task)
            {
                taskQueue.Enqueue(task);
            }

            // Use this method to pass a task from the AgentController and make it jump the queue
            public void ForceTask(IAgentTask task)
            {
                Queue<IAgentTask> tempQueue = new Queue<IAgentTask>();
                tempQueue.Enqueue(task);
                while (taskQueue.Count > 0)
                {
                    tempQueue.Enqueue(taskQueue.Dequeue());
                }
                taskQueue = tempQueue;
            }
            public void ForceTasks(Queue<IAgentTask> forcedTaskQueue)
            {
                while (taskQueue.Count > 0)
                {
                    forcedTaskQueue.Enqueue(taskQueue.Dequeue());
                }
                taskQueue = forcedTaskQueue;
            }
        }
    }
}
