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
          if (Input.GetKey("s"))
          {
                animator.SetBool("isWalking", true);
                Debug.Log("Player pressed \"s\" and invoked isWalking = \"" + animator.GetBool("isWalking") + "\"");
          }
          if (Input.GetKey("w"))
          {
                animator.SetBool("isRunning", true);
                Debug.Log("Player pressed \"w\" and invoked isRunning = \"" + animator.GetBool("isRunning") + "\"");
          }
          else if (!Input.GetKey("s") && !Input.GetKey("w"))
          {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                Debug.Log("Nothing is pressed, so isRunning == \"" + animator.GetBool("isRunning") + "\" and isWalking == \"" + animator.GetBool("isWalking") + "\"");
          }
    }
}
