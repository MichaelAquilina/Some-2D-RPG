Game Development
================

*Screenshot of Current State*
![Concept Screenshot](ConceptScreenshot.png)

This GIT hub project is my way of getting familiar with game development techniques used in many modern day games. I chose 
to first focus on 2D development since this would provide me with a nice starting point to focus on all the while being
able to apply advanced techniques like making use of shaders.

I decided to make use of XNA becauase i feel very comfortable using C# as a development language but still have a lot to learn with C++.
XNA also allows me to concentrate on the actual game development rather than lower level programing details and memory concerns. In the future
i could possibly take this code and try make it run on the cross-platform implementation of XNA - MonoGame.

Game Engine
-----------
Because my main focus of this project was to gain some insight into the technical details about how games tick, i chose
to develop the Game Engine from scratch rather than make use of XNA Game Engines like [Flat Red Ball](http://flatredball.com/)
The engine provides an interface to develop 2d top down games. Some notable features:
* Composite Animation Support (Including loading from XML files)
* Entity framework
* Input Extension Methods for quicker development
* Custom Game Shader interface for easy application of shaders to levels
* Support for loading [Tiled](http://www.mapeditor.org/) map files. (Current no zlib support however)

Some Future features that i aim to include:
* Automatic Asset management (Loading/Unloading content when switching between maps and levels)
* Animated Tiles
* AI framework for Entities

Game
----

Currently, any game design that is going on in this project is purely for testing and expanding upon the requirements of the
game engine. Once Game Engine development is close to completion i intend to try and perform some actual game design that
could demo the capabilities of the engine.

Assets
------

All my artistic Assets come from the fantastic website [OpenGameArt.org](http://opengameart.org/). In particular, i am making use of
Assets that were made for the [Liberated Pixel](http://lpc.opengameart.org/) Cup challenge that was being organised by the same site.

I may attempt to design/draw some pixel art myself in the future, but currently i have decided to focus on development 
rather than creative design.
