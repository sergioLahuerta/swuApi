using System;

namespace swuApi.Enums
{
    [Flags]
    public enum AspectType
    {
        // 0. Valor Cero para cartas incoloras
        None = 0,
        Vigilance = 1,
        Command = 2,
        Aggression = 4,
        Cunning = 8,
        Villainy = 16,
        Heroism = 32
    }
}