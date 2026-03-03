using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public sealed class DuplicateIssueCharacterTypeException:Exception
    {
        public string IssueCharacterType { get; }

        public DuplicateIssueCharacterTypeException(string issueCharacterType)
            : base($"IssueCharacterType '{issueCharacterType}' already exists.")
        {
            IssueCharacterType = issueCharacterType;
        }
    }
}
