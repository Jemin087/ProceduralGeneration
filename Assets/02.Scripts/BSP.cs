using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Node parNode;
    public RectInt nodeRect;
    public RectInt roomRect;

    public Vector2Int center
    {
        get
        {
            return new Vector2Int(roomRect.x * roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }
}

public class BSP : MonoBehaviour
{
    [SerializeField]
    Vector2Int mapSize;

    [SerializeField]
    float minDevideRate;

    [SerializeField]
    float maxDevideRate;

    [SerializeField]
    GameObject line;

    [SerializeField]
    GameObject map;

    [SerializeField]
    GameObject roomLine;


    [SerializeField]
    int maxDepth;       //트리의높이 높을수록 방을 자세히 나눈다


    private void Start()
    {
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        DrawMap(0, 0);
        Divide(root, 0);
    }

    void DrawMap(int x,int y)
    {
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(x+mapSize.x, y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(x+mapSize.x, y+mapSize.y) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(x, y+mapSize.y) - mapSize / 2);
    }

    void Divide(Node tree,int n)
    {
        if (n == maxDepth) return;

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        int split = Mathf.RoundToInt(Random.Range(maxLength * minDevideRate, maxLength * maxDevideRate));

        if(tree.nodeRect.width>=tree.nodeRect.height)
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));

            DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
           
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width,split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));

            DrawLine(new Vector2(tree.nodeRect.x, tree.nodeRect.y + split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y + split));
        }
        tree.leftNode.parNode = tree;
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1);
        Divide(tree.rightNode,n + 1);
    }

    void DrawLine(Vector2 from, Vector2 to)
    {
        LineRenderer line = Instantiate(this.line).GetComponent<LineRenderer>();
        line.SetPosition(0, from - mapSize / 2);
        line.SetPosition(1, to - mapSize / 2);
    }


    RectInt GenerateRoom(Node tree,int n)
    {
        RectInt rect;

        if(n==maxDepth)
        {
            rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);
            int height = Random.Range(rect.height / 2, rect.height - 1);


            int x = rect.x = Random.Range(1, rect.width - width);
            int y = rect.y = Random.Range(1, rect.height - height);

            rect = new RectInt(x, y, width, height);
            DrawRectangle(rect);
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }

    void DrawRectangle(RectInt rect)
    {

    }
}
