using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Grid : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public  GameObject tilePrefab;

    public GameObject[] Candies;

    public GameObject[,] tiles;

    private Vector2 offset;
    
    private Vector2 startPosition;

    void Start()
    {
        tiles = new GameObject[gridSizeX, gridSizeY];
        
        //Menentukan offset, didapatkan dari size prefab
        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        
        //Menentukan posisi awal
        startPosition = transform.position 
                                + (Vector3.left * (offset.x * gridSizeX / 2)) 
                                + (Vector3.down * (offset.y * gridSizeY / 3));
        
        CreateGrid();
    }

    public Vector2 getOffsetPos()
    {
        return offset;
    }

    public Vector2 getStartPos()
    {
        return startPosition;
    }

    void CreateGrid ()
    {
        //Looping untuk membuat tile
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 pos = new Vector3(startPosition.x + (x * offset.x), startPosition.y + (y * offset.y));
                GameObject backgroundTile = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation);
                
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + x + "," + y + ")";
                
                int id = Random.Range(0, Candies.Length);
                
                int MAX_ITERATION = 0;
                while (MatchesAt(x, y, Candies[id]) && MAX_ITERATION < 100){
                    id = Random.Range(0, Candies.Length);
                    MAX_ITERATION++;
                }
                MAX_ITERATION = 0;

                GameObject candy = Instantiate(Candies[id], pos, Quaternion.identity);
                candy.transform.parent = transform;
                candy.name = "Candy (" + x + "," + y + ")";
                tiles[x, y] = candy;                
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        //Cek jika ada tile yang sama dengan dibawah dan samping nya
        if (column > 1 && row > 1){
            if ((tiles[column - 1, row].tag == piece.tag && tiles[column - 2, row].tag == piece.tag)
                ||(tiles[column, row - 1].tag == piece.tag && tiles[column, row - 2].tag == piece.tag))
            {
                return true;
            }
        }
        else 
        {
            //Cek jika ada tile yang sama dengan atas dan sampingnya
            if (row == 1){
                if (tiles[column, row - 1].tag == piece.tag){
                    return true;
                }
            }

            if (column == 1){
                if (tiles[column - 1, row].tag == piece.tag){
                    return true;
                }
            }
        }

        return false;
    }

   private void DestroyMatchesAt(int column, int row)
    {
        if (tiles[column, row].GetComponent<Tile>().isMatched)
        {
            GameManager.instance.GetScore(10);
            tiles[column,row].gameObject.SetActive(false);
            //Destroy(tiles[column, row]);
            tiles[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    tiles[i, j].GetComponent<Tile>().row -= nullCount;
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        Vector2 startPos = getStartPos();
        Vector2 offsetPos = getOffsetPos();
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (tiles[x, y] == null)
                {
                    Vector2 tempPosition = new Vector3(startPos.x + (x * offsetPos.x), startPos.y + (y * offsetPos.y));
                    int candyToUse = Random.Range(0, Candies.Length);
                    //GameObject tileToRefill = Instantiate(candies[candyToUse], tempPosition, Quaternion.identity);
                    GameObject tileToRefill = ObjectPooler.Instance.SpawnFromPool(candyToUse.ToString(), tempPosition, Quaternion.identity);
                    // tileToRefill.GetComponent<Tile>().Init();
                    tiles[x, y] = tileToRefill;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j] != null)
                {
                    if (tiles[i, j].GetComponent<Tile>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }
}