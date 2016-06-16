// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;

namespace NuGetGallery.Auditing
{
    /// <summary>
    /// Writes audit records to a specific directory in the file system
    /// </summary>
    public class FileSystemAuditingService : AuditingService
    {
        public static readonly string DefaultContainerName = "auditing";

        private readonly string _auditingPath;
        private readonly Func<Task<AuditActor>> _onBehalfOfThunk;
        
        public FileSystemAuditingService(string auditingPath, Func<Task<AuditActor>> onBehalfOfThunk)
        {
            _auditingPath = auditingPath;
            _onBehalfOfThunk = onBehalfOfThunk;
        }

        public static Task<AuditActor> AspNetActorThunk()
        {
            // Use HttpContext to build an actor representing the user performing the action
            var context = HttpContext.Current;
            if (context == null)
            {
                return null;
            }

            // Try to identify the client IP using various server variables
            var clientIpAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(clientIpAddress)) // Try REMOTE_ADDR server variable
            {
                clientIpAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(clientIpAddress)) // Try UserHostAddress property
            {
                clientIpAddress = context.Request.UserHostAddress;
            }

            string user = null;
            string authType = null;
            if (context.User != null)
            {
                user = context.User.Identity.Name;
                authType = context.User.Identity.AuthenticationType;
            }

            return Task.FromResult(new AuditActor(
                null,
                clientIpAddress,
                user,
                authType,
                DateTime.UtcNow));
        }

        protected override async Task<AuditActor> GetActor()
        {
            // Construct an actor representing the user the service is acting on behalf of
            AuditActor onBehalfOf = null;
            if (_onBehalfOfThunk != null)
            {
                onBehalfOf = await _onBehalfOfThunk();
            }

            return await AuditActor.GetCurrentMachineActor(onBehalfOf);
        }

        protected override Task<Uri> SaveAuditRecord(string auditData, string resourceType, string filePath, string action, DateTime timestamp)
        {
            // Build relative file path
            var relativeFilePath = string.Concat(
                resourceType.ToLowerInvariant(), Path.DirectorySeparatorChar, 
                filePath, Path.DirectorySeparatorChar,
                timestamp.ToString("s").Replace(":", string.Empty), "-", // Sortable DateTime format
                action.ToLowerInvariant(), ".audit.v1.json");

            // Build full file path
            var fullFilePath = Path.Combine(_auditingPath, relativeFilePath);

            // Ensure the directory exists
            var directoryName = Path.GetDirectoryName(fullFilePath);
            if (!Directory.Exists(directoryName))
            {
                // TODO: should we catch the exception and log? or throw like we do now?
                // in my opinion, auditing is super important and should throw on failure
                Directory.CreateDirectory(directoryName);
            }

            // Write the data
            File.WriteAllText(fullFilePath, auditData);

            // Generate a local URL
            var uri = new Uri($"https://auditing.local/{relativeFilePath.Replace(Path.DirectorySeparatorChar, '/')}");

            return Task.FromResult(uri);
        }
    }
}
