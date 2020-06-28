using UnityEngine;

public class CameraController : MonoBehaviour
{ 
   public void ArrangeCamera(int width, int height, int border)
   {
      transform.position = new Vector3((width - 1) / 2.0f, (height - 1) / 2.0f, -10.0f);
      var aspectRatio = (float)Screen.width / Screen.height;
      var vertical = height / 2.0f + border;
      var horizontal = (width / 2.0f + border) / aspectRatio;
      if (Camera.main != null) Camera.main.orthographicSize = vertical > horizontal ? vertical : horizontal;
   }
}