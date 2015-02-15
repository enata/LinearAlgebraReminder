using System;

namespace LinearAlgebraReminder.Utility
{
    public class LinearAlgebraReminderException : Exception
    {
        public LinearAlgebraReminderException()
        { }

        public LinearAlgebraReminderException(string message) : base(message)
        { }

        public LinearAlgebraReminderException(string message, Exception inner) : base(message, inner)
        { }
    }
}
