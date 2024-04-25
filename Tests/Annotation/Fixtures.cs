using IFacts = EasyRules.IFacts;
using EasyRules.Attributes;
using EasyRules;
using System;

namespace Tests.Annotation
{
	[Rule(nameof(AnnotatedRuleWithActionMethodHavingMoreThanOneArgumentOfTypeFacts))]
	public sealed class AnnotatedRuleWithActionMethodHavingMoreThanOneArgumentOfTypeFacts
	{
		[Condition]
		public bool When() => true;

		[Action]
		public void Then(IFacts facts, IFacts otherFacts) { }
	}

	[Rule(nameof(AnnotatedRuleWithActionMethodHavingOneArgumentNotOfTypeFacts))]
	public sealed class AnnotatedRuleWithActionMethodHavingOneArgumentNotOfTypeFacts
	{
		private bool executed;

		[Condition]
		public bool When() => true;

		[Action]
		public void Then(int i)
		{
			if (i == 1) executed = true;
		}

		public bool IsExecuted() => executed;
	}

	[Rule(nameof(AnnotatedRuleWithActionMethodHavingOneArgumentOfTypeFacts))]
	public sealed class AnnotatedRuleWithActionMethodHavingOneArgumentOfTypeFacts
	{
		[Condition]
		public bool When() => true;

		[Action]
		public void Then(IFacts facts) { }
	}

	[Rule(nameof(AnnotatedRuleWithActionMethodThatReturnsNonVoidType))]
	public sealed class AnnotatedRuleWithActionMethodThatReturnsNonVoidType
	{
		[Condition]
		public bool When() => true;

		[Action]
		public int Then() => 0;
	}

	[Rule(nameof(AnnotatedRuleWithConditionMethodHavingNonBooleanReturnType))]
	public sealed class AnnotatedRuleWithConditionMethodHavingNonBooleanReturnType
	{
		[Condition]
		public int When() => 0;

		[Action]
		public void Then() { }
	}

	[Rule(nameof(AnnotatedRuleWithConditionMethodHavingOneArgumentNotOfTypeFacts))]
	public sealed class AnnotatedRuleWithConditionMethodHavingOneArgumentNotOfTypeFacts
	{
		[Condition]
		public bool When(int _) => true;

		[Action]
		public void Then() { }
	}

	[Rule(nameof(AnnotatedRuleWithMultipleAnnotatedParametersAndOneParameterOfTypeFacts))]
	public sealed class AnnotatedRuleWithMultipleAnnotatedParametersAndOneParameterOfTypeFacts
	{
		[Condition]
		public bool When([Fact("Fact1")] object fact1, [Fact("Fact2")] object fact2, IFacts facts) => true;

		[Action]
		public void Then([Fact("Fact1")] object fact1, [Fact("Fact2")] object fact2, IFacts facts) { }
	}

	[Rule(nameof(AnnotatedRuleWithNonPublicActionMethod))]
	public sealed class AnnotatedRuleWithNonPublicActionMethod
	{
		[Condition]
		public bool When(int _) => true;

		[Action]
		private void Then() { }
	}

	[Rule(nameof(AnnotatedRuleWithNonPublicConditionMethod))]
	public sealed class AnnotatedRuleWithNonPublicConditionMethod
	{
		[Condition]
		private bool When(int _) => true;

		[Action]
		public void Then() { }
	}

	[Rule(nameof(AnnotatedRuleWithOneParameterNotAnnotatedWithFactAndNotOfTypeFacts))]
	public sealed class AnnotatedRuleWithOneParameterNotAnnotatedWithFactAndNotOfTypeFacts
	{
		[Condition]
		public bool When([Fact("Fact1")] object fact1, object fact2) => true;

		[Action]
		public void Then([Fact("Fact1")] object fact1, object fact2) { }
	}

	[Rule(nameof(AnnotatedRuleWithoutActionMethod))]
	public sealed class AnnotatedRuleWithoutActionMethod
	{
		[Condition]
		public bool When(int _) => true;
	}

	[Rule(nameof(AnnotatedRuleWithoutConditionMethod))]
	public sealed class AnnotatedRuleWithoutConditionMethod
	{
		[Action]
		public void Then() { }
	}

	public sealed class WeatherRule : IRule
	{
		public string Name => "weather rule";
		public string Description => "if it rains then take an umbrella";
		public int Priority => int.MaxValue;
		public bool Evaluate(IFacts facts) => facts.IsTrue("rain");
		public void Execute(IFacts facts) => Console.WriteLine("It rains, take an umbrella!");
	}

	[Rule(nameof(AnnotatedWeatherRule))]
	public sealed class AnnotatedWeatherRule
	{
		[Condition]
		public bool Evaluate([Fact("rain")] bool rain) => rain;

		[Action]
		public void Execute() => Console.WriteLine("It rains, take an umbrella!");
	}
}
