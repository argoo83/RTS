using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Player : MonoBehaviour {

	public string Username;
	public bool Human;
	public HUD hud;
	public WorldObject SelectedObject{ get; set; }
	public int startMoney, startMoneyLimit, startPower, startPowerLimit;
	private Dictionary<ResourceType, int> resources, resourceLimits;

	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren<HUD> ();
		AddStartResourceLimits ();
		AddStartResources ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Human) {
			hud.SetResourceValue(resources, resourceLimits);
		}
	}

	void Awake(){
		resources = InitResourceList ();
		resourceLimits = InitResourceList ();
	}

	Dictionary<ResourceType, int> InitResourceList ()
	{
		Dictionary<ResourceType, int> list = new Dictionary<ResourceType, int> ();
		list.Add (ResourceType.Money, 0);
		list.Add (ResourceType.Power, 0);
		return list;
	}

	void AddStartResourceLimits ()
	{
		IncrementResourceLimit (ResourceType.Money, startMoneyLimit);
		IncrementResourceLimit (ResourceType.Power, startPowerLimit);
	}

	void AddStartResources ()
	{
		AddResource (ResourceType.Money, startMoney);
		AddResource (ResourceType.Power, startPower);
	}

	void AddResource (ResourceType type, int amount)
	{
		resources [type] += amount;
	}

	void IncrementResourceLimit (ResourceType type, int amount)
	{
		resourceLimits [type] += amount;
	}
}
