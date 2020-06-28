using System.Collections;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    private float _swapAnimationTime = 0.6f;
    private Swap _swap;

    public enum DropColor
    {
        Blue,
        Green,
        Red,
        Yellow
    }
    public DropColor dropColor;
    public void InitDrop(int x, int y, Swap swap)
    {
        xIndex = x;
        yIndex = y;
        _swap = swap;
    }
    public void Move(int destX, int destY, MoveType moveType, Board board)
    {
        xIndex = destX;
        yIndex = destY;
        StartCoroutine(MoveAnimation(new Vector3(destX, destY, -1.0f), moveType, board));
    }
    IEnumerator MoveAnimation(Vector3 destination, MoveType moveType, Board board)
    {
        Vector3 startPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0.0f;
        while (!reachedDestination)
        {
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                break;
            }

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / _swapAnimationTime, 0.0f, 1.0f);
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, destination, t);
            yield return null;
        }

        switch (moveType)
        {
            case MoveType.Swap:
                _swap.SwapAnimationCallback();
                break;
            case MoveType.Reverse:
                _swap.ReverseSwapAnimationCallback();
                break;
            case MoveType.AutoMove:
                board.AutoDropAnimationCallback();
                break;
            default:
                Debug.Log("MoveAnimation MoveType");
                break;
        }
        
    }
}
