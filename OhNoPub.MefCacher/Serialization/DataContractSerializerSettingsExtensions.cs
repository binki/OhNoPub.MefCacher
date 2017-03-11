using System;
using System.Linq;
using System.Runtime.Serialization;

namespace OhNoPub.MefCacher.Serialization
{
    /// <summary>
    ///   Utilities for working with settings, particularly with <see cref="IInfoDataContractSurrogate"/>.
    /// </summary>
    public static class DataContractSerializerSettingsExtensions
    {
        /// <summary>
        ///   Update the settings to include the surrogates.
        /// </summary>
        public static DataContractSerializerSettings Apply(
            this DataContractSerializerSettings settings,
            params IInfoDataContractSurrogate[] surrogates)
        {
            if (settings.DataContractSurrogate != null) throw new InvalidOperationException($"Attempt to add more surrogates to a {nameof(DataContractSerializerSettings)} which already has a surrogate set.");

            // No-op
            if (surrogates.Length < 1) return settings;

            // Turn into a single surrogate, wrapping if necessary.
            var surrogate = surrogates.Length == 1 ? surrogates[0] : new AggregatingDataContractSurrogate(surrogates);
            settings.DataContractSurrogate = surrogate;
            settings.KnownTypes = (settings.KnownTypes ?? new Type[] { }).Concat(surrogate.KnownTypes);
            return settings;
        }
    }
}
