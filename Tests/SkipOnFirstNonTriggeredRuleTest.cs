using EasyRules;
using System;
using Xunit;

namespace Tests
{
	public class SkipOnFirstNonTriggeredRuleTest : AbstractTest
	{
		[Fact]
		public void TestSkipOnFirstNonTriggeredRule()
		{
			var parameters = new RulesEngineParameters(skipOnFirstNonTriggeredRule: true);
			var engine = new DefaultRulesEngine(parameters);

			var rule1 = new Rule("Rule1", (f) => false, (f) => throw new Exception());
			var rule2 = new Rule("Rule2", (f) => throw new Exception(), (f) => throw new Exception());

			var rules = new Rules([rule1, rule2]);
			engine.Execute(rules, Facts);
		}
	}
}
