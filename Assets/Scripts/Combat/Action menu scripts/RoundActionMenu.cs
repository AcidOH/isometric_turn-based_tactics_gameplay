using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundActionMenu : MonoBehaviour
{
    private Camera cam;
    private Cursor cursor;
    private GameObject menu_selector;

    private const int numberOfOptions = 8;
    private const float anglePerOption = 360f/ numberOfOptions;
    private Vector2 selector_original_height;

    [SerializeField]
    private float orbitRadius
    {
        get 
        {
            float radius = Mathf.Sqrt(Mathf.Pow(selector_original_height.x,2) + Mathf.Pow(selector_original_height.y, 2));
            return radius;
        }
    }

    //[SerializeField]
    //private float facingRadius = 1f;

    private int currentOptionIndex = -1;

    private Vector2 cursor_pos;
    private Vector2 curr_selector_pos;

    private void Start()
    {
        cam = Camera.main;
        cursor = GameObject.Find("Cursor").GetComponent<Cursor>();
        menu_selector = GameObject.Find("Selector");
        selector_original_height = new Vector2(0f, menu_selector.transform.localPosition.y);
    }
    void Update()
    {
        cursor_pos = cursor.worldMousePos;
        curr_selector_pos = new Vector2(menu_selector.transform.position.x, menu_selector.transform.position.y);
        
        UpdateSelectorPosition();
    }

    private void UpdateSelectorPosition() //this is most likely where it is messing up.
    {
        Vector2 direction = cursor_pos - curr_selector_pos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360f;
        }

        int optionIndex = Mathf.RoundToInt(angle / 45f /*anglePerOption*/) % numberOfOptions;

        if (optionIndex != currentOptionIndex)
        {
            currentOptionIndex = optionIndex;
            MoveSelectorTo(optionIndex);
        }  
    }

    private void MoveSelectorTo(int optionIndex)
    {
        float targetAngle = optionIndex * anglePerOption;

        Vector2 orbitOffset = new Vector2(Mathf.Cos(targetAngle * Mathf.Deg2Rad), Mathf.Sin(targetAngle * Mathf.Deg2Rad)) * orbitRadius;

        menu_selector.transform.localPosition = curr_selector_pos + orbitOffset; // + selector_original_height;

        //Vector2 facingDirection = (new Vector2(menu_selector.transform.localPosition.x - curr_selector_pos.x, menu_selector.transform.localPosition.y - curr_selector_pos.y)).normalized;

        float rotationAngle = Mathf.Atan2(orbitOffset.y, orbitOffset.x) * Mathf.Rad2Deg;
        menu_selector.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle - 90f);
    }

}
