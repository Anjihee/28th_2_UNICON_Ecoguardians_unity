using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{

    // ���� �߰��� ����(2��)
    public GameObject StartPanel_hj; // start �г�
    public AudioSource S_start_hj;  // start �Ҹ�
    // ���� �߰� ��

    public static Board Instance { get; private set; }

    [SerializeField] private AudioClip collectSound;

    [SerializeField] private AudioSource audioSource;

    public Row[] rows;

    public Tile[,] Tiles { get; private set; }

    public int width => Tiles.GetLength(0);
    public int height => Tiles.GetLength(1);

    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.25f;

    private void Awake() => Instance = this;

    /*private void Start()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];


        for (var y=0; y<height; y++)
        {
            for(var x=0; x<width; x++)
            {

                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];

                Tiles[x, y] = tile;
            }
        }
        //Pop();
    }*/

    private void Start()
    {
        // ������� ���� �߰�, ������ �� 3�ʵ��� ���� ���� ȭ�� ��. 
        // Show the StartPanel
        S_start_hj.Play();
        StartPanel_hj.SetActive(true);

        // Use a coroutine to hide the StartPanel after 3 seconds
        StartCoroutine(HideStartPanel_hj());

        // ���� �߰� ��


        // ���� ������ �����ϱ� ���� �ִ� �õ� Ƚ��
        const int maxTries = 1000;
        int currentTry = 0;

        do
        {
            // �ʱ� ���� ����
            CreateInitialBoard();

            // �� �� �̻��� ���ӵ� ������ ������ Ȯ��
            if (!HasConsecutiveBubbles())
            {
                // ������ ������ ������ �ٽ� �õ�
                currentTry++;
            }
            else
            {
                // ������ �����ϸ� ���� ����
                break;
            }
        } while (currentTry < maxTries);

        // ���� �ִ� �õ� Ƚ���� �����ߴµ��� �� �� �̻��� ���ӵ� ������ ���ٸ� ���� ó�� �� �ʿ�
    }

    // ���� �߰� ����
    // 3�� �� ���� ������
    IEnumerator HideStartPanel_hj()
    {
        yield return new WaitForSeconds(3f);

        // Hide the StartPanel after 3 seconds
        StartPanel_hj.SetActive(false);
    }
    // ���� �߰� ��


    public void CreateInitialBoard()

    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var tile = rows[y].tiles[x];

                tile.x = x;
                tile.y = y;

                // �Ʒ� ���� ���� �ڵ忡�� Ư�� Ÿ���� ���ӵǴ��� Ȯ��
                do
                {
                    tile.Item = ItemDatabase.Items[Random.Range(0, ItemDatabase.Items.Length)];
                } while (IsConsecutiveTile(x, y, tile.Item));

                Tiles[x, y] = tile;
            }
        }
    }

    public bool HasConsecutiveBubbles()
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width - 2; x++)
            {
                if (AreConsecutiveTiles(Tiles[x, y], Tiles[x + 1, y], Tiles[x + 2, y]))
                    return true;
            }
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height - 2; y++)
            {
                if (AreConsecutiveTiles(Tiles[x, y], Tiles[x, y + 1], Tiles[x, y + 2]))
                    return true;
            }
        }

        return false;
    }

    private bool IsConsecutiveTile(int x, int y, Item item)
    {
        // ���� Ÿ�� �ֺ��� Ÿ�ϵ��� ���� ���������� Ȯ��
        return (x > 1 && Tiles[x - 1, y].Item == item && Tiles[x - 2, y].Item == item) ||
               (y > 1 && Tiles[x, y - 1].Item == item && Tiles[x, y - 2].Item == item);
    }



    /*private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.A)) return;

        foreach (var connectedTile in Tiles[0, 0].GetConnectedTiles())
            connectedTile.icon.transform.DOScale(1.25f, TweenDuration).Play();
    }*/

    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                _selection.Add(tile);
            }
        } 



        if (_selection.Count < 2) return;

        Debug.Log($"Selected tiles at ({_selection[0].x}, {_selection[0].y}) and ({_selection[1].x}, {_selection[1].y})");

        await Swap(_selection[0], _selection[1]);

        if (CanPop())
        {
            while (CanPop()) await Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
        }

        _selection.Clear();



        _selection.Clear();
           

    }
    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;

        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();

        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration))
            .Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));

        await sequence.Play().AsyncWaitForCompletion();

        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);

        tile1.icon = icon2;
        tile2.icon = icon1;

        var tile1Item = tile1.Item;

        tile1.Item = tile2.Item;
        tile2.Item = tile1Item;
    }
    /*private bool CanPop()
    {
        for(var y=0; y<height; y++)
        {
            for (var x=0; x < width; x++)
            {
                if (Tiles[x, y].GetConnectedTiles().Skip(1).Count() >= 2) 
                    return true;
            }
        }
        return false;

    }*/

    private bool CanPop()
    {
        // �� ���� Ȯ��
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width - 2; x++)
            {
                if (AreConsecutiveTiles(Tiles[x, y], Tiles[x + 1, y], Tiles[x + 2, y]))
                    return true;
            }
        }

        // �� ���� Ȯ��
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height - 2; y++)
            {
                if (AreConsecutiveTiles(Tiles[x, y], Tiles[x, y + 1], Tiles[x, y + 2]))
                    return true;
            }
        }

        return false;
    }

    private bool AreConsecutiveTiles(Tile tile1, Tile tile2, Tile tile3)
    {
        // Ÿ���� ���� ���� �����̸鼭 ���ӵǾ� �ִ��� Ȯ��
        return (tile1.Item == tile2.Item && tile2.Item == tile3.Item);
    }


    /*private async Task Pop()
    {
        for(var y=0; y<height; y++)
        {
            for(var x=0; x<width; x++)
            {
                var tile = Tiles[x, y];

                var connectedTiles = tile.GetConnectedTiles();

                if (connectedTiles.Skip(1).Count() < 2) continue;

                var deflateSequence = DOTween.Sequence();

                foreach(var connectedTile in connectedTiles)
                {
                    deflateSequence.Join(connectedTile.icon.transform.
                        DOScale(Vector3.zero, TweenDuration));

                }

                audioSource.PlayOneShot(collectSound);

                Score.Instance.Score_ += tile.Item.value * connectedTiles.Count;

                await deflateSequence.Play().AsyncWaitForCompletion();

                var inflateSequence = DOTween.Sequence();

                foreach(var connectedTile in connectedTiles)
                {
                    connectedTile.Item = ItemDatabase.
                        Items[Random.Range(0, ItemDatabase.Items.Length)];

                    inflateSequence.Join(connectedTile.icon.transform.
                        DOScale(Vector3.one, TweenDuration));


                }
                await inflateSequence.Play().AsyncWaitForCompletion();

                x = 0; y = 0;
            }
        }
    }*/

    private async Task<bool> Pop()
    {
        // �Ϸķ� ���ӵ� ���� Ÿ���� 3�� �̻� �ִ� ��쿡�� ó��
        if (CanPop())
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var tile = Tiles[x, y];

                    // Ÿ�� �ֺ��� ���� Ÿ�� ���� Ȯ��
                    int horizontalCount = CountSameTilesInDirection(tile, Vector2.right);
                    int verticalCount = CountSameTilesInDirection(tile, Vector2.up);

                    if (horizontalCount >= 2 && x + horizontalCount - 1 < width)
                    {
                        var connectedTiles = new List<Tile>();

                        for (int i = 0; i < horizontalCount; i++)
                        {
                            connectedTiles.Add(Tiles[x + i, y]);
                        }

                        await PopConnectedTiles(connectedTiles);
                    }

                    if (verticalCount >= 2 && y + verticalCount - 1 < height)
                    {
                        var connectedTiles = new List<Tile>();

                        for (int i = 0; i < verticalCount; i++)
                        {
                            connectedTiles.Add(Tiles[x, y + i]);
                        }

                        await PopConnectedTiles(connectedTiles);
                    }
                }
            }

            // ���ӵ� ������ �ϳ��� ó���Ǿ��ٸ� true ��ȯ
            return true;
        }

        // ���ӵ� ������ �ϳ��� ������ false ��ȯ
        return false;
    }

    /*private async Task PopConnectedTiles(List<Tile> connectedTiles)
    {
        if (connectedTiles.Count >= 3)
        {
            var deflateSequence = DOTween.Sequence();

            foreach (var connectedTile in connectedTiles)
            {
                deflateSequence.Join(connectedTile.icon.transform.
                    DOScale(Vector3.zero, TweenDuration));
            }

            audioSource.PlayOneShot(collectSound);

            Score.Instance.Score_ += connectedTiles[0].Item.value * connectedTiles.Count;

            await deflateSequence.Play().AsyncWaitForCompletion();

            var inflateSequence = DOTween.Sequence();

            foreach (var connectedTile in connectedTiles)
            {
                connectedTile.Item = ItemDatabase.
                    Items[Random.Range(0, ItemDatabase.Items.Length)];

                inflateSequence.Join(connectedTile.icon.transform.
                    DOScale(Vector3.one, TweenDuration));
            }

            await inflateSequence.Play().AsyncWaitForCompletion();
        }
    }*/ //��¥�ڵ�

    private async Task PopConnectedTiles(List<Tile> connectedTiles)
    {
        if (connectedTiles.Count >= 3)
        {
            var deflateSequence = DOTween.Sequence();
            var colors = new Dictionary<Item, int>(); // �� ������ Ÿ�� ������ ������ ��ųʸ�

            foreach (var connectedTile in connectedTiles)
            {
                deflateSequence.Join(connectedTile.icon.transform.
                    DOScale(Vector3.zero, TweenDuration));

                // ���� �� Ÿ�� ���� ����
                if (!colors.ContainsKey(connectedTile.Item))
                {
                    colors[connectedTile.Item] = 1;
                }
                else
                {
                    colors[connectedTile.Item]++;
                }
            }

            audioSource.PlayOneShot(collectSound);

            // �� ���� ���� ���������� ���� ���
            foreach (var colorCount in colors)
            {
                Score.Instance.AddScore(colorCount.Key, colorCount.Key.value * colorCount.Value);
            }

            await deflateSequence.Play().AsyncWaitForCompletion();

            var inflateSequence = DOTween.Sequence();

            foreach (var connectedTile in connectedTiles)
            {
                connectedTile.Item = ItemDatabase.
                    Items[Random.Range(0, ItemDatabase.Items.Length)];

                inflateSequence.Join(connectedTile.icon.transform.
                    DOScale(Vector3.one, TweenDuration));
            }

            await inflateSequence.Play().AsyncWaitForCompletion();
        }
    }


    private int CountSameTilesInDirection(Tile startTile, Vector2 direction)
    {
        int count = 1; // �ڱ� �ڽ� ����

        int x = startTile.x;
        int y = startTile.y;

        while (true)
        {
            x += (int)direction.x;
            y += (int)direction.y;

            if (x < 0 || x >= width || y < 0 || y >= height)
                break;

            if (Tiles[x, y].Item == startTile.Item)
                count++;
            else
                break;
        }

        return count;
    }


}
