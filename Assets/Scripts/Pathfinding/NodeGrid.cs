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
	//Overlay overlay;

    void Start()
    {
	    //overlay = GetComponent<Overlay>();
		//List<Vector3> overlayGrid = overlay.tilesWorldPos;
		
		nodeDiameterX = nodeRadiusX*2;
        nodeDiameterY = nodeRadiusY*2;
    	gridSizeX =  Mathf.RoundToInt(gridWorldSize.x/nodeDiameterX);
    	gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameterY);
	    CreateGrid();
    }

    void CreateGrid()
    {
	    grid = new Node[gridSizeX, gridSizeY];
    	Vector3 worldBottomLeft = transform.position - Vector3.right*gridWorldSize.x/2 - Vector3.forward*gridWorldSize.y/2;
	    for (int x = 0; x < gridSizeX; x ++)
	    {
		    for (int y = 0; y < gridSizeY; y ++)
		    {
			    Vector3 worldPoint = worldBottomLeft + new Vector3(1,0,0) *(x*nodeDiameterX + nodeRadiusX) + new Vector3(0,1,0) * (y*nodeDiameterY + nodeRadiusY);
			    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusX, unwalkableMask));
                //^THIS MAY NOT BE THE WAY YOU WANT TO CHECK FOR COLLISIONS!
			    grid[x,y] = new Node(walkable, worldPoint, x, y);
		    }
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

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
	    float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
	    float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
	    //^watch out for the 3D y and z switching
	
        percentX = Mathf.Clamp01(percentX);
	    percentY = Mathf.Clamp01(percentY);
	
        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
	    int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
	
        return grid[x,y];
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