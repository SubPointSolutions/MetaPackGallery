// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;

namespace NuGetGallery.Auditing
{
    public class PackageAuditRecord : AuditRecord<PackageAuditAction>
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Hash { get; set; }

        public DataTable PackageRecord { get; set; }
        public DataTable RegistrationRecord { get; set; }

        public string Reason { get; set; }

        public PackageAuditRecord(string id, string version, string hash, DataTable packageRecord, DataTable registrationRecord, PackageAuditAction action, string reason)
            : base(action)
        {
            Id = id;
            Version = version;
            Hash = hash;
            PackageRecord = packageRecord;
            RegistrationRecord = registrationRecord;
            Reason = reason;
        }

        public PackageAuditRecord(Package package, PackageAuditAction action, string reason)
            : this(package.PackageRegistration.Id, package.Version, package.Hash, null, null, action, reason)
        {
            PackageRecord = DataTableUtility.ConvertToDataTable(package);
            RegistrationRecord = DataTableUtility.ConvertToDataTable(package.PackageRegistration);
        }

        public PackageAuditRecord(Package package, PackageAuditAction action)
            : this(package.PackageRegistration.Id, package.Version, package.Hash, null, null, action, null)
        {
            PackageRecord = DataTableUtility.ConvertToDataTable(package);
            RegistrationRecord = DataTableUtility.ConvertToDataTable(package.PackageRegistration);
        }
        
        public override string GetPath()
        {
            return $"{Id}/{NuGetVersionNormalizer.Normalize(Version)}"
                .ToLowerInvariant();
        }
    }
}