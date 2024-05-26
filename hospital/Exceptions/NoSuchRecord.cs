namespace hospital.Exceptions
{
    public class NoSuchRecord : Exception
    {

        public NoSuchRecord()
        {
        }

        public NoSuchRecord(string message)
            : base(message)
        {
        }

        public NoSuchRecord(string message, Exception inner)
            : base(message, inner)
        {
        }


    }
}

