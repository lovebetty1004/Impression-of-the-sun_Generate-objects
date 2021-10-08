using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class house_generator : MonoBehaviour
{
    // public Texture2D mypicture; 

    // void start()
    // {
    //     LoadRoadPicture test_picture = new LoadRoadPicture();
    //     test_picture.loadPicture(mypicture);
    // }
  

    // public GameObject cube;
    public GameObject[] buildings;
    public GameObject[] windows;
    GameObject tmp;
    GameObject windowright;
    GameObject windowleft;
    GameObject door;
    public GameObject plane;
    public GameObject plan1;
    [Range(0, 20)]
    public int number_of_building_length;
    [Range(0, 20)]
    public int number_of_building_width;

    [Header("House Width Padding")]
    [Range(0, 20)]
	public float width;
    public Vector2 randomWidth = new Vector2(0,1);
    [Header("House Length Padding")]
    [Range(0, 20)]
	public float length;
    public Vector2 randomLenghth = new Vector2(0,1);
    [Header("Floor Height")]
    public int min, max;
    [Range(0,30)]
    public float rotaterange;
    private List<GameObject> houseList;
    // Start is called before the first frame update
    // void Start()
    // {
    //     PlaceCubes();
    // }
    ////////////////////////////////////////////////
    //          load picture                      //
    ////////////////////////////////////////////////
    private Texture2D levelBitmap;
    public Texture2D sampleBitMap;
    public GameObject sampleCubeRange;
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
//     
// 
// 
//  
//     
    public Rect blue_area_bound(int i, int j, Texture2D colormap, Rect bounding_box, bool[,] have_count){
        if (i >= colormap.width || j >= colormap.height || i < 0 || j < 0 || have_count[i, j] || colormap.GetPixel(i, j).b < 0.8)
            return bounding_box;
        else{
            Debug.LogFormat("create houses with length {0}, width{1} from position {2} {3} to position{4} {5}", bounding_box.height, bounding_box.width, bounding_box.xMin, bounding_box.yMin, bounding_box.xMax, bounding_box.yMax);   
            have_count[i, j] = true;
            if (i > bounding_box.xMax)
                bounding_box.xMax = i;
            if (j > bounding_box.yMax)
                bounding_box.yMax = j;
            if (i < bounding_box.xMin)
                bounding_box.xMin = i;
            if (j < bounding_box.yMax)
                bounding_box.yMin = j;
            blue_area_bound(i+1, j, colormap, bounding_box, have_count);
            blue_area_bound(i, j+1, colormap, bounding_box, have_count);
            blue_area_bound(i-1, j, colormap, bounding_box, have_count);
            blue_area_bound(i, j-1, colormap, bounding_box, have_count);
        }
        return bounding_box;
    }
// 
// 
// 
// 
// 
//     
	public void loadPicture(){
        levelBitmap = Resources.Load("houseroad_test2") as Texture2D;
        Debug.Log(levelBitmap);
        // Texture2D readable = duplicateTexture(levelBitmap);
        Texture2D readable = sampleBitMap;
        //loadPicture(readable);
		// float red , green, blue, tmp;
        bool[,] have_count = new bool[readable.width, readable.height];
        List<Rect> position_list = new List<Rect>();
        Debug.LogFormat("picture size {0}x{1}", readable.height, readable.width);
        for(int i = 0 ; i < readable.width; i++)
            for (int j = 0; j < readable.height; j++)
            {
                bool have_built = false;
                for (int k = 0; k < position_list.Count; k++){
                    if (i >= position_list[k].xMin && i <= position_list[k].xMax && j >= position_list[k].yMin && j <= position_list[k].yMax)
                        have_built = true;
                }
                if (have_built)
                {
                    continue;
                }
                Color pixel = readable.GetPixel(i, j);
                if (pixel.b > 0.8 && have_count[i,j] == false)
                {
                    Rect bounding_box = new Rect(i, j, 0, 0);
                    bounding_box = blue_area_bound(i, j, readable, bounding_box, have_count);
                   // Debug.Log(bounding_box.xMax+bounding_box.yMax);
                    
                    position_list.Add(bounding_box);
                    Debug.LogFormat("create houses with length {0}, width{1} from position {2} {3} to position{4} {5}", bounding_box.height, bounding_box.width, bounding_box.xMin, bounding_box.yMin, bounding_box.xMax, bounding_box.yMax);   
                    // GameObject tmpRange = Instantiate(sampleCubeRange, this.transform.position, this.transform.rotation)   ;
                    // tmpRange.transform.localScale = new Vector3(width, 1, height);
                    // tmpRange.transform.position = new Vector3(i+width/2, 0, j+height/2);
                    // tmpRange.transform.parent = this.transform;
				    PlaceCubes((int)bounding_box.height , (int)bounding_box.width, i, j);      
                }                                                                                                                                                                        
			}
		
        Debug.LogFormat("count = {0}", Count);
    }
    ///////////////////////////////////////////////////
   
    public void PlaceCubes(int bluewidth, int bluelength, float start_pos_x, float start_pos_z)
    {   
        Count++;
        if( houseList == null ){
            houseList = new List<GameObject>();
        }else{
            houseList.Clear();
        }
        if(buildings == null){
            return;
        }
        float y;
        float height;
        float tmp1;
        float scale_x = 5;
        float scale_z = 6.25f;
		float w = scale_x + length;
		float s = scale_z +　width;
        //float range_length = number_of_building_length * w;
        //float range_width = number_of_building_width * s;
        // float start_pos_x = this.transform.position.x - range_length/2;
        // Debug.Log(start_pos_x);
        // float start_pos_z = this.transform.position.z - range_width/2;
        // Debug.Log(start_pos_z);
        float end_pos_x = start_pos_x + bluelength;
        float end_pos_z = start_pos_z + bluewidth;
        // Debug.Log(start_pos_x);
        // Debug.Log(start_pos_z);
        for (float i =start_pos_x; i <= end_pos_x; i+=w)
            for(float j = start_pos_z; j <= end_pos_z; j+=s)
            {
                y = UnityEngine.Random.Range(min, max);
                Vector3 position = new Vector3(i, 0, j);
                GameObject chooseBuilding = buildings[Random.Range(0, buildings.Length)];
                GameObject chooseWindow = windows[Random.Range(0, windows.Length)];
                tmp =  Instantiate(chooseBuilding, position , Quaternion.identity);
                tmp.transform.localScale += new Vector3(0, 1.5f*y, 0);
                height = tmp.transform.localScale.y;
                float setToSameY = 0;
                if(height > chooseBuilding.transform.localScale.y)
                {
                    tmp1 = (height - chooseBuilding.transform.localScale.y)/2;
                    setToSameY = tmp1+1.5f;
                    // position = new Vector3(i, tmp1, j);
                }
                else if(height <= chooseBuilding.transform.localScale.y)
                {
                    tmp1 = (chooseBuilding.transform.localScale.y - height)/2;
                    setToSameY = -tmp1+1.5f;
                    // position = new Vector3(i, -tmp1, j);
                }
                position.y += setToSameY;
                tmp.transform.localPosition = position;
                
                if(y >= 1)
                {
					float building_y;
					float center_y = tmp.transform.localPosition.y;
                    building_y = 2f;
                    for (float f = 0; f <= y; f++)
                    {
                        // Debug.Log(building_y);
                        Vector3 window1 = new Vector3(i + 1.05f, building_y, j + 0.55f);
                        windowright = Instantiate(chooseWindow, window1, Quaternion.identity);
                        windowright.transform.Rotate(0, 0, -90);
                        windowright.transform.parent = tmp.transform;
                        Vector3 window2 = new Vector3(i + 1.05f, building_y, j - 0.55f);
                        windowleft = Instantiate(chooseWindow, window2, Quaternion.identity);
                        windowleft.transform.Rotate(0, 0, -90);
                        windowleft.transform.parent = tmp.transform;
                        building_y = building_y + 1.5f;
                    }
                }
                else
                {
                    Vector3 window1 = new Vector3(i + 1.05f, 2.1f, j + 0.55f);
                    windowright = Instantiate(chooseWindow, window1, Quaternion.identity);
                    windowright.transform.Rotate(0, 0, -90);
                    windowright.transform.parent = tmp.transform;
                    Vector3 window2 = new Vector3(i + 1.05f, 2.1f, j - 0.55f);
                    windowleft = Instantiate(chooseWindow, window2, Quaternion.identity);
                    windowleft.transform.Rotate(0, 0, -90);
                    windowleft.transform.parent = tmp.transform;
                }
                float door_y;
                door_y = 0.73f;
                Vector3 door_position = new Vector3(i + 1.1f, door_y, j);
                door = Instantiate(plan1, door_position, Quaternion.identity);
                door.transform.Rotate(0, 0, -90);
                door.transform.parent = tmp.transform;
                
                // Shuffle That
                Vector3 randPosition = new Vector3(
                    Random.Range(randomWidth.x, randomWidth.y), 
                    0, 
                    Random.Range(randomLenghth.x, randomLenghth.y));
                //Debug.Log(randPosition);
                tmp.transform.position = tmp.transform.position + randPosition;
                // houseList.Add(tmp);
                tmp.transform.Rotate(0, Random.Range(-rotaterange,rotaterange), 0);
                if( tmp.transform.position.z <= start_pos_z+randomWidth.y)
                {
                    tmp.transform.Rotate(0, 90, 0);
                }
                else if( tmp.transform.position.z >= end_pos_z-randomWidth.y-5f)
                {
                    tmp.transform.Rotate(0, -90, 0);
                }
                else if( tmp.transform.position.x <= start_pos_x+randomWidth.y)
                {
                    tmp.transform.Rotate(0, 180, 0);
                }
                // tmp.transform.localScale*=2.5f;
                // position = tmp.transform.localScale;
                position.y = tmp.transform.localScale.y / 2; 
                tmp.transform.localPosition = position;
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