using UnityEditor;
using UnityEngine;

public class DualScreenManager : ScreenManager
{
    Camera camera1;
    Camera camera2;
    int display_index = 0;

    // NOTE: 對 camera1 而言，從左邊螢幕跨到右邊螢幕，其實是從最大值再變為最小值，即左邊是 (-8 ~ 8) 右邊也是 (-8 ~ 8)，而非從 9 繼續計算
    // NOTE: 而 camera2 也相同，左邊是 (9 ~ 17) 右邊也是 (9 ~ 17)，而非從 0 開始計算
    // NOTE: 要正確的處理雙螢幕，應該動態切換所使用的相機，跨過中間線後就要切換相機

    public override void Start()
    {
        base.Start();
        camera1 = GameObject.Find("Camera1").GetComponent<Camera>();
        camera2 = GameObject.Find("Camera2").GetComponent<Camera>();

        #region 邊界值考慮 SpriteRenderer 尺寸，避免 SpriteRenderer 跑到視窗之外
        Vector3 min_point = camera1.ScreenToWorldPoint(Vector3.zero);
        Debug.Log($"camera1: {camera1.transform.position}, min_point: {min_point}");

        x_min = min_point.x + size;
        y_min = min_point.y + size;

        // (size) x_min: -8.388889, y_min: -4.5
        Debug.Log($"x_min: {x_min}, y_min: {y_min}");

        Vector3 max_point = camera2.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        Debug.Log($"camera2: {camera2.transform.position}, max_point: {max_point}");

        x_max = max_point.x - size;
        y_max = max_point.y - size;

        // (size) x_max: 26.16667, y_max: 4.5
        Debug.Log($"x_max: {x_max}, y_max: {y_max}");
        #endregion
    }

    public override void Update()
    {
        base.Update();

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.D))
        {
            display_index = 1;
        }
        else
        {
            display_index = 0;
        }
#endif

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.LogWarning($"[Info] display: {display_index}, position: {transform.position.formatString()}");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.position = Vector3.zero;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            x_min = -8.388889f;
            x_max = 26.16667f;
        }
    }

    public override void getLeftMouseDown()
    {
        Vector3 mouse_position = Input.mousePosition;
        Debug.Log($"mouse_position: {mouse_position.formatString()}");

#if UNITY_EDITOR
        // 
#else
        display_index = getDisplayIndex();
#endif

#if DEBUG
        Debug.Log($"Display index: {display_index}");
#endif

        Vector3 world_point;

        if (display_index.Equals(0))
        {
            world_point = camera1.ScreenToWorldPoint(mouse_position);
        }
        else
        {
            world_point = camera2.ScreenToWorldPoint(mouse_position);
        }

        BoxCollider2D collider = (BoxCollider2D)Physics2D.OverlapPoint(world_point);
        Debug.Log($"world_point: {world_point.formatString()}");

        if (collider != null)
        {
            is_dragging = true;
            offset = collider.bounds.center - world_point;
            Debug.Log($"offset: {offset.formatString()}, center: {collider.bounds.center.formatString()}");
        }
    }

    public override void getLeftMouse()
    {
        if (is_dragging)
        {
            Vector3 mouse_position = Input.mousePosition;

#if UNITY_EDITOR
            // 
#else
            display_index = getDisplayIndex();
#endif

#if DEBUG
            Debug.Log($"Display index: {display_index}");
#endif

            Vector3 world_point;

            if (display_index.Equals(0))
            {
                world_point = camera1.ScreenToWorldPoint(mouse_position) + offset;
            }
            else
            {
                world_point = camera2.ScreenToWorldPoint(mouse_position) + offset;
            }

            if (Vector3.Distance(transform.position, world_point) > 0.05f)
            {
                //Debug.Log($"transform.position: {transform.position.formatString()}, world_point: {world_point.formatString()}, " +
                //          $"Distance: {Vector3.Distance(transform.position, world_point)}");
                transform.position = world_point;
            }
        }
    }

    public override void getLeftMouseUp()
    {
        if (is_dragging)
        {
            is_dragging = false;
        }

#if UNITY_EDITOR
        // 
#else
            display_index = getDisplayIndex();
#endif

        //Debug.Log($"Display index: {display_index}, cursor_position: {getCursorPosition()}");
    }

    public override void getRightMouseDown()
    {
        throw new System.NotImplementedException();
    }

    public override void getRightMouse()
    {
        throw new System.NotImplementedException();
    }

    public override void getRightMouseUp()
    {
        throw new System.NotImplementedException();
    }
}
