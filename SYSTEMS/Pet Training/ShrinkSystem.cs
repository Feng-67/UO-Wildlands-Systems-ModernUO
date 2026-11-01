/**********************************
* Author: Expa                    *
* Script Name: ShrinkSystem.cs    *
* Optimized for ModernUO 0.15.4.18*
**********************************/
using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using ModernUO.Serialization;
using Server.Regions;

namespace Server.Items;

public interface IShrinkItem
{
    BaseCreature ShrunkenPet { get; }
}

public static class ShrinkConfig
{
    public static bool Enabled { get; set; } = true;
    public const int BottleItemID = 0x0F0E, BottleHue = 88, PetLeashItemID = 0x1374, PetLeashChargesDefault = 10, ShrinkItemID = 0x14EF;
    public const string BottleName = "a bottle of shrinking", PetLeashName = "a pet leash";
    
    public enum L : int
    {
        MustBeInBackpack = 1042001,
        ShrinkSystemEnabled = 1063320,
        ShrinkSystemDisabled = 1063321,
        ShrinkSystemOffline = 1063300,
        WhatToShrink = 1063301,
        CannotShrinkThat = 1063302,
        CannotShrinkInCombat = 1063303,
        CannotShrinkSummoned = 1063304,
        CannotShrinkPolymorphed = 1063305,
        PetIsDead = 1063306,
        NeedToBeCloser = 1063307,
        UnloadPack = 1063308,
        MustTame = 1063309,
        NotYourPet = 1063310,
        UnshrinkMessage = 1063311,
        TooManyFollowers = 1063312,
        LackSkill = 1063313,
        CannotClaimInCombat = 1063314
    }
}

public static class ShrinkSystem
{
    public static bool Enabled { get; set; } = ShrinkConfig.Enabled;
    
    public static void Initialize() => CommandSystem.Register("SSToggle", AccessLevel.Administrator, e =>
    {
        Enabled = !Enabled;
        e.Mobile.SendLocalizedMessage((int)(Enabled ? ShrinkConfig.L.ShrinkSystemEnabled : ShrinkConfig.L.ShrinkSystemDisabled));
    });
}

[SerializationGenerator(0)]
public partial class BaseShrinker : Item
{
    [SerializableField(0)]
    [SerializedCommandProperty(AccessLevel.GameMaster)]
    private int _charges;
    
    protected BaseShrinker(int itemID, int charges = 0) : base(itemID) => _charges = charges;
    
    public override void OnDoubleClick(Mobile from)
    {
        if (from.AccessLevel == AccessLevel.Player && !IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.MustBeInBackpack);
            return;
        }
        
        if (!ShrinkSystem.Enabled)
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.ShrinkSystemOffline);
            return;
        }
        
        from.Target = new ShrinkTarget(this);
        from.SendLocalizedMessage((int)ShrinkConfig.L.WhatToShrink);
    }
    
    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);
        if (!Stackable && _charges > 0)
            list.Add(1060584, _charges.ToString());
    }
    
    private class ShrinkTarget : Target
    {
        private readonly BaseShrinker _shrinker;
        
        public ShrinkTarget(BaseShrinker shrinker) : base(10, false, TargetFlags.None) => _shrinker = shrinker;
        
        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is not BaseCreature bc)
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.CannotShrinkThat);
                return;
            }
            
            if (SpellHelper.CheckCombat(from) || SpellHelper.CheckCombat(bc))
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.CannotShrinkInCombat);
                return;
            }
            
            if (bc.Summoned)
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.CannotShrinkSummoned);
                return;
            }
            
            if (bc.BodyMod != 0)
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.CannotShrinkPolymorphed);
                return;
            }
            
            if (bc.IsDeadPet)
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.PetIsDead);
                return;
            }
            
            if (from.AccessLevel == AccessLevel.Player && !from.InRange(bc, 5))
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.NeedToBeCloser);
                return;
            }
            
            if ((bc is PackLlama or PackHorse or Beetle) && bc.Backpack?.Items.Count > 0)
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.UnloadPack);
                return;
            }
            
            if (!bc.Controlled && from.AccessLevel < AccessLevel.Seer)
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.MustTame);
                return;
            }
            
            if (!((bc.Controlled && bc.ControlMaster == from) || from.AccessLevel >= AccessLevel.Seer))
            {
                from.SendLocalizedMessage((int)ShrinkConfig.L.NotYourPet);
                return;
            }
            
            Effects.SendMovingParticles(
                new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 50), from.Map),
                new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z), from.Map),
                ShrinkTable.Lookup(bc), 1, 0, true, false, 0, 3, 1153, 1, 0, EffectLayer.Head, 0x100
            );
            
            from.PlaySound(492);

            // Подготовка моба к internalize (следуем прежней логике, чтобы ShrinkItem хранил serial internalize'ного моба)
            bc.Controlled = true;
            bc.ControlMaster = null;
            bc.Internalize();
            bc.OwnerAbandonTime = DateTime.MinValue;
            bc.IsStabled = true;
            
            from.AddToBackpack(new ShrinkItem { Creature = bc, Owner = from, Hue = bc.Hue, ItemID = ShrinkTable.Lookup(bc) });
            
            if (_shrinker.Stackable)
            {
                if (_shrinker.Amount > 1)
                    --_shrinker.Amount;
                else
                    _shrinker.Delete();
            }
            else
            {
                if (--_shrinker._charges <= 0)
                    _shrinker.Delete();
            }
        }
    }
}

[SerializationGenerator(0)]
public partial class BottleOfShrink : BaseShrinker
{
    [Constructible]
    public BottleOfShrink() : this(1) => Name = ShrinkConfig.BottleName;
    
    [Constructible]
    public BottleOfShrink(int amount) : base(ShrinkConfig.BottleItemID, 0)
    {
        Name = ShrinkConfig.BottleName;
        Hue = ShrinkConfig.BottleHue;
        Weight = 1.0;
        Stackable = true;
        Amount = Math.Max(1, amount);
    }
}

[SerializationGenerator(0)]
public partial class PetLeash : BaseShrinker
{
    [Constructible]
    public PetLeash() : this(ShrinkConfig.PetLeashChargesDefault) => Name = ShrinkConfig.PetLeashName;
    
    [Constructible]
    public PetLeash(int charges) : base(ShrinkConfig.PetLeashItemID, Math.Max(1, charges)) => Name = ShrinkConfig.PetLeashName;
}

[SerializationGenerator(0)]
public partial class ShrinkItem : Item
{
    public BaseCreature ShrunkenPet => Creature as BaseCreature;

    [SerializableField(0)]
    private Serial _creatureSerial;
    
    [SerializableField(1)]
    private Serial _ownerSerial;
    
    public Mobile Creature
    {
        get => World.FindMobile(_creatureSerial);
        set => _creatureSerial = value?.Serial ?? Serial.Zero;
    }
    
    public Mobile Owner
    {
        get => World.FindMobile(_ownerSerial);
        set => _ownerSerial = value?.Serial ?? Serial.Zero;
    }
    
    [Constructible]
    public ShrinkItem() : base(ShrinkConfig.ShrinkItemID)
    {
        Name = "a shrunken pet";
        LootType = LootType.Blessed;
        Weight = 25.0;
    }
    
    public override void OnDoubleClick(Mobile from)
    {
        if (from.AccessLevel == AccessLevel.Player && !IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.MustBeInBackpack);
            return;
        }
        
        if (Creature == null)
        {
            Delete();
            from.SendLocalizedMessage((int)ShrinkConfig.L.CannotShrinkThat);
            return;
        }
        
        if (!ShrinkSystem.Enabled)
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.ShrinkSystemOffline);
            return;
        }
        
        if (from.AccessLevel == AccessLevel.Player && Owner != from)
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.NotYourPet);
            return;
        }
        
        if (Creature is not BaseCreature bc)
        {
            Delete();
            return;
        }
        
        if (bc is BaseMount && IsInDungeonRegion(from, from.Location))
        {
            from.SendMessage(33, "You cannot unpack your mount while inside a dungeon.");
            return;
        }
        
        if (from.AccessLevel == AccessLevel.Player && from.Skills.AnimalTaming.Fixed < bc.MinTameSkill)
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.LackSkill);
            return;
        }
        
        if (from.AccessLevel == AccessLevel.Player && (bc.ControlSlots + from.Followers) > from.FollowersMax)
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.TooManyFollowers);
            return;
        }
        
        if (SpellHelper.CheckCombat(from))
        {
            from.SendLocalizedMessage((int)ShrinkConfig.L.CannotClaimInCombat);
            return;
        }
        
        Effects.SendMovingParticles(
            new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z), from.Map),
            new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 50), from.Map),
            ShrinkTable.Lookup(Creature), 1, 0, true, false, 0, 3, 1153, 1, 0, EffectLayer.Head, 0x100
        );
        
        from.PlaySound(492);
        bc.ControlMaster = from;
        bc.ControlTarget = from;
        bc.ControlOrder = OrderType.Follow;
        bc.Direction = Direction.South;
        bc.MoveToWorld(from.Location, from.Map);
        from.SendLocalizedMessage((int)ShrinkConfig.L.UnshrinkMessage);
        Delete();
    }
    
    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);
        
        if (Creature is not BaseCreature bc)
            return;
        
        var type = bc.GetType().Name;
        var breed = type;
        
        for (int i = 1; i < type.Length; i++)
        {
            if (char.IsUpper(type[i]))
            {
                breed = $"{type[..i]} {type[i..]}";
                break;
            }
        }
        
        list.Add($"Breed: {breed} {(bc.Female ? "Female" : "Male")}");
        list.Add(1060660, $"Stats\tStr <BASEFONT COLOR=#FF0000>{bc.RawStr}</BASEFONT> | Dex <BASEFONT COLOR=#00FF00>{bc.RawDex}</BASEFONT> | Int <BASEFONT COLOR=#0000FF>{bc.RawInt}</BASEFONT>");
        list.Add(1060660, $"Pool\tHits <BASEFONT COLOR=#FF0000>{bc.Hits}/{bc.HitsMax}</BASEFONT> | Stam <BASEFONT COLOR=#00FF00>{bc.Stam}/{bc.StamMax}</BASEFONT> | Mana <BASEFONT COLOR=#0000FF>{bc.Mana}/{bc.ManaMax}</BASEFONT>");
        
        foreach (var skill in bc.Skills)
            if (skill.BaseFixedPoint > 0)
                list.Add(1060659, $"{skill.Info.Name}\t{skill.BaseFixedPoint / 10.0:F1}");
    }
    
    [AfterDeserialization]
    private void AfterDeserialization()
    {
        if (Creature == null && _creatureSerial.IsValid)
            Delete();
    }
    
    private static bool IsInDungeonRegion(Mobile m, Point3D loc)
    {
        if (m?.Map == null)
            return false;
        
        for (var r = Region.Find(loc, m.Map); r != null; r = r.Parent)
            if (r is DungeonRegion)
                return true;
        
        return false;
    }
}
