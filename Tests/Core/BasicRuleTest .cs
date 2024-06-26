﻿using EasyRules;
using System.Linq;
using Xunit;

namespace Tests.Core
{
    public class BasicRuleTest : AbstractTest
    {
        private readonly IRule rule1;
        private readonly IRule rule2;
        private readonly IRule rule3;

        public BasicRuleTest()
        {
            rule1 = new Rule(name: nameof(rule1), priority: 1, predicate: ReturnTrue, action: DoNothing);
            rule2 = new Rule(name: nameof(rule2), priority: 3, predicate: ReturnTrue, action: DoNothing);
            rule3 = new Rule(name: nameof(rule3), priority: 2, predicate: ReturnTrue, action: DoNothing);
        }

        private static bool ReturnTrue(IFacts _) => true;
        private static void DoNothing(IFacts _) { }

        [Fact]
        public void TestCompareTo()
        {
            Assert.Equal(-1, OrderRulesByPriority.Instance.Compare(rule1, rule2));
            Assert.Equal(1, OrderRulesByPriority.Instance.Compare(rule2, rule1));
        }

        [Fact]
        public void TestSortSequence()
        {
            var rules = new Rules([rule1, rule2, rule3]);

            var engine = new DefaultRulesEngine();

            var evaluated = engine.Evaluate(rules, Facts);
            Assert.Equal(evaluated.Select(kv => kv.Key), [rule1, rule3, rule2], EquateRulesByName.Instance);
        }
    }
}
