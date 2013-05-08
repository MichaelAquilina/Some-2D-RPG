using Microsoft.Xna.Framework;

namespace GameEngine.GameObjects
{
    /// <summary>
    /// Represents a Static object on the map that is not associated with any particular Entity
    /// subclass. By wrapping the Entity in this StaticEntity class, the user may specify Update,
    /// PreCreate, PreDestroy, PostCreate and PostDestroy routines to run within the tile editor 
    /// through the use of event hooks.
    /// </summary>
    public class StaticEntity : Entity
    {
        public delegate void EntityEventHandler(Entity caller, GameTime gameTime, TeeEngine engine);

        public event EntityEventHandler UpdateEvent;
        public event EntityEventHandler PreCreateEvent;
        public event EntityEventHandler PostCreateEvent;
        public event EntityEventHandler PreDestroyEvent;
        public event EntityEventHandler PostDestroyEvent;

        public override bool PreCreate(GameTime gameTime, TeeEngine engine)
        {
            if (PreCreateEvent != null)
                PreCreateEvent(this, gameTime, engine);

            return true;
        }

        public override void PostCreate(GameTime gameTime, TeeEngine engine)
        {
            if (PostCreateEvent != null)
                PostCreateEvent(this, gameTime, engine);
        }

        public override bool PreDestroy(GameTime gameTime, TeeEngine engine)
        {
            if (PreDestroyEvent != null)
                PreDestroyEvent(this, gameTime, engine);

            return true;
        }

        public override void PostDestroy(GameTime gameTime, TeeEngine engine)
        {
            if (PostDestroyEvent != null)
                PostDestroyEvent(this, gameTime, engine);
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (UpdateEvent != null)
                UpdateEvent(this, gameTime, engine);
        }
    }
}
