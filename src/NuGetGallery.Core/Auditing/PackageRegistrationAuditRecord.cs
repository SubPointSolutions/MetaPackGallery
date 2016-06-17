// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;

namespace NuGetGallery.Auditing
{
    public class PackageRegistrationAuditRecord : AuditRecord<PackageRegistrationAuditAction>
    {
        public string Id { get; set; }
        public DataTable RegistrationRecord { get; set; }

        public string Reason { get; set; }

        public PackageRegistrationAuditRecord(string id,  DataTable registrationRecord, PackageRegistrationAuditAction action, string reason)
            : base(action)
        {
            Id = id;
            RegistrationRecord = registrationRecord;
            Reason = reason;
        }

        public PackageRegistrationAuditRecord(PackageRegistration packageRegistration, PackageRegistrationAuditAction action, string reason)
            : this(packageRegistration.Id, DataTableUtility.ConvertToDataTable(packageRegistration), action, reason)
        {
        }
        
        public override string GetPath()
        {
            return $"{Id}".ToLowerInvariant();
        }
    }
}