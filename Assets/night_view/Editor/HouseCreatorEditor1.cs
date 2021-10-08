using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(house_generator))]
public class HouseCreatorEditor1 : Editor {
	private bool D_flag = true;
	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		house_generator house = (house_generator)target;

		GUILayout.Space(5);

		string showCtrStr = D_flag ? "Destory" : "Generate";
		
        if (GUILayout.Button(showCtrStr))
        {
            if (D_flag) {
                house.previous_house_destroy();
                D_flag = false;
            }else{
				house.loadPicture();
            	D_flag = true;
			}
        }
	}
}
