using Xunit;
using EasyRules;
using System;

namespace Tests.Core
{
	public sealed class InferenceRulesEngineTest
	{
		[Fact]
		public void WhenFireRules_ThenNullRulesShouldNotBeAccepted()
		{
			var engine = new InferenceRulesEngine();
			Assert.Throws<ArgumentNullException>(() => engine.Execute(null, new Facts()));
		}

		[Fact]
		public void WhenFireRules_ThenNullFactsShouldNotBeAccepted()
		{
			var engine = new InferenceRulesEngine();
			Assert.Throws<ArgumentNullException>(() => engine.Execute(new Rules(), null));
		}

		[Fact]
		public void WhenCheckRules_ThenNullRulesShouldNotBeAccepted()
		{
			var engine = new InferenceRulesEngine();
			Assert.Throws<ArgumentNullException>(() => engine.Evaluate(null, new Facts()));
		}

		[Fact]
		public void WhenCheckRules_ThenNullFactsShouldNotBeAccepted()
		{
			var engine = new InferenceRulesEngine();
			Assert.Throws<ArgumentNullException>(() => engine.Evaluate(new Rules(), null));
		}

		[Fact]
		public void TestCandidateSelection()
		{
			// Given
			var facts = new Facts()
			{
				{ "foo", true }
			};

			var dummyRule = new DummyRule();
			var anotherDummyRule = new AnotherDummyRule();
			var rules = new Rules([dummyRule, anotherDummyRule]);
			var rulesEngine = new InferenceRulesEngine();

			// When
			rulesEngine.Execute(rules, facts);

			// Then
			Assert.True(dummyRule.IsExecuted);
			Assert.False(anotherDummyRule.IsExecuted);
		}

		[Fact]
		public void TestCandidateOrdering()
		{
			// Given
			var facts = new Facts()
			{
				{ "foo", true },
				{ "bar", true }
			};

			var dummyRule = new DummyRule();
			var anotherDummyRule = new AnotherDummyRule();
			var rules = new Rules([dummyRule, anotherDummyRule]);
			var rulesEngine = new InferenceRulesEngine();

			// When
			rulesEngine.Execute(rules, facts);

			// Then
			Assert.True(dummyRule.IsExecuted);
			Assert.True(anotherDummyRule.IsExecuted);
			Assert.True(dummyRule.Timestamp <= anotherDummyRule.Timestamp);
		}

		private sealed class DummyRule : IRule
		{
			private bool isExecuted = false;
			private DateTime timestamp;

			public string Name => nameof(DummyRule);
			public string Description => Constants.DEFAULT_RULE_DESCRIPTION;
			public int Priority => 1;

			public bool Evaluate(IFacts facts) => facts.True("foo");

			public void Execute(IFacts facts)
			{
				isExecuted = true;
				timestamp = DateTime.Now;
				facts.Remove("foo");
			}

			public bool IsExecuted => isExecuted;
			public DateTime Timestamp => timestamp;
		}

		private sealed class AnotherDummyRule : IRule
		{
			private bool isExecuted = false;
			private DateTime timestamp;

			public string Name => nameof(AnotherDummyRule);
			public string Description => Constants.DEFAULT_RULE_DESCRIPTION;
			public int Priority => 2;

			public bool Evaluate(IFacts facts) => facts.True("bar");

			public void Execute(IFacts facts)
			{
				isExecuted = true;
				timestamp = DateTime.Now;
				facts.Remove("bar");
			}

			public bool IsExecuted => isExecuted;
			public DateTime Timestamp => timestamp;
		}
	}
}
