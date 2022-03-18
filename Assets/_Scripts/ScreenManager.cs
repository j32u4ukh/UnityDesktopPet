using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// �޲z Pet �P�ϥΪ̤���������
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

    // ���U����
    public abstract void getLeftMouseDown();

    // �즲����
    public abstract void getLeftMouse();

    // ��}����
    public abstract void getLeftMouseUp();

    // ���U�k��
    public abstract void getRightMouseDown();

    // �즲�k��
    public abstract void getRightMouse();

    // ��}�k��
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
        // �ϥ� else if�A�קK�h�Ӱʵe�P��Ĳ�o
        #region �w�q���s�PĲ�o�ʵe
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

        #region ����
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

        #region �k��
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

        // �Y���b���ʡA�h�u�����񨫸��ʵe
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

        // �����ʵe�|���_��L���b���񪺰ʵe
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

        // Walk �ʵe�ݦP�ɺ���: 1. Walk == true 2. ���ʼƭȤj�󲾰ʪ��e�A�~�|�QĲ�o
        animator.SetFloat("Move", Mathf.Abs(x_move) + Mathf.Abs(y_move));

        // �@���}�l���ʡA�N�N Walk �]�� false�A�קK����Ĳ�o
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            // Walk �ʵe�������A�ݦP�ɺ���: 1. Walk == false 2. ���ʼƭȤp���R����e
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
