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
public abstract class ScreenManager : MonoBehaviour
{        
    public float SPEED = 0.05f;
    float speed;

    protected SpriteRenderer sprite_renderer;
    protected BoxCollider2D m_collider;
    protected Animator animator;
    protected Anim anim;

    protected Vector3 offset;
    protected bool is_moving;
    protected bool is_dragging = false;

    protected readonly float size = 0.5f;
    protected float x_position, y_position, x_move, y_move, x_min, y_min, x_max, y_max;

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    // 按下左鍵
    public abstract void getLeftMouseDown();

    // 拖曳左鍵
    public abstract void getLeftMouse();

    // 放開左鍵
    public abstract void getLeftMouseUp();

    // 按下右鍵
    public abstract void getRightMouseDown();

    // 拖曳右鍵
    public abstract void getRightMouse();

    // 放開右鍵
    public abstract void getRightMouseUp();

    // Start is called before the first frame update
    public virtual void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        anim = Anim.Idle;
        offset = Vector3.zero;
    }

    // Update is called once per frame
    public virtual void Update()
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
            getLeftMouseDown();
        }
        else if (Input.GetMouseButton(0))
        {
            getLeftMouse();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            getLeftMouseUp();
        }

        if (Input.GetMouseButtonDown(1))
        {
            getRightMouseDown();
        }
        else if (Input.GetMouseButton(1))
        {
            getRightMouse();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            getRightMouseUp();
        }
    }

    private void FixedUpdate()
    {
        x_move = Input.GetAxisRaw("Horizontal");
        y_move = Input.GetAxisRaw("Vertical");
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

    protected bool move()
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

        speed = SPEED / Mathf.Sqrt(Mathf.Pow(x_move, 2.0f) + Mathf.Pow(y_move, 2.0f));
        x_move *= speed;
        y_move *= speed;

        x_position = Mathf.Min(x_max, Mathf.Max(transform.position.x + x_move, x_min));
        y_position = Mathf.Min(y_max, Mathf.Max(transform.position.y + y_move, y_min));
        transform.position = new Vector3(x_position, y_position, transform.position.z);

        return true;
    }

    protected static void setCursorPosition(float x, float y, bool new_input_system = true)
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
