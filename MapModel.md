*This document is a big Work In Progress. Please give me some time to flesh out the details.*

Map Controller Design
=====================

Each map should have numerous levels of logic associated with it. These are commonly referred to as Map Scripts, and produce a level of interactivity that could provide the player with a engaging experience. Because the maps that are loaded from Tee are from the generic map editor Tiled, we need some way of specifying what code should be run for a map. 

Typically, on web pages that are produced dynamically by server side code - the concept of a View and a Model are used. The view would specify what should be visually produced (i.e. rendered) and the model would specify what logic should be present on the page to make the page seem interactive. We can adopt this mechanism in the same way for maps where the tiled map .tmx files would be the views and classes in the game project could be the models/controllers.

Specifying a Model
------------------
In order to allow map to have logic, a tiled map should specify the *Model* property which specifies the name of the class that will handle all its logic. This class should inherit from the *MapModel* abstract class which specifies numerous virtual methods which should be overriden.

public virtual void MapLoaded(TeeEngine engine, TiledMap map);

public virtual void Update(TeeEngine engine, GameTime gameTime);

public virtual void MapUnloading(TeeEngine engine, TiledMap map);