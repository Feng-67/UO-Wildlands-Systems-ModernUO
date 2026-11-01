using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(0, false)]
public partial class FeudalCloakOfElements : FurCape
{
    public override int LabelNumber => 1161974; // Feudal Cloak of Elements
    public override int InitMinHits => 255;
    public override int InitMaxHits => 255;

    [Constructible]
    public FeudalCloakOfElements()
    {
        Hue = 2764;
        Weight = 4.0;
        Attributes.RegenHits = 2;
        Attributes.RegenStam = 3;
        Attributes.RegenMana = 2;
        Attributes.Luck = 150;

        
        switch (Utility.Random(5))
        {
            case 0: // Physical
                
                Resistances.Physical = 15;
                break;
            case 1: // Fire
                
                Resistances.Fire = 15;
                break;
            case 2: // Cold
                
                Resistances.Cold = 15;
                break;
            case 3: // Poison
                
                Resistances.Poison = 15;
                break;
            case 4: // Energy
                
                Resistances.Energy = 15;
                break;
        }
    }
}
