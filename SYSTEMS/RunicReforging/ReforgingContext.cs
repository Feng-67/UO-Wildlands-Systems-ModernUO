using System.Collections.Generic;
using Server.Gumps;

namespace Server.Items;

public class ReforgingContext
{
    // Retained so the persistence stays registered with the world save system.
    private static ReforgingContextPersistence Persistence { get; set; }

    public static Dictionary<Mobile, ReforgingContext> ReforgingContexts { get; set; } = new();

    public Dictionary<BaseTool, ReforgingOption> Contexts { get; set; }

    public ReforgedPrefix Prefix { get; set; }
    public ReforgedSuffix Suffix { get; set; }

    public ReforgingContext(Mobile m)
    {
        Contexts = new Dictionary<BaseTool, ReforgingOption>();

        if (m != null)
        {
            ReforgingContexts[m] = this;
        }
    }

    public ReforgingContext(IGenericReader reader)
    {
        Contexts = new Dictionary<BaseTool, ReforgingOption>();

        reader.ReadEncodedInt(); // version

        Prefix = (ReforgedPrefix)reader.ReadEncodedInt();
        Suffix = (ReforgedSuffix)reader.ReadEncodedInt();

        var count = reader.ReadEncodedInt();
        for (var i = 0; i < count; ++i)
        {
            var tool = reader.ReadEntity<BaseTool>();
            var option = (ReforgingOption)reader.ReadEncodedInt();

            if (tool != null)
            {
                Contexts[tool] = option;
            }
        }
    }

    public void Serialize(IGenericWriter writer)
    {
        writer.WriteEncodedInt(0); // version

        writer.WriteEncodedInt((int)Prefix);
        writer.WriteEncodedInt((int)Suffix);

        writer.WriteEncodedInt(Contexts.Count);
        foreach (var (tool, option) in Contexts)
        {
            writer.Write(tool);
            writer.WriteEncodedInt((int)option);
        }
    }

    public static ReforgingContext GetContext(Mobile m)
    {
        if (m == null)
        {
            return null;
        }

        if (ReforgingContexts.TryGetValue(m, out var context))
        {
            return context;
        }

        return new ReforgingContext(m);
    }

    public static void Configure() => Persistence ??= new ReforgingContextPersistence();

    private sealed class ReforgingContextPersistence : GenericPersistence
    {
        // Priority 10: this persistence stores entity references (Mobile/BaseTool), so it must load
        // after mobiles (priority 1) and items (priority 2); ties at priority 1 sort by hash order.
        public ReforgingContextPersistence() : base("ReforgingContexts", 10)
        {
        }

        public override void Serialize(IGenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            var count = 0;

            foreach (var (mobile, _) in ReforgingContexts)
            {
                if (mobile?.Deleted == false)
                {
                    ++count;
                }
            }

            writer.WriteEncodedInt(count);

            foreach (var (mobile, context) in ReforgingContexts)
            {
                if (mobile?.Deleted != false)
                {
                    continue;
                }

                writer.Write(mobile);
                context.Serialize(writer);
            }
        }

        public override void Deserialize(IGenericReader reader)
        {
            reader.ReadEncodedInt(); // version

            ReforgingContexts = new Dictionary<Mobile, ReforgingContext>();

            var count = reader.ReadEncodedInt();

            for (var i = 0; i < count; ++i)
            {
                // The context must be read even when the mobile could not be resolved, otherwise the
                // stream would be left mid-record for the remaining entries.
                var mobile = reader.ReadEntity<Mobile>();
                var context = new ReforgingContext(reader);

                if (mobile != null)
                {
                    ReforgingContexts[mobile] = context;
                }
            }
        }
    }
}
