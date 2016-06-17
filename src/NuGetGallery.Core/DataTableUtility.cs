// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace NuGetGallery
{
    public static class DataTableUtility
    {
        public static DataTable ConvertToDataTable<T>(T instance)
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