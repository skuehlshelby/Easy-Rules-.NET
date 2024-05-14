using EasyRules;

namespace Tests
{
	public class AbstractTest
	{
		protected IFacts Facts = new Facts()
		{
			{ "fact1", string.Empty },
			{ "fact2", string.Empty }
		};
	}
}
