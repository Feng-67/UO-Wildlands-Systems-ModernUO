/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core and Community scripts (Original Authors Unknown)
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System; 
using Server; 
using ModernUO.Serialization;

//--Magery Full Spellbook Start-----------------------------------------------------------

namespace Server.Items 
{ 
   [SerializationGenerator(1, false)]
   public partial class FullMagerySpellbook : Spellbook
   { 

      [Constructible] 
      public FullMagerySpellbook()
      {
          
            this.Content = ulong.MaxValue; 
      } 

       private void Deserialize(IGenericReader reader, int version)
       {
       }
} 
}

//--Full Necromancer Spellbook Start--------------------------------------------------------

namespace Server.Items
{
[SerializationGenerator(1, false)]
	public partial class FullNecroSpellbook : Spellbook
	{
		public override SpellbookType SpellbookType => SpellbookType.Necromancer;
		public override int BookOffset => 100;
		public override int BookCount => ((Core.SE) ? 17 : 17);
      
        [Constructible]
        public FullNecroSpellbook() : this((ulong)0xFFFF)
        {
            this.Content = (ulong)0xFFFF;
        }
       	[Constructible]
		public FullNecroSpellbook( ulong content ) : base( content, 0x2253 )
		{
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
		}
    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }

	}
}

//--Full Chivalry Spellbook Start--------------------------------------------------------

namespace Server.Items
{
[SerializationGenerator(1, false)]
    public partial class FullChivalrySpellbook : Spellbook
    {
        public override SpellbookType SpellbookType => SpellbookType.Paladin;
        public override int BookOffset => 200;
        public override int BookCount => 10;
        
        [Constructible]
        public FullChivalrySpellbook() : this((ulong)0x3FF)
        {
            this.Content = (ulong)0x3FF;
        }
        [Constructible]
        public FullChivalrySpellbook(ulong content) : base(content, 0x2252)
        {
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
        }
    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }

    }
}

//--Full Bushido Spellbook Start--------------------------------------------------------

namespace Server.Items
{
[SerializationGenerator(1, false)]
    public partial class FullBushidoSpellbook : Spellbook
    {
        public override SpellbookType SpellbookType => SpellbookType.Samurai;
        public override int BookOffset => 400;
        public override int BookCount => 6;

        [Constructible]
        public FullBushidoSpellbook()  : this((ulong)0x3F)
        {
            this.Content = (ulong)0x3F;
        }
        [Constructible]
        public FullBushidoSpellbook(ulong content) : base(content, 0x238C)
        {
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
        }
    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }

    }
}

//--Full Ninjitsu Spellbook Start--------------------------------------------------------

namespace Server.Items
{
[SerializationGenerator(1, false)]
    public partial class FullNinjitsuSpellbook : Spellbook
    {
        public override SpellbookType SpellbookType => SpellbookType.Ninja;
        public override int BookOffset => 500;
        public override int BookCount => 8;

        [Constructible]
        public FullNinjitsuSpellbook(): this((ulong)0xFF)
        {
            this.Content = (ulong)0xFF;
        }
        [Constructible]
        public FullNinjitsuSpellbook(ulong content)
            : base(content, 0x23A0)
        {
            Layer = (Core.ML ? Layer.OneHanded : Layer.Invalid);
        }
    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }

    }
}

//--Full Spellweaving Spellbook Start--------------------------------------------------------

namespace Server.Items
{
[SerializationGenerator(1, false)]
    public partial class FullSpellweavingSpellbook : Spellbook
    {
        public override SpellbookType SpellbookType => SpellbookType.Arcanist;
        public override int BookOffset => 600;
        public override int BookCount => 16;

        [Constructible]
        public FullSpellweavingSpellbook() : this((ulong)0)
        {
            this.Content = (ulong)0xFFFF;
        }
        [Constructible]
        public FullSpellweavingSpellbook(ulong content) : base(content, 0x2D50)
        {
            Hue = 0x8A2;

            Layer = Layer.OneHanded;
        }
    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }

    }
}
//--Full Mystic Spell Book ((This part done by Nylo))----------------------------------------
namespace Server.Items
{
[SerializationGenerator(1, false)]
    public partial class FullMysticBook : Spellbook
    {
        public override SpellbookType SpellbookType => SpellbookType.Mystic;
        public override int BookOffset => 677;
        public override int BookCount => 16;

        [Constructible]
        public FullMysticBook() : this((ulong)0)
        {
            this.Content = (ulong)0xFFFF;
        }
        [Constructible]
        public FullMysticBook(ulong content) : base(content, 0x2D9D)
        {
            Layer = Layer.OneHanded;
        }
    // Legacy hook: loads saves written by the ServUO version (version 0 held no payload).
    private void Deserialize(IGenericReader reader, int version)
    {
    }

    }
}
