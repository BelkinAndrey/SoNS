using UnityEngine;
using System.Collections;

/* Градиент фона: http://wiki.unity3d.com/index.php?title=CameraGradientBackground */
public class GradientBackground : MonoBehaviour {

	public Color topColor = Color.blue;
	public Color bottomColor = Color.white;
	public int gradientLayer = 7;

    void Awake()
    {	/* Здесть создаютя два обьета "Gradient Cam" и "Gradient Plane"
         * которые с следующем скрипте StartSceneScript - DontDestroyOnLoad(GameObject.Find("Gradient Cam"));
         * защищаются от удаления при загрузки новых сцен.
         * Поэтому важно что бы этот Awake() выполнялся раньше (что и происходит)
         * скрипт градиента хотелось оставить в оригинале, поэтому ничего не добавлял */
		gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
		if (!GetComponent<Camera>()) {
			Debug.LogError ("Must attach GradientBackground script to the camera");
			return;
		}
		
		GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
		GetComponent<Camera>().cullingMask = GetComponent<Camera>().cullingMask & ~(1 << gradientLayer);
		Camera gradientCam = new GameObject("Gradient Cam",typeof(Camera)).GetComponent<Camera>();
		gradientCam.depth = GetComponent<Camera>().depth-1;
		gradientCam.cullingMask = 1 << gradientLayer;
		
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[4]
		{new Vector3(-100f, .577f, 1f), new Vector3(100f, .577f, 1f), new Vector3(-100f, -.577f, 1f), new Vector3(100f, -.577f, 1f)};
		
		mesh.colors = new Color[4] {topColor,topColor,bottomColor,bottomColor};
		
		mesh.triangles = new int[6] {0, 1, 2, 1, 3, 2};

        //Material mat = new Material("Shader \"Vertex\"{Subshader{BindChannels{Bind \"vertex\", vertex Bind \"color\", color}Pass{}}}"); //пришлось убрать
        Material mat = new Material(Shader.Find("GUI/Text Shader")); // при смене версии с 4 на 5 градиент стал работать только так.
		GameObject gradientPlane = new GameObject("Gradient Plane", typeof(MeshFilter), typeof(MeshRenderer));
		
		((MeshFilter)gradientPlane.GetComponent(typeof(MeshFilter))).mesh = mesh;
		gradientPlane.GetComponent<Renderer>().material = mat;
		gradientPlane.layer = gradientLayer;
	}
}
