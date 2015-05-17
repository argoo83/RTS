using UnityEngine;
using System.Collections;
using RTS;

public class UserInput : MonoBehaviour {

	private Player _player;
	// Use this for initialization
	void Start () {
		_player = transform.root.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_player.Human) {
			MoveCamera ();
			RotateCamera ();
			MouseActivity();
		}
	}

	private void MoveCamera ()
	{
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3 (0, 0, 0);

		bool mouseScroll = false;

		//Horizontal camera movement (Scroll when mouse is at the edge - "ScrollWidth")
		if (xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
			movement.x -= ResourceManager.ScrollSpeed;
			_player.hud.SetCursorState(CursorState.PanLeft);
			mouseScroll = true;
		} else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth) {
			movement.x += ResourceManager.ScrollSpeed;
			_player.hud.SetCursorState(CursorState.PanRight);
			mouseScroll = true;
		}
			
		//Vertical camera movement (Scroll when mouse is at the edge - "ScrollWidth")
		if (ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
			movement.z -= ResourceManager.ScrollSpeed;
			_player.hud.SetCursorState(CursorState.PanDown);
			mouseScroll = true;
		} else if (ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
			movement.z += ResourceManager.ScrollSpeed;
			_player.hud.SetCursorState(CursorState.PanUp);
			mouseScroll = true;
		}

		//make sure movement is in the direction the camera is pointing
		//but ignore vertical til of the camera to get sensible scrolling
		movement = Camera.main.transform.TransformDirection (movement);
		movement.y = 0;

		//away from ground movement
		movement.y -= ResourceManager.ScrollSpeed + Input.GetAxis ("Mouse ScrollWheel");

		//calculate desired camera position based in recieved input
		Vector3 origin = Camera.main.transform.position;
		Vector3 destination = origin;
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;

		//limit away from ground movement to be between a minimum and maximum distance
		if (destination.y > ResourceManager.MaxCameraHeight) {
			destination.y = ResourceManager.MaxCameraHeight;
		} else if (destination.y < ResourceManager.MinCameraHeight) {
			destination.y = ResourceManager.MinCameraHeight;
		}

		//if a change position is detected perform a necessary update
		if (destination != origin) {
			Camera.main.transform.position= Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
		}

		if (!mouseScroll) {
			_player.hud.SetCursorState(CursorState.Select);
		}

	}

	private void RotateCamera ()
	{
		Vector3 origin = Camera.main.transform.eulerAngles;
		Vector3 destination = origin;

		//detect rotation amount if ALT is being held and the rigt mouse button i down
		if ((Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt)) && Input.GetMouseButton (1)) {
			destination.x -= Input.GetAxis("Mouse Y") * ResourceManager.RotateAmount;
			destination.y -= Input.GetAxis("Mouse X") * ResourceManager.RotateAmount;
		}

		//if a change in position i detected perform the necessary update
		if (destination != origin) {
			Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime + ResourceManager.RotateSpeed);
		}
	}

	private void MouseHover() {
		if(_player.hud.MouseInBounds()) {
			GameObject hoverObject = FindHitObject();
			if(hoverObject) {
				if(_player.SelectedObject) _player.SelectedObject.SetHoverState(hoverObject);
				else if(hoverObject.name != "Ground") {
					Player owner = hoverObject.transform.root.GetComponent< Player >();
					if(owner) {
						Unit unit = hoverObject.transform.parent.GetComponent< Unit >();
						Building building = hoverObject.transform.parent.GetComponent< Building >();
						if(owner.Username == _player.Username && (unit || building)) _player.hud.SetCursorState(CursorState.Select);
					}
				}
			}
		}
	}

	private void MouseActivity ()
	{
		if (Input.GetMouseButtonDown (0))
			LeftMouseClick ();
		else if (Input.GetMouseButtonDown (1))
			RightMouseClick ();

		MouseHover ();
	}

	private void LeftMouseClick ()
	{
		if (_player.hud.MouseInBounds ()) {
			GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoints();
			if(hitObject && hitPoint != ResourceManager.InvalidPosition){
				if(_player.SelectedObject) _player.SelectedObject.MouseClick(hitObject, hitPoint, _player);
				else if(hitObject.name != "Ground"){
					WorldObject worldObject = hitObject.transform.parent.GetComponent<WorldObject>();
					if(worldObject){
						_player.SelectedObject = worldObject;
						worldObject.SetSelections(true, _player.hud.GetPlayingArea());
					}
				}
			}
		}
	}

	void RightMouseClick ()
	{
		if (_player.hud.MouseInBounds () && !Input.GetKey (KeyCode.LeftAlt) && _player.SelectedObject) {
			_player.SelectedObject.SetSelections(false, _player.hud.GetPlayingArea());
			_player.SelectedObject = null;
		}
	}

	private GameObject FindHitObject(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit))
			return hit.collider.gameObject;
		return null;
	}

	private Vector3 FindHitPoints(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit))
			return hit.point;
		return ResourceManager.InvalidPosition;
	}
}
