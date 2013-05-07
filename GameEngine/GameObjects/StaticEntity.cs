using Microsoft.Xna.Framework;

namespace GameEngine.GameObjects
{
    /// <summary>
    /// Represents a Static object on the map that is not associated with any particular Entity
    /// subclass. By wrapping the Entity in this StaticEntity class, the user may specify Update
    /// routines to run within the tile editor through the use of event hooks.
    /// </summary>
    public class StaticEntity : Entity
    {
        public delegate void UpdateEventHandler(Entity caller, GameTime gameTime, TeeEngine engine);

        public event UpdateEventHandler UpdateEvent;

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (UpdateEvent != null)
                UpdateEvent(this, gameTime, engine);
        }
    }
}
