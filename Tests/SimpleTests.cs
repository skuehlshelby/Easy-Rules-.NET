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

			var rule = new RuleBuilder()
				.WithName("weather rule")
				.WithDescription("if it rains then take an umbrella")
				.ThatWhen(facts => facts.GetFact("rain", true))
				.ThenDoes(facts => output = "It rains, take an umbrella!")
				.Build();

			var rules = new Rules(rule);

			var rulesEngine = new DefaultRulesEngine();
			rulesEngine.Fire(rules, facts);

			Assert.Equal("It rains, take an umbrella!", output);
		}
	}
}