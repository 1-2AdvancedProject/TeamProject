using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowMotionEffect : MonoBehaviour
{
	Material cameraMeterial;

	[Header("Ένµµ")]
	public float grayScale = 0.0f;

	private void Start()
	{
		cameraMeterial = new Material(Shader.Find("CustomRenderTexture/Grayscale"));
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		
	}
}
