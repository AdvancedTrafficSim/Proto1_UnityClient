using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for basically every dynamic object within the simulation
/// encapsulates LOD simulation updates. stores the absolute time of when it was last updated, and when Tick is called, processes its simulation from its last update up to that point
/// todo: not sure how to store the time. real-world time won't help, time since client startup is too fragile, we probably need something bound to the in-game datetime,
/// with precision from years up to milliseconds. for now, for demo simplicity, i'll go with probably non-ideal option - Int64 of milliseconds since gameworld inception
/// </summary>
public abstract class TickedObject {
	/// <summary>
	/// In milliseconds, meaning with int64 with maxvalue of 9,223,372,036,854,775,807
	/// this gives us 9,223,372,036,854,775,807 / (1000*60*60*24) == 106751991167 days of playtime on a map, until things go horribly wrong =D
	/// (that's about 292.5 million years, that should be enough i think)
	/// </summary>
	protected Int64 lastTicked;

	public Int64 LastTicked
	{
		get { return lastTicked; }
	}

	/// <summary>
	/// Constructor, duh
	/// </summary>
	/// <param name="currentTime">we need to provide absolute world time at which this object started existing, so it doesn't go crazy when it tries to update its simulation from
	/// 0 up to current time in the next tick</param>
	public TickedObject(Int64 currentTime)
	{
		lastTicked = currentTime; //so that our first tick just simulates the span from creation until current point
	}

	/// <summary>
	/// Update function. Updates object's simulation. Yeah, it's ugly that one param is int and other one is float, but (at least now) i have to make some concessions because of
	/// how unity works - it provides the time as float seconds.
	/// </summary>
	/// <param name="upToTime">Absolute world time up to which we want to update the object - in milliseconds</param>
	/// <param name="timeStep">Precision of the simulation iteration - each iteration advances the simulation of this object by this much time (in seconds). For "real-time" updates, this should be fed unity's Time.deltaTime when the function is being called each Update frame</param>
	public virtual void Tick(Int64 upToTime, float timeStep)
	{
		if (timeStep == 0) timeStep = Time.deltaTime;

		if (doTick(upToTime, timeStep)) lastTicked = upToTime; //if simulation happened successfully, our new state is now if not, assume our state has not changed, so last updated is still the same
	}

	/// <summary>
	/// Override to implement the actual update/simulation logic. shielded from the public Tick for few future-proofing reasons.
	/// </summary>
	/// <returns>was the attempt to update the object successful?</returns>
	protected abstract bool doTick(Int64 endTime, float timeStep);
}
