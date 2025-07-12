using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MyVersonRAC : MonoBehaviour
{
    [SerializeField]
    private float radius; //radius about center of menu

    [SerializeField]
    private Dictionary<int, Vector2> selector_positions;

    [SerializeField]
    private int current_option;
    [SerializeField]
    private int previous_option;

    public GameObject selector;
    public GameObject menu_bknd;

    private Vector3 origin;
    private Vector3 selector_initial_pos;

    private Camera cam;
    
    private Cursor cursor;
    private Vector3 world_mouse_pos;


    void Start()
    {
        cam = Camera.main;
        Vector2 originVec2 = cam.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        origin = new Vector3(originVec2.x, originVec2.y, menu_bknd.transform.position.z);
        
        selector_initial_pos = selector.transform.position;

        Debug.Log("origin: " + originVec2);
        Debug.Log("selector_initial_pos: " + selector_initial_pos);

        radius = Mathf.Abs((selector_initial_pos - origin).magnitude);
        float rad45 = radius * Mathf.Sin(45);

        selector_positions = new Dictionary<int, Vector2>
        {
            {0, new Vector2(0, radius) + originVec2},
            {45, new Vector2(rad45, rad45) + originVec2},
            {90, new Vector2(radius, 0) + originVec2},
            {135, new Vector2(rad45, -rad45) + originVec2},
            {180, new Vector2(0, -radius) + originVec2},
            {225, new Vector2(-rad45, -rad45) + originVec2},
            {270, new Vector2(-radius, 0) + originVec2},
            {315, new Vector2(-rad45, rad45) + originVec2}
        };

        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        world_mouse_pos = cursor.worldMousePos;

        MoveSelector(current_option);
    }

    void Update()
    {
        world_mouse_pos = cursor.worldMousePos;

        float cursor_angle = DetermineAngle(world_mouse_pos);
        int option = DetermineOptionFromAngle(cursor_angle);

        /*Debug.Log("prev_option = " + previous_option);
        Debug.Log("curr_option = " + option);

        previous_option = current_option;
        current_option = option;

        if (current_option != previous_option)*/
        MoveSelector(option);
    
    }

    void MoveSelector(int option)
    {
        Vector3 pos = new Vector3(selector_positions[option].x, selector_positions[option].y, selector.transform.position.z);
        selector.transform.position = pos;
        selector.transform.rotation = Quaternion.Euler(0,0,-option);
        Debug.Log("selector_pos: " + pos);
    }

    float DetermineAngle(Vector3 initial_pos)
    //Determines the angle of initial_pos with respect to the origin;
    //Clockwise direction, 0 is directly up.
    {
        Vector3 direction = initial_pos - origin;
        float angle = 0;

        if (direction.x == 0)
        {
            //if (direction.y > 0)
                //return 0;
            if (direction.y < 0)
                angle = 180;
        }
        else if (direction.y == 0)
        {
            if (direction.x > 0)
                angle = 90;
            else if (direction.y < 0)
                angle = 270;
        }

        float inner_angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        //somethign goofy occurs here for angles greater than 180

        if (Mathf.Abs(direction.y) < 0)
            angle = 180 + inner_angle;
        else if (Mathf.Abs(direction.x) < 0)
            angle = 360 + inner_angle;
        else if (Mathf.Abs(direction.x) > 0)
            angle = inner_angle;

        Debug.Log("DetermineAngle: " + angle);
        return angle;
    }

    int DetermineOptionFromAngle(float angle)
    //Determines what area the mouse is hovering over.
    {
        int option = 0;
        
        if (angle >= 22.5 && angle < 67.5f)
            option = 45;
        else if (angle >= 67.5 && angle < 112.5f)
            option = 90;
        else if (angle >= 112.5 && angle < 157.5f)
            option = 135;
        else if (angle >= 157.5 && angle < 202.5f)
            option = 180;
        else if (angle >= 202.5 && angle < 247.5f)
            option = 225;
        else if (angle >= 247.5 && angle < 292.5f)
            option = 270;
        else if (angle >= 292.5 && angle < 337.5f)
            option = 315;

        Debug.Log("DetermineOptionFromAngle: " + option);
        return option;
    }

}
