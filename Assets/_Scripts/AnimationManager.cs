using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Anim
{
    Idle = -1,
    Maguma = 0,
    Walk
}

[RequireComponent(typeof(Animator))]
public class AnimationManager : MonoBehaviour
{
    SpriteRenderer sprite_renderer;
    Animator animator;
    Anim anim;

    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        anim = Anim.Idle;

        if (animator == null)
        {
            Debug.LogError("There is no Animator.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim = Anim.Maguma;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim = Anim.Walk;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            turnAround();
        }
    }

    private void FixedUpdate()
    {
        switch (anim)
        {
            case Anim.Maguma:
                animator.SetTrigger("Maguma");
                anim = Anim.Idle;
                break;

            case Anim.Walk:
                animator.SetTrigger("Walk");
                anim = Anim.Idle;
                break;
        }
    }

    private void turnAround()
    {
        sprite_renderer.flipX = !sprite_renderer.flipX;
    }
}
