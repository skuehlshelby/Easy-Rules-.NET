using EasyRules;
using EasyRules.Attributes;
using System;
using Xunit;
using FactAttribute = Xunit.FactAttribute;

namespace Tests.Annotation
{
	public sealed class AnnotationTests
	{
		[Fact]
		public void NotAnnotatedRuleMustNotBeAccepted()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new object()));
		}

		[Fact]
		public void ConditionMethodMustBeDefined()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithoutConditionMethod()));
		}

		[Fact]
		public void ConditionMethodMustBePublic()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithNonPublicConditionMethod()));
		}

		[Fact]
		public void WhenConditionMethodHasOneNonAnnotatedParameter_ThenThisParameterMustBeOfTypeFacts()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithConditionMethodHavingOneArgumentNotOfTypeFacts()));
		}

		[Fact]
		public void ConditionMethodMustReturnBooleanType()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithConditionMethodHavingNonBooleanReturnType()));
		}

		[Fact]
		public void ConditionMethodParametersShouldAllBeAnnotatedWithFactUnlessExactlyOneOfThemIsOfTypeFacts()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithOneParameterNotAnnotatedWithFactAndNotOfTypeFacts()));
		}

		[Fact]
		public void ActionMethodMustHaveExactlyOneArgumentOfTypeFactsIfAny()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithActionMethodHavingMoreThanOneArgumentOfTypeFacts()));
		}

		[Fact]
		public void ActionMethodMustBeDefined()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithoutActionMethod()));
		}

		[Fact]
		public void ActionMethodMustBePublic()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithNonPublicActionMethod()));
		}

		[Fact]
		public void ValidAnnotationsShouldBeAccepted()
		{
			try
			{
				RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithMultipleAnnotatedParametersAndOneParameterOfTypeFacts());
				RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithActionMethodHavingOneArgumentOfTypeFacts());
			}
			catch (Exception)
			{
				Assert.Fail("Should not throw exception for valid rule definitions");
			}
		}

		[Fact]
		public void AsRuleForPoco()
		{
			Assert.Throws<ArgumentException>(() => RuleProxy.AsRule(new object()));
		}

		[Fact]
		public void AsRuleForObjectThatImplementsIRule()
		{
			try
			{
				var rule = RuleProxy.AsRule(new WeatherRule());
			}
			catch (Exception)
			{
				Assert.Fail("Should not throw exception for valid rule definitions");
			}
		}

		[Fact]
		public void AsRuleForObjectThatHasProxied()
		{
			var rule = new DummyRule();
			var proxy1 = RuleProxy.AsRule(rule);
			var proxy2 = RuleProxy.AsRule(proxy1);

			Assert.Equal(proxy1.Name, proxy2.Name);
			Assert.Equal(proxy1.Description, proxy2.Description);
		}

		[Fact]
		public void AsRuleWellFormedAnnotatedRule()
		{
			try
			{
				var rule = RuleProxy.AsRule(new AnnotatedWeatherRule());
			}
			catch (Exception)
			{
				Assert.Fail("Should not throw exception for valid rule definitions");
			}
		}

		[Fact]
		public void InvokeConditionForProxyRule()
		{
			try
			{
				var rule = RuleProxy.AsRule(new AnnotatedWeatherRule());
				rule.Evaluate(new Facts() { {"rain", true } });
			}
			catch (Exception)
			{
				Assert.Fail("Should not throw exception for valid rule definitions");
			}
		}

		[Fact]
		public void InvokeActionForProxyRule()
		{
			try
			{
				var rule = RuleProxy.AsRule(new AnnotatedWeatherRule());
				rule.Execute(new Facts() { { "rain", true } });
			}
			catch (Exception)
			{
				Assert.Fail("Should not throw exception for valid rule definitions");
			}
		}

		[Fact]
		public void InvokeEquals()
		{
			object rule = new DummyRule();
			var proxy1 = RuleProxy.AsRule(rule);
			var proxy2 = RuleProxy.AsRule(proxy1);
			var proxy3 = RuleProxy.AsRule(proxy2);

			// Reflexive
			Assert.Equal(rule, rule);
			Assert.Equal(proxy1, proxy1);
			Assert.Equal(proxy2, proxy2);
			Assert.Equal(proxy3, proxy3);

			// Symmetric
			Assert.NotEqual(rule, proxy1);
			Assert.NotEqual(proxy1, rule);
			Assert.Equal(proxy1, proxy2);
			Assert.Equal(proxy2, proxy1);

			// Transitive Consistent
			Assert.Equal(proxy1, proxy2);
			Assert.Equal(proxy2, proxy3);
			Assert.Equal(proxy3, proxy1);

			// Non-Null
			Assert.NotNull(rule);
			Assert.NotNull(proxy1);
			Assert.NotNull(proxy2);
			Assert.NotNull(proxy3);
		}
	}
}
