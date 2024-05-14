using EasyRules;
using System.Linq;
using Xunit;

namespace Tests
{
	public class BasicRuleTest : AbstractTest
	{
		[Fact]
		public void BasicRuleEvaluateShouldReturnFalse()
		{
			var basicRule = new BasicRule();
			Assert.False(basicRule.Evaluate(Facts));
		}

		[Fact]
		public void TestCompareTo()
		{
			var rule1 = new FirstRule();
			var rule2 = new FirstRule();
			var compareByPriority = new OrderRulesByPriority();

			Assert.Equal(0, compareByPriority.Compare(rule1, rule2));
			Assert.Equal(0, compareByPriority.Compare(rule2, rule1));
		}

		[Fact]
		public void TestSortSequence()
		{
			var rule1 = new FirstRule();
			var rule2 = new SecondRule();
			var rule3 = new ThirdRule();
			var rules = new Rules([rule1, rule2, rule3]);
			
			var comparer = new EquateRulesByName();
			var engine = new DefaultRulesEngine();

			var evaluated = engine.Evaluate(rules, Facts);
			Assert.Equal(evaluated.Select(kv => kv.Key), [rule1, rule3, rule2], comparer);
		}

		public class FirstRule : BasicRule
		{
			public override string Name => "rule1";
			public override int Priority => 1;
			public override bool Evaluate(IFacts facts) => true;
		}

		public class SecondRule : BasicRule
		{
			public override string Name => "rule2";
			public override int Priority => 3;
			public override bool Evaluate(IFacts facts) => true;
		}

		class ThirdRule : BasicRule
		{
			public override string Name => "rule3";
			public override int Priority => 2;
			public override bool Evaluate(IFacts facts) => true;
		}
	}
}
