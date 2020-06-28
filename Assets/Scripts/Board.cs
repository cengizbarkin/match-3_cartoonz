using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    
    public GameObject tilePrefab;
    public List<GameObject> dropPrefabs;

    private Tile[,] _tiles;
    private Drop[,] _drops;
    private GameObject[,] _dropsGameObjects;
    private int _width;
    private int _height;
    private Swap _swap;
    private List<Drop> _autoMovedDrops;
    private int _autoDropAnimationCallbackCount;
    
    private void Awake()
    {
        _swap = GetComponent<Swap>();
        _autoMovedDrops = new List<Drop>();
    }
    public void InitBoard(int width, int height)
    {
        _width = width;
        _height = height;
        _tiles = new Tile[width,height];
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tile.transform.parent = transform;
                _tiles[i, j] = tile.GetComponent<Tile>();
                _tiles[i, j].InitTile(i, j, _swap);
            }
        }
    }
    public void FillTheBoard()
    {
        _drops = new Drop[_width, _height];
        _dropsGameObjects = new GameObject[_width, _height];
        
        for (var i = 0; i < _width; i++)
        {
            for (var j = 0; j < _height; j++)
            {
                do
                {
                    if (_dropsGameObjects[i, j] != null)
                    {
                        Destroy(_dropsGameObjects[i, j].gameObject);
                        _drops[i, j] = null;
                    }
                    var position = new Vector3(i, j, -1);
                    var randomDrop = Instantiate(GetRandomDrop(), position, Quaternion.identity);
                    _drops[i, j] = randomDrop.GetComponent<Drop>();
                    _drops[i, j].InitDrop(i, j, _swap);
                    _dropsGameObjects[i, j] = randomDrop;
                } while (CheckDropForInit(i, j));
            }
        }
        _swap.InitSwap(this, _tiles, _drops, _width, _height, _dropsGameObjects);
    }
    private GameObject GetRandomDrop()
    {
        return dropPrefabs.ElementAt(Random.Range(0, dropPrefabs.Count));
    }

    //Look for Horizontal Left and Vertical Down
    //If any matches more than 2, return false
    private bool CheckDropForInit(int x, int y)
    {
        return HorizontalLeftMatchedDrops(x, y).Count > 2 || VerticalDownMatchedDrops(x, y).Count > 2;
    }
    private List<Drop> HorizontalLeftMatchedDrops(int x, int y)
    {
        var starterDrop = _drops[x, y];
        if (!starterDrop)
            return null;
        var horizontalLeftMatchedDrops = new List<Drop>();
        var dummyX = x - 1;
        while (dummyX >= 0 && _drops[dummyX, y] != null &&_drops[dummyX, y].dropColor == _drops[x, y].dropColor)
        {
            if(!horizontalLeftMatchedDrops.Contains(_drops[dummyX, y]))
                horizontalLeftMatchedDrops.Add(_drops[dummyX, y]);
            if(!horizontalLeftMatchedDrops.Contains(_drops[x, y]))
                horizontalLeftMatchedDrops.Add(_drops[x, y]);
            dummyX--;
        }
        return horizontalLeftMatchedDrops;
    }
    private List<Drop> HorizontalRightMatchedDrops(int x, int y)
    {
        var starterDrop = _drops[x, y];
        if (!starterDrop)
            return null;
        var horizontalRightMatchedDrops = new List<Drop>();
        var dummyX = x + 1;
        while (dummyX < _width && _drops[dummyX, y] != null &&_drops[dummyX, y].dropColor == _drops[x, y].dropColor)
        {
            if(!horizontalRightMatchedDrops.Contains(_drops[dummyX, y]))
                horizontalRightMatchedDrops.Add(_drops[dummyX, y]);
            if(!horizontalRightMatchedDrops.Contains(_drops[x, y]))
                horizontalRightMatchedDrops.Add(_drops[x, y]);
            dummyX++;
        }
        return horizontalRightMatchedDrops;
    }
    private List<Drop> VerticalDownMatchedDrops(int x, int y)
    {
        var starterDrop = _drops[x, y];
        if (!starterDrop)
            return null;
        var verticalDownMatchedDrops = new List<Drop>();

        var dummyY = y - 1;
        while (dummyY >= 0 && _drops[x, dummyY] != null &&_drops[x, dummyY].dropColor == _drops[x, y].dropColor)
        {
            if(!verticalDownMatchedDrops.Contains(_drops[x, dummyY]))
                verticalDownMatchedDrops.Add(_drops[x, dummyY]);
            if (!verticalDownMatchedDrops.Contains(_drops[x, y]))
                verticalDownMatchedDrops.Add(_drops[x, y]);
            dummyY--;
        }
        return verticalDownMatchedDrops;
    }
    private List<Drop> VerticalUpMatchedDrops(int x, int y)
    {
        var starterDrop = _drops[x, y];
        if (!starterDrop)
            return null;
        var verticalUpMatchedDrops = new List<Drop>();
        var dummyY = y + 1;
        while (dummyY < _height && _drops[x, dummyY] != null && _drops[x, dummyY].dropColor == _drops[x, y].dropColor)
        {
            if(!verticalUpMatchedDrops.Contains(_drops[x, dummyY]))
                verticalUpMatchedDrops.Add(_drops[x, dummyY]);
            if (!verticalUpMatchedDrops.Contains(_drops[x, y]))
                verticalUpMatchedDrops.Add(_drops[x, y]);
            dummyY++;
        }
        return verticalUpMatchedDrops;
    }
    private List<Drop> HorizontalMatchedDrops(int x, int y)
    {
        List<Drop> horizontalRightMatchedDrops = HorizontalRightMatchedDrops(x, y);
        if (horizontalRightMatchedDrops == null)
        {
            horizontalRightMatchedDrops = new List<Drop>();
        }
        List<Drop> horizontalLeftMatchedDrops = HorizontalLeftMatchedDrops(x, y);
        if (horizontalLeftMatchedDrops == null)
        {
            horizontalLeftMatchedDrops = new List<Drop>();
        }
        return horizontalRightMatchedDrops.Union(horizontalLeftMatchedDrops).ToList();

    }
    private List<Drop> VerticalMatchedDrops(int x, int y)
    {
        List<Drop> verticalUpMatchedDrops = VerticalUpMatchedDrops(x, y);
        if (verticalUpMatchedDrops == null)
        {
            verticalUpMatchedDrops = new List<Drop>();
        }

        List<Drop> verticalDownMatchedDrops = VerticalDownMatchedDrops(x, y);
        if (verticalDownMatchedDrops == null)
        {
            verticalDownMatchedDrops = new List<Drop>();
        }

        return verticalUpMatchedDrops.Union(verticalDownMatchedDrops).ToList();
    }
    public bool ValidateSwap(Drop firstDrop, Drop secondDrop)
    {
        List<Drop> totalMatchedDrops = MatchedDropsAfterSwap(firstDrop, secondDrop);
        bool valid = totalMatchedDrops.Count > 2;
        AutoCompleteMatchingDropsAfterSwap(totalMatchedDrops, valid);
        return valid;
    }
    private List<Drop> MatchedDropsAfterSwap(Drop firstDrop, Drop secondDrop)
    {
        List<Drop> firstMatchedHorizontalDrops = HorizontalLeftMatchedDrops(firstDrop.xIndex, firstDrop.yIndex)
            .Union(HorizontalRightMatchedDrops(firstDrop.xIndex, firstDrop.yIndex)).ToList();
        List<Drop> firstMatchedVerticalDrops = VerticalDownMatchedDrops(firstDrop.xIndex, firstDrop.yIndex)
            .Union(VerticalUpMatchedDrops(firstDrop.xIndex, firstDrop.yIndex)).ToList();
        List<Drop> secondMatchedHorizontalDrops = HorizontalLeftMatchedDrops(secondDrop.xIndex, secondDrop.yIndex)
            .Union(HorizontalRightMatchedDrops(secondDrop.xIndex, secondDrop.yIndex)).ToList();
        List<Drop> secondMatchedVerticalDrops = VerticalDownMatchedDrops(secondDrop.xIndex, secondDrop.yIndex)
            .Union(VerticalUpMatchedDrops(secondDrop.xIndex, secondDrop.yIndex)).ToList();
        List<Drop> totalMatchedDrops = new List<Drop>();
        if (firstMatchedHorizontalDrops.Count > 2)
            totalMatchedDrops = totalMatchedDrops.Union(firstMatchedHorizontalDrops).ToList();
        if(firstMatchedVerticalDrops.Count > 2)
            totalMatchedDrops = totalMatchedDrops.Union(firstMatchedVerticalDrops).ToList();
        if(secondMatchedHorizontalDrops.Count > 2)
            totalMatchedDrops = totalMatchedDrops.Union(secondMatchedHorizontalDrops).ToList();
        if(secondMatchedVerticalDrops.Count > 2)
            totalMatchedDrops = totalMatchedDrops.Union(secondMatchedVerticalDrops).ToList();
        return totalMatchedDrops;
    }
    private void CheckAutoMovedDrops()
    {
        List<Drop> totalMatchedDrops = new List<Drop>();
        foreach (var drop in _autoMovedDrops)
        {
            List<Drop> horizontalMatchedDrops = HorizontalMatchedDrops(drop.xIndex, drop.yIndex);
            if (horizontalMatchedDrops == null || horizontalMatchedDrops.Count < 3)
            {
                horizontalMatchedDrops = new List<Drop>();
            }
            List<Drop> verticalMatchedDrops = VerticalMatchedDrops(drop.xIndex, drop.yIndex);
            if (verticalMatchedDrops == null || verticalMatchedDrops.Count < 3)
            {
                verticalMatchedDrops = new List<Drop>();
            }

            totalMatchedDrops = totalMatchedDrops.Union(horizontalMatchedDrops.Union(verticalMatchedDrops)).ToList();
        }

        if (totalMatchedDrops.Count > 2)
        {
            AutoCompleteMatchingDropsAfterSwap(totalMatchedDrops, true);
        }
        else
        {
            _swap.ValidSwap();
        }
    }
    private void AutoCompleteMatchingDropsAfterSwap(List<Drop> matchedDrops, bool valid)
    {
        _autoMovedDrops.Clear();
        List<int> xValues = new List<int>();

        foreach (var drop in matchedDrops)
        {
            if(!xValues.Contains(drop.xIndex))
                xValues.Add(drop.xIndex);
            Destroy(_dropsGameObjects[drop.xIndex, drop.yIndex]);
            _dropsGameObjects[drop.xIndex, drop.yIndex] = null;
            _drops[drop.xIndex, drop.yIndex] = null;
        }

        foreach (var x in xValues)
        {
            int destVal = 0;
            for (int i = 0; i < _height ; i++)
            {
                if (_drops[x, i] == null) 
                    destVal++;
                if (_drops[x, i] != null && destVal > 0)
                {
                    _autoMovedDrops.Add(_drops[x, i]);
                    _drops[x, i].Move(x, i - destVal, MoveType.AutoMove, this);
                    _drops[x, i - destVal] = _drops[x, i];
                    _dropsGameObjects[x, i - destVal] = _dropsGameObjects[x, i];
                    _dropsGameObjects[x, i] = null;
                    _drops[x, i] = null;
                }
            }
        }

        if (_autoMovedDrops.Count == 0 && valid)
        {
            _swap.ValidSwap();
        }
    }
    public void AutoDropAnimationCallback()
    {
        _autoDropAnimationCallbackCount++;
        if (_autoDropAnimationCallbackCount == _autoMovedDrops.Count)
        {
            _autoDropAnimationCallbackCount = 0;
            CheckAutoMovedDrops();
        }
    }
}
