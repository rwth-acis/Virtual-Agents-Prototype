using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
#endif


public class AgentAnimationUpdater : MonoBehaviour
{
    CharacterController controller;
    NavMeshAgent agent;
	Animator animator;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDGrounded;
	private int _animIDJump;
	private int _animIDFreeFall;
	private int _animIDMotionSpeed;

	private void Awake()
	{
		AssignAnimationIDs();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
	}

    private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}

	/// <summary>
	/// Updates the animation parameters for the blend trees
	/// </summary>
	public void updateAnimatiorParameters()
	{
		// update animator if using character
		animator.SetFloat(_animIDSpeed, agent.desiredVelocity.magnitude);
		animator.SetFloat(_animIDMotionSpeed, 1);
	}
}
