using System.Collections.Generic;
using UnityEngine;

public class Swap : MonoBehaviour
{
    private Board _board;
    private Tile[,] _tiles;
    private Drop[,] _drops;
    private GameObject[,] _dropsGameObjects;
    private List<Tile> _activatedTilesForSwapping;
    private int _width;
    private int _height;
    private bool _swapIsStarted;
    private bool _reverseIsStarted;
    private int _movingDropsCount;
    private int _firstX;
    private int _firstY;
    private int _secondX;
    private int _secondY;
    private int _reverseSwapMoveCount;
    
    private void LockAllTilesToClick()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _tiles[i, j].LockTile();
            }
        }
    }
    private void UnlockAllTilesToClick()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _tiles[i, j].UnlockTile();
            }
        }
    }
    private void SwapIsStarted()
    {
        _drops[_firstX, _firstY].Move(_secondX, _secondY, MoveType.Swap, null);
        _drops[_secondX, _secondY].Move(_firstX, _firstY, MoveType.Swap, null); 
        ClearActivatedTilesForListening();
    }
    private void ClearActivatedTilesForListening()
    {
        foreach (var tile in _activatedTilesForSwapping)
        {
            tile.DeactivateListeningForMouseEnter();
        }
        _activatedTilesForSwapping.Clear();
    } 
    
    public void InitSwap(Board board, Tile[,] tiles, Drop[,] drops, int width, int height, GameObject[,] dropsGameObjects)
    {
        _board = board;
        _width = width;
        _height = height;
        _dropsGameObjects = dropsGameObjects;
        _activatedTilesForSwapping = new List<Tile>();
        _tiles = tiles;
        _drops = drops;
    }
    public void SelectedFirstTile(int x, int y)
    {
        if (!_swapIsStarted && !_reverseIsStarted)
        {
            _firstX = x;
            _firstY = y;
        
            LockAllTilesToClick();
        
            _activatedTilesForSwapping.Add(_tiles[x, y]);
            if (x > 0 && _drops[x - 1, y] != null)
            {
                _tiles[x - 1, y].ActivateListeningForMouseEnter();
                _activatedTilesForSwapping.Add(_tiles[x - 1, y]);
            }

            if (x < _width - 1 && _drops[x + 1, y] != null)
            {
                _tiles[x + 1, y].ActivateListeningForMouseEnter();
                _activatedTilesForSwapping.Add(_tiles[x + 1, y]);
            }

            if (y > 0 && _drops[x, y - 1] != null)
            {
                _tiles[x, y - 1].ActivateListeningForMouseEnter();
                _activatedTilesForSwapping.Add(_tiles[x, y - 1]);
            }
            if (y < _height - 1 && _drops[x, y + 1] != null)
            {
                _tiles[x, y + 1].ActivateListeningForMouseEnter();
                _activatedTilesForSwapping.Add(_tiles[x, y + 1]);
            }
        }
    }
    public void SelectedSecondTile(int x, int y)
    {
        if (!_swapIsStarted && !_reverseIsStarted)
        {
            _secondX = x;
            _secondY = y;
            _swapIsStarted = true;
            SwapIsStarted();    
        }
        
    }
    public void CancelSelection(int x, int y)
    {
        ClearActivatedTilesForListening();
        UnlockAllTilesToClick();
    }
    
    public void SwapAnimationCallback()
    {
        _movingDropsCount++;
        if (_movingDropsCount == 2)
        { 
            _movingDropsCount = 0;
            SwitchDropsAndGameObjects();
            if(!_board.ValidateSwap(_drops[_firstX, _firstY], _drops[_secondX, _secondY]))
                InvalidSwap();
        }
    }
    //It should be called after all animations are finished
    public void ValidSwap()
    {
        _swapIsStarted = false;
        _reverseIsStarted = false;
        UnlockAllTilesToClick();
    }
    private void InvalidSwap()
    {
        _reverseIsStarted = true;
        ReverseSwap();
    }
    private void ReverseSwap()
    {
        _drops[_firstX, _firstY].Move(_secondX, _secondY, MoveType.Reverse, null);
        _drops[_secondX, _secondY].Move(_firstX, _firstY, MoveType.Reverse, null);
    }
    public void ReverseSwapAnimationCallback()
    {
        _reverseSwapMoveCount++;
        if (_reverseSwapMoveCount == 2)
        {
            UnlockAllTilesToClick();
            _reverseIsStarted = false;
            _swapIsStarted = false;
            _reverseSwapMoveCount = 0;
            SwitchDropsAndGameObjects();
        }
    }
    private void SwitchDropsAndGameObjects()
    {
        GameObject dummyDropGameObject = _dropsGameObjects[_secondX, _secondY];
        _dropsGameObjects[_secondX, _secondY] =
            _dropsGameObjects[_firstX, _firstY];
        _dropsGameObjects[_firstX, _firstY] = dummyDropGameObject;
        
        Drop dummyDrop = _drops[_secondX, _secondY];
        _drops[_secondX, _secondY] =
            _drops[_firstX, _firstY];
        _drops[_firstX, _firstY] = dummyDrop;
    }
}
