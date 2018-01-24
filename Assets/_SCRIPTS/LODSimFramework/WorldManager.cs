using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sadly, the top-level object needs to be a monobehaviour =D
/// Basically just a container for the worldTicker, binding it to unity engine/update/time system
/// </summary>
public class WorldManager : MonoBehaviour {
	public GameObject carPrefab;
	public Vector3 genArea = new Vector3(10, 1, 10);
	public List<int> carsInGroups = new List<int>() { 1, 1, 1, 1};
	public List<bool> doRenderGroups = new List<bool> { true, true, false, false };

	private WorldTicker ticker;

	// Use this for initialization
	void Start () {
		ticker = new WorldTicker();

		generateCarLodGroups();
	}
	
	// Update is called once per frame
	void Update () {
		ticker.Tick(ticker.LastTicked + (long)(Time.deltaTime*1000), Time.deltaTime); //float seconds to long milliseconds, stupid to do it by casting, i know
	}

	List<TickedObject> generateCars(int number, Color lodGroupColor, bool doRender)
	{
		Vector3 startVector, endVector;
		float speed;

		List<TickedObject> tmpGroup = new List<TickedObject>(number);

		for (int c = 0; c < number; c++)
		{
			startVector = new Vector3(Random.value * genArea.x, Random.value * genArea.y, Random.value * genArea.z);
			endVector = startVector + new Vector3(Random.value * 100, 0, Random.value * 100);

			speed = 1; //max speed of 2

			tmpGroup.Add(makeCar(startVector, endVector, speed, lodGroupColor, doRender));
		}

		return tmpGroup;
	}


	void generateCarLodGroups()
	{
		WorldTicker.SimLodGroup realtime = new WorldTicker.SimLodGroup(0, 0); //each frame
		WorldTicker.SimLodGroup almostRealtime = new WorldTicker.SimLodGroup(100, 0.2f); //10x per second
		WorldTicker.SimLodGroup lod3 = new WorldTicker.SimLodGroup(1000, 0.5f); //once per second, fluent simstep
		WorldTicker.SimLodGroup lod4 = new WorldTicker.SimLodGroup(5000, 1); //once per 5 seconds, fluent simstep

		//realtime group
		realtime.Members = generateCars(carsInGroups[0], Color.white, doRenderGroups[0]);
		//almostrealtime group
		almostRealtime.Members = generateCars(carsInGroups[1], Color.red, doRenderGroups[1]);
		//lod3
		lod3.Members = generateCars(carsInGroups[2], Color.green, doRenderGroups[2]);
		//lod4
		lod4.Members = generateCars(carsInGroups[3], Color.blue, doRenderGroups[3]);
		//assign to ticker
		ticker.groups.Add(realtime);
		ticker.groups.Add(almostRealtime);
		ticker.groups.Add(lod3);
		ticker.groups.Add(lod4);
	}

	TickedCar makeCar(Vector3 position, Vector3 target, float speed, Color color, bool doRender)
	{
		GameObject temp = Instantiate<GameObject>(carPrefab, position, Quaternion.identity);
		temp.transform.position = position;

		temp.GetComponent<Renderer>().material.color = color;

		if (!doRender) temp.GetComponent<Renderer>().enabled = false;

		TickedCar tempCar = new TickedCar(0, temp.transform);
		tempCar.targetPoint = target;
		tempCar.speed = speed;

		return tempCar;
	}

	private void OnDrawGizmos()
	{
		if(ticker != null && ticker.groups != null)
			for (int c1 = 0; c1 < ticker.groups.Count; c1++)
			{
				for(int c2 = 0; c2 < ticker.groups[c1].Members.Count; c2++)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawWireSphere(((TickedCar)ticker.groups[c1].Members[c2]).targetPoint, 1);
					if(((TickedCar)ticker.groups[c1].Members[c2]).visual != null)
						Gizmos.DrawLine(((TickedCar)ticker.groups[c1].Members[c2]).targetPoint, ((TickedCar)ticker.groups[c1].Members[c2]).visual.position);
				}
			}
	}
}
