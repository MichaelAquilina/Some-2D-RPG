Game Development
================

This GIT hub project is my way of getting familiar with game development techniques used in many modern day games. I chose 
to first focus on 2D development since this would provide me with a nice starting point, all the while being
able to make use of more advanced techniques like shaders. I also have a personal love for retro 2D games like the older
final fantasy series and other SNES titles.

Development Platform
--------------------
I decided to make use of XNA becauase i feel very comfortable using C# as a development language, and still have a lot to learn when it comes to C++.
Using XNA also allows me to concentrate on the actual game development rather than dealing with lower level programing details and memory concerns. In the future
i could possibly take this code and try make it run on the cross-platform implementation of XNA - [MonoGame](https://github.com/mono/MonoGame).

Game Engine
-----------
Because my main focus of this project was to gain some insight into the technical details about how games tick, i chose
to develop the Game Engine from scratch rather than make use of XNA Game Engines like [Flat Red Ball](http://flatredball.com/)
The engine in development currently provides an interface to create top down 2D games. 

Some current features found in the Engine include:
* Composite Animation Support (Including loading from XML files)
* Entity framework for Players, NPCs, Monsters, objects etc..
* Input Extension Methods for quicker development (Currently only for the keyboard)
* Custom Game Shader interface for easy application of shaders to levels (Like 2D lighting). *In its current state, these are technically only post-effect shaders however i plan to support an interface for tile and pre entity shadering*
* Support for loading [Tiled](http://www.mapeditor.org/) map files. (Current no zlib support - however this is planned)
* Efficient Collision Detection using a QuadTree implementation (see GIF below)
* Adjustable viewports of the current Game state, including the ability to Scale.
* Detailed Diagnostic information about engine performance that will allow easy detection of bottlenecks
* The ability to Zoom at any specified level when drawing the game world in a viewport

Some other features present outside the engine
* 2D Light Shader
* Customize character appearance (Hair, Gender, Armour, Weapon etc..)

Some Future features that i aim to include:
* Automatic Asset management (Loading/Unloading content when switching between maps and levels)
* Animated Tiles
* AI framework for Entities (Possibly pre-built Pathfinding techniques)
* Inheritence in Animation files to reduce redundancy
* Pause/Resume functionality
* Map Scripts
* Sound Management
* Application to quickly specify and design animations from spritesheets

Game
----

Currently, any game design that is being done, is purely for testing and expanding upon the requirements of the game engine. 
Once Game Engine development is close to completion i intend to try and perform some actual game design that could demo the 
capabilities of the engine and allow me to further my understanding.

Assets
------

All my art Assets come from the fantastic website [OpenGameArt.org](http://opengameart.org/). In particular, i am making use of
Assets that were made for the [Liberated Pixel Cup](http://lpc.opengameart.org/) challenge that was being organised by the same site.

I may attempt to design/draw some pixel art myself in the future, but i have decided to currently focus on development 
rather than the creative design aspects (including sound/music).

Contribution
------------

If anyone wishes to contact me about possibly contributing to the project they may do so by emailing me on michaelaquilina (AT) gmail.com.

I am mainly in need of artists and sound engineers - however programmers and game designers are also welcome to contribute.

Screenshots
-----------

<img src="Images/HighEntities.png" width="630" height="350"></img>
<img src="Images/QuadTree.gif" width="630" height="350"></img>
<img src="Images/GameProgress3.png" width="630" height="350"></img>
<img src="Images/GameProgress2.png" width="630" height="350"></img>
<img src="Images/GameProgress.png" width="630" height="350"></img>

