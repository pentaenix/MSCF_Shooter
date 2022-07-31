using UnityEngine;

public class Boundary : MonoBehaviour
{
	#region VARIABLES
	static public float window_width;
    static public float window_height;
    [SerializeField] EdgeCollider2D limit;
	#endregion
	// Start is called before the first frame update
	private void Awake() {
        SetWindowDimensions();
        limit = GetComponent<EdgeCollider2D>();
    }
   
    void Update()
    {
        //Dynamic resizing of the screen limits
        limit.points = SetPointsVectorArray();
    }

    Vector2[] SetPointsVectorArray() {
        Vector2[] pointArray = new Vector2[5];
        SetWindowDimensions();
        pointArray[0] = new Vector2(window_width / 2, window_height / 2);
        pointArray[1] = new Vector2(window_width / 2, -window_height / 2);
        pointArray[2] = new Vector2(-window_width / 2,- window_height / 2);
        pointArray[3] = new Vector2(-window_width / 2, window_height / 2);
        pointArray[4] = new Vector2(window_width / 2, window_height / 2);

        return pointArray;
    }

    void SetWindowDimensions() {
        window_width = 1 / (Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)).x - 0.5f);
        window_height = 1 / (Camera.main.WorldToViewportPoint(new Vector3(1, 1, 0)).y - 0.5f);
    }
}
