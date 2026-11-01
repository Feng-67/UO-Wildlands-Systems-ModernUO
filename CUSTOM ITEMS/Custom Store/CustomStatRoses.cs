/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 *
 * Uses the RoseInAVase graphic (0x0EB0) rather than the RoseOfTrinsic graphic.
 * BaseStatRose is self-contained and does not inherit from RoseOfTrinsic; the spawn-timer and petal-count
 * logic is reproduced here.
 */
using System;
using ModernUO.Serialization;
using Server.Network;

namespace Server.Items;

// ---------------------------------------------------------------------------
// PETALS
// ---------------------------------------------------------------------------

// The three petals differ only in stat, name and hue, so the eating logic is shared
internal static class StatPetalHelper
{
    public static void Eat(Mobile from, Item petal, StatType stat)
    {
        if (!petal.IsChildOf(from.Backpack))
        {
            from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
        }
        else if (from.GetStatMod("RoseOfTrinsicPetal") != null)
        {
            // You have eaten one of these recently and eating another would provide no benefit.
            from.SendLocalizedMessage(1062927);
        }
        else
        {
            from.PlaySound(0x1EE);
            from.AddStatMod(new StatMod(stat, "RoseOfTrinsicPetal", 5, TimeSpan.FromMinutes(5.0)));

            petal.Consume();
        }
    }
}

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class IntelligencePetal : RoseOfTrinsicPetal
{
    public override string DefaultName => "Petal of Intelligence";

    [Constructible]
    public IntelligencePetal() : base(1)
    {
        Hue = 2498;
    }

    public override void OnDoubleClick(Mobile from) => StatPetalHelper.Eat(from, this, StatType.Int);

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class DexterityPetal : RoseOfTrinsicPetal
{
    public override string DefaultName => "Petal of Dexterity";

    [Constructible]
    public DexterityPetal() : base(1)
    {
        Hue = 33;
    }

    public override void OnDoubleClick(Mobile from) => StatPetalHelper.Eat(from, this, StatType.Dex);

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class StrengthPetal : RoseOfTrinsicPetal
{
    public override string DefaultName => "Petal of Strength";

    [Constructible]
    public StrengthPetal() : base(1)
    {
        Hue = 1645;
    }

    public override void OnDoubleClick(Mobile from) => StatPetalHelper.Eat(from, this, StatType.Str);

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

// ---------------------------------------------------------------------------
// BASE ROSE
// ---------------------------------------------------------------------------

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public abstract partial class BaseStatRose : Item
{
    // One petal regenerates every 4 hours, matching the RoseOfTrinsic cadence
    private static readonly TimeSpan _spawnInterval = TimeSpan.FromHours(4.0);

    // Runtime only: restarted from NextSpawnTime after load, never saved
    private TimerExecutionToken _spawnTimerToken;

    [SerializableField(1, setter: "private")]
    private DateTime _nextSpawnTime;

    public override double DefaultWeight => 1.0;

    // Subclasses return the petal type that belongs to their stat
    public abstract Item GetNewPetal();

    protected BaseStatRose() : base(0x0EB0) // RoseInAVase graphic
    {
        LootType = LootType.Blessed;

        // Starts full; no timer is needed until petals are taken
        _petals = 10;
    }

    // The generator creates the _petals backing field from this property
    [SerializableProperty(0)]
    [CommandProperty(AccessLevel.GameMaster)]
    public int Petals
    {
        get => _petals;
        set
        {
            if (value >= 10)
            {
                _petals = 10;
                StopSpawnTimer();
            }
            else
            {
                _petals = value <= 0 ? 0 : value;
                StartSpawnTimer(_spawnInterval);
            }

            InvalidateProperties();
            this.MarkDirty();
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1062925, Petals); // Petals:  ~1_COUNT~
    }

    // Hands out all current petals as one stack
    public override void OnDoubleClick(Mobile from)
    {
        if (!from.InRange(GetWorldLocation(), 2))
        {
            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }
        else if (_petals > 0)
        {
            var petal = GetNewPetal();
            petal.Amount = _petals;
            from.AddToBackpack(petal);

            Petals = 0;
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0).
    private void Deserialize(IGenericReader reader, int version)
    {
        _petals = reader.ReadEncodedInt();
        _nextSpawnTime = reader.ReadDeltaTime();
    }

    // Resume the timer if the rose was not full when saved
    [AfterDeserialization]
    private void AfterDeserialization()
    {
        if (_petals < 10)
        {
            StartSpawnTimer(_nextSpawnTime - Core.Now);
        }
    }

    public override void OnAfterDelete()
    {
        base.OnAfterDelete();

        StopSpawnTimer();
    }

    private void StartSpawnTimer(TimeSpan delay)
    {
        StopSpawnTimer();

        // The server may have been offline past the due time
        if (delay < TimeSpan.Zero)
        {
            delay = TimeSpan.Zero;
        }

        Timer.StartTimer(delay, OnSpawnTick, out _spawnTimerToken);

        NextSpawnTime = Core.Now + delay;
    }

    private void StopSpawnTimer() => _spawnTimerToken.Cancel();

    private void OnSpawnTick()
    {
        if (Deleted)
        {
            return;
        }

        Petals++; // The setter restarts the timer while still below 10
    }
}

// ---------------------------------------------------------------------------
// CONCRETE ROSES: each sets its own name and hue; the graphic comes from BaseStatRose
// ---------------------------------------------------------------------------

[SerializationGenerator(1, false)]
public partial class RoseOfStrength : BaseStatRose
{
    public override string DefaultName => "Rose of Strength";

    [Constructible]
    public RoseOfStrength()
    {
        Hue = 1645;
    }

    public override Item GetNewPetal() => new StrengthPetal();

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class RoseOfIntelligence : BaseStatRose
{
    public override string DefaultName => "Rose of Intelligence";

    [Constructible]
    public RoseOfIntelligence()
    {
        Hue = 2498;
    }

    public override Item GetNewPetal() => new IntelligencePetal();

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}

[SerializationGenerator(1, false)]
public partial class RoseOfDexterity : BaseStatRose
{
    public override string DefaultName => "Rose of Dexterity";

    [Constructible]
    public RoseOfDexterity()
    {
        Hue = 33;
    }

    public override Item GetNewPetal() => new DexterityPetal();

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
