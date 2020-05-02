namespace fidelizPlus_back.LogDomain
{
	public class LogService
	{
		public LogContext LogContext { get; }

		public LogService(LogContext logContext)
		{
			LogContext = logContext;
		}

		public void AddError(string s)
		{
			LogContext.Error.Add(new Error() { Content = s });
			LogContext.SaveChanges();
		}

		public void AddTest(string s)
		{
			LogContext.Test.Add(new Test() { Content = s });
			LogContext.SaveChanges();
		}
	}
}
