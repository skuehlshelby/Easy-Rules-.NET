using EasyRules;
using System;
using System.Collections.Generic;
using System.Linq;
using Tests.Annotation;
using Xunit;
using RuleProxy = EasyRules.Attributes.RuleProxy;

namespace Tests.Api
{

	public class RulesTest
	{
		private static bool ReturnTrue(IFacts _) => true;
		private static void DoNothing(IFacts _) { }

		[Fact]
		public void Register()
		{
			var rules = new Rules() { RuleProxy.AsRule(new DummyRule()) };

			Assert.Single(rules);
		}

		[Fact]
		public void RulesMustHaveUniqueName()
		{
			var rules = new Rules()
			{
				new Rule("rule", ReturnTrue, DoNothing),
				new Rule("rule", ReturnTrue, DoNothing)
			};

			Assert.Single(rules);
		}

		[Fact]
		public void Unregister()
		{
			var rule = RuleProxy.AsRule(new DummyRule());

			var rules = new Rules()
			{
				rule
			};
			
			rules.Remove(rule);

			Assert.Empty(rules);
		}

		[Fact]
		public void UnregisterByName()
		{
			var rules = new Rules()
			{
				new Rule("rule1", ReturnTrue, DoNothing),
				new Rule("rule2", ReturnTrue, DoNothing)
			};

			rules.Remove("rule2");

			Assert.Single(rules);
		}

		[Fact]
		public void UnregisterByNameNonExistingRule()
		{
			var rules = new Rules()
			{
				new Rule("rule1", ReturnTrue, DoNothing)
			};

			rules.Remove("rule2");

			Assert.Single(rules);
		}

		[Fact]
		public void IsEmpty()
		{
			Assert.Empty(new Rules());
		}

		[Fact]
		public void Clear()
		{
			var rules = new Rules()
			{
				new Rule("rule1", ReturnTrue, DoNothing),
				new Rule("rule2", ReturnTrue, DoNothing)
			};

			rules.Clear();

			Assert.Empty(rules);
		}

		[Fact]
		public void Sort()
		{
			var r1 = new Rule("rule1", 1, ReturnTrue, DoNothing);
			var r2 = new Rule("rule2", int.MaxValue, ReturnTrue, DoNothing);
			var r3 = new DummyRule();

			var rules = new Rules()
			{
				r1,
				r2,
				RuleProxy.AsRule(r3)
			};

			Assert.Equal(r1, rules.First());
			Assert.Equal(r2, rules.Last());
		}

		[Fact]
		public void Size()
		{
			var rules = new Rules();

			Assert.Empty(rules);

			rules.Add(RuleProxy.AsRule(new DummyRule()));

			Assert.Single(rules);

			rules.Remove(RuleProxy.AsRule(new DummyRule()));

			Assert.Empty(rules);
		}

		[Fact]
		public void Register_Multiple()
		{
			var rules = new Rules()
			{
				new Rule("rule1", ReturnTrue, DoNothing),
				new Rule("rule2", ReturnTrue, DoNothing)
			};

			Assert.Equal(2, rules.Count);
		}

		[Fact]
		public void Unregister_NoneLeft()
		{
			var rules = new Rules()
			{
				new Rule("ruleA", ReturnTrue, DoNothing),
				new Rule("ruleB", ReturnTrue, DoNothing)
			};
			
			Assert.Equal(2, rules.Count);

			rules.Remove(new Rule("ruleA", ReturnTrue, DoNothing));
			rules.Remove(new Rule("ruleB", ReturnTrue, DoNothing));
			
			Assert.Empty(rules);
		}

		[Fact]
		public void Unregister_OneLeft()
		{
			var rules = new Rules()
			{
				new Rule("ruleA", ReturnTrue, DoNothing),
				new Rule("ruleB", ReturnTrue, DoNothing)
			};

			Assert.Equal(2, rules.Count);

			rules.Remove(new Rule("ruleB", ReturnTrue, DoNothing));

			Assert.Single(rules);
		}

		[Fact]
		public void WhenRegisterNullRule_ThenShouldThrowNullPointerException()
		{
			Assert.Throws<ArgumentNullException>(() => new Rules() { null });
		}
	}
}
