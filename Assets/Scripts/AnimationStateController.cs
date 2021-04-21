using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
          animator = GetComponent<Animator>();
          Debug.Log(animator);
    }

    // Update is called once per frame
    void Update()
    {
          if (Input.GetKey("w"))
          {
                animator.SetBool("isWalking", true);
                Debug.Log("Player pressed \"w\"");
          }
          if (Input.GetKey("s"))
          {
                animator.SetBool("isRunning", true);
                Debug.Log("Player pressed \"s\"");
          }
          else
          {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
          }
    }
}
