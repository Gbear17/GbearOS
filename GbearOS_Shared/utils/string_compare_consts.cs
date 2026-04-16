// Copyright (c) 2026 Garrett Wyrick.

// [GbearOS Component]
// Name: string_compare_consts.cs
// Purpose: Single shared OrdinalIgnoreCase comparison constant to avoid scattered StringComparer usage.
// PB Association: Shared
// Dependencies: None
// Key Methods: — (StrIX.C constant)

using System;

namespace IngameScript
{
    public static class StrIX
    {
        public const StringComparison C = StringComparison.OrdinalIgnoreCase;
    }
}
