// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuGetGallery
{
    public class Credential
        : IEntity
    {
        public Credential()
        {
            Created = DateTime.UtcNow;
        }

        public Credential(string type, string value)
            : this()
        {
            Type = type;
            Value = value;
        }

        public Credential(string type, string value, TimeSpan expiration)
            : this(type, value)
        {
            Expires = DateTime.UtcNow.Add(expiration);
        }

        public int Key { get; set; }

        [Required]
        public int UserKey { get; set; }

        [Required]
        [StringLength(maximumLength: 64)]
        public string Type { get; set; }

        [Required]
        [StringLength(maximumLength: 256)]
        public string Value { get; set; }

        [StringLength(maximumLength: 256)]
        public string Identity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }

        public DateTime? Expires { get; set; }

        public virtual User User { get; set; }

        public bool HasExpired()
        {
            if (Expires.HasValue)
            {
                return DateTime.UtcNow > Expires.Value;
            }

            return false;
        }
    }
}
