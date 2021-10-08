using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadRoadPicture : MonoBehaviour {

	// Use this for initialization
	private Texture2D levelBitmap;
	Texture2D duplicateTexture(Texture2D source)
	{
		RenderTexture renderTex = RenderTexture.GetTemporary(
					source.width,
					source.height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear);

		Graphics.Blit(source, renderTex);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = renderTex;
		Texture2D readableText = new Texture2D(source.width, source.height);
		readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
		readableText.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(renderTex);
		return readableText;
	}
	void Start () {
		levelBitmap = Resources.Load("houseroad_test") as Texture2D;
		Debug.Log(levelBitmap);
		Texture2D readable = duplicateTexture(levelBitmap);
		loadPicture(readable);
	}
	public void loadPicture(Texture2D texture){
		randomcube house_creator = new randomcube();
		float red , green, blue, tmp;
		Color[] pix = texture.GetPixels(0, 0, texture.width, texture.height);
		Debug.Log(pix[0]);
		Debug.Log(pix.Length);
		for (int i = 0; i < pix.Length; i++){
			if (pix[i].r > 0.8)
				Debug.Log("red");
			else if (pix[i].g > 0.8)
				Debug.Log("green");
			else
			{
				Debug.Log("blue");
				int length = 0, width = 0;
				while(pix[i].b  > 0.8)
				{
					length++;
					i++;
				}
				int j = i ;       
				while(pix[j].b > 0.8)
				{
					width++;
					j += texture.width;
				}     
			// house_creator.PlaceCubes(length, width);                                                                                                                                                                              
			}
		}

		// for (int i = 0; i <texture.width; i++)
		// {
		// 	for(int j = 0; j < texture.height; j++)
		// 	{
		// 		Color pixel = texture.GetPixel(i, j);
		// 		red = pixel.r;
		// 		green = pixel.g;
		// 		blue = pixel.b;
		// 		tmp = pixel.maxColorComponent;
		// 		if(tmp == green)
		// 		{
		// 			Debug.Log("is green");
		// 			continue;
		// 		}
		// 		else if(tmp == red)
		// 		{
		// 			Debug.Log("is red");
		// 			continue;
		// 		}
		// 		else
		// 		{
		// 			Debug.Log("is blue");
		// 			continue;
		// 		}
					
		// 	}
		// }
	}
 /// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI()
	{
		//GUI.DrawTexture(new Rect(0,0,Screen.height, Screen.width) ,  levelBitmap);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
