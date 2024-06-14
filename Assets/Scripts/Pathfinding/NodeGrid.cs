using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    //public Transform player;

    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadiusX, nodeRadiusY;
    Node[,] grid;
    //^these are set in the inspector

    float nodeDiameterX, nodeDiameterY;
    int gridSizeX, gridSizeY;
	
	Overlay overlay;
	public List<Vector3Int> overlayGrid;
	public List<Vector3> overlayWorldGrid;

    void Start()
    {
	    overlay = GetComponent<Overlay>();
		overlayGrid = overlay.tiles;
		overlayWorldGrid = overlay.tilesWorldPos;

		Debug.Log("overlayGrid.Count = " + overlayGrid.Count);
		Debug.Log("overlayWorldGrid.Count = " + overlayWorldGrid.Count);
		

		nodeDiameterX = nodeRadiusX*2;
        nodeDiameterY = nodeRadiusY*2;
    	gridSizeX =  Mathf.RoundToInt(gridWorldSize.x/nodeDiameterX);
    	gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameterY);
	    CreateGrid();
    }

    void CreateGrid()
    {
	    /*grid = new Node[gridSizeX, gridSizeY]; //the furthest node from origin
    	Vector3 worldBottomLeft = transform.position - Vector3.right*gridWorldSize.x/2 - Vector3.forward*gridWorldSize.y/2;
	    for (int x = 0; x < gridSizeX; x ++) // starts creating nodes starting from the origin until it gets to the further node (created at start of fcn)
	    {
		    for (int y = 0; y < gridSizeY; y ++)
		    {
			    Vector3 worldPoint = worldBottomLeft + new Vector3(1,0,0) *(x*nodeDiameterX + nodeRadiusX) + new Vector3(0,1,0) * (y*nodeDiameterY + nodeRadiusY);
			    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusX, unwalkableMask));
                //^THIS MAY NOT BE THE WAY YOU WANT TO CHECK FOR COLLISIONS!
			    grid[x,y] = new Node(walkable, worldPoint, x, y);
		    }
	    }*/
		for (int index = 0; index < overlayGrid.Count; index++)
		{
			Vector3Int gridPoint = overlayGrid[index];
			Vector3 worldPoint = overlayWorldGrid[index];
			bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusX, unwalkableMask));
            //^THIS MAY NOT BE THE WAY YOU WANT TO CHECK FOR COLLISIONS!
			
			grid[gridPoint.x, gridPoint.y] = new Node(walkable, worldPoint, gridPoint.x, gridPoint.y);
		}
    }

    public List<Node> GetNeighbours(Node node)
    {
	    List<Node> neighbours = new List<Node>();
	    for (int x = -1; x <= 1; x++)
	    {
		    for (int y = -1; y <= 1; y++)
		    {
			    if (x == 0 && y == 0)
			    {
				    continue;
			    }
			    
                int checkX = node.gridX + x;
			    int checkY = node.gridY + y;
			
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
		    }
	    }
	    return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 targetWorldPosition) //can get world point from raycast in from Cursor script
    {
	    /*float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
	    float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
	    //^watch out for the 3D y and z switching
	
        percentX = Mathf.Clamp01(percentX);
	    percentY = Mathf.Clamp01(percentY);
	
        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
	    int y = Mathf.RoundToInt((gridSizeY-1) * percentY);*/


		//THIS IS BORROWED FROM CURSOR SCRIPT! (You need to turn it into a function that you can reuse)
        RaycastHit2D hit = Physics2D.Raycast(targetWorldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitOverlayTile = hit.collider.gameObject;
            Vector3 hitOverlayTilePos = hit.transform.position;

			foreach (var overlayTilePos in overlayWorldGrid)
			{
				if (hitOverlayTilePos == overlayTilePos)
				{
					Debug.Log("hitOverlayTilePos is in overlayWorldGrid");
				}
			}
			
			//int index = overlayWorldGrid.IndexOf(hitOverlayTilePos); //returns -1
			//Debug.Log("Index of hitOverlayTilePos in overlayWorldGrid: " + index);
		
			/*int x = overlayGrid[index].x;
			int y = overlayGrid[index].y;
	
        	return grid[x,y];*/
        }

		return null;
    }

    public List<Node> path;

    void OnDrawGizmos()
    {
	    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1)); 
	    //^dimensions are weird bc code is for 3D
	    if (grid != null)
        {
            //Node playerNode = NodeFromWorldPoint(player.position);
		
            foreach (Node n in grid)
		    {
			    Gizmos.color = (n.walkable)?Color.white:Color.red;
			
                if (path != null)
			    {
				    if (path.Contains(n))
				    {
					    Gizmos.color = Color.black;
				    }
			    }
			
                /*if (playerNode == n) 
			    {
				    Gizmos.color = Color.cyan;
			    }*/

			    Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameterX-0.1f, nodeDiameterY-0.1f, -0.5f) /*Vector3.one*(nodeDiameterX-0.1f)*/);
		    }
        }
    }
}