using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class randomcube : MonoBehaviour
{
    GameObject tmp;
    GameObject windowright;
    GameObject windowleft;
    GameObject door;

    public house[] buildings_array = new house[5];
    // public GameObject cube;
    
    [Header("House Width Padding")]
    // [Range(0, 20)]
	// public float width;
    // public Vector2 randomWidth = new Vector2(0,1);
    // [Header("House Length Padding")]
    // [Range(0, 20)]
	// public float length;
    // public Vector2 randomLenghth = new Vector2(0,1);

    private List<GameObject> houseList;

    ////////////////////////////////////////////////
    //          load picture                      //
    ////////////////////////////////////////////////
    private Texture2D levelBitmap;
    public Texture2D pic;
    // public GameObject sampleCubeRange;
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
	
    int Count = 0;
    public struct blue_area_pos
    {
        public int start_x, start_y, end_x, end_y;
        public blue_area_pos(int a, int b, int c, int d){
            start_x = a;
            start_y = b;
            end_x = c;
            end_y = d;
        }
    }
    public float Scale;
    [Range(0, 20)]
    public int building_range;
    List<Rect> bounding_box_list;
	List<Vector2> edge_list;
    public void bouding_test(){
        bounding_box_list = new List<Rect>();
		edge_list = new List<Vector2>();
		Debug.LogFormat("picture size {0}x{1}", pic.height, pic.width);
		bool[,] have_count = new bool[pic.width, pic.height];
		bool[,] blue = new bool[pic.width, pic.height];
		bool[,] red = new bool[pic.width, pic.height];
		bool[,] green = new bool[pic.width, pic.height];
        bool[,] built = new bool[pic.width, pic.height];
		int width = pic.width;
		int height = pic.height;
		for(int i = 0 ; i < pic.width; i++)
            for (int j = 0; j < pic.height; j++){
                //if (i % 10 == 0)
                    // Debug.Log(pic.GetPixel(i, j));
				if (pic.GetPixel(i, j).b > 0.8)
					blue[i, j] = true;
				if (pic.GetPixel(i, j).r > 0.8)
					red[i, j] = true;
                if(pic.GetPixel(i, j).g > 0.8)
                    green[i, j] = true;
			}
       	for(int i = 0 ; i < pic.width - building_range; i++)
            for (int j = 0; j < pic.height - building_range; j++){
                if (check(i, j, building_range, blue, built)){
                    PlaceCubes(building_range, building_range, i, j);
                    for (int m = 0; m < building_range; m++)
                        for (int n = 0; n < building_range; n++){
                            built[i+m, j+n] = true; 
                        }
                }

            }

		// for(int i = 0 ; i < pic.width; i++)
        //     for (int j = 0; j < pic.height; j++){
		// 		// Debug.Log("i "+i+"j "+j);
		// 	 	if (pic.GetPixel(i, j).b > 0.8 && have_count[i, j] == false){
		// 			bool inside = false;
		// 			for (int k = 0; k < bounding_box_list.Count; k++){
		// 				if (i >= bounding_box_list[k].xMin && i <= bounding_box_list[k].xMax &&  j >= bounding_box_list[k].yMin && j <= bounding_box_list[k].yMax){
		// 					inside = true;
		// 				}
		// 			}
		// 			if (!inside){
		// 				Rect bounding_box = new Rect(i, j, 0, 0);
		// 				bounding_box = blue_area_bound(i, j, width, height, bounding_box, have_count,  blue);
        //                 PlaceCubes((int)bounding_box.height ,(int)bounding_box.width, bounding_box.xMin, bounding_box.yMin);
		// 				bounding_box_list.Add(bounding_box);
		// 				// if (bounding_box.height != 0 && bounding_box.width != 0){
		// 				// 	Debug.LogFormat("create houses with length {0}, width{1} from position {2} {3} to position{4} {5}", bounding_box.height, bounding_box.width, bounding_box.xMin, bounding_box.yMin, bounding_box.xMax, bounding_box.yMax);   
		// 				// 	GameObject tmpRange = Instantiate(sampleCubeRange, this.transform.position, this.transform.rotation);
		// 				// 	tmpRange.transform.localScale = new Vector3(bounding_box.width, 1, bounding_box.height);
		// 				// 	tmpRange.transform.position = new Vector3(bounding_box.center.x, 0, bounding_box.center.y);
		// 				// 	tmpRange.transform.parent = this.transform;
		// 				// }
		// 			}
					
		// 		}
		// 	}
    }
    public bool check(int i, int j, int range, bool[,] blue, bool[,] built){
        if (blue[i, j] && !built[i, j] && blue[i + range, j] && !built[i + range, j] && blue[i, j+ range] && !built[i, j+range] && blue[i+range, j+range] && !built[i+range, j+range] && blue[i+range/2, j+range/2] && !built[i+range/2, j+range/2])
            return true;
        else
            return false;
    }
    public Rect blue_area_bound(int i, int j, int width, int height, Rect bounding_box, bool[,] have_count, bool[,] blue){
		// Debug.LogFormat("width = {0}, height = {1}, have count = {2}, color= {3}, i {4}, j{5}", colormap.width, colormap.height, have_count[i, j], colormap.GetPixel(i, j), i, j);

        if (i >= width || j >= height || i < 0 || j < 0 || have_count[i, j] || blue[i,j] == false){
			// have_count[i, j] = true;
			// Debug.Log("return");
            return bounding_box;
		}
        else{
			have_count[i, j] = true;
			if (i > bounding_box.xMax) 
				bounding_box.xMax = i;
            int leftmost = j;
			for (int j_ = j; j_ >= 0 && blue[i, j_]; j_--){
				if (j_ < bounding_box.yMin)
					bounding_box.yMin = j_;
				if (i+1 < width && blue[i+1, j_])
					leftmost = j_;
				have_count[i, j_] = true;
			}
			if (leftmost != j) bounding_box = blue_area_bound(i+1, leftmost, width, height, bounding_box, have_count, blue);

			int rightmost = j;
			for (int j_ = j; j_ < height && blue[i, j_]; j_++){
				if (j_ > bounding_box.yMax)
					bounding_box.yMax = j_;
				if (i+1 < width && blue[i+1, j_])
					rightmost = j_;
				have_count[i, j_] = true;
			}
			if (rightmost != j) bounding_box = blue_area_bound(i+1, rightmost, width, height, bounding_box, have_count, blue);
			
			if (leftmost == j && rightmost == j && i+1 < width && blue[i+1, j])
				bounding_box = blue_area_bound(i+1, j, width, height, bounding_box, have_count, blue);
				// else			
				// 	return bounding_box;
        }
		return bounding_box;
    }
    public void setMaterial(){
        for (int i = 0; i < buildings_array.Length; i++){
            house chooseHouse = buildings_array[i];
            if(chooseHouse.material.Length > 8){
                System.Array.Resize(ref chooseHouse.material, 8);
            }
            for (int j = 0; j < chooseHouse.material.Length; j++){
                mat_color chooseColor = chooseHouse.material[j];
                Material mat = chooseColor.mat;
                Color color = chooseColor.color;

                GameObject chooseBuilding = chooseHouse.structure; 
                MeshRenderer rend = chooseBuilding.GetComponent<MeshRenderer>();
                rend.material = mat;

                rend = chooseBuilding.GetComponent<MeshRenderer>();
                rend.sharedMaterial.shader = Shader.Find("Standard");
                rend.sharedMaterial.SetColor("_Color", color);
            }
        }
        
        
        
        
    }
    public void PlaceCubes(int bluewidth, int bluelength, float start_pos_x, float start_pos_z)
    {   
        Count++;
        if( houseList == null ){
            houseList = new List<GameObject>();
        }else{
            houseList.Clear();
        }
        if(buildings_array == null){
            return;
        }
        float y;
        float height;
        float tmp1;
        float scale_x = 1;
        float scale_z = 1;
		float w = scale_x;
		float s = scale_z;
        float end_pos_x = start_pos_x + bluelength;
        float end_pos_z = start_pos_z + bluewidth;
        int count = 0;
        for (float i =start_pos_x; i <= end_pos_x; i+=w)
            for(float j = start_pos_z; j <= end_pos_z; j+=s)
            {
                count++;
                if(count > 1)
                    break;
                // y = UnityEngine.Random.Range(min, max);
                Vector3 position = new Vector3(i, 0, j);
                house chooseHouse = buildings_array[Random.Range(0, buildings_array.Length)];
                if(chooseHouse.material.Length > 8){
                    System.Array.Resize(ref chooseHouse.material, 8);
                }
                mat_color chooseColor = chooseHouse.material[Random.Range(0, chooseHouse.material.Length)];

                Material mat = chooseColor.mat;
                Color color = chooseColor.color;


                GameObject chooseBuilding = chooseHouse.structure; 
                MeshRenderer rend = chooseBuilding.GetComponent<MeshRenderer>();
                rend.material = mat;

                rend = chooseBuilding.GetComponent<MeshRenderer>();
                rend.sharedMaterial.shader = Shader.Find("Standard");
                rend.sharedMaterial.SetColor("_Color", color);

                tmp =  Instantiate(chooseBuilding, position , Quaternion.identity);
                // // tmp.transform.localScale += new Vector3(0, 1.5f*y, 0);
                // height = tmp.transform.localScale.y;
                // float setToSameY = 0;
                // if(height > chooseBuilding.transform.localScale.y)
                // {
                //     tmp1 = (height - chooseBuilding.transform.localScale.y)/2;
                //     setToSameY = tmp1+1.5f;
                //     // position = new Vector3(i, tmp1, j);
                // }
                // else if(height <= chooseBuilding.transform.localScale.y)
                // {
                //     tmp1 = (chooseBuilding.transform.localScale.y - height)/2;
                //     setToSameY = -tmp1+1.5f;
                //     // position = new Vector3(i, -tmp1, j);
                // }
                // position.y += setToSameY;
                //tmp.transform.localPosition = position;
               
                
                // Shuffle That
                Vector3 randPosition = new Vector3(
                    //Random.Range(randomWidth.x, randomWidth.y), 
                    0,
                    0, 
                    0);
                    //Random.Range(randomLenghth.x, randomLenghth.y));
                //Debug.Log(randPosition);
                tmp.transform.position = tmp.transform.position + randPosition;
                // houseList.Add(tmp);
                //tmp.transform.Rotate(0, Random.Range(-rotaterange,rotaterange), 0);
                // if( tmp.transform.position.z <= start_pos_z+randomWidth.y)
                // {
                //     tmp.transform.Rotate(0, 90, 0);
                // }
                // else if( tmp.transform.position.z >= end_pos_z-randomWidth.y-5f)
                // {
                //     tmp.transform.Rotate(0, -90, 0);
                // }
                // else if( tmp.transform.position.x <= start_pos_x+randomWidth.y)
                // {
                //     tmp.transform.Rotate(0, 180, 0);
                // }
                tmp.transform.localScale*=Scale;
                // position = tmp.transform.localScale;
                // position.y = tmp.transform.localScale.y / 2; 
                // tmp.transform.localPosition = position;
                // Put into this GameObject
                tmp.transform.parent = this.transform;
                if(houseList.Count == 0)
                    houseList.Add(tmp);
                else
                {
                    bool flag = false;
                    for(int k = 0; k < houseList.Count; k++)
                    {
                        if((Mathf.Abs(houseList[k].transform.position.x -  tmp.transform.position.x) <=scale_x) &&  (Mathf.Abs(houseList[k].transform.position.z -  tmp.transform.position.z )<=scale_z))
                        {                      
                            // DestroyImmediate(tmp);
                            flag = true;
                            break;
                        }
                    }
                    if(!flag)
                        houseList.Add(tmp);
                    else 
                        DestroyImmediate(tmp);
                }
            }

    }

    public void previous_house_destroy(){
        // while (cube.transform.childCount != 0) {
        //     DestroyImmediate(cube.transform.GetChild(0).gameObject);
        // }
        while (this.transform.childCount != 0) {
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        }

        // for( int i=0 ; i<houseList.Count ; i++ ){

        //     DestroyImmediate(houseList[i]);
        // }
        // houseList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
           
    }
}

[System.Serializable] 
public class house{
    public GameObject structure;
    public mat_color[] material = new mat_color[8];
}
[System.Serializable] 
public class mat_color{
    public Material mat;
    public Color color;
}
