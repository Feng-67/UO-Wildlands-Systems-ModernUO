/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using ModernUO.Serialization;

namespace Server.Items;

[SerializationGenerator(1, false)] // ServUO wrote version 0; generator version is old + 1
public partial class CitiesOfBritannia : RunicAtlas
{
    public override string DefaultName => "Cities of Britannia (T & F)";

    [Constructible]
    public CitiesOfBritannia()
    {
        Hue = 1154;

        MaxCharges = 100;
        CurCharges = 100;
        Quality = BookQuality.Exceptional;
        LootType = LootType.Blessed;

        // --- TRAMMEL CITIES ---
        AddCity("(T) Britain", new Point3D(1431, 1698, 10), Map.Trammel);
        AddCity("(T) Moonglow", new Point3D(4471, 1178, 0), Map.Trammel);
        AddCity("(T) Trinsic", new Point3D(1910, 2687, 0), Map.Trammel);
        AddCity("(T) Skara Brae", new Point3D(590, 2156, 0), Map.Trammel);
        AddCity("(T) Yew", new Point3D(636, 865, 0), Map.Trammel);
        AddCity("(T) Minoc", new Point3D(2512, 564, 0), Map.Trammel);
        AddCity("(T) Vesper", new Point3D(2891, 686, 0), Map.Trammel);
        AddCity("(T) Jhelom", new Point3D(1332, 3772, 0), Map.Trammel);
        AddCity("(T) Magincia", new Point3D(3795, 2252, 20), Map.Trammel);
        AddCity("(T) Nujel'm", new Point3D(3770, 1308, 0), Map.Trammel);
        AddCity("(T) Buccaneer's Den", new Point3D(2723, 2186, 0), Map.Trammel);
        AddCity("(T) Serpent's Hold", new Point3D(2882, 3469, 15), Map.Trammel);
        AddCity("(T) New Haven", new Point3D(3498, 2572, 14), Map.Trammel);

        // --- FELUCCA CITIES ---
        AddCity("(F) Britain", new Point3D(1431, 1698, 10), Map.Felucca);
        AddCity("(F) Moonglow", new Point3D(4471, 1178, 0), Map.Felucca);
        AddCity("(F) Trinsic", new Point3D(1910, 2687, 0), Map.Felucca);
        AddCity("(F) Skara Brae", new Point3D(590, 2156, 0), Map.Felucca);
        AddCity("(F) Yew", new Point3D(636, 865, 0), Map.Felucca);
        AddCity("(F) Minoc", new Point3D(2512, 564, 0), Map.Felucca);
        AddCity("(F) Vesper", new Point3D(2891, 686, 0), Map.Felucca);
        AddCity("(F) Jhelom", new Point3D(1332, 3772, 0), Map.Felucca);
        AddCity("(F) Magincia", new Point3D(3795, 2252, 20), Map.Felucca);
        AddCity("(F) Nujel'm", new Point3D(3770, 1308, 0), Map.Felucca);
        AddCity("(F) Buccaneer's Den", new Point3D(2723, 2186, 0), Map.Felucca);
        AddCity("(F) Serpent's Hold", new Point3D(2888, 3474, 15), Map.Felucca);

        // --- OTHER ---
        AddCity("Zento", new Point3D(739, 1255, 30), Map.Tokuno);
        AddCity("Umbra", new Point3D(1997, 1381, -90), Map.Malas);
        AddCity("Royal City", new Point3D(810, 3479, -20), Map.TerMur);
    }

    private void AddCity(string cityName, Point3D loc, Map map)
    {
        if (Entries.Count < MaxEntries)
        {
            Entries.Add(new RunebookEntry(this, loc, map, cityName));
            this.MarkDirty();
        }
    }

    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }
}
