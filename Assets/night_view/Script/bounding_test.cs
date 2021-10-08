using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class bounding_test : MonoBehaviour {
	public 	Texture2D pic;
	public GameObject sampleCubeRange;
	// Use this for initialization
	public GameObject cylinder;
	List<Rect> bounding_box_list;
	List<Vector2> edge_list;
	void Start () {
		bounding_box_list = new List<Rect>();
		edge_list = new List<Vector2>();
		Debug.LogFormat("picture size {0}x{1}", pic.height, pic.width);
		bool[,] have_count = new bool[pic.width, pic.height];
		bool[,] blue = new bool[pic.width, pic.height];
		bool[,] red = new bool[pic.width, pic.height];
		int width = pic.width;
		int height = pic.height;
		for(int i = 0 ; i < pic.width; i++)
            for (int j = 0; j < pic.height; j++){
				if (pic.GetPixel(i, j).b > 0.8)
					blue[i, j] = true;
				if (pic.GetPixel(i, j).r > 0.8)
					red[i, j] = true;
			}
		for(int i = 0 ; i < pic.width; i++)
            for (int j = 0; j < pic.height; j++){
				// Debug.Log("i "+i+"j "+j);
			 	if (pic.GetPixel(i, j).b > 0.8 && have_count[i, j] == false){
					bool inside = false;
					for (int k = 0; k < bounding_box_list.Count; k++){
						if (i >= bounding_box_list[k].xMin && i <= bounding_box_list[k].xMax &&  j >= bounding_box_list[k].yMin && j <= bounding_box_list[k].yMax){
							inside = true;
						}
					}
					if (!inside){
						Rect bounding_box = new Rect(i, j, 0, 0);
						bounding_box = blue_area_bound(i, j, width, height, bounding_box, have_count,  blue);
						bounding_box_list.Add(bounding_box);
						if (bounding_box.height != 0 && bounding_box.width != 0){
							Debug.LogFormat("create houses with length {0}, width{1} from position {2} {3} to position{4} {5}", bounding_box.height, bounding_box.width, bounding_box.xMin, bounding_box.yMin, bounding_box.xMax, bounding_box.yMax);   
							GameObject tmpRange = Instantiate(sampleCubeRange, this.transform.position, this.transform.rotation);
							tmpRange.transform.localScale = new Vector3(bounding_box.width, 1, bounding_box.height);
							tmpRange.transform.position = new Vector3(bounding_box.center.x, 0, bounding_box.center.y);
							tmpRange.transform.parent = this.transform;
						}
					}
					
				}
			}
		Debug.Log("# of Block "+bounding_box_list.Count);
		for (int i = 2; i < pic.width-2; i++){
			for (int j = 2; j < pic.height-2; j++){
				if(red[i, j]){
					if (color_gradient(i, j, i-2, j) > 0.5 || color_gradient(i, j, i+2, j) > 0.5 || color_gradient(i, j, i, j-2) > 0.5 || color_gradient(i, j, i, j+2) > 0.5){
						// if (Random.Range(0.0f, 1.0f) > 0.95f){
						// 	Debug.Log("light cylinder at"+i+","+j);
						// 	GameObject light = Instantiate(cylinder, this.transform.position, this.transform.rotation);
						// 	light.transform.position = new Vector3(i, 0, j);
						// 	light.transform.parent = this.transform;
						// }
						edge_list.Add(new Vector2(i, j));
					}
						
				}
			}
		}
		for (int k = 0; k < edge_list.Count; k+= 35){
			int i = (int)edge_list[k].x;
			int j = (int)edge_list[k].y;
			Debug.Log("light cylinder at"+i+","+j);
			GameObject light = Instantiate(cylinder, this.transform.position, this.transform.rotation);
			light.transform.position = new Vector3(i, 0, j);
			light.transform.parent = this.transform;
		}
	}
	public float color_gradient(int i1, int j1, int i2, int j2){
		Color color1 = pic.GetPixel(i1, j1);
		Color color2 = pic.GetPixel(i2, j2);
		Vector3 gradient = new Vector3(color1.a - color2.a, color1.b - color2.b, color1.g - color2.g);
		return gradient.magnitude;
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
// 
// 
// 
// This method would could stack overflow since its recursive call would be too deep
// 	
	// public Rect blue_area_bound(int i, int j, int width, int height, Rect bounding_box, bool[,] have_count, bool[,] blue){
	// 	// Debug.LogFormat("width = {0}, height = {1}, have count = {2}, color= {3}, i {4}, j{5}", colormap.width, colormap.height, have_count[i, j], colormap.GetPixel(i, j), i, j);

    //     if (i >= width || j >= height || i < 0 || j < 0 || have_count[i, j] || blue[i,j] == false){
	// 		// have_count[i, j] = true;
	// 		// Debug.Log("return");
    //         return bounding_box;
	// 	}
    //     else{
    //         // Debug.LogFormat("blue ! create houses with length {0}, width{1} from position {2} {3} to position{4} {5}", bounding_box.height, bounding_box.width, bounding_box.xMin, bounding_box.yMin, bounding_box.xMax, bounding_box.yMax);   
    //         have_count[i, j] = true;
    //         if (i > bounding_box.xMax)
    //             bounding_box.xMax = i;
    //         if (j > bounding_box.yMax)
    //             bounding_box.yMax = j;
    //         if (i < bounding_box.xMin)
    //             bounding_box.xMin = i;
    //         if (j < bounding_box.yMin)
    //             bounding_box.yMin = j;
    //         bounding_box = blue_area_bound(i+1, j, width, height, bounding_box, have_count, blue);
    //         bounding_box = blue_area_bound(i-1, j, width, height, bounding_box, have_count, blue);
    //         bounding_box = blue_area_bound(i, j+1, width, height, bounding_box, have_count, blue);
    //         bounding_box = blue_area_bound(i, j-1, width, height, bounding_box, have_count, blue);
    //     }
    //     return bounding_box;
    // }
// 	
// 	
// Revised version
// 
// 
	
	// Update is called once per frame
	void Update () {
		
	}
}
