using UnityEngine;
using System.Collections;

public class ItweenMove : MonoBehaviour
{
    public string pathName;
    public float time;

	void Start(){
		iTween.MoveTo(gameObject, iTween.Hash("Path", ItweenPath.GetPath(pathName), "easeType",iTween.EaseType.easeInOutSine, "time", time));
	}
}

