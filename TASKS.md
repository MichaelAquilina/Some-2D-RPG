Game Engine Tasks
=================

Todo
----

- [] - Change *KeyboardExtensions* class to make use of a Dictionary&lt;string, HashSet<<Keys>>>> which should be much faster and scalable.
- [] - Iplement extended IntersetsWith function. IntersectsWith(Entity entity, GameTime gameTime, string thisGroup=null, string entityGroup=null)
- [] - Investigate what is 'slow' in entity updates and updating the bounding box. Is the QuadTree being more ineffecient than effecient?
- [IP] - AI for bats. Use A* for path finding techniques
- [] - better entity integration in map loading
- [] - Entities at certain zoom levels show pixels from outside their source frame (example Bat and Tree2)
- [] - Might be smarter to convert floats to integers * 100 for faster performance
- [] - Give a better look to QuadTree validate code to see if we can reduce the amount of times we remove and add QuadTreeNodes

Must Have
---------

- [Done] - The ability to draw and render a 2D top down game world
- [Done] - an entity framework
- [IP] - an animation framework
- [Done] - a post-rendering shader framework
- [Done] - the ability to load from tiled maps (prevents from having to develop a map creation application)

Should Have
-----------
- [] - ICollision interface should be implemented and used by the QuadTreeNode to allow extensions to be added.
- [Done] - the ability to zoom in and out (scale)
- [Done] - an effecient collision detection engine (QuadTree)
- [] - a per entity / per tile shader framework
- [] - an application to be able to design animations and speficy frames (however try look into alternatives such as texture compressor)
  - try out QT in the case where an application will be designed from scratch
- [] - an Asset management system for loading and unloading items in and out of memory
- [] - Map scripting in the form of python, lua or external C#. Need to do research to see which one is the best to use. Make sure to consider **performance**, **ease of use**, **support** and **stability of package**.
       - example: http://www.gamedev.net/page/resources/_/technical/game-programming/using-lua-with-c-r2275 *LUA*
       - example: http://mail.python.org/pipermail/pythondotnet/2003-November/000037.html *PYTHON*
       - example: http://stackoverflow.com/questions/826398/is-it-possible-to-dynamically-compile-and-execute-c-sharp-code-fragments *C#*
- [] - Animated tile support
- [Done] - Composite drawing support in entities (allowing entities to make use of more than one animation at one go)
- [Done] - Detailed diagnostic information in terms of performance counters
- [Done] - the ability to reset animations

Nice to Have
------------

- [] - zlib compression support in tmx files
- [] - the ability to pause and resume the state of the world from within the engine itself
- [] - the ability to run on MonoGame
