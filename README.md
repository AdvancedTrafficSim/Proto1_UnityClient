# Proto1_UnityClient
Repo for prototyping some stuff that might end up in the final unity3d client

Just the most trivial sketch of simulation LOD groups, and "pseudo-cars" (just travelling from start to target point).

Very trivial, sometimes skips sending ticks to groups (because MOD is not a good solution for finding out when to send them, but it still kinda works out after a bit, and even if groups are skipped, on the next tick they get, they will get to where they should be at that point).

Needs some more work before we can really start getting an idea of performance from it, but so far it can handle 2250 rendered cars (250 realtime, 2000 almost-realtime), and 15000 unrendered but still simulated (in the lowest sim LOD) at between 60 to 30 FPS (in editor, which gives some overhead).
making for 17250 "simulated" and moving entities total.

Again, very trivial and rough first sketch, there's many more optimizations to the base system that can and need to be done (some staggering of when the sim ticks occur to the groups and even members in groups themselves), but I want to try and push it to git now, just to get into the habit of it.