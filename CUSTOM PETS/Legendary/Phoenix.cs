/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;
using Server.Items;

namespace Server.Mobiles;

[SerializationGenerator(0, false)]
public partial class Phoenix : BaseCreature, ILegendaryPet
{
    [SerializableField(0, setter: "private")]
    private bool _isLegendary;

    [Constructible]
    public Phoenix() : base(AIType.AI_Mage, FightMode.Aggressor)
    {
        ActiveSpeed = 0.2;
        PassiveSpeed = 0.4;

        Body = 0x340;
        BaseSoundID = 0x8F;

        SetStr(504, 700);
        SetDex(202, 300);
        SetInt(504, 700);

        SetHits(340, 383);

        SetDamage(20, 25);

        SetDamageType(ResistanceType.Physical, 50);
        SetDamageType(ResistanceType.Fire, 50);

        SetResistance(ResistanceType.Physical, 45, 55);
        SetResistance(ResistanceType.Fire, 60, 70);
        SetResistance(ResistanceType.Poison, 25, 35);
        SetResistance(ResistanceType.Energy, 40, 50);

        SetSkill(SkillName.EvalInt, 90.2, 100.0);
        SetSkill(SkillName.Magery, 90.2, 100.0);
        SetSkill(SkillName.Meditation, 75.1, 100.0);
        SetSkill(SkillName.MagicResist, 86.0, 135.0);
        SetSkill(SkillName.Tactics, 80.1, 90.0);
        SetSkill(SkillName.Wrestling, 90.1, 100.0);
        SetSkill(SkillName.DetectHidden, 70.0, 80.0);

        Fame = 15000;
        Karma = 0;

        VirtualArmor = 60;

        Tamable = true;
        ControlSlots = 4;
        MinTameSkill = 102.0;
    }

    public override string CorpseName => "a phoenix corpse";
    public override string DefaultName => "a phoenix";

    public override bool CanAngerOnTame => true;
    public override int Meat => 1;
    public override MeatType MeatType => MeatType.Bird;
    public override int Feathers => 36;
    public override bool CanFly => true;

    public override void OnBeforeSpawn(Point3D location, Map map)
    {
        base.OnBeforeSpawn(location, map);

        // 15% chance a Legendary version spawns, equally split across 3 variants
        if (Utility.RandomDouble() < 0.15)
        {
            switch (Utility.Random(3))
            {
                case 0:
                    ConvertToLegendaryMagery();
                    break;
                case 1:
                    ConvertToLegendaryMysticism();
                    break;
                case 2:
                    ConvertToLegendarySpellweaving();
                    break;
            }
        }
    }

    public void ConvertToLegendaryMagery()
    {
        IsLegendary = true;
        Name = "a magery phoenix";
        Hue = 2753;

        ChangeAIType(AIType.AI_Mage);
        ControlSlots = 3;
        MinTameSkill = 120.0;

        SetSkill(SkillName.EvalInt, 200.0, 250.0);
        SetSkill(SkillName.Magery, 100.0, 100.0);

        var profile = PetTrainingHelper.GetAbilityProfile(this, create: true);
        profile.MagicalAbility = MagicalAbility.MageryMastery;

        ApplyLegendaryTrainingDefinition();
    }

    public void ConvertToLegendaryMysticism()
    {
        IsLegendary = true;
        Name = "a mysticism phoenix";
        Hue = 1995;

        ChangeAIType(AIType.AI_Mystic);
        ControlSlots = 3;
        MinTameSkill = 120.0;

        SetSkill(SkillName.Mysticism, 100.0, 100.0);
        SetSkill(SkillName.Focus, 150.0, 225.0);
        SetSkill(SkillName.Magery, 0, 0);
        SetSkill(SkillName.EvalInt, 0, 0);

        var profile = PetTrainingHelper.GetAbilityProfile(this, create: true);
        profile.MagicalAbility = MagicalAbility.Mysticism;

        ApplyLegendaryTrainingDefinition();
    }

    public void ConvertToLegendarySpellweaving()
    {
        IsLegendary = true;
        Name = "a spellweaving phoenix";
        Hue = 2953;

        ChangeAIType(AIType.AI_Spellweaving);
        ControlSlots = 3;
        MinTameSkill = 120.0;

        SetSkill(SkillName.Spellweaving, 225.0, 225.0);
        SetSkill(SkillName.Magery, 0, 0);
        SetSkill(SkillName.EvalInt, 0, 0);

        var profile = PetTrainingHelper.GetAbilityProfile(this, create: true);
        profile.MagicalAbility = MagicalAbility.Spellweaving;

        ApplyLegendaryTrainingDefinition();
    }

    // All three legendary variants share the same training definition.
    private void ApplyLegendaryTrainingDefinition()
    {
        TrainingDefinition = new TrainingDefinition(
            typeof(Phoenix),
            Class.MagicalAndClawed,
            MagicalAbility.Poisoning | MagicalAbility.Discordance | MagicalAbility.Chivalry,
            PetTrainingDefinitions.SpecialAbilityPhoenix,
            PetTrainingDefinitions.WepAbility1,
            PetTrainingDefinitions.AreaEffectArea2,
            3,
            5
        );
    }

    [AfterDeserialization]
    private void AfterDeserialization()
    {
        // TrainingDefinition isn't serialized; re-apply on load for legendaries.
        // MagicalAbility is persisted on the profile itself, so no reapply needed.
        if (_isLegendary)
        {
            ApplyLegendaryTrainingDefinition();
        }
    }

    public override void GenerateLoot()
    {
        AddLoot(LootPack.FilthyRich);
        AddLoot(LootPack.Rich);
    }

    public override void AddNameProperties(IPropertyList list)
    {
        base.AddNameProperties(list);

        if (_isLegendary)
        {
            list.Add("<BASEFONT COLOR=#FFD700>Legendary</BASEFONT>");
        }
    }
}
