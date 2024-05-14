using Xunit;
using EasyRules;

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
				new Rule(
					name: "weather rule",
					description: "if it rains then take an umbrella",
					condition: f => f.IsTrue("rain"),
					action: f => output = "It rains, take an umbrella!")
			};

			var rulesEngine = new DefaultRulesEngine();
			rulesEngine.Execute(rules, facts);

			Assert.Equal("It rains, take an umbrella!", output);
		}
	}
}