using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using System;

public enum Anim
{
    Idle = -1,
    Maguma = 0,
    Walk
}

[RequireComponent(typeof(Animator))]
public class PetManager : MonoBehaviour
{
    SpriteRenderer sprite_renderer;
    BoxCollider2D m_collider;
    Animator animator;
    Anim anim;

    Vector3 collider_offset, offset;

    bool is_dragging = false;
    Vector3 last_screen_point = Vector3.zero;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        anim = Anim.Idle;

        collider_offset = new Vector3(m_collider.size.x / 2, m_collider.size.y / 2, 0f);
        offset = Vector3.zero;

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

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            //Vector3 mousePosition = Mouse.current.position.ReadValue();
            Debug.Log($"mousePosition: {vector3ToString(mousePosition)}");

            // z = -10
            Vector3 world_point = Camera.main.ScreenToWorldPoint(mousePosition);
            //world_point.z = 0f;
            Debug.Log($"world_point: {vector3ToString(world_point)}");

            BoxCollider2D collider = (BoxCollider2D)Physics2D.OverlapPoint(world_point);

            if (collider != null)
            {
                is_dragging = true;
                offset = collider.bounds.center - world_point;
                Debug.Log($"offset: {vector3ToString(offset)}, center: {vector3ToString(collider.bounds.center)}, world_point: {vector3ToString(world_point)}");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (is_dragging)
            {
                is_dragging = false;
            }
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

    private void OnMouseDrag()
    {
        if (is_dragging)
        {
            // z = -10
            Vector3 world_point = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            //world_point.z = 0f;

            if(Vector3.Distance(transform.position, world_point) > 0.05f)
            {
                Debug.Log($"transform.position: {transform.position}, world_point: {world_point}, Distance: {Vector3.Distance(transform.position, world_point)}");
                transform.position = world_point;
            }
        }
    }

    private void turnAround()
    {
        sprite_renderer.flipX = !sprite_renderer.flipX;
    }

    public void setCursorPosition(int x, int y)
    {
        SetCursorPos(x, Screen.height - y);
    }

    string vector2ToString(Vector2 v)
    {
        return string.Format("({0:F4}, {1:F4})", v.x, v.y);
    }

    string vector3ToString(Vector3 v)
    {
        return string.Format("({0:F4}, {1:F4}, {2:F4})", v.x, v.y, v.z);
    }
}
