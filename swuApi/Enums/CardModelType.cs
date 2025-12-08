using System.Runtime.Serialization;

namespace swuApi.Enums
{
    public enum CardModelType
    {
        Standard,
        Foil,
        Hyperspace,

        [EnumMember(Value = "Hyperspace Foil")]
        HyperspaceFoil,
        Showcase
    }
}