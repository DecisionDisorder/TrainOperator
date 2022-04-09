using UnityEngine;
using System.Collections;

public class Resolution_Controller : MonoBehaviour {

	int s_width;
	int s_height;

	float f_width;
	float f_height;
	void Awake()
	{
		s_width = Screen.width;
		s_height = Screen.height;

		f_width = (float)s_width;
		f_height = (float)s_height;
	}
	void Start () {
		Screen.SetResolution (Screen.width,Screen.width/16*9,true);

		/*if ((float)(f_width / f_height) == 16/10){
			Screen.SetResolution (Screen.width,(int)Screen.width/16*9,true);
		}
		if ((float)(f_width / f_height) == 5 / 3) {
			Screen.SetResolution (Screen.width,Screen.width/5*3,true);
		}
		if ((float)(f_width / f_height) == 4/3) {
			Screen.SetResolution (Screen.width,Screen.width/4*3,true);
		}*/
	}
}
