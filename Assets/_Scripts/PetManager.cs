using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public enum Anim
{
    Idle = -1,
    Maguma = 0,
    Walk
}

// 管理 Pet 與使用者之間的互動
[RequireComponent(typeof(Animator))]
public class PetManager : MonoBehaviour
{
    SpriteRenderer sprite_renderer;
    BoxCollider2D m_collider;
    Animator animator;
    Anim anim;

    Vector3 offset;
    bool is_dragging = false;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        anim = Anim.Idle;
        offset = Vector3.zero;      
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
            Debug.Log($"mousePosition: {mousePosition.formatString()}");

            Vector3 world_point = Camera.main.ScreenToWorldPoint(mousePosition);
            Debug.Log($"world_point: {world_point.formatString()}");

            BoxCollider2D collider = (BoxCollider2D)Physics2D.OverlapPoint(world_point);

            if (collider != null)
            {
                is_dragging = true;
                offset = collider.bounds.center - world_point;
                Debug.Log($"offset: {offset.formatString()}, center: {collider.bounds.center.formatString()}, world_point: {world_point.formatString()}");
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

            if(Vector3.Distance(transform.position, world_point) > 0.05f)
            {
                Debug.Log($"transform.position: {transform.position.formatString()}, world_point: {world_point.formatString()}, " +
                          $"Distance: {Vector3.Distance(transform.position, world_point)}");
                transform.position = world_point;
            }
        }
    }

    private void turnAround()
    {
        sprite_renderer.flipX = !sprite_renderer.flipX;
    }

    private static void setCursorPosition(float x, float y, bool new_input_system = true)
    {
        if (new_input_system)
        {
            Vector2 destination = new Vector2(x, y);
            Mouse.current.WarpCursorPosition(destination);
            InputState.Change(Mouse.current.position, destination);
        }
        else
        {
            SetCursorPos((int)x, Screen.height - (int)y);
        }
    }
}
