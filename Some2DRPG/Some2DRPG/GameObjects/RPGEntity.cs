using System;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Drawing;
using GameEngine.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Some2DRPG.GameObjects.Misc;
using Some2DRPG.Items;

namespace Some2DRPG.GameObjects
{
    public enum Direction { Left, Up, Right, Down };

    public enum AttackStance { NotAttacking, Preparing, Attacking };

    public class RPGEntity : CollidableEntity
    {
        public static Random RPGRandomGenerator = new Random();

        public static string HUMAN_MALE = @"Animations/Characters/male_npc.anim";
        public static string HUMAN_FEMALE = @"Animations/Characters/female_npc.anim";
        public static string CREATURES_BAT = @"Animations/Monsters/bat.anim";
        public static string CREATURES_DUMMY = @"Animations/Monsters/combat_dummy.anim";
        public static string CREATURES_SKELETON = @"Animations/Monsters/skeleton.anim";

        public string Faction { get; set; }

        public List<Item> Backpack { get; set; }
        public Dictionary<ItemType, Item> Equiped { get; set; }

        public Direction Direction { get; set; }

        public AttackStance AttackStance { get; set; }

        public string BaseRace { get; set; }

        /// <summary>
        /// The priority givent to attack this entity by other rivalling
        /// RPG Entities. The higher the value assigned, the more likely
        /// this Entity will be attacked first.
        /// </summary>
        public int AttackPriority { get; set; }

        #region Private/Internal members

        protected List<Entity> _hitEntityList = new List<Entity>();
        protected float _moveSpeed = 1.2f;

        #endregion

        #region Attributes

        public int HP { get; set; }
        public int MP { get; set; }
        public int XP { get; set; }
        public int Coins { get; set; }

        #endregion

        #region Skill Attributes

        public int Strength { get; set; }
        public int Constitution { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }
        public int Luck { get; set; }
        public int Perception { get; set; }

        #endregion

        public string MovePrefix { get; set; }
        public string IdlePrefix { get; set; }

        public RPGEntity()
        {
            Construct(0, 0, RPGEntity.HUMAN_MALE);
        }

        public RPGEntity(string baseRace)
        {
            Construct(0, 0, baseRace);
        }

        public RPGEntity(float x, float y, string baseRace)
        {
            Construct(x, y, baseRace);
        }

        private void Construct(float x, float y, string baseRace)
        {
            this.HP = 0;
            this.XP = 0;
            this.Coins = 0;
            this.Pos = new Vector2(x, y);
            this.ScaleX = 1.0f;
            this.ScaleY = 1.0f;
            this.CollisionGroup = null;
            this.Backpack = new List<Item>();
            this.Equiped = new Dictionary<ItemType, Item>();
            this.BaseRace = baseRace;

            this.AttackStance = AttackStance.NotAttacking;
            this.Direction = Direction.Right;
        }

        public override void LoadContent(ContentManager content)
        {
            DrawableSet.LoadDrawableSetXml(Drawables, BaseRace, content);

            CurrentDrawableState = "Idle_" + Direction;
        }

        #region Helper Methods

        public RPGEntity GetHighestPriorityTarget(GameTime gameTime, TeeEngine engine, float distance)
        {
            Rectangle agroRegion = new Rectangle(
                (int)Math.Floor(Pos.X - distance),
                (int)Math.Floor(Pos.Y - distance),
                (int)Math.Ceiling(distance * 2),
                (int)Math.Ceiling(distance * 2)
                );

            // DETECT NEARBY ENTITIES AND DETERMINE A TARGET.
            List<RPGEntity> nearbyEntities = engine.Collider.GetIntersectingEntities<RPGEntity>(agroRegion);
            float currDistance = float.MaxValue;
            int maxPriority = Int32.MinValue;
            RPGEntity resultTarget = null;

            foreach (RPGEntity entity in nearbyEntities)
            {
                if (entity.Faction != this.Faction && entity.HP > 0)
                {
                    float entityDistance = Vector2.Distance(entity.Pos, Pos);

                    if (entityDistance <= distance &&
                        (entity.AttackPriority > maxPriority ||
                        (entity.AttackPriority == maxPriority && distance < currDistance))
                        )
                    {
                        currDistance = distance;
                        maxPriority = entity.AttackPriority;
                        resultTarget = entity;
                    }
                }
            }

            return resultTarget;
        }

        public void Approach(Vector2 target)
        {
            Vector2 difference = target - this.Pos;
            difference.Normalize();

            this.Pos.X += _moveSpeed * difference.X;
            this.Pos.Y += _moveSpeed * difference.Y;
        }

        #endregion

        #region Interaction Methods

        public virtual bool IsAttacking(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual void OnAttack(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsFinishedAttacking(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method called when the RPG Entity has been interacted with through some medium by
        /// another Entity object residing within the same engine. Override this method in
        /// order to allow interactions to occur with this entity.
        /// </summary>
        public virtual void OnInteract(Entity sender, GameTime gameTime, TeeEngine engine)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method called when the RPG Entity has been hit by some Entity residing within the
        /// game engine. Override this method in order to perform custom functionality
        /// during a Hit event.
        /// </summary>
        public virtual void OnHit(Entity sender, int damageDealt, GameTime gameTime, TeeEngine engine)
        {
            HP -= damageDealt;

            BattleText text = new BattleText(damageDealt.ToString(), Color.Red);
            text.Pos = Pos;
            text.Pos.Y -= CurrentBoundingBox.Height;

            engine.AddEntity(text);
        }

        #endregion

        #region Equipment Methods

        public void QuickEquip(string itemName)
        {
            Equip(ItemRepository.GameItems[itemName]);
        }

        public void Equip(Item item)
        {
            Unequip(item.ItemType);

            Equiped[item.ItemType] = item;
            Drawables.Union(item.Drawables);
        }

        public void QuickUnequip(string itemName)
        {
            Unequip(ItemRepository.GameItems[itemName]);
        }

        public void Unequip(Item item)
        {
            Unequip(item.ItemType);
        }

        public void Unequip(ItemType itemType)
        {
            if( Equiped.ContainsKey(itemType) )
                Drawables.Remove(Equiped[itemType].Drawables);

            Equiped.Remove(itemType);
        }

        public bool QuickIsEquiped(string itemName)
        {
            return IsEquiped(ItemRepository.GameItems[itemName]);
        }

        public bool IsEquiped(Item item)
        {
            if (Equiped.ContainsKey(item.ItemType))
                return Equiped[item.ItemType] == item;
            else
                return false;
        }

        #endregion

        public override void Update(GameTime gameTime, TeeEngine engine)
        {
            if (IsAttacking(gameTime))
            {
                if (IsFinishedAttacking(gameTime))
                {
                    CurrentDrawableState = IdlePrefix + "_" + Direction;
                    _hitEntityList.Clear();
                }
                else
                {
                    List<RPGEntity> intersectingEntities = engine.Collider.GetIntersectingEntities<RPGEntity>(CurrentBoundingBox);
                    foreach (RPGEntity entity in intersectingEntities)
                    {
                        if (this != entity && !_hitEntityList.Contains(entity) && entity.Faction != this.Faction)
                        {
                            _hitEntityList.Add(entity);
                            entity.OnHit(this, RollForDamage(), gameTime, engine);
                        }
                    }
                }
            }
            else
            {
                if (prevPos != Pos)
                {
                    Vector2 difference = Pos - prevPos;
                    if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                        Direction = (difference.X > 0) ? Direction.Right : Direction.Left;
                    else
                        Direction = (difference.Y > 0) ? Direction.Down : Direction.Up;

                    CurrentDrawableState = MovePrefix + "_" + Direction;
                }
                else CurrentDrawableState = IdlePrefix + "_" + Direction;
            }
            
            base.Update(gameTime, engine);
        }

        /// <summary>
        /// Performs a "Roll" to see how much Damage this Entity will do based on the engines calculation
        /// of damage using factors such as strength, weapon damage and some elements of randomness.
        /// </summary>
        public int RollForDamage()
        {
            return Strength + Strength * RPGEntity.RPGRandomGenerator.Next(6) / 6;
        }

        public override string ToString()
        {
            return string.Format("RPGEntity: Faction={0}, HP={1}, Name={2}", Faction, HP, Name);
        }
    }
}
