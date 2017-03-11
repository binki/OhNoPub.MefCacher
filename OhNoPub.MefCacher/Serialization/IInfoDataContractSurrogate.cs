using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OhNoPub.MefCacher.Serialization
{
    public interface IInfoDataContractSurrogate
        : IDataContractSurrogate
    {
        /// <summary>
        ///   The things that should be added to <see cref="DataContractSerializerSettings.KnownTypes"/> when using
        ///   this surrogate.
        /// </summary>
        IEnumerable<Type> KnownTypes { get; }
    }
}
