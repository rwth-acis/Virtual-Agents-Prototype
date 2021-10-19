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
// Rigs
using UnityEngine.Animations.Rigging;

namespace VirtualAgentsFramework
{
    namespace AgentTasks
    {
        /// <summary>
        /// Define animation tasks for playing specific animations on agents
        /// </summary>
        public class AgentAnimationTask : IAgentTask
        {
            private Agent agent;
            private Animator animator;

            private string currentState;
            private const string idleAnimationName = "Grounded"; // CHANGE_ME if the idle animation has a different name
            private string animationName;

            public event Action OnTaskFinished;

            /// <summary>
            /// Create an AgentMovementTask using the animation's name
            /// </summary>
            /// <param name="animationName">Name of the animation to be played</param>
            public AgentAnimationTask(string animationName)
            {
                this.animationName = animationName;
            }

            /// <summary>
            /// Get the agent's Animator and change the agent's animation state
            /// </summary>
            /// <param name="agent">Agent to be animated</param>
            public void Execute(Agent agent)
            {
                this.agent = agent;
                animator = agent.GetComponent<Animator>();
                ChangeAnimationState(animationName);
            }

            /// <summary>
            /// Change the agent's animation state and perform a custom animation
            /// </summary>
            /// <param name="newState">New animation state's name</param>
            private void ChangeAnimationState(string newState)
            {
                if(currentState == newState) return; // Same animation is already playing
                animator.Play(newState);
                animator.SetBool("CustomAnimation", true);
                currentState = newState;
            }

            /// <summary>
            /// Return the agent's animation state to idle
            /// Gets called by the agent in response to the Animator's event
            /// </summary>
            public void ReturnToIdle()
            {
                animator.SetBool("CustomAnimation", false);
                currentState = idleAnimationName;
                // Trigger the TaskFinished event
                OnTaskFinished();
            }

            // No frame-to-frame functionality thanks to the Animator component and its event system
            public void Update() {}
        }
    }
}
