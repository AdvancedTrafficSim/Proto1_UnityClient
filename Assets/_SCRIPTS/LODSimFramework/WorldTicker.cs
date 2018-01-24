using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Broadcasts ticks across the whole world according to which objects in it need to get them how often
/// </summary>
public class WorldTicker : TickedObject {

	/// <summary>
	/// rough sketch, doesn't account for delays caused by time aligning at creation and such
	/// </summary>
	public class SimLodGroup
	{
		public long TickInterval; // how often (in milliseconds) we want to update this group
		public float TickTimeStep; //what do we send to the objects of the group as their simulation timestep (in seconds) - value of this makes sense if it's the same or smaller than TickInterval
		public List<TickedObject> Members;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tickInterval">how often (in milliseconds) we want to update this group</param>
		/// <param name="tickTimeStep">what do we send to the objects in the group as their simulation timesteps (in seconds)</param>
		public SimLodGroup(long tickInterval, float tickTimeStep)
		{
			TickInterval = tickInterval; TickTimeStep = tickTimeStep;
			Members = new List<TickedObject>();
		}
	}

	public List<SimLodGroup> groups; //todo: i'm lazy, for demo purposes I can fill this in directly from outside

	public WorldTicker() : base(0) // worldTicker is the base of our whole world, so when it is created, that's the Zero time of our whole world (for now, while we can ignore server stuff)
	{
		groups = new List<SimLodGroup>();
	}

	/// <summary>
	/// For now assume WorldTicker gets each frame update as a tick, as it should, meaning
	/// </summary>
	/// <param name="endTime">will be its own LastTicked + Time.deltaTime, and timeStep will be Time.deltaTime</param>
	/// <param name="timeStep"></param>
	/// <returns></returns>
	protected override bool doTick(long endTime, float timeStep)
	{
		//each group that should be ticked (determined just by modulo right now) is ticked
		for (int c1 = 0; c1 < groups.Count; c1++)
		{
			//Debug.Log("SHOULDTICK?: group: " + c1 + "; tickInterval: " + groups[c1].TickInterval + "; ti%ts: " + Mathf.Round((groups[c1].TickInterval % timeStep) * 1000));
			if (groups[c1].TickInterval == 0 || Mathf.Round((groups[c1].TickInterval % timeStep) * 1000) == 0) //compare with millisecond precision
			{
				//Debug.Log("DOTICK: group: " + c1 + "; tickInterval: " + groups[c1].TickInterval + "; ti%ts: " + groups[c1].TickInterval % timeStep);
				for (int c2 = 0; c2 < groups[c1].Members.Count; c2++) groups[c1].Members[c2].Tick(endTime, groups[c1].TickTimeStep);
			}
		}

		return true; //todo: because for now we're stupid and don't check for errors, because a demo sketch
	}
}
