using System;
using ModernUO.Serialization;
using Server.Items;
using Server.Spells;
using Server.Engines.Pathing;

namespace Server.Mobiles;

[CorpseName("a tyrannosaurus rex corpse")]
[SerializationGenerator(0, false)]
public partial class TRex : BaseCreature
{
    private DateTime _nextFreeze;

    [Constructible]
    public TRex() : base(AIType.AI_Melee, FightMode.Weakest)
    {
        Name = "Tyrannosaurus Rex";
        Body = 1400;
        BaseSoundID = 362;

        SetStr(500, 700);
        SetDex(500, 700);
        SetInt(100, 180);

        SetHits(15000);

        SetDamage(33, 55);

        SetDamageType(ResistanceType.Physical, 100);

        SetResistance(ResistanceType.Physical, 80, 90);
        SetResistance(ResistanceType.Fire, 60, 80);
        SetResistance(ResistanceType.Cold, 60, 70);
        SetResistance(ResistanceType.Poison, 80, 100);
        SetResistance(ResistanceType.Energy, 60, 70);

        SetSkill(SkillName.Anatomy, 100.0);
        SetSkill(SkillName.MagicResist, 140.0, 150.0);
        SetSkill(SkillName.Tactics, 110.0, 130.0);
        SetSkill(SkillName.Wrestling, 130.0, 150.0);
        SetSkill(SkillName.Poisoning, 60.0, 70.0);
        SetSkill(SkillName.Parry, 100);

        Fame = 24000;
        Karma = -24000;

        _nextFreeze = Core.Now;

        CanSwim = true;
    }

    // ModernUO: SetWeaponAbility → GetWeaponAbility
    public override WeaponAbility GetWeaponAbility() => WeaponAbility.ArmorIgnore;

    // ModernUO: SetSpecialAbility → GetMonsterAbilities
    private static readonly MonsterAbility[] _abilities = { SpecialAbility.TailSwipe };

    public override MonsterAbility[] GetMonsterAbilities() => _abilities;

    public override bool AutoDispel => true;
    public override Poison PoisonImmune => Poison.Lethal;    
    public override bool ReacquireOnMovement => true;
    public override bool CanFlee => false;
    public override int TreasureMapLevel => 7;

    public override void OnThink()
    {
        base.OnThink();

        if (Combatant == null)
        {
            return;
        }

        if (_nextFreeze < Core.Now)
        {
            DoFreeze();
        }
    }

    public void DoFreeze()
    {
        DoEffects(Direction.North);
        DoEffects(Direction.West);
        DoEffects(Direction.South);
        DoEffects(Direction.East);

        _nextFreeze = Core.Now + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 60));
    }

    private bool DoEffects(Direction d)
    {
        var x = X;
        var y = Y;
        var z = Z;
        const int range = 10;
        const int offset = 8;

        switch (d)
        {
            case Direction.North:
                x = X + Utility.RandomMinMax(-offset, offset);
                y = Y - range;
                break;
            case Direction.West:
                x = X - range;
                y = Y + Utility.RandomMinMax(-offset, offset);
                break;
            case Direction.South:
                x = X + Utility.RandomMinMax(-offset, offset);
                y = Y + range;
                break;
            case Direction.East:
                x = X + range;
                y = Y + Utility.RandomMinMax(-offset, offset);
                break;
        }

        for (var i = 0; i < range; i++)
        {
            switch (d)
            {
                case Direction.North: y += i; break;
                case Direction.West: x += i; break;
                case Direction.South: y -= i; break;
                case Direction.East: x -= i; break;
            }

            z = Map.GetAverageZ(x, y);
            var p = new Point3D(x, y, z);

            if (SpellHelper.AdjustField(ref p, Map, 12, false))
            {
                var path = new MovementPath(this, p);

                if (path.Success)
                {
                    DropCrack(path);
                    return true;
                }
            }
        }

        return false;
    }

    private void DropCrack(MovementPath path)
    {
        var time = 10;
        var x = X;
        var y = Y;

        for (var i = 0; i < path.Directions.Length; ++i)
        {
            Server.Movement.Movement.Offset(path.Directions[i], ref x, ref y);
            IPoint3D p = new Point3D(x, y, Map.GetAverageZ(x, y));

            Timer.DelayCall(TimeSpan.FromMilliseconds(time), ManaDrainEffects_Callback, p, Map);

            time += 200;
        }
    }

    private void ManaDrainEffects_Callback(IPoint3D p, Map map)
    {
        var item = new FreezeItem(Utility.RandomList(6913, 6915, 6917, 6919), this);
        SpellHelper.GetSurfaceTop(ref p);

        item.MoveToWorld(new Point3D(p), map);
    }

    [SerializationGenerator(0, false)]
    private partial class FreezeItem : Item
    {
        public Item Static { get; private set; }
        public BaseCreature Owner { get; private set; }

        public FreezeItem(int id, BaseCreature owner) : base(id)
        {
            Owner = owner;

            Movable = false;
            Hue = 1152;
            Timer.DelayCall(TimeSpan.FromSeconds(5), ChangeHue);
        }

        private void ChangeHue()
        {
            Hue = 1153;

            if (Static != null)
            {
                Static.Hue = 1153;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), Delete);
        }

        public override void Delete()
        {
            if (Static != null)
            {
                Static.Delete();
            }

            Static = null;
            base.Delete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            Static = new Static(ItemID + 1);
            Static.MoveToWorld(Location, Map);

            foreach (var m in Map.GetMobilesInRange(Location, 0))
            {
                OnMoveOver(m);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Owner == null)
            {
                return true;
            }

            if ((m is PlayerMobile || m is BaseCreature bc && bc.GetMaster() is PlayerMobile) && m.CanBeHarmful(Owner, false))
            {
                m.Freeze(TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10)));

                if (Owner.CanBeHarmful(m))
                {
                    m.AggressiveAction(Owner);
                    Owner.Combatant = m;
                }
            }

            return true;
        }

        [AfterDeserialization]
        private void AfterDeserialization()
        {
            Timer.StartTimer(Delete);
        }
    }

    public override void GenerateLoot()
    {
        AddLoot(LootPack.SuperBoss, 2);
    }
}
