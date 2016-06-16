// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

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
            PackageRecord = ConvertToDataTable(package);
            RegistrationRecord = ConvertToDataTable(package.PackageRegistration);
        }

        public PackageAuditRecord(Package package, PackageAuditAction action)
            : this(package.PackageRegistration.Id, package.Version, package.Hash, null, null, action, null)
        {
            PackageRecord = ConvertToDataTable(package);
            RegistrationRecord = ConvertToDataTable(package.PackageRegistration);
        }

        public PackageAuditRecord(PackageRegistration packageRegistration, PackageAuditAction action, string reason)
            : this(packageRegistration.Id, "0.0", null, null, null, action, reason)
        {
            RegistrationRecord = ConvertToDataTable(packageRegistration);
        }

        public override string GetPath()
        {
            return $"{Id}/{NuGetVersionNormalizer.Normalize(Version)}"
                .ToLowerInvariant();
        }

        private static DataTable ConvertToDataTable<T>(T instance)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable() { Locale = CultureInfo.CurrentCulture };

            List<object> values = new List<object>();
            for (int i = 0; i < properties.Count; i++)
            {
                var propertyDescriptor = properties[i];
                var propertyType = Nullable.GetUnderlyingType(propertyDescriptor.PropertyType) ?? propertyDescriptor.PropertyType;
                if (!IsComplexType(propertyType))
                {
                    table.Columns.Add(propertyDescriptor.Name, propertyType);
                    values.Add(propertyDescriptor.GetValue(instance) ?? DBNull.Value);
                }
            }

            table.Rows.Add(values.ToArray());

            return table;
        }

        private static bool IsComplexType(Type type)
        {
            if (type.IsSubclassOf(typeof(ValueType)) || type == typeof(string))
            {
                return false;
            }
            return true;
        }
    }
}