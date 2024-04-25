using EasyRules;
using EasyRules.Attributes;
using System;
using Xunit;

namespace Tests.Annotation
{
	public sealed class AnnotationTests
	{
		[Xunit.Fact]
		public void NotAnnotatedRuleMustNotBeAccepted()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new object()));
		}

		[Xunit.Fact]
		public void ConditionMethodMustBeDefined()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithoutConditionMethod()));
		}

		[Xunit.Fact]
		public void ConditionMethodMustBePublic()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithNonPublicConditionMethod()));
		}

		[Xunit.Fact]
		public void WhenConditionMethodHasOneNonAnnotatedParameter_ThenThisParameterMustBeOfTypeFacts()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithConditionMethodHavingOneArgumentNotOfTypeFacts()));
		}

		[Xunit.Fact]
		public void ConditionMethodMustReturnBooleanType()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithConditionMethodHavingNonBooleanReturnType()));
		}

		[Xunit.Fact]
		public void ConditionMethodParametersShouldAllBeAnnotatedWithFactUnlessExactlyOneOfThemIsOfTypeFacts()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithOneParameterNotAnnotatedWithFactAndNotOfTypeFacts()));
		}

		[Xunit.Fact]
		public void ActionMethodMustHaveExactlyOneArgumentOfTypeFactsIfAny()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithActionMethodHavingMoreThanOneArgumentOfTypeFacts()));
		}

		[Xunit.Fact]
		public void ActionMethodMustBeDefined()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithoutActionMethod()));
		}

		[Xunit.Fact]
		public void ActionMethodMustBePublic()
		{
			Assert.Throws<ArgumentException>(() => RuleDefinitionValidator.ValidateRuleDefinition(new AnnotatedRuleWithNonPublicActionMethod()));
		}

		[Xunit.Fact]
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

		[Xunit.Fact]
		public void AsRuleForPojo()
		{
			Assert.Throws<ArgumentException>(() => RuleProxy.AsRule(new object()));
		}

		[Xunit.Fact]
		public void AsRuleObjectThatImplementsIRule()
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

		[Xunit.Fact]
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

		[Xunit.Fact]
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

		[Xunit.Fact]
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
	}
}
