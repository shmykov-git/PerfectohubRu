namespace PerfectohubRu.Model
{
    public class OperationResult
    {
        public bool Success => Error == null;
        public string Error { get; set; }

        public static OperationResult Successfull() => new OperationResult();
    }
}
