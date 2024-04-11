
# Easy Rules: *The simple, stupid rules engine for .NET*

## What is Easy Rules?

Easy Rules is a .NET port of the [Easy Rules](https://github.com/j-easy/easy-rules) Java-based rules engine, which was inspired by an article called *"[Should I use a Rules Engine?](http://martinfowler.com/bliki/RulesEngine.html)"* by [Martin Fowler](http://martinfowler.com/) in which he states:

> You can build a simple rules engine yourself. All you need is to create a bunch of objects with conditions and actions, store them in a collection, and run through them to evaluate the conditions and execute the actions.

This is exactly what Easy Rules does, it provides the `Rule` abstraction to create rules with conditions and actions, and the `RulesEngine` API that runs through a set of rules to evaluate conditions and execute actions.

## Core features

 * Lightweight library and easy to learn API
 * POCO based development with an annotation programming model
 * Useful abstractions to define business rules and apply them easily with .NET
 * The ability to create composite rules from primitive ones
 * ~~The ability to define rules using an Expression Language~~ <- Maybe Later

 ## Example

### 1. First, define your rule...

#### Either in a declarative way using annotations:

```c#
[Rule(Name = "weather rule", Description = "if it rains then take an umbrella")]
public sealed class WeatherRule
{
    [Condition]
    public bool ItRains([Fact("rain")] bool rain) => rain;
    
    [Action]
    public void TakeAnUmbrella() {
        Console.WriteLine("It rains, take an umbrella!");
    }
}
```

#### Or in a programmatic way with a fluent API:

```c#
var weatherRule = new RuleBuilder()
        .Name("weather rule")
        .Description("if it rains then take an umbrella")
        .When(facts => facts.get("rain").Equals(true))
        .Then(facts => Console.WriteLine("It rains, take an umbrella!"))
        .Build();
```

### 2. Then, fire it!

```c#
public static class Test
{
    public static void Main(String[] args) {
        // define facts
        var facts = new Facts()
        {
            { "rain", true }
        };

        // define rules
        var weatherRule = ...
        var rules = new Rules();
        rules.register(weatherRule);

        // fire rules on known facts
        var rulesEngine = new DefaultRulesEngine();
        rulesEngine.fire(rules, facts);
    }
}
```

This is the hello world of Easy Rules. You can find other examples on the original [Easy Rules Wiki](https://github.com/j-easy/easy-rules/wiki).