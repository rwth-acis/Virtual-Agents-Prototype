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
	NavMeshAgent navMeshAgent;

	private Vector2 previousForwardVector;
	private Vector3 previousPosition;
	private GameObject target;

	//Animation Parameter Names

	[SerializeField] private string forwardSpeed = "Speed";
	[SerializeField] private string angularSpeed = "Turn";
	//The speed where the full running anmation is synchrone with the floor
	[SerializeField] private float movmentTopSpeed = 4;
	[SerializeField] private float angularTopSpeed = 20;

	public bool movementHandeldByAnimator = false;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDAngularSpeed;

	private void Awake()
	{
		AssignAnimationIDs();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

    private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash(forwardSpeed);
		_animIDAngularSpeed = Animator.StringToHash(angularSpeed);
	}



	/// <summary>
	/// Updates the animation parameters for the blend trees
	/// </summary>
	public void updateAnimatiorParameters()
	{
		//Update forward speed
		float speed = (transform.position - previousPosition).magnitude / Time.deltaTime;
		previousPosition = transform.position;
		animator.SetFloat(_animIDSpeed, speed / movmentTopSpeed, 0.1f, Time.deltaTime);

		//Update angeluar speed
		Vector2 currentForwarVector = new Vector2(agent.transform.forward.x, agent.transform.forward.z);
		float curSpeed = Vector2.SignedAngle(currentForwarVector,previousForwardVector) / Time.deltaTime;
		previousForwardVector = currentForwarVector;
		animator.SetFloat(_animIDAngularSpeed, curSpeed/ angularTopSpeed, 0.1f, Time.deltaTime);
		Debug.Log(curSpeed);
	}

    private void Update()
    {
		if (target != null && navMeshAgent.destination != target.transform.position)
		{
			navMeshAgent.SetDestination(target.transform.position);
		}
		updateAnimatiorParameters();
    }

	public void OnAnimatorMove()
	{
		//Override, to disabel all movment from the animator, otherwise it would interfere with the navmeshAgent movement.
	}
}
