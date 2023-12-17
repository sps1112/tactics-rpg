# Tactics RPG

A tactics style turn-based RPG game made in Unity. The game is set isometric view on a 3D grid. The game is based on other tactics styled RPGs such as *Final Fantasy Tactics: Advance* for the GBA.

Current Features of the project:
- A customizable **Level Editor** tool built upon the **Unity Editor Scripting** to design 3D Grids to act as template levels for the game.
- An **Obstacle Editor** tool built in a similar manner to define various layouts for obstacles and spawn points to be placed on the levels.
- **A\* Pathfinding** which acts as the pathfinding algorithm for traversal of the characters on the 3D Grid
- **Character Scriptable Object** script to define various template characters with their own stats. These can be used in our levels as enemies or used as teammates in the player's party.
- A rudimentary implementation of the **Enemy AI** which follows the player character on the 3D Grid.
- **Smooth camera** transitions for the changing phases during the player's and enemy's turn.

Features to be added:

- [ ] The flow of turn events goes through the defined turn phases
- [ ] Update Inputmanager script to be based on the turn phases
- [ ] Delink dragging click and go back click button for move and attack phase
- [ ] Moving to the same position should consume 0 steps/actions for the player and enemy
- [ ] UI window shows the phase of the current turn
- [ ] Update the transitions through states, through turns and tweak time of each phase
- [ ] Update phase delays for the enemy's turn
- [ ] Enemy spawned should face the player spawn points region
- [ ] At the end of the turn, enemy should face the path which leads to the player on that turn
- [ ] UI shows whether the camera is snapping to the target or is snapped on it
- [ ] Multiple player type prefabs. Choose one to them to spawn
- [ ] Multiple enemy types prefabs. Enemy spawn must be from the mission's enemy list
- [ ] Allow choosing starting direction for each character when spawned
- [ ] Choosing end direction from mouse cursor position
- [ ] Hover over characters for stats. Highlighting current character
- [ ] Camera Drag Bounds
- [ ] AI should be implemented as an interface
- [ ] Update Enemy AI to be inherited from the AI interface
- [ ] Enemy prefers target grids at the same height level in case of multiple grids same distance away
- [ ] Update the scripts to support party based logic (Is player and Is enemy checks)
- [ ] Party based gameplay. 3 membered teams for player and enemy
- [ ] Party based pathfinding logic for the enemy team
- [ ] Multiple enemies based on mission enemy list
- [ ] Add a new template [level](https://www.spriters-resource.com/game_boy_advance/fftacticsadv/sheet/10629/)
- [ ] Main Menu, Pause Menu, Settings
- [ ] Battle Option through action button
- [ ] Combat between the player and enemy teams
- [ ] Battle Over, Game End
- [ ] Radiant combat mode (battles one after the another)
- [ ] Enemy prioritise less turns >> back attack >> side attack >> front attack
- [ ] RPG combat using the states
- [ ] Item system for the battles
- [ ] Particle system for the battle effects
- [ ] Campaign mode. Quest system.
- [ ] Level up system of the characters
- [ ] Animations for the characters and the interactions
- [ ] Persistence through save states
- [ ] Equipment and Inventory system
- [ ] Add a new template [level](https://www.spriters-resource.com/game_boy_advance/fftacticsadv/sheet/10637/)
