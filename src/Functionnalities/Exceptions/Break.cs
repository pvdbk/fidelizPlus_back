using System;

namespace fidelizPlus_back
{
	public class Break : Exception
	{
		public object Content { get; }
		public BreakCode Code { get; }
		public int Status { get; }

		public Break()
		{ }

		public Break(object content, BreakCode code, int status = 500) : base()
		{
			Content = content;
			Code = code;
			Status = status;
		}

		public T Throw<T>() => throw this;

		public Break WithStatus(int status) => new Break(this.Content, this.Code, status);
	}
}
