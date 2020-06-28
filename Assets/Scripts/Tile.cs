using UnityEngine;

public class Tile : MonoBehaviour
{

    public int x;
    public int y;
    private Swap _swap;
    private bool _listenForMouseEnter;
    private bool _tileLocked;
    
    public void InitTile(int xValue, int yValue, Swap swap)
    {
        x = xValue;
        y = yValue;
        _swap = swap;
    }
    private void OnMouseDown()
    {
        if (!_tileLocked)
        {
            _swap.SelectedFirstTile(x, y);
        }
            
    }
    public void ActivateListeningForMouseEnter()
    {
        _listenForMouseEnter = true;
    }
    public void DeactivateListeningForMouseEnter()
    {
        _listenForMouseEnter = false;
    }
    public void LockTile()
    {
        _tileLocked = true;
    }
    public void UnlockTile()
    {
        _tileLocked = false;
    }
    private void OnMouseEnter()
    {
        if (_listenForMouseEnter)
        { 
            _swap.SelectedSecondTile(x, y);
        }
    }
    private void OnMouseUp()
    {
        _swap.CancelSelection(x, y);
    }
}
