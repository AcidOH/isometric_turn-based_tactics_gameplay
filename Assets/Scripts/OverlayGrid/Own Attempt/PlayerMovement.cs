using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Cursor cursor;
    private GameObject tempPlayerSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posToMoveTo = cursor.GetComponent<Cursor>().hitOverlayTilePos;
        
        Debug.Log(gameObject.GetInstanceID().ToString());
        Debug.Log(gameObject.name.ToString());
        Debug.Log(this.GetInstanceID().ToString());
        Debug.Log(this.name.ToString());
        //gameObject.GetComponent<SpriteRenderer>().color = new Color(0.5f,1,1,1);
        transform.position = new Vector3(posToMoveTo.x, posToMoveTo.y, posToMoveTo.z - 0.5f);
    }
}
