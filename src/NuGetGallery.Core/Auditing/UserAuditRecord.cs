// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace NuGetGallery.Auditing
{
    public class UserAuditRecord : AuditRecord<UserAuditAction>
    {
        public string Username { get; }
        public string EmailAddress { get; }
        public string UnconfirmedEmailAddress { get; }
        public string[] Roles { get; }
        public CredentialAuditRecord[] Credentials { get; }
        public CredentialAuditRecord[] AffectedCredential { get; }
        public string AffectedEmailAddress { get; }
        
        public UserAuditRecord(User user, UserAuditAction action)
            : this(user, action, Enumerable.Empty<Credential>()) { }
        public UserAuditRecord(User user, UserAuditAction action, Credential affected)
            : this(user, action, SingleEnumerable(affected)) { }
        public UserAuditRecord(User user, UserAuditAction action, IEnumerable<Credential> affected)
            : base(action)
        {
            Username = user.Username;
            EmailAddress = user.EmailAddress;
            UnconfirmedEmailAddress = user.UnconfirmedEmailAddress;
            Roles = user.Roles.Select(r => r.Name).ToArray();
            Credentials = user.Credentials.Select(c => new CredentialAuditRecord(c, removed: false)).ToArray();

            if (affected != null)
            {
                AffectedCredential = affected.Select(c => new CredentialAuditRecord(c, action == UserAuditAction.RemovedCredential)).ToArray();
            }

            Action = action;
        }
        
        public UserAuditRecord(User user, UserAuditAction action, string affectedEmailAddress)
            : this(user, action, Enumerable.Empty<Credential>()) {
            AffectedEmailAddress = affectedEmailAddress;
        }
        
        public override string GetPath()
        {
            return Username.ToLowerInvariant();
        }

        private static IEnumerable<Credential> SingleEnumerable(Credential affected)
        {
            yield return affected;
        }
    }
}
