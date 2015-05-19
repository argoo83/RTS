using UnityEngine;
using System.Collections;
using RTS;
using System.Collections.Generic;

public class HUD : MonoBehaviour {

	private const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40, SELECTION_NAME_HEIGHT = 15;
	private const int ICON_WIDTH = 32, ICON_HEIGHT = 32, TEXT_WIDTH = 128, TEXT_HEIGHT = 32;
	private Player _player;
	private CursorState activeCursorState;
	private int currentFrame = 0;

	private Dictionary<ResourceType, int> resourceValues;
	private Dictionary<ResourceType, int> resourceLimits;
	private Dictionary<ResourceType, Texture2D> resourceImages;

	public Texture2D activeCursor;
	public Texture2D selectCursor, leftCursor, rightCursor, upCursor, downCursor;
	public Texture2D[] moveCursor, attackCursor, harvestCursor;
	public Texture2D[] resources;

	public GUISkin resourceSkin, ordersSkin, selectBoxSkin, mouseCursorSkin;

	// Use this for initialization
	void Start () {
		resourceValues = new Dictionary<ResourceType, int> ();
		resourceLimits = new Dictionary<ResourceType, int> ();
		resourceImages = new Dictionary<ResourceType, Texture2D> ();
		for (int i = 0; i < resources.Length; i++) {
			switch(resources[i].name){
			case "Money":
				resourceImages.Add(ResourceType.Money, resources[i]);
				resourceValues.Add(ResourceType.Money, 0);
				resourceLimits.Add(ResourceType.Money, 0);
				break;
			case "Power":
				resourceImages.Add(ResourceType.Power, resources[i]);
				resourceValues.Add(ResourceType.Power, 0);
				resourceLimits.Add(ResourceType.Power, 0);
				break;
			default: break;
			}
		}
		_player = transform.root.GetComponent<Player> ();
		ResourceManager.StoreSelectBoxItem (selectBoxSkin);
		SetCursorState (CursorState.Select);
	}
	
	// Update is called once per frame
	void OnGUI () {
	
		if (_player && _player.Human) {
			DrawOrdersBar();
			DrawResourceBar();
			DrawMouseCursor();
		}
	}

	public Rect GetPlayingArea(){
		return new Rect (0, RESOURCE_BAR_HEIGHT, Screen.width - ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT);
	}

	private void DrawMouseCursor(){

		bool mouseOverHud = !MouseInBounds () && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp;

		if (mouseOverHud) {
			Cursor.visible = true;
		} else {
			Cursor.visible = false;
			GUI.skin = mouseCursorSkin;
			GUI.BeginGroup( new Rect(0,0,Screen.width, Screen.height));
			UpdateCursorAnimation();
			Rect cursorPosition = GetCursorDrawPosition();
			GUI.Label(cursorPosition, activeCursor);
			GUI.EndGroup();
		}
	}

	private void DrawOrdersBar ()
	{
		GUI.skin = ordersSkin;
		GUI.BeginGroup (new Rect (Screen.width - ORDERS_BAR_WIDTH, RESOURCE_BAR_HEIGHT, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
		GUI.Box (new Rect (0, 0, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT), "");

		string selectionName = "";
		if (_player.SelectedObject) {
			selectionName = _player.SelectedObject.objectName;
		}

		if (!selectionName.Equals ("")) {
			GUI.Label(new Rect(0,10, ORDERS_BAR_WIDTH, SELECTION_NAME_HEIGHT), selectionName);
		}

		GUI.EndGroup ();
	}



	private void DrawResourceBar ()
	{
		GUI.skin = resourceSkin;
		GUI.BeginGroup (new Rect (0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
		GUI.Box (new Rect (0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
		int topPos = 4, iconLeft = 4, textLeft = 40;
		DrawResourceIcon (ResourceType.Money, iconLeft, textLeft, topPos);
		iconLeft += TEXT_WIDTH;
		textLeft += TEXT_WIDTH;
		DrawResourceIcon (ResourceType.Power, iconLeft, textLeft, topPos);
		GUI.EndGroup ();
	}

	void DrawResourceIcon (ResourceType type, int iconLeft, int textLeft, int topPos)
	{
		Texture2D icon = resourceImages [type];
		string text = resourceValues [type].ToString () + "/" + resourceLimits [type].ToString ();
		GUI.DrawTexture (new Rect (iconLeft, topPos, ICON_WIDTH, ICON_HEIGHT), icon);
		GUI.Label(new Rect(textLeft, topPos, TEXT_WIDTH, TEXT_HEIGHT), text);
	}

	public bool MouseInBounds(){
		//Screen coordinates start in the lower-left corner of the screen
		//not the top-left of the screen like the drawing coordinates do
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - ORDERS_BAR_WIDTH;
		bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
		return insideHeight && insideWidth;
	}




	private void UpdateCursorAnimation ()
	{
		//sequence animation for cursor (based on more than one image for the cursor)
		//change once per second, loops through array of images

		if (activeCursorState == CursorState.Move) {
			currentFrame = (int)Time.time % moveCursor.Length;
			activeCursor = moveCursor [currentFrame];
		} else if (activeCursorState == CursorState.Attack) {
			currentFrame = (int)Time.time % attackCursor.Length;
			activeCursor = attackCursor [currentFrame];
		} else if (activeCursorState == CursorState.Harvest) {
			currentFrame = (int)Time.time % harvestCursor.Length;
			activeCursor = harvestCursor [currentFrame];
		}
	}

	private Rect GetCursorDrawPosition ()
	{
		//set base position for custom cursor image
		float leftPos = Input.mousePosition.x;
		float topPos = Screen.height - Input.mousePosition.y; //screen draw coordinates are inverted

		//adjust position base in the type of cursor being shown
		if (activeCursorState == CursorState.PanRight)
			leftPos = Screen.width - activeCursor.width;
		else if (activeCursorState == CursorState.PanDown)
			topPos = Screen.height - activeCursor.height;
		else if (activeCursorState == CursorState.Move || activeCursorState == CursorState.Select || activeCursorState == CursorState.Harvest) {
			topPos -= activeCursor.height / 2;
			leftPos -= activeCursor.width / 2;
		}

		return new Rect (leftPos, topPos, activeCursor.width, activeCursor.height);
	}

	public void SetCursorState(CursorState newState){
		activeCursorState = newState;

		switch (newState) {
		case CursorState.Select:
			activeCursor = selectCursor;
			break;
		case CursorState.Attack:
			currentFrame = (int)Time.time % attackCursor.Length;
			activeCursor = attackCursor[currentFrame];
			break;
		case CursorState.Harvest:
			currentFrame = (int)Time.time % harvestCursor.Length;
			activeCursor = harvestCursor[currentFrame];
			break;
		case CursorState.Move:
			currentFrame = (int)Time.time % moveCursor.Length;
			activeCursor = moveCursor[currentFrame];
			break;
		case CursorState.PanLeft:
			activeCursor = leftCursor;
			break;
		case CursorState.PanRight:
			activeCursor = rightCursor;
			break;
		case CursorState.PanUp:
			activeCursor = upCursor;
			break;
		case CursorState.PanDown:
			activeCursor = downCursor;
			break;
		default: break;
		}

	}

	public void SetResourceValue (Dictionary<ResourceType, int> resourceValues, Dictionary<ResourceType, int> resourceLimits)
	{
		this.resourceValues = resourceValues;
		this.resourceLimits = resourceLimits;
	}
}
