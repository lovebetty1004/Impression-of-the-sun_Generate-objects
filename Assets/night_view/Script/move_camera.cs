using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_camera : MonoBehaviour {

	// Use this for initialization
	[Range(0, 0.2f)]
	public float Speed_x;
	[Range(0, 0.2f)]
	public float Speed_y;
	[Range(0, 0.2f)]
	public float Speed_z;
	public Vector3 start_position;
	public GameObject startPosObj;

	Vector3 position;
	//public bool gogo = false;
	void Start () {
		// this.transform.position = new Vector3(121, -1, 74);
		// Quaternion rotation = Quaternion.Euler (45, -90, 0);
		// this.transform.rotation = rotation;
		if(startPosObj != null){
			position = startPosObj.transform.position;
		}else{
			position = start_position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (position.y <= 30){
			position += new Vector3(Speed_x, Speed_y, -Speed_z);
			this.transform.position = position;
		}

	}
}
