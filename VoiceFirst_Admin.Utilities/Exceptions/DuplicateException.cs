using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceFirst_Admin.Utilities.Exceptions
{
    public sealed class DuplicateException:Exception
    {
        public string DuplicateValue { get; }

        public DuplicateException(string duplicateValue)
            : base($" '{duplicateValue}' already exists.")
        {
            DuplicateValue = duplicateValue;
        }
    }
}
