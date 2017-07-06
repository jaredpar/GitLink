﻿// Copyright (c) Andrew Arnott. All rights reserved.

namespace GitLinkTask
{
    using System;
    using Catel.Logging;
    using global::GitLink;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class LinkPdbToGitRemote : Task
    {
        [Required]
        public ITaskItem PdbFile { get; set; }

        public string Method
        {
            get { return MethodEnum.ToString(); }
            set { MethodEnum = string.IsNullOrEmpty(value) ? LinkMethod.Http : (LinkMethod)Enum.Parse(typeof(LinkMethod), value); }
        }

        public bool SkipVerify { get; set; }

        public string GitRemoteUrl { get; set; }

        public string GitCommitId { get; set; }

        public string GitWorkingDirectory { get; set; }

        public bool WarnOnMissingFiles { get; set; }

        private LinkMethod MethodEnum { get; set; }

        public override bool Execute()
        {
            LogManager.AddListener(new MSBuildListener(Log));

            var options = new LinkOptions
            {
                Method = MethodEnum,
                SkipVerify = SkipVerify,
                SkipWarningOnMissingFiles = !WarnOnMissingFiles,
                GitRemoteUrl = GitRemoteUrl != null ? new Uri(GitRemoteUrl, UriKind.Absolute) : null,
                CommitId = GitCommitId,
                GitWorkingDirectory = GitWorkingDirectory,
            };
            bool success = Linker.Link(PdbFile.GetMetadata("FullPath"), options);

            return success && !Log.HasLoggedErrors;
        }

        private class MSBuildListener : LogListenerBase
        {
            private readonly TaskLoggingHelper _log;

            internal MSBuildListener(TaskLoggingHelper log)
            {
                _log = log;
            }

            protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
            {
                switch (logEvent)
                {
                    case LogEvent.Error:
                        _log.LogError(message);
                        break;
                    case LogEvent.Warning:
                        _log.LogWarning(message);
                        break;
                    case LogEvent.Info:
                        _log.LogMessage(MessageImportance.Normal, message);
                        break;
                    case LogEvent.Debug:
                        _log.LogMessage(MessageImportance.Low, message);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
