using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public string Username;
	public bool Human;
	public HUD hud;
	public WorldObject SelectedObject{ get; set; }


	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren<HUD> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
