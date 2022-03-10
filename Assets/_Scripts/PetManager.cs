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
    public float speed = 0.05f;

    SpriteRenderer sprite_renderer;
    BoxCollider2D m_collider;
    Animator animator;
    Anim anim;

    Vector3 offset;
    bool is_moving;
    bool is_dragging = false;

    readonly float size = 0.5f;
    float x_position, y_position, x_move, y_move, x_min, y_min, x_max, y_max;

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

        #region 邊界值考慮 SpriteRenderer 尺寸，避免 SpriteRenderer 跑到視窗之外
        Vector3 min_point = Camera.main.ScreenToWorldPoint(Vector3.zero);
        x_min = min_point.x + size;
        y_min = min_point.y + size;
        Debug.Log($"x_min: {x_min}, y_min: {y_min}");

        Vector3 max_point = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        x_max = max_point.x - size;
        y_max = max_point.y - size;
        Debug.Log($"x_max: {x_max}, y_max: {y_max}"); 
        #endregion
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
        x_move = Input.GetAxisRaw("Horizontal") * speed;
        y_move = Input.GetAxisRaw("Vertical") * speed;
        is_moving = move();

        if (is_moving)
        {
            anim = Anim.Walk;
        }

        switch (anim)
        {
            case Anim.Maguma:
                animator.SetTrigger("Maguma");
                anim = Anim.Idle;
                break;

            case Anim.Walk:
                // TODO: 移動時，觸發走路動畫，改用 float 來觸發
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

    private bool move()
    {
        if(x_move == 0f && y_move == 0)
        {
            return false;
        }

        if(x_move > 0)
        {
            sprite_renderer.flipX = false;
        }
        else if (x_move < 0)
        {
            sprite_renderer.flipX = true;
        }

        x_position = Mathf.Min(x_max, Mathf.Max(transform.position.x + x_move, x_min));
        y_position = Mathf.Min(y_max, Mathf.Max(transform.position.y + y_move, y_min));
        transform.position = new Vector3(x_position, y_position, transform.position.z);

        return true;
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
