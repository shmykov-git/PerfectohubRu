namespace PerfectohubRu.Model
{
    public class OperationResult
    {
        public bool Success => Error == null;
        public bool HasWarning => Warning == null;

        public string Error { get; set; }
        public string Warning { get; set; }

        public static OperationResult Successfull() => new OperationResult();
        public static OperationResult Warn(string warning) => new OperationResult() { Warning = warning };
    }
}
