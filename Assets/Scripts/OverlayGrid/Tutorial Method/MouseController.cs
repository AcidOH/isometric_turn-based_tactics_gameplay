using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var focusedTileHit = GetFocusedOnTile();
        
        Debug.Log(focusedTileHit);

        if (focusedTileHit.HasValue)
        {
            GameObject overlayTile = focusedTileHit.Value.collider.gameObject;
            transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
        }
    }
    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2d, Vector2.zero);
        
        if(hits.Length > 0) //if no object was found, there is no minimum
        {
            if (hits.Length != 1 && hits[0].transform != gameObject.transform)
            //if only one object is found and it is the player, there is also no minimum
            {
                float min = hits[0].distance; //minimum distance is at the first object
                int minIndex = 0; //store the index of the min in order to find object later

                for (int i = 1; i < hits.Length; i++) //for i in hits from index 1 to last item in list
                {
                    if (hits[i].transform != gameObject.transform && hits[i].distance < min)
                    //checks is theres a value lower that the already defined min that isn't the player 
                    {
                        min = hits[i].distance; //updates min to be the new minimum
                        minIndex = i; //refreshes the distance
                    }
                }

                Debug.Log ("minIndex = " + minIndex + ", hits[minIndex] = " + hits[minIndex]);
                
                return hits[minIndex]; //gets game object from index of smallest distance
                
                //return hits[minIndex];
                //return hits[minIndex].transform.position.z;
            }
        }
        return null;
    }
}
