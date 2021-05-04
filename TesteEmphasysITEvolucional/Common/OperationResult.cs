namespace TesteEmphasysITEvolucional.Common
{
    public class OperationResult
    {
        public bool Successful { get; private set; }
        public string Message { get; private set; }

        protected OperationResult(bool successful, string message)
        {
            Successful = successful;
            Message = message;
        }

        public static OperationResult Success(string message = null)
            => new OperationResult(true, message);
        public static OperationResult Failure(string message = null)
            => new OperationResult(false, message);
    }

    public class OperationResult<T> : OperationResult
    {
        public T Data { get; private set; }

        private OperationResult(T data, bool successful, string message) : base(successful, message)
        {
            Data = data;
        }

        public static OperationResult<T> Success(T data, string message = null)
            => new OperationResult<T>(data, true, message);

        public static OperationResult<T> Failure(T data, string message = null)
            => new OperationResult<T>(data, false, message);

        new public static OperationResult<T> Failure(string message = null)
            => new OperationResult<T>(default, false, message);
    }
}
