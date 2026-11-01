namespace Server.Items;

public interface ICombatEquipment
{
    ItemPower ItemPower { get; set; }
}

// Reforging state is intentionally not serialized in this port: these properties are runtime-only,
// so a reforged item keeps its prefix, suffix and power only until the next server restart.
// Persisting them would need a serialized field on each of these serialization-generated classes
// plus a version bump.
public abstract partial class BaseWeapon : ICombatEquipment
{
    public ItemPower ItemPower { get; set; }

    public ReforgedPrefix ReforgedPrefix { get; set; }

    public ReforgedSuffix ReforgedSuffix { get; set; }
}

// BaseShield derives from BaseArmor, so it inherits the reforging state and must not redeclare it.
public abstract partial class BaseArmor : ICombatEquipment
{
    public ItemPower ItemPower { get; set; }

    public ReforgedPrefix ReforgedPrefix { get; set; }

    public ReforgedSuffix ReforgedSuffix { get; set; }
}

public abstract partial class BaseJewel : ICombatEquipment
{
    public ItemPower ItemPower { get; set; }

    public ReforgedPrefix ReforgedPrefix { get; set; }

    public ReforgedSuffix ReforgedSuffix { get; set; }
}

public abstract partial class BaseClothing : ICombatEquipment
{
    public ItemPower ItemPower { get; set; }

    public ReforgedPrefix ReforgedPrefix { get; set; }

    public ReforgedSuffix ReforgedSuffix { get; set; }
}
