// Copyright (c) 2026 Garrett Wyrick.
//
// [GbearOS Component]
// Name: block_utils.cs
// Purpose: Shared terminal-block name helpers (tag substring checks) for PB1/PB2.
// PB Association: Shared
// Dependencies: None
// Key Methods: HasTag

using System;

namespace IngameScript
{
    public static class BlockUtils
    {
        public static bool HasTag(string name, string tag)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(tag))
                return false;
            return name.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
