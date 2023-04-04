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
    [SerializeField] float minimumDevideRate; //������ �������� �ּ� ����
    [SerializeField] float maximumDivideRate; //������ �������� �ִ� ����
    [SerializeField] private GameObject line; //lineRenderer�� ����ؼ� ������ �������� ���������� �����ֱ� ����
    [SerializeField] private GameObject map; //lineRenderer�� ����ؼ� ù ���� ����� �����ֱ� ����
    [SerializeField] private GameObject roomLine; //lineRenderer�� ����ؼ� ���� ����� �����ֱ� ����
    [SerializeField] private int maximumDepth; //Ʈ���� ����, ���� ���� ���� �� �ڼ��� ������ ��
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile roomTile; //���� �����ϴ� Ÿ��
    [SerializeField] Tile wallTile; //��� �ܺθ� ���������� �� Ÿ��
    [SerializeField] Tile outTile; //�� �ܺ��� Ÿ��
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
