using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int width;
    public int height;
    public int border;


    private Board _board;
    private void Start()
    {
        _board = FindObjectOfType<Board>();
        
        //Init board
        _board.InitBoard(width, height);
        
        //Arrange Camera
        if (Camera.main != null) Camera.main.GetComponent<CameraController>().ArrangeCamera(width, height, border);
        
        //Fill the board
        _board.FillTheBoard();
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main");
    }
}
