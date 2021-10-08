using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(randomcube))]
public class HouseCreatorEditor : Editor {
	private bool D_flag = true;
	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		randomcube house = (randomcube)target;

		GUILayout.Space(1);

		string showCtrStr = D_flag ? "Destory" : "Generate";
		string setCtrStr = "Material Update";
		if(GUILayout.Button(setCtrStr)){
			house.setMaterial();
		}
        if (GUILayout.Button(showCtrStr))
        {
            if (D_flag) {
                house.previous_house_destroy();
                D_flag = false;
            }else{
				house.bouding_test();
            	D_flag = true;
			}
        }
	}
}
