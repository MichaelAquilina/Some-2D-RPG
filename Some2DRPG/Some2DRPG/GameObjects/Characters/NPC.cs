using System.Collections.Generic;
using GameEngine;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;

namespace Some2DRPG.GameObjects.Characters
{
    // Could possibly be renamed to AiRpgEntity if is generic enough.
    public class NPC : RPGEntity
    {
        double _lastAttack = 0;
        List<Entity> _hitEntityList = new List<Entity>();

        public NPC()
        {
            this.Strength = 5;
        }

        public virtual void Attack(GameTime gameTime)
        {
            CurrentDrawableState = "Slash_" + Direction;
            Drawables.ResetState(CurrentDrawableState, gameTime);
        }

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (CurrentDrawableState.Contains("Slash"))
            {
                if (Drawables.IsStateFinished(CurrentDrawableState, gameTime))
                {
                    CurrentDrawableState = "Idle_" + Direction;

                    _lastAttack = gameTime.TotalGameTime.TotalMilliseconds;
                    _hitEntityList.Clear();
                }
                else
                {
                    List<Entity> intersectingEntities = engine.Collider.GetIntersectingEntites(CurrentBoundingBox);
                    foreach (Entity entity in intersectingEntities)
                    {
                        if (this != entity && entity is RPGEntity && !_hitEntityList.Contains(entity))
                        {
                            RPGEntity rpgEntity = (RPGEntity)entity;
                            if (rpgEntity.Faction != this.Faction)
                            {
                                _hitEntityList.Add(rpgEntity);

                                rpgEntity.OnHit(this, RollForDamage(), gameTime, engine);
                            }
                        }
                    }
                }
            }
            else if(gameTime.TotalGameTime.TotalMilliseconds - _lastAttack > 2000)
            {
                Attack(gameTime);
            }

            base.Update(gameTime, engine);
        }
    }
}
