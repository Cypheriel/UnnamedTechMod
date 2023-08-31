using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Terraria;

namespace UnnamedTechMod;

public class ItemBuffer
{
    public Queue<Item> Buffer { get; } = new();

    public virtual List<int> WhitelistedItemTypes => new();
    public virtual List<int> BlacklistedItemTypes => new();
    public virtual ushort Capacity => 1;
    
    /// <summary>
    /// Try to insert an item into the buffer queue.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Whether a complete insertion was successful. <c>false</c> may indicate a partial insertion.</returns>
    public bool Insert(Item item)
    {
        Console.WriteLine($"Inserting {item.Name} x{item.stack}");
        // TODO: Disallow partial insertions(?)
        if (TryMergeStack(item)) return true;

        if (
            Buffer.Count >= Capacity // Too many items
            || (WhitelistedItemTypes.Count > 0 && !WhitelistedItemTypes.Contains(item.type)) // Item not whitelisted
            || BlacklistedItemTypes.Contains(item.type) // Item blacklisted
        ) return false;

        Buffer.Enqueue(item);
        return true;
    }
    
    /// <summary>
    /// Retrieves up to <c>maxAmount</c> of the first <c>Item</c> in the <c>Buffer</c>.
    /// </summary>
    /// <param name="maxAmount">The max amount of items to remove from the stack.</param>
    /// <returns>The retrieved item.</returns>
    public Item RetrieveFirst(int maxAmount = 9999)
    {
        var peekResult = Buffer.TryPeek(out var firstItem);
        
        if (!peekResult) return null;

        Item result;
        if (firstItem.stack <= maxAmount)
        {
            result = Buffer.Dequeue();
        }
        else
        {
            result = firstItem.Clone();
            result.stack = maxAmount;
            firstItem.stack -= maxAmount;
        }

        return result;
    }
    
    /// <summary>
    /// Try merging an item into the current buffer to prevent unnecessary extra queues.
    /// </summary>
    /// <param name="item">The item to try merging into other stacks.</param>
    /// <returns>Whether a complete merge was successful.</returns>
    public bool TryMergeStack(Item item)
    {
        foreach (var otherItem in Buffer.ToImmutableList())
        {
            // Ensure item and otherItem are the same type and that otherItem can have a higher stack.
            var maxStack = MaxStackForItem(otherItem);
            if (item.type != otherItem.type || otherItem.stack >= maxStack) continue;
            
            var toMerge = Math.Min(otherItem.maxStack - otherItem.stack, item.stack);
            otherItem.stack += toMerge;
            item.stack -= toMerge;
        }

        return item.stack == 0;
    }
    
    /// <summary>
    /// The max stack an item can have within this item buffer.
    /// Use this if you want to change an item's effective max stack to lower than its actual max stack.
    /// </summary>
    /// <param name="item">The item whose effective max stack will be lowered.</param>
    /// <returns></returns>
    public virtual int MaxStackForItem(Item item)
    {
        return item.maxStack;
    }
}