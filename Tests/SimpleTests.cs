using EasyRules.API;
using EasyRules.Core;

namespace Tests
{
	public class SimpleTests
	{
		[Fact]
		public void HelloWorld()
		{
			var facts = new Facts()
			{
				{ "rain", true }
			};

			var output = string.Empty;

			var rules = new Rules()
			{
				r => r.WithName("weather rule")
				.WithDescription("if it rains then take an umbrella")
				.ThatWhen(facts => facts.IsTrue("rain"))
				.ThenDoes(facts => output = "It rains, take an umbrella!")
			};

			var rulesEngine = new DefaultRulesEngine();
			rulesEngine.Fire(rules, facts);

			Assert.Equal("It rains, take an umbrella!", output);
		}
	}
}