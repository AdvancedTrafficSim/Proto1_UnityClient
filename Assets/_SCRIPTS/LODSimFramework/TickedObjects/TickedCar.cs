using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickedCar : TickedObject {
	public Transform visual; //visual representation of the car
	[SerializeField] protected Vector3 startPoint; //todo: temporary for demo purposes, where are we starting at?
	[SerializeField] public Vector3 targetPoint; //todo: temporary for demo purposes, where are we trying to get?
	[SerializeField] protected float percentTravelled; //todo: temporary, demo
	[SerializeField] public float speed; //unity units per second (real-world-units agnostic for now, later we'll define how much real-world space is 1 unity unit

	public bool IsAtDestination { get { return percentTravelled == 1; } }

	public TickedCar(Int64 currentTime, Transform visual) : base(currentTime)
	{
		this.visual = visual;
		startPoint = visual.position; //todo: temporary for demo purposes
		percentTravelled = 0;
	}

	protected override bool doTick(long endTime, float timeStep)
	{
		//Debug.Log("Start: " + startPoint + "; Current: " + visual.position + "; End: " + targetPoint + " Percent: " + percentTravelled);

		if (percentTravelled == 1) return true; //we done already

		try
		{
			//todo: this is just a very simple thing for now, move the car towards its target point, according to its speed and how much time elapsed. that's all
			if (timeStep < 0.4f) //20FPS precision, let's call this the minimum for realtime simulation, player can directly see the thing and interact with it, meaning this should be the most detailed variant
			{
				return simLod1(endTime, timeStep);
			}
			else if (timeStep < 1f) //this is "almost-realtime simulation" range, player can see the thing, but it's far enough that it can be a tiny bit choppy and doesn't need to be immediately reactive
			{
				return simLod2(endTime, timeStep);
			}
			else if (timeStep < 5) //another LOD level, up to just 1 update per second
			{
				return simLod3(endTime, timeStep);
			}
			else //less than 1 update per second, let's say we're in pure statistics territory right now
			{
				return simLod4(endTime, timeStep);
			}
		} catch(Exception e)
		{
			//don't care right now, in final should reset object's state to where we started this tick at, and signals upstream that the sim tick failed
			return false;
		}
	}

	//right now, i'm not implementing real car logic so these are going to be the same, but i've separated them just to show the principle
	private bool simLod1(long endTime, float timeStep)
	{
		//snap to our proper journey percentage (for LOD transitions, if the only most recent thing is that percentage we first need to jump the object's position where it should be)
		visual.position = startPoint + ((targetPoint - startPoint) * percentTravelled);

		//direction of our movement
		Vector3 moveDir = (targetPoint - startPoint).normalized;
		Vector3 previousPos; //so we can snap back to most accurate endpos
		long curTime = lastTicked;

		while(curTime < endTime) {
			previousPos = visual.position;
			//move according to speed, derive %travelled from there (more accurate visually)
			visual.position += moveDir * speed * timeStep;

			//update journey percentage
			percentTravelled = Vector3.Distance(visual.position, startPoint) / Vector3.Distance(startPoint, targetPoint);

			//Debug.Log("SimLOD1: dist-pos-start: " + Vector3.Distance(visual.position, startPoint) + "; dist-start-target: " + Vector3.Distance(startPoint, targetPoint) + "; perc: " + percentTravelled);
			
			//if we're beyond the target, snap back to it
			if (percentTravelled > 1) percentTravelled = 1;

			//todo: in this logic there would be probably some mechanism to extract events from nearby TickedObjects along with the times of the events, so we can simulate and process their interactions properly
			curTime += (long)(timeStep * 1000); //todo: 1. timestep is in seconds, curTime is in milliseconds. 2. this is idiotic, even rounding would be, we need something more accurate, i know
		}

		//todo: doesn't work in this way... adjust for inaccuracies (snap back/forward to the most accurate final position)
		//long diff = endTime - curTime;
		//visual.position += moveDir * speed * diff;

		//update journey percentage
		percentTravelled = Vector3.Distance(visual.position, startPoint) / Vector3.Distance(startPoint, targetPoint);

		//if we're beyond the target, snap back to it
		if (percentTravelled > 1) percentTravelled = 1;

		return true; //because we're stupid within the demo and have no possible fail states
	}

	private bool simLod2(long endTime, float timeStep)
	{
		//snap to our proper journey percentage (for LOD transitions, if the only most recent thing is that percentage we first need to jump the object's position where it should be)
		visual.position = startPoint + ((targetPoint - startPoint) * percentTravelled);

		//direction of our movement
		Vector3 moveDir = (targetPoint - startPoint).normalized;
		Vector3 previousPos; //so we can snap back to most accurate endpos
		long curTime = lastTicked;

		while (curTime < endTime)
		{
			previousPos = visual.position;
			//move according to speed, derive %travelled from there (more accurate visually)
			visual.position += moveDir * speed * timeStep;

			//update journey percentage
			percentTravelled = Vector3.Distance(visual.position, startPoint) / Vector3.Distance(startPoint, targetPoint);

			//Debug.Log("SimLOD2: dist-pos-start: " + Vector3.Distance(visual.position, startPoint) + "; dist-start-target: " + Vector3.Distance(startPoint, targetPoint) + "; perc: " + percentTravelled);
			
			//if we're beyond the target, snap back to it
			if (percentTravelled > 1) percentTravelled = 1;

			//todo: in this logic there would be probably some mechanism to extract events from nearby TickedObjects along with the times of the events, so we can simulate and process their interactions properly
			curTime += (long)(timeStep * 1000); //todo: 1. timestep is in seconds, curTime is in milliseconds. 2. this is idiotic, even rounding would be, we need something more accurate, i know
		}

		//todo: doesn't work in this way... adjust for inaccuracies (snap back/forward to the most accurate final position)
		//long diff = endTime - curTime;
		//visual.position += moveDir * speed * diff;

		//update journey percentage
		percentTravelled = Vector3.Distance(visual.position, startPoint) / Vector3.Distance(startPoint, targetPoint);

		//if we're beyond the target, snap back to it
		if (percentTravelled > 1) percentTravelled = 1;

		return true; //because we're stupid within the demo and have no possible fail states
	}

	//let's pretend these are different. too low LOD to actually move the transform, so just change the info about "how many percent along the path have we travelled?"
	private bool simLod3(long endTime, float timeStep)
	{
		float curTime = lastTicked;

		while(curTime < endTime)
		{
			//only update journey percentage
			percentTravelled += (speed * timeStep) / Vector3.Distance(startPoint, targetPoint);

			//Debug.Log("SimLOD3: %: " + percentTravelled);

			curTime += (long)(timeStep * 1000); //funny tidbit: if you forget this line, you get into infinite loop that hard-freezes the game! =D =D
		}

		//if we're beyond the target, snap back to it
		if(percentTravelled > 1) percentTravelled = 1;
		//snap visual to where it should be
		visual.position = startPoint + ((targetPoint - startPoint) * percentTravelled);

		return true;
	}

	private bool simLod4(long endTime, float timeStep)
	{
		float curTime = lastTicked;

		while (curTime < endTime)
		{
			//only update journey percentage
			percentTravelled += (speed * timeStep) / Vector3.Distance(startPoint, targetPoint);

			//Debug.Log("SimLOD3: %: " + percentTravelled);

			curTime += (long)(timeStep * 1000); //funny tidbit: if you forget this line, you get into infinite loop that hard-freezes the game! =D =D
		}

		//if we're beyond the target, snap back to it
		if (percentTravelled > 1) percentTravelled = 1;
		//snap visual to where it should be
		visual.position = startPoint + ((targetPoint - startPoint) * percentTravelled);

		return true;
	}
}
