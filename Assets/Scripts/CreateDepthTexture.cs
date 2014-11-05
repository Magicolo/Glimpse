using UnityEngine;
using System.Collections;

public class CreateDepthTexture : MonoBehaviour {

	public Material mat;

	void Start () {
		camera.depthTextureMode = DepthTextureMode.Depth;
		camera.Render ();
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit (source, destination, mat);
	}
}
