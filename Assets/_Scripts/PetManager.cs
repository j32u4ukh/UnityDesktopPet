using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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

    Vector3 collider_offset, cursor_offset;

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

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Debug.Log($"Mouse: {vector2ToString(Mouse.current.position.ReadValue())}");
            Debug.Log($"mousePosition: {vector2ToString(Input.mousePosition)}");
            
            Mouse.current.WarpCursorPosition(new Vector2(1920.0f, 1080.0f));

            Debug.Log($"Mouse: {vector2ToString(Mouse.current.position.ReadValue())}");
            Debug.Log($"mousePosition: {vector2ToString(Input.mousePosition)}");
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Debug.Log($"mousePosition: {vector2ToString(Input.mousePosition)}, Mouse: {vector2ToString(Mouse.current.position.ReadValue())}");
            Mouse.current.WarpCursorPosition(new Vector2(960.0f, 540.0f));

            InputState.Change(Mouse.current.position, new Vector2(960.0f, 540.0f));

            Debug.Log($"x: {Mouse.current.position.x.ReadValue()}, y: {Mouse.current.position.y.ReadValue()}");

            Debug.Log($"mousePosition: {vector2ToString(Input.mousePosition)}, Mouse: {vector2ToString(Mouse.current.position.ReadValue())}");
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Debug.Log($"mousePosition: {vector3ToString(mousePosition)}");

            // z = -10
            Vector3 world_point = Camera.main.ScreenToWorldPoint(mousePosition);
            //Debug.Log($"world_point: {world_point}");

            //Vector3 ScreenPoint = Camera.main.WorldToScreenPoint(world_point);
            //Debug.Log($"ScreenPoint: {ScreenPoint}");

            BoxCollider2D collider = (BoxCollider2D)Physics2D.OverlapPoint(world_point);

            //if (collider != null)
            //{
            //    is_dragging = true;

            //    Vector3 world_top_right = collider.bounds.center + collider_offset;
            //    (Vector3 screen, Vector3 world) cursor = getCursorPosition(world_position: world_top_right);
            //    //Debug.Log($"world_top_right: {vector3ToString(world_top_right)}, cursor.world: {vector3ToString(cursor.world)}");

            //    //Vector3 screen_top_right = Camera.main.WorldToScreenPoint(world_point);
            //    //Debug.Log($"screen_top_right: {vector3ToString(screen_top_right)}, cursor.screen: {vector3ToString(cursor.screen)}");

            //    // real offset in world coordinate(z = 0)
            //    cursor_offset = cursor.world - collider.bounds.center;

            //    //Vector3 tr_world = Camera.main.ScreenToWorldPoint(tr_screen);
            //    //Vector3 tr_screen_modify = Camera.main.WorldToScreenPoint(new Vector3((int)tr_world.x, (int)tr_world.y, -10f));
            //    //screen_cursor_offset = tr_screen_modify - collider.bounds.center;

            //    //// z = 10
            //    //Vector3 tr_cursor = tr_screen_modify - screen_cursor_offset;
            //    //tr_cursor.z = -10f;
            //    //Vector3 cursor = Camera.main.WorldToScreenPoint(tr_cursor);

            //    //Debug.Log($"center: {collider.bounds.center}, tr_screen: {tr_screen}, tr_world: {tr_world}\n" +
            //    //          $"tr_world_modify: {new Vector3((int)tr_world.x, (int)tr_world.y, tr_world.z)}, tr_screen_modify: {tr_screen_modify}, cursor_offset: {screen_cursor_offset}");
            //    //Debug.Log($"(mousePosition: {mousePosition}, tr_screen_modify - cursor_offset: {tr_screen_modify - screen_cursor_offset}, cursor: {cursor})");

            //    last_screen_point = cursor.screen;
            //    //last_point.z = -10f;

            //    //setCursorPosition((int)cursor.screen.x, (int)cursor.screen.y);
            //    Mouse.current.WarpCursorPosition(cursor.screen);
            //    Debug.Log($"Input.mousePosition: {Mouse.current.position.ReadDefaultValue()}, cursor.screen: {vector3ToString(cursor.screen)}");
            //}
        }
        //else if (Input.GetMouseButton(0) && is_dragging)
        //{
        //    Debug.Log($"Input.mousePosition: {Input.mousePosition}, last_point: {last_screen_point}");

        //    if (Input.mousePosition != last_screen_point)
        //    {
        //        // z = -10
        //        Vector3 world_point = Camera.main.ScreenToWorldPoint(Input.mousePosition) - cursor_offset;
        //        transform.position = world_point;

        //        //transform.position = new Vector3(point.x - SIZE, point.y - SIZE, 0f);
        //        //transform.position = new Vector3(point.x, point.y, 0f);
        //    }
        //}
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    if (is_dragging)
        //    {
        //        //Vector3 cursor = Camera.main.WorldToScreenPoint(last_point + new Vector3(SIZE, SIZE, 0f));
        //        //Debug.Log($"(last_point: {last_point}, cursor: {cursor})");
        //        //SetCursorPos((int)cursor.x, (int)cursor.y);
        //        is_dragging = false;
        //    }
        //}

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

    public (Vector3 screen, Vector3 world) getCursorPosition(Vector3 world_position)
    {
        Vector3 screen_position = Camera.main.WorldToScreenPoint(world_position);
        Vector3 screen_cursor = new Vector3((int)screen_position.x, (int)screen_position.y, screen_position.z);
        screen_cursor.z = 0;
        Vector3 world_cursor = Camera.main.ScreenToWorldPoint(screen_cursor);

        return (screen_cursor, world_cursor);
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
