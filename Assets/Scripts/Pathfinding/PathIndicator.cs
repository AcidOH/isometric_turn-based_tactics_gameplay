using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PathIndicator : MonoBehaviour
{
    Overlay overlay;

    public List<Sprite> arrowSprites;

    [SerializeField]
    private bool useGradient;

    [SerializeField]
    private Color startColor, endColor;

    void Start()
    {
        overlay = GetComponent<Overlay>();
    }

    public enum ArrowDirection
    {
        None = 0,
        UpOrDown = 1,
        LeftOrRight = 2,
        TopRight = 3,
        BottomRight = 4,
        TopLeft = 5,
        BottomLeft = 6,
        UpEnd = 7,
        DownEnd = 8,
        LeftEnd = 9,
        RightEnd = 10
    }

    public ArrowDirection TranslateDirection(Vector2Int prev, Vector2Int next)
    {
        //FINDING THE DIRECTION
        //(you don't need to use else if because of the return)
        //remember that prev and next are vectors that point in the direction of movement (rather than the tile pos w/r to curr)

        if (prev == Vector2Int.up) //(below curr, pointing up)
        {
            if (next == Vector2Int.up) return ArrowDirection.UpOrDown;
            if (next == Vector2Int.left) return ArrowDirection.BottomLeft;
            if (next == Vector2Int.right) return ArrowDirection.BottomRight;
            if (next == Vector2Int.zero) return ArrowDirection.UpEnd;
        }
        if (prev == Vector2Int.down)
        {
            if (next == Vector2Int.down) return ArrowDirection.UpOrDown;
            if (next == Vector2Int.left) return ArrowDirection.TopLeft;
            if (next == Vector2Int.right) return ArrowDirection.TopRight;
            if (next == Vector2Int.zero) return ArrowDirection.DownEnd;
        }
        if (prev == Vector2Int.left)
        {
            if (next == Vector2Int.up) return ArrowDirection.TopRight;
            if (next == Vector2Int.down) return ArrowDirection.BottomRight;
            if (next == Vector2Int.left) return ArrowDirection.LeftOrRight;
            if (next == Vector2Int.zero) return ArrowDirection.LeftEnd;
        }
        if (prev == Vector2Int.right)
        {
            if (next == Vector2Int.up) return ArrowDirection.TopLeft;
            if (next == Vector2Int.down) return ArrowDirection.BottomLeft;
            if (next == Vector2Int.right) return ArrowDirection.LeftOrRight;
            if (next == Vector2Int.zero) return ArrowDirection.RightEnd;
        }
        if (prev == Vector2Int.zero)
        {
            if (next == Vector2Int.up || next == Vector2Int.down) return ArrowDirection.UpOrDown;
            if (next == Vector2Int.left || next == Vector2Int.right) return ArrowDirection.LeftOrRight;
        }
        return ArrowDirection.None;
    }

    public void DrawPath()
    {
        if (overlay.path != null)
        {
            if (useGradient != true)
            {
                for (int i = 0; i < overlay.path.Count - 1; i++)
                {
                    int currentIndex = i;

                    var arrow = overlay.path[i].overlayTile.GetComponentInChildren<SpriteRenderer>();
                    arrow.color = new Color(1, 1, 1, 1);

                    (Vector2Int, Vector2Int) prevAndnext = GetPrevAndNextNode(currentIndex);
                    PathIndicator.ArrowDirection direction = TranslateDirection(prevAndnext.Item1, prevAndnext.Item2);

                    SetArrowSprite(overlay.path[i], direction);
                }
            }
            else 
            {
                Gradient gradient = CreateGradient(startColor, endColor, (float)overlay.path.Count -1);

                for (int i = 0; i < overlay.path.Count; i++)
                {
                    float numer = (float)i;
                    float denom = (float)overlay.path.Count;

                    float t = numer / denom; //I don't think theres a way to round to specific number of decimal places D:
                    
                    var arrow = overlay.path[i].overlayTile.GetComponentInChildren<SpriteRenderer>();
                    arrow.color = gradient.Evaluate(t);

                    //Debug.Log("Place in path is " + i + ", color is " + arrow.color);
                }
            }
        }
    }

    public void ErasePath()
    {
        if (overlay.path != null)
        {
            for (int i = 0; i < overlay.path.Count; i++)
            {
                var arrow = overlay.path[i].overlayTile.GetComponentInChildren<SpriteRenderer>();
                arrow.color = new Color(1, 1, 1, 0);
                arrow.sprite = arrowSprites[0];
            }
        }
    }

    private (Vector2Int, Vector2Int) GetPrevAndNextNode(int i)
    {
        Vector2Int curr = new Vector2Int(overlay.path[i].gridX, overlay.path[i].gridY);
        int h = i - 1;
        int j = i + 1;

        if (i == 0)
        {
            h = 0;
        }
        else if (i == overlay.path.Count - 2)
        {
            j = overlay.path.Count - 2;
        }

        Vector2Int prev = curr - new Vector2Int(overlay.path[h].gridX, overlay.path[h].gridY);
        Vector2Int next = new Vector2Int(overlay.path[j].gridX, overlay.path[j].gridY) - curr;

        return (prev, next);
    }

    private void SetArrowSprite(Node _node, PathIndicator.ArrowDirection _direction)
    {
        var arrow = _node.overlayTile.GetComponentInChildren<SpriteRenderer>();

        //arrow.color = new Color(1, 1, 1, 1);
        arrow.sprite = arrowSprites[(int)_direction];
    }

    private Gradient CreateGradient(Color _startColor, Color _endColor, float pathLength)
    {   
        var _gradient = new Gradient();
        var colors = new GradientColorKey[2];
        var alphas = new GradientAlphaKey[2];
        
        colors[0] = new GradientColorKey(_startColor, 0.0f); //second number is time
        colors[1] = new GradientColorKey(_endColor, 1.0f);

        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        _gradient.SetKeys(colors, alphas);

        return _gradient;
    }
}
