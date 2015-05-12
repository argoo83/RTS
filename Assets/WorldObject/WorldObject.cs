using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour {

	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;

	public string objectName;
	public Texture2D buildImage;
	public int cost, sellValue, hitPoints, maxHitPoints;



	protected virtual void Awake(){

	}

	protected virtual void Start(){
		player = transform.root.GetComponentInChildren<Player> ();
	}

	protected virtual void Update(){

	}

	protected virtual void OnGUI(){

	}

	public void SetSelections(bool selected){
		currentlySelected = selected;
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
		SetSelections (true);
		if (controller.SelectedObject)
			controller.SelectedObject.SetSelections (false);
		controller.SelectedObject = worldObject;
		worldObject.SetSelections (true);
	}
}
