/********************************************
 * Talow's Stairs Addon                     *
 * version 1.0                              *
 *                                          *
 * Optimized for ModernUO in June 4th, 2024 *
 * By Nerun                                 *
 ********************************************/

using ModernUO.Serialization;
using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Commands;

namespace Server.Gumps
{
    public class AddStairsGump : Gump
    {
        private int m_Type;
        
        public void AddBlueBack(int width, int height)
        {
            AddBackground(0, 0, width - 00, height - 00, 0xE10);
            AddBackground(8, 5, width - 16, height - 11, 0x053);
            AddImageTiled(15, 14, width - 29, height - 29, 0xE14);
            AddAlphaRegion(15, 14, width - 29, height - 29);
        }
        
        public AddStairsGump() : this(-1) { }

        public AddStairsGump(int type) : this(type, 4) { }
        public AddStairsGump(int type, int level) : base(50, 40)
        {
            m_Type = type;

            AddPage(0);

            if (m_Type >= 0 && m_Type < m_Types.Length)
            {
                AddBlueBack(44 + (49 * 4), 150);
                
                if (m_Types[m_Type].m_Type == typeof(StoneStairs))
                {
                    AddItem(22, 28, 0x071F);
                    AddItem(22 + 49, 28, 0x0736);
                    AddItem(22 + (2 * 49), 28, 0x0737);
                    AddItem(22 + (3 * 49), 28, 0x0749);
                }
                else
                {
                    int baseID = m_Types[m_Type].m_BaseID;
                    
                    for (int i = 0; i < 4; ++i)
                    {
                        AddItem(22 + (i * 49), 28, baseID + i + 1);
                    }
                }
                for (int i = 0; i < 4; ++i)
                {
                    AddButton(35 + (i * 49), 13, 0x2624, 0x2625, i + 1, GumpButtonType.Reply, 0);
                }
                
                AddLabel(50, 100, 0x480, "Levels:");
                AddBackground(98, 98, 54, 24, 0x2486);
                AddTextEntry(102, 102, 50, 16, 0, 0, String.Format("{0}", level));
            }
            else
            {
                AddBlueBack(44 + (49 * m_Types.Length), 110);

                for (int i = 0; i < m_Types.Length; ++i)
                {
                    AddButton(35 + (i * 49), 13, 0x2624, 0x2625, i + 1, GumpButtonType.Reply, 0);
                    AddItem(22 + (i * 49), 30, m_Types[i].m_BaseID);
                }
            }
        }
        
        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int button = info.ButtonID - 1;

            if (m_Type == -1)
            {
                if (button >= 0 && button < m_Types.Length)
                    from.SendGump(new AddStairsGump(button));
            }
            else
            {
                if (button >= 0 && button < 4)
                {
                    var levelEntry = info.GetTextEntry(0);
                    string slevel = levelEntry?.Trim();
                    int level = 0;
                    
                    if (!string.IsNullOrEmpty(slevel))
                        int.TryParse(slevel, out level);
                    
                    if (level < 1) level = 1;
                    if (level > 20) level = 20;
                    
                    from.SendGump(new AddStairsGump(m_Type, level));
                    CommandSystem.Handle(from, String.Format("{0}Add {1} {2} {3}", CommandSystem.Prefix,
                                         m_Types[m_Type].m_Type.Name, (Facing)button, level));
                }
                else
                {
                    from.SendGump(new AddStairsGump());
                }
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("AddStairs", AccessLevel.GameMaster, new CommandEventHandler(AddStairs_OnCommand));
        }

        [Usage("AddStairs")]
        [Description("Displays a menu from which you can interactively add stairs.")]
        public static void AddStairs_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new AddStairsGump());
        }

        public static StairInfo[] m_Types = new StairInfo[]
        {
            new StairInfo(typeof(WornSandStoneStairs), 0x03EE),
            new StairInfo(typeof(MarbleStairs), 0x0709),
            new StairInfo(typeof(StoneStairs), 0x071E),
            new StairInfo(typeof(LightWoodStairs), 0x0721),
            new StairInfo(typeof(WoodStairs), 0x0738),
            new StairInfo(typeof(LightStoneStairs), 0x0750),
            new StairInfo(typeof(SandStoneStairs), 0x076C),
            new StairInfo(typeof(DarkStoneStairs), 0x0788),
            new StairInfo(typeof(BrickStairs), 0x07A3)
        };
    }

    public class StairInfo
    {
        public Type m_Type;
        public int m_BaseID;

        public StairInfo(Type type, int baseID)
        {
            m_Type = type;
            m_BaseID = baseID;
        }
    }
}

namespace Server.Items
{
    public enum Facing {
        South,
        West,
        North,
        East
    }
    
    [SerializationGenerator(0, false)]
    public partial class BaseStairAddon : BaseAddon
    {
        public BaseStairAddon(Facing f, int block, int step, int levels)
        {
            AddComponent( new AddonComponent( step ), 0, 0, 0 );
            
            switch(f){
                case Facing.South: {
                    _AddComponent(0,-1);
                    break;
                }
                case Facing.West: {
                    _AddComponent(-1,0);
                    break;
                }
                case Facing.North: {
                    _AddComponent(0,1);
                    break;
                }
                case Facing.East: {
                    _AddComponent(1,0);
                    break;
                }
            }
            
            void _AddComponent(int a, int b)
            {
                for ( int i = 0; i < levels; i++ ){
                    for ( int j = 0; j < i; j++ ){
                        AddComponent( new AddonComponent( block ), i * a, i * b, j * 5);
                    }
                    AddComponent( new AddonComponent( step ), i * a, i * b, i * 5);
                }
            }
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class WornSandStoneStairs : BaseStairAddon
    {
        [Constructible]
        public WornSandStoneStairs(Facing f, int levels) : base(f, 0x03EE, 0x03EF + (int)f, levels)
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class MarbleStairs : BaseStairAddon
    {
        [Constructible]
        public MarbleStairs(Facing f, int levels) : base(f, 0x0709, 0x070A + (int)f, levels)
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class StoneStairs : BaseStairAddon
    {
        [Constructible]
        public StoneStairs(Facing f, int levels) : base(f, 0x071E,
                                                        f == Facing.South ? 0x071F :
                                                        f == Facing.West ? 0x0736 :
                                                        f == Facing.North ? 0x0737 :
                                                        0x0749, levels)
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class LightWoodStairs : BaseStairAddon
    {
        [Constructible]
        public LightWoodStairs(Facing f, int levels) : base(f, 0x0721, 0x0722 + (int)f, levels )
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class WoodStairs : BaseStairAddon
    {
        [Constructible]
        public WoodStairs(Facing f, int levels) : base(f, 0x0738, 0x0739 + (int)f, levels )
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class LightStoneStairs : BaseStairAddon
    {
        [Constructible]
        public LightStoneStairs(Facing f, int levels) : base(f, 0x0750, 0x0751 + (int)f, levels)
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class SandStoneStairs : BaseStairAddon
    {
        [Constructible]
        public SandStoneStairs(Facing f, int levels) : base(f, 0x076C, 0x076D + (int)f, levels)
        {
        }
    }
    
    [SerializationGenerator(0, false)]
    public partial class DarkStoneStairs : BaseStairAddon
    {
        [Constructible]
        public DarkStoneStairs(Facing f, int levels) : base(f, 0x0788, 0x0789 + (int)f, levels)
        {
        }
    }

    [SerializationGenerator(0, false)]
    public partial class BrickStairs : BaseStairAddon
    {
        [Constructible]
        public BrickStairs(Facing f, int levels) : base(f, 0x07A3, 0x07A4 + (int)f, levels)
        {
        }
    }
}
