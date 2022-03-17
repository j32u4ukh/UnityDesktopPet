using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// 管理 Pet 與使用者之間的互動
[RequireComponent(typeof(Animator))]
public class PetManager : MonoBehaviour
{
    public float speed = 1.0f;
    //[SerializeField] float animation_during;

    SpriteRenderer sprite_renderer;
    Animator animator;
    AudioManager audio_manager;
    Anim anim;
    WaitForSeconds delta_time = new WaitForSeconds(0.01f);

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
        animator = GetComponent<Animator>();
        audio_manager = GetComponent<AudioManager>();
        anim = Anim.Idle;
        offset = Vector3.zero;

        #region 邊界值考慮 SpriteRenderer 尺寸，避免 SpriteRenderer 跑到視窗之外
        Vector3 min_point = Camera.main.ScreenToWorldPoint(Vector3.zero);
        x_min = min_point.x + size;
        //x_min = min_point.x;
        y_min = min_point.y + size;
        Debug.Log($"x_min: {x_min}, y_min: {y_min}");

        Vector3 max_point = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        x_max = max_point.x - size;
        //x_max = max_point.x;
        y_max = max_point.y - size;
        Debug.Log($"x_max: {x_max}, y_max: {y_max}"); 
        #endregion
    }

    // Update is called once per frame
    void Update()
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

                //case Anim.BeforeSleeping:
                //    animator.SetTrigger("BeforeSleeping");
                //    anim = Anim.Sleeping;
                //    break;

                //case Anim.Sleeping:
                //    //playAnimation(anim_name: "Sleeping", time: 5.0f);
                //    animator.SetBool("Sleeping", true);
                //    anim = Anim.Idle;
                //    break;

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

        x_position = Mathf.Min(x_max, Mathf.Max(transform.position.x + x_move * Time.deltaTime * speed, x_min));
        y_position = Mathf.Min(y_max, Mathf.Max(transform.position.y + y_move * Time.deltaTime * speed, y_min));
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

        while(time > 0f)
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

        Debug.Log($"[PetManager] iterAnimation | time: {time}");
        animator.SetBool(anim_name, false);
        anim = Anim.Idle;
    }

    //private IEnumerator ienumSleeping()
    //{
    //    animator.SetTrigger("BeforeSleeping");
    //    anim = Anim.Sleeping;
    //    animator.SetBool("Sleeping", true);
    //}

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
