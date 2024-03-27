using System;
using UnityEngine;

namespace Arch.Unity.Conversion
{
    [Serializable]
    public sealed class EntityConversionOptions : IEquatable<EntityConversionOptions>
    {
        public static readonly EntityConversionOptions Default = new();

        [SerializeField] ConversionMode conversionMode = ConversionMode.ConvertAndDestroy;
        [SerializeField] bool convertHybridComponents = false;

        public ConversionMode ConversionMode
        {
            get => conversionMode;
            set => conversionMode = value;
        }
        
        public bool ConvertHybridComponents
        {
            get => convertHybridComponents;
            set => convertHybridComponents = value;
        }

        public bool Equals(EntityConversionOptions other)
        {
            return other.ConversionMode == ConversionMode &&
                other.ConvertHybridComponents == ConvertHybridComponents;
        }

        public override bool Equals(object obj)
        {
            if (obj is EntityConversionOptions options) return Equals(options);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ConversionMode, ConvertHybridComponents);
        }
    }
}