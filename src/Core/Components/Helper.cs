using System;

namespace Core.Components
{
	public static class Helper
	{
		public static TYPE CheckServiceExists<TYPE>(this TYPE? instance) where TYPE : class
		{
			return instance ?? throw new NotImplementedException($"Service {typeof(TYPE)} not found.");
		}

		public static TYPE CheckComponentExists<TYPE>(this TYPE? instance) where TYPE : class
		{
			return instance ?? throw new NotImplementedException($"Component {typeof(TYPE)} not found.");
		}
	}
}
