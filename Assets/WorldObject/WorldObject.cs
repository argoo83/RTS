using UnityEngine;
using System.Collections;
using RTS;

public class WorldObject : MonoBehaviour {

	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;
	protected Bounds selectionBounds;
	protected Rect playingArea = new Rect (0.0f, 0.0f, 0.0f, 0.0f);

	public string objectName;
	public Texture2D buildImage;
	public int cost, sellValue, hitPoints, maxHitPoints;



	protected virtual void Awake(){
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds ();
	}

	protected virtual void Start(){
		player = transform.root.GetComponentInChildren<Player> ();
	}

	protected virtual void Update(){

	}

	protected virtual void OnGUI(){
		if (currentlySelected)
			DrawSelection ();
	}

	public void SetSelections(bool selected, Rect playingArea){
		currentlySelected = selected;
		if (selected)
			this.playingArea = playingArea;
	}

	private void DrawSelection ()
	{
		GUI.skin = ResourceManager.SelectBoxSkin;
		Rect selectBox = WorkManager.CalculateSelectionBox (selectionBounds, playingArea);
		//Draw the selectionbox around the currently selected object, within the bounds of the playing area
		GUI.BeginGroup (playingArea);
		DrawSelectionBox (selectBox);
		GUI.EndGroup ();
	}


	public void CalculateBounds(){
		selectionBounds = new Bounds (transform.position, Vector3.zero);
		foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
			selectionBounds.Encapsulate(r.bounds);
		}
	}

	protected void DrawSelectionBox (Rect selectBox) {
		GUI.Box (selectBox, "");
	}

	public string[] GetActions(){
		return actions;
	}

	public virtual void PerformAction(string actionToPerform){

	}

	public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller){
		//only handle input if currently selected
		if (currentlySelected && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();
			//clicked another selectable object
			if(worldObject) ChangeSelection(worldObject, controller);
		}
	}

	void ChangeSelection (WorldObject worldObject, Player controller)
	{
		//this should be called by the following line, but there is an outside change it will not
		SetSelections (true, playingArea);
		if (controller.SelectedObject)
			controller.SelectedObject.SetSelections (false, playingArea);
		controller.SelectedObject = worldObject;
		worldObject.SetSelections (true, controller.hud.GetPlayingArea());
	}
}
