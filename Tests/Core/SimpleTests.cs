using Xunit;
using EasyRules;
using System;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace Tests.Core
{
    public class SimpleTest
    {
        [Fact]
        public void HelloWorld()
        {
            var facts = new Facts()
            {
                Fact.Create("rain", true),
				Fact.Create("saying", "In spain, rain falls mainly on the plain.").Key(out var saying)
			};

            var output = string.Empty;

            var rules = new Rules()
            {
                new Rule(
                    name: "spain rule",
                    description: "where the rain falls, in spain",
				    condition: f => f.True("rain"),
                    action: f => output = f.Get(saying)
                )
            };

            var rulesEngine = new DefaultRulesEngine();
            rulesEngine.Execute(rules, facts);

            Assert.Equal("In spain, rain falls mainly on the plain.", output);
        }
    }

	public class WeatherRule : Rule<bool>
	{
		public override string Name => "weather rule";
		public override string Description => "if it rains then take an umbrella";
		public override bool Evaluate(bool rain) => rain;
		public override void Execute(bool rain) => Console.WriteLine("It rains, take an umbrella!");
	}
}