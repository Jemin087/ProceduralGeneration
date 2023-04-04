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
            return new Vector2Int(roomRect.x + roomRect.width / 2, roomRect.y + roomRect.height / 2);
        }
    }
    public Node(RectInt rect)
    {
        this.nodeRect = rect;   
    }
}

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private GameObject line; //lineRenderer를 사용해서 공간이 나눠진걸 시작적으로 보여주기 위함
    [SerializeField] private GameObject map; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    [SerializeField] private GameObject roomLine; //lineRenderer를 사용해서 방의 사이즈를 보여주기 위함
    [SerializeField] private int maximumDepth; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile roomTile; //방을 구성하는 타일
    [SerializeField] Tile wallTile; //방과 외부를 구분지어줄 벽 타일
    [SerializeField] Tile outTile; //방 외부의 타일
    // Start is called before the first frame update
    void Start()
    {
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        DrawMap(0, 0);
        DivideDugeon(root, 0);
        GeneratorRoom(root, 0);
    }



    void DivideDugeon(Node tree,int n)
    {
        if (n == maximumDepth) return;

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));

        if(tree.nodeRect.width>=tree.nodeRect.height)
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));

            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            DrawLine(new Vector2(tree.nodeRect.x + split, tree.nodeRect.y), new Vector2(tree.nodeRect.x + split, tree.nodeRect.y + tree.nodeRect.height));
        }
        else
        {
            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width - split, tree.nodeRect.height));

            tree.leftNode.parNode = tree;
            tree.rightNode.parNode = tree;
            DivideDugeon(tree.leftNode, n + 1);
            DivideDugeon(tree.rightNode, n + 1);

        }

    }

    RectInt GeneratorRoom(Node tree,int n)
    {
        RectInt rect;
        
        if(n==maximumDepth)
        {
            rect = tree.nodeRect;
            int width = Random.Range(rect.width / 2, rect.width - 1);

            int height = Random.Range(rect.height / 2, rect.height - 1);

            int x = rect.x + Random.Range(1, rect.width - width);
            int y = rect.y + Random.Range(1, rect.height - height);

            rect = new RectInt(x, y, width, height);
            DrawRectangle(rect);
           // FillRoom(rect);
        }
        else
        {
            tree.leftNode.roomRect = GeneratorRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GeneratorRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }

        return rect;
    }


    void DrawLine(Vector2 from,Vector2 to)
    {
        LineRenderer lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }

    void DrawMap(int x,int y)
    {
        LineRenderer lineRenderer = Instantiate(map).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(x+mapSize.x, y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(x+mapSize.x, y+mapSize.y) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(x, y+mapSize.y) - mapSize / 2);
    }

    void DrawRectangle(RectInt rect)
    {
        LineRenderer lineRenderer = Instantiate(roomLine).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(rect.x, rect.y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(rect.x+rect.width, rect.y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(rect.x+rect.width, rect.y+rect.height) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(rect.x, rect.y+rect.height) - mapSize / 2);

    }

    void FillBackground()
    {
        for(int i=-10; i<mapSize.x+10; i++)
        {
            for(int j=-10; j<mapSize.y+10; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }
}
