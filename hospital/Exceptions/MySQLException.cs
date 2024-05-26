namespace hospital.Exceptions
{
    public class MySQLException : Exception
    {

        public MySQLException()
        {
        }

        public MySQLException(string message)
            : base(message)
        {
        }

        public MySQLException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
