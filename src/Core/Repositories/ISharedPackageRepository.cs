﻿
using NuGet.Versioning;

namespace NuGet
{
    public interface ISharedPackageRepository : IPackageRepository
    {
        bool IsReferenced(string packageId, NuGetVersion version);

        /// <summary>
        /// Gets whether the repository contains a solution-level package with the specified id and version.
        /// </summary>
        bool IsSolutionReferenced(string packageId, NuGetVersion version);
        
        /// <summary>
        /// Registers a new repository for the shared repository
        /// </summary>
        void RegisterRepository(string path);

        /// <summary>
        /// Removes a registered repository
        /// </summary>
        void UnregisterRepository(string path);
    }
}
