using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class SterlingSilverRing : SilverRing
{
    public static readonly SkillName[] Skills =
    {
        SkillName.Archery,
        SkillName.Fencing,
        SkillName.Macing,
        SkillName.Swords,
        SkillName.Throwing,
        SkillName.Wrestling
    };

    [Constructible]
    public SterlingSilverRing()
    {
        Attributes.RegenHits = 3;
        Attributes.RegenMana = 5;
        Attributes.WeaponDamage = 75;
    }

    public override int LabelNumber => 1155606; // Stirling Silver Ring

    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    public bool HasSkillBonus => SkillBonuses.Skill_1_Value != 0;

    // TODO: ApplySkillBonusGump is not in ModernUO. Until it is ported, the skill bonus can't be chosen in game
    // and the "Double Click to Set Skill Bonus" tooltip line is left out so players aren't told to do something
    // that does nothing. Restore both when the gump exists:
    //
    // public override void OnDoubleClick(Mobile m)
    // {
    //     if (IsChildOf(m.Backpack) && m is PlayerMobile pm && !HasSkillBonus)
    //     {
    //         ApplySkillBonusGump.DisplayTo(pm, SkillBonuses, Skills, 20, 0);
    //     }
    //     else
    //     {
    //         base.OnDoubleClick(m);
    //     }
    // }
    //
    // public override void GetProperties(IPropertyList list)
    // {
    //     base.GetProperties(list);
    //
    //     if (!HasSkillBonus)
    //     {
    //         list.Add(1155609); // Double Click to Set Skill Bonus
    //     }
    // }
}
