using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// 管理 Pet 與使用者之間的互動
[RequireComponent(typeof(Animator))]
public abstract class ScreenManager : MonoBehaviour
{        
    public float SPEED = 1.0f;
    float speed;
    //[SerializeField] float animation_during;

    protected SpriteRenderer sprite_renderer;
    protected Animator animator;
    AudioManager audio_manager;
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
        animator = GetComponent<Animator>();
        audio_manager = GetComponent<AudioManager>();
        anim = Anim.Idle;
        offset = Vector3.zero;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // 使用 else if，避免多個動畫同時觸發
        #region 定義按鈕與觸發動畫
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim = Anim.Maguma;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim = Anim.BeforeSleeping;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim = Anim.Petting;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim = Anim.Drinking;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            anim = Anim.Valentine;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim = Anim.Knocking;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            anim = Anim.Fire;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            anim = Anim.Lost;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            anim = Anim.Confuse;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            anim = Anim.Petted;
        }
        #endregion

        #region 左鍵
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
        #endregion

        #region 右鍵
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
        #endregion
    }

    public virtual void FixedUpdate()
    {
        x_move = Input.GetAxisRaw("Horizontal");
        y_move = Input.GetAxisRaw("Vertical");
        is_moving = move();

        // 若正在移動，則優先播放走路動畫
        if (is_moving)
        {
            anim = Anim.Walk;

            if (animator.GetBool("Sleeping"))
            {
                animator.SetBool("Sleeping", false);
            }
        }
        else if (anim.Equals(Anim.Sleeping))
        {
            // Do nothing
        }
        else if (anim.Equals(Anim.BeforeSleeping))
        {
            //StartCoroutine(ienumSleeping());
            animator.SetTrigger("BeforeSleeping");
            anim = Anim.Sleeping;
            animator.SetBool("Sleeping", true);
        }
        else
        {
            if (animator.GetBool("Sleeping"))
            {
                animator.SetBool("Sleeping", false);
            }

            switch (anim)
            {
                case Anim.Maguma:
                    animator.SetTrigger("Maguma");
                    anim = Anim.Idle;
                    break;

                case Anim.Petting:
                    playAnimation(anim_name: "Petting", time: 2.2f);
                    break;

                case Anim.Drinking:
                    playAnimation(anim_name: "Drinking", time: 1.5f);
                    break;

                case Anim.Valentine:
                    playAnimation(anim_name: "Valentine", time: 4.25f);
                    break;

                case Anim.Knocking:
                    playAnimation(anim_name: "Knocking", time: 1.8f);
                    break;

                case Anim.Fire:
                    animator.SetTrigger("Fire");
                    anim = Anim.Idle;
                    break;

                case Anim.Lost:
                    animator.SetTrigger("Lost");
                    anim = Anim.Idle;
                    break;

                case Anim.Confuse:
                    playAnimation(anim_name: "Confuse", time: 1.68f);
                    break;

                case Anim.Petted:
                    playAnimation(anim_name: "Petted", time: 2.0f);
                    break;
            }
        }
    }

    protected bool move()
    {
        if (x_move == 0f && y_move == 0)
        {
            animator.SetFloat("Move", 0f);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.SetBool("Walk", false);
            }

            return false;
        }

        // 走路動畫會打斷其他正在播放的動畫
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            animator.SetBool("Walk", true);
            audio_manager.interruptAudio();
        }

        if (x_move > 0)
        {
            sprite_renderer.flipX = false;
        }
        else if (x_move < 0)
        {
            sprite_renderer.flipX = true;
        }

        speed = SPEED / Mathf.Sqrt(Mathf.Pow(x_move, 2.0f) + Mathf.Pow(y_move, 2.0f));
        x_move *= Time.deltaTime * speed;
        y_move *= Time.deltaTime * speed;

        x_position = Mathf.Min(x_max, Mathf.Max(transform.position.x + x_move, x_min));
        y_position = Mathf.Min(y_max, Mathf.Max(transform.position.y + y_move, y_min));
        transform.position = new Vector3(x_position, y_position, transform.position.z);

        // Walk 動畫需同時滿足: 1. Walk == true 2. 移動數值大於移動門檻，才會被觸發
        animator.SetFloat("Move", Mathf.Abs(x_move) + Mathf.Abs(y_move));

        // 一旦開始移動，就將 Walk 設為 false，避免重複觸發
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            // Walk 動畫的結束，需同時滿足: 1. Walk == false 2. 移動數值小於靜止門檻
            animator.SetBool("Walk", false);
        }

        return true;
    }

    private void playAnimation(string anim_name, float time)
    {
        StartCoroutine(ienumAnimation(anim_name: anim_name, time: time));
    }

    private IEnumerator ienumAnimation(string anim_name, float time)
    {
        animator.SetBool(anim_name, true);

        while (time > 0f)
        {
            if (is_moving)
            {
                time = 0f;
            }
            else
            {
                time -= Time.deltaTime;
                yield return null;
            }
        }

        Debug.Log($"[ScreenManager] iterAnimation | time: {time}");
        animator.SetBool(anim_name, false);
        anim = Anim.Idle;
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
