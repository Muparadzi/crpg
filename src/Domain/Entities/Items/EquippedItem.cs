﻿using Crpg.Domain.Common;
using Crpg.Domain.Entities.Characters;

namespace Crpg.Domain.Entities.Items
{
    /// <summary>
    /// Item equipped by a character.
    /// </summary>
    public class EquippedItem : AuditableEntity
    {
        public int CharacterId { get; set; }
        public ItemSlot Slot { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }

        public Character? Character { get; set; }
        public Item? Item { get; set; }
        public OwnedItem? OwnedItem { get; set; }
    }
}