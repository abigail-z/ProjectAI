# ProjectAI

An AI car avoids walls and navigates through a small race track.

### AI Components

The AI car uses a path following algorithm to stay on track on a given path.  
A binary decision tree is used to decide which path it wants to follow. It decides which path it wants to follow based off of how many coins it has compared to the player.  
A finite state machine is used to define its normal driving behaviour, and its "aggressive" behaviour, where it ignores coins and instead tries to collide with the player.
