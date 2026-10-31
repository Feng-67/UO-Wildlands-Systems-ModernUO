using ModernUO.Serialization;
using Server.Gumps;
using Server.Items;

namespace Server.Mobiles;

// TODO: ModernUO has no ICreatureStatuette interface or ConfirmMountStatuetteGump, so the redeem
// confirmation below is a small stand-in built on BaseConfirmGump.
[SerializationGenerator(0, false)]
public partial class TritonStatue : Item
{
    [Constructible]
    public TritonStatue() : base(0xA2D8) => Hue = 2713;

    public override int LabelNumber => 1158929; // Triton

    public override void OnDoubleClick(Mobile from)
    {
        if (!IsChildOf(from.Backpack))
        {
            SendLocalizedMessageTo(from, 1010095); // This must be on your person to use.
            return;
        }

        if (from.Skills.AnimalTaming.Value >= 100)
        {
            from.SendGump(new ConfirmTritonGump(this));
        }
        else
        {
            from.SendLocalizedMessage(1158959, "100"); // ~1_SKILL~ Animal Taming skill is required to redeem this pet.
        }
    }

    public override void GetProperties(IPropertyList list)
    {
        base.GetProperties(list);

        list.Add(1158954); // *Redeemable for a pet*<br>*Requires Grandmaster Taming to Claim Pet*
    }

    private class ConfirmTritonGump : BaseConfirmGump
    {
        private readonly TritonStatue _statue;

        public ConfirmTritonGump(TritonStatue statue) => _statue = statue;

        public override void Confirm(Mobile from)
        {
            if (_statue?.Deleted != false || !_statue.IsChildOf(from.Backpack))
            {
                return;
            }

            if (from.Skills.AnimalTaming.Value < 100)
            {
                from.SendLocalizedMessage(1158959, "100"); // ~1_SKILL~ Animal Taming skill is required to redeem this pet.
                return;
            }

            var triton = new Triton();

            if (from.Followers + triton.ControlSlots > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                triton.Delete();
                return;
            }

            if (!triton.SetControlMaster(from))
            {
                triton.Delete();
                return;
            }

            triton.MoveToWorld(from.Location, from.Map);
            _statue.Delete();
        }
    }
}

[SerializationGenerator(0, false)]
public partial class Triton : BaseCreature
{
    [Constructible]
    public Triton() : base(AIType.AI_Melee, FightMode.Closest)
    {
        Body = 0x2D0;

        var chance = Utility.RandomDouble() * 100;

        if (chance < 5) // 5% - Ultra Rare
        {
            Hue = 0x48E;
        }
        else if (chance < 20) // 15% - Very Rare
        {
            Hue = Utility.RandomList(0x48B, 0x497, 0x4B2, 0x4AB, 0x4B0);
        }
        else if (chance < 40) // 20% - Uncommon
        {
            Hue = Utility.RandomList(0x88C, 0x87E, 0x879, 0x895, 0x89E);
        }
        else // 60% - Common
        {
            Hue = 2713;
        }

        BaseSoundID = 0x5A;

        SetStr(103, 250);
        SetDex(151, 220);
        SetInt(101, 121);

        SetHits(651, 700);
        SetStam(150);

        SetDamage(13, 24);

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Energy, 50);

        SetResistance(ResistanceType.Physical, 45, 55);
        SetResistance(ResistanceType.Fire, 50, 60);
        SetResistance(ResistanceType.Cold, 45, 55);
        SetResistance(ResistanceType.Poison, 35, 45);
        SetResistance(ResistanceType.Energy, 85, 90);

        SetSkill(SkillName.MagicResist, 150.0, 190.0);
        SetSkill(SkillName.Tactics, 80.0, 95.0);
        SetSkill(SkillName.Wrestling, 110.0, 130.0);
        SetSkill(SkillName.Healing, 70.0, 99.0);
        SetSkill(SkillName.DetectHidden, 50.1);
        SetSkill(SkillName.Parry, 55.0, 70.0);

        Fame = 300;
        Karma = 300;

        Tamable = true;
        ControlSlots = 2;
        MinTameSkill = 96.0;

        PackItem(new Bandage(11));                
    }

    public override MonsterAbility[] GetMonsterAbilities() =>
    new MonsterAbility[] { SpecialAbility.Heal };
    public override string CorpseName => "a triton corpse";
    public override string DefaultName => "Triton";

    public override bool DeleteOnRelease => true;
    public override int Meat => 3;
    public override int Hides => 10;
    public override FoodType FavoriteFood => FoodType.Meat;
}
