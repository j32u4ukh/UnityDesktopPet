using UnityEditor;
using UnityEngine;

public class SingleScreenManager : ScreenManager
{
    Camera camera1;

    public override void Start()
    {
        base.Start();
        camera1 = GameObject.Find("Camera1").GetComponent<Camera>();

        #region 邊界值考慮 SpriteRenderer 尺寸，避免 SpriteRenderer 跑到視窗之外
        Vector3 min_point = camera1.ScreenToWorldPoint(Vector3.zero);
        x_min = min_point.x + size;
        y_min = min_point.y + size;
        Debug.Log($"x_min: {x_min}, y_min: {y_min}");

        Vector3 max_point = camera1.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        x_max = max_point.x - size;
        y_max = max_point.y - size;
        Debug.Log($"x_max: {x_max}, y_max: {y_max}");
        #endregion
    }

    public override void getLeftMouseDown()
    {
        Vector3 mousePosition = Input.mousePosition;
        Debug.Log($"mousePosition: {mousePosition.formatString()}");

        Vector3 world_point = camera1.ScreenToWorldPoint(mousePosition);
        Debug.Log($"world_point: {world_point.formatString()}");

        BoxCollider2D collider = (BoxCollider2D)Physics2D.OverlapPoint(world_point);

        if (collider != null)
        {
            is_dragging = true;
            offset = collider.bounds.center - world_point;
            Debug.Log($"offset: {offset.formatString()}, center: {collider.bounds.center.formatString()}, world_point: {world_point.formatString()}");
        }
    }

    public override void getLeftMouse()
    {
        if (is_dragging)
        {
            // z = -10
            Vector3 world_point = camera1.ScreenToWorldPoint(Input.mousePosition) + offset;

            if (Vector3.Distance(transform.position, world_point) > 0.05f)
            {
                Debug.Log($"transform.position: {transform.position.formatString()}, world_point: {world_point.formatString()}, " +
                          $"Distance: {Vector3.Distance(transform.position, world_point)}");
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
