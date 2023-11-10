using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private Transform gameArea;

    [SerializeField]
    private List<GameObject> orangePieces;
    private Dictionary<GameObject, Vector3Int> piecesPosition;

    private List<Vector3> cellsPosition;

    [SerializeField]
    private GameObject obstaclePrefab;
    [SerializeField]
    private Transform obstaclesContainer;
    private Dictionary<GameObject, Vector3Int> obstaclesPosition;

    #region movement
    private Vector2 mouseDownPos;
    private Vector2 mouseUpPos;

    private float moveTime = .2f;
    [SerializeField]
    private Vector3 moveDirection;
    #endregion

    private void Awake()
    {
        GameObject map = Instantiate(LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex).map, gameArea);
        GridCellManager.instance.SetTileMap(map.transform.GetChild(0).GetChild(0).GetComponent<Tilemap>());
    }

    private void Start()
    {
        cellsPosition = GridCellManager.instance.GetCellsPosition();
        piecesPosition = new Dictionary<GameObject, Vector3Int>();
        obstaclesPosition = new Dictionary<GameObject, Vector3Int>();
        PrepareObstacles();
        PreparePosition();

    }

    void Update()
    {
        InputHandler();

        if (moveDirection != Vector3.zero)
        {
            orangePieces.ForEach(x => MoveObj(x));
            moveDirection = Vector3.zero;
            CheckWin();
        }

    }

    private void CheckWin()
    {
        if (piecesPosition[orangePieces[0]] == piecesPosition[orangePieces[1]] + Vector3Int.right
            && piecesPosition[orangePieces[0]] == piecesPosition[orangePieces[2]] + Vector3Int.right + Vector3Int.up
            && piecesPosition[orangePieces[0]] == piecesPosition[orangePieces[3]] + Vector3Int.up)
        {
            if(!GameManager.instance.IsGameWin() && !GameManager.instance.isGameLose())
            {
                StartCoroutine(WaitToWin());
            }
        }
    }

    private IEnumerator WaitToWin()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.instance.Win();
    }

    private void MoveObj(GameObject obj)
    {
        Vector3Int cellPosition = GridCellManager.instance.GetObjCell(obj.transform.position);
        Vector3Int nextCellPosition = cellPosition + Vector3Int.FloorToInt(moveDirection);

        if (GridCellManager.instance.IsPlaceableArea(nextCellPosition) && !obstaclesPosition.ContainsValue(nextCellPosition))
        {
            if (CheckNeighborPiece(nextCellPosition, moveDirection))
            {
                obj.transform.DOMove(GridCellManager.instance.PositonToMove(nextCellPosition), moveTime);
                piecesPosition[obj] = nextCellPosition;
            }
        }
    }

    private void PrepareObstacles()
    {
        foreach (var x in LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex).obstaclesPos)
        {
            GameObject obstacle = Instantiate(obstaclePrefab, GridCellManager.instance.PositonToMove(x), Quaternion.identity);
            obstacle.transform.SetParent(obstaclesContainer);
            obstacle.transform.localScale = Vector3.one;
            obstaclesPosition.Add(obstacle, x);
        }
    }

    private void PreparePosition()
    {
        for(int i = 0; i < orangePieces.Count; i++)
        {
            orangePieces[i].transform.position = GridCellManager.instance.PositonToMove(LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex).piecesPos[i]);
            piecesPosition.Add(orangePieces[i], LevelManager.instance.levelData.GetLevelAt(LevelManager.instance.currentLevelIndex).piecesPos[i]);
        }
    }

    private bool CheckNeighborPiece(Vector3Int positionToCheck, Vector3 direction)
    {
        Collider2D isHaveNext =  Physics2D.OverlapPoint(GridCellManager.instance.PositonToMove(positionToCheck), LayerMask.GetMask("OrangePiece"));
        Vector3Int nextCellPosition = positionToCheck;
        for(int i = 0; i < orangePieces.Count - 1; i++)
        {
            if (isHaveNext)
            {
                nextCellPosition += Vector3Int.FloorToInt(direction);
                if (GridCellManager.instance.IsPlaceableArea(nextCellPosition) && !obstaclesPosition.ContainsValue(nextCellPosition))
                {
                    isHaveNext = Physics2D.OverlapPoint(GridCellManager.instance.PositonToMove(nextCellPosition), LayerMask.GetMask("OrangePiece"));
                }else
                {
                    return false;
                }
            }
            else 
            {
                if (GridCellManager.instance.IsPlaceableArea(nextCellPosition) && !obstaclesPosition.ContainsValue(nextCellPosition))
                {
                    return true;
                }
                return false;
            }
        }
        return true;
    }


    private void InputHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseUpPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateDirection(mouseDownPos, mouseUpPos);
        }
    }
    private void CalculateDirection(Vector2 startPos, Vector2 endPos)
    {
        if (Mathf.Abs(startPos.x - endPos.x) > Mathf.Abs(startPos.y - endPos.y))
        {
            if (startPos.x > endPos.x)
            {
                moveDirection = Vector2.left;
            }
            else
            {
                moveDirection = Vector2.right;
            }
        }
        else
        {
            if (startPos.y > endPos.y)
            {
                moveDirection = Vector2.down;
            }
            else
            {
                moveDirection = Vector2.up;
            }
        }
    }
}
