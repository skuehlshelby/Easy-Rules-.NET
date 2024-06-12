Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Logging.Abstractions

Public NotInheritable Class Constants
	Public Const DEFAULT_RULE_DESCRIPTION As String = "No Description Provided"
	Public Const DEFAULT_RULE_PRIORITY As Integer = Integer.MaxValue - 1
End Class

Public NotInheritable Class Rule
	Implements IRule

	Private ReadOnly _predicate As Predicate(Of IFacts)
	Private ReadOnly _action As Action(Of IFacts)

	Public Sub New(name As String, predicate As Predicate(Of IFacts), action As Action(Of IFacts))
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, Constants.DEFAULT_RULE_PRIORITY, predicate, action)
	End Sub

	Public Sub New(name As String, priority As Integer, predicate As Predicate(Of IFacts), action As Action(Of IFacts))
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, priority, predicate, action)
	End Sub

	Public Sub New(name As String, description As String, condition As Predicate(Of IFacts), action As Action(Of IFacts))
		Me.New(name, description, Constants.DEFAULT_RULE_PRIORITY, condition, action)
	End Sub

	Public Sub New(name As String, description As String, priority As Integer, predicate As Predicate(Of IFacts), action As Action(Of IFacts))
		Me.Name = Guard.NotNullOrWhiteSpace(name, NameOf(name))
		Me.Description = Guard.NotNullOrWhiteSpace(description, NameOf(description))
		Me.Priority = priority
		_predicate = Guard.NotNull(predicate, NameOf(predicate))
		_action = Guard.NotNull(action, NameOf(action))
	End Sub

	Public ReadOnly Property Name As String Implements IRule.Name
	Public ReadOnly Property Description As String Implements IRule.Description
	Public ReadOnly Property Priority As Integer Implements IRule.Priority

	Public Sub Execute(facts As IFacts) Implements IRule.Execute
		_action.Invoke(Guard.NotNull(facts, NameOf(facts)))
	End Sub

	Public Function Evaluate(facts As IFacts) As Boolean Implements IRule.Evaluate
		Return _predicate.Invoke(Guard.NotNull(facts, NameOf(facts)))
	End Function

	Public Overrides Function ToString() As String
		Return $"{Name}: {Description}"
	End Function
End Class

Public MustInherit Class Rule(Of TFact)
	Implements IRule

	Private ReadOnly _evaluateArgumentName As String
	Private ReadOnly _executeArgumentName As String

	Public Sub New()
		Dim evaluateMethod = [GetType]().GetMethod(NameOf(Evaluate), New Type() {GetType(TFact)})
		_evaluateArgumentName = evaluateMethod.GetParameters().Single().Name
		Dim executeMethod = [GetType]().GetMethod(NameOf(Execute), New Type() {GetType(TFact)})
		_executeArgumentName = executeMethod.GetParameters().Single().Name
	End Sub

	Public MustOverride ReadOnly Property Name As String Implements IRule.Name
	Public Overridable ReadOnly Property Description As String Implements IRule.Description
	Public Overridable ReadOnly Property Priority As Integer Implements IRule.Priority

	Private Overloads Sub Execute(facts As IFacts) Implements IRule.Execute
		Guard.NotNull(facts, NameOf(facts))

		Execute(facts.Get(Of TFact)(_executeArgumentName))
	End Sub

	Private Overloads Function Evaluate(facts As IFacts) As Boolean Implements IRule.Evaluate
		Guard.NotNull(facts, NameOf(facts))

		Dim fact As IFact = Nothing

		If facts.TryGetFactByName(_evaluateArgumentName, fact) AndAlso TypeOf fact Is IFact(Of TFact) Then
			Evaluate(DirectCast(fact, IFact(Of TFact)).Value)
		End If

		Return False
	End Function

	Public MustOverride Overloads Sub Execute(fact As TFact)
	Public MustOverride Overloads Function Evaluate(fact As TFact) As Boolean
End Class

Public NotInheritable Class Fact
	Private Sub New()
	End Sub

	Public Shared Function Create(Of T)(name As String, value As T) As IFact(Of T)
		Return New Fact(Of T)(Guard.NotNullOrWhiteSpace(name, NameOf(name)), value)
	End Function
End Class

Public NotInheritable Class Fact(Of T)
	Implements IFact(Of T)

	Public Sub New(name As String, value As T)
		Me.Value = value
		IFact_Value = value
		Me.Name = Guard.NotNullOrWhiteSpace(name, NameOf(name))
	End Sub

	Public ReadOnly Property Name As String Implements IFact.Name
	Public ReadOnly Property Value As T Implements IFact(Of T).Value
	Private ReadOnly Property IFact_Value As Object Implements IFact.Value

	Public Overrides Function ToString() As String
		Return $"{Name}: {Value}"
	End Function
End Class

Public NotInheritable Class FactKey(Of T)
	Implements IFactKey(Of T)

	Public Sub New(name As String)
		Me.Name = Guard.NotNullOrWhiteSpace(name, NameOf(name))
	End Sub

	Public ReadOnly Property Name As String Implements IFactKey(Of T).Name

	Public Overrides Function ToString() As String
		Return $"({Name}, {GetType(T).Name})"
	End Function
End Class

Public NotInheritable Class EquateRulesByName
	Inherits EqualityComparer(Of IRule)

	Public Overrides Function Equals(firstRule As IRule, secondRule As IRule) As Boolean
		If firstRule Is Nothing AndAlso secondRule Is Nothing Then
			Return True
		ElseIf firstRule Is Nothing OrElse secondRule Is Nothing Then
			Return False
		Else
			Return firstRule.Name.Equals(secondRule.Name, StringComparison.OrdinalIgnoreCase)
		End If
	End Function

	Public Overrides Function GetHashCode(rule As IRule) As Integer
		Return rule.Name.GetHashCode()
	End Function

	Public Shared ReadOnly Instance As IEqualityComparer(Of IRule) = New EquateRulesByName()
End Class

Public NotInheritable Class EquateFactsByName
	Inherits EqualityComparer(Of IFact)

	Public Overrides Function Equals(firstFact As IFact, secondFact As IFact) As Boolean
		If firstFact Is Nothing AndAlso secondFact Is Nothing Then
			Return True
		ElseIf firstFact Is Nothing OrElse secondFact Is Nothing Then
			Return False
		Else
			Return firstFact.Name.Equals(secondFact.Name, StringComparison.OrdinalIgnoreCase)
		End If
	End Function

	Public Overrides Function GetHashCode(fact As IFact) As Integer
		Return fact.Name.GetHashCode()
	End Function

	Public Shared ReadOnly Instance As IEqualityComparer(Of IFact) = New EquateFactsByName()
End Class

Public NotInheritable Class OrderRulesByPriority
	Inherits Comparer(Of IRule)

	Public Overrides Function Compare(firstRule As IRule, secondRule As IRule) As Integer
		Return firstRule.Priority.CompareTo(secondRule.Priority)
	End Function

	Public Shared ReadOnly Instance As IComparer(Of IRule) = New OrderRulesByPriority()
End Class

Public NotInheritable Class Rules
	Implements IRules

	Private ReadOnly _rules As HashSet(Of IRule)

	Public Sub New()
		_rules = New HashSet(Of IRule)(EquateRulesByName.Instance)
	End Sub

	Public Sub New(rules As IEnumerable(Of IRule))
		_rules = New HashSet(Of IRule)(rules, EquateRulesByName.Instance)
	End Sub

	Public Sub New(comparer As IEqualityComparer(Of IRule))
		_rules = New HashSet(Of IRule)(comparer)
	End Sub

	Public Sub New(rules As IEnumerable(Of IRule), comparer As IEqualityComparer(Of IRule))
		_rules = New HashSet(Of IRule)(rules, comparer)
	End Sub

	Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of IRule).Count
		Get
			Return _rules.Count
		End Get
	End Property

	Public Function Add(rule As IRule) As Boolean Implements IRules.Add
		Return _rules.Add(Guard.NotNull(rule, NameOf(rule)))
	End Function

	Public Function Remove(rule As IRule) As Boolean Implements IRules.Remove
		Return _rules.Remove(rule)
	End Function

	Public Function Remove(name As String) As Boolean
		Return 0 < _rules.RemoveWhere(Function(r) r.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
	End Function

	Public Sub Clear() Implements IRules.Clear
		_rules.Clear()
	End Sub

	Public Function GetEnumerator() As IEnumerator(Of IRule) Implements IEnumerable(Of IRule).GetEnumerator
		Return _rules.OrderBy(Function(r) r.Priority).GetEnumerator()
	End Function

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function

	Public Overrides Function ToString() As String
		Return $"{NameOf(Rules)}, {NameOf(Count)}: {Count}"
	End Function
End Class

Public NotInheritable Class Facts
	Implements IFacts

	Private ReadOnly _facts As HashSet(Of IFact)

	Public Sub New()
		_facts = New HashSet(Of IFact)(EquateFactsByName.Instance)
	End Sub

	Public Sub New(facts As IEnumerable(Of IFact))
		_facts = New HashSet(Of IFact)(facts, EquateFactsByName.Instance)
	End Sub

	Public Sub New(comparer As IEqualityComparer(Of IFact))
		_facts = New HashSet(Of IFact)(comparer)
	End Sub

	Public Sub New(facts As IEnumerable(Of IFact), comparer As IEqualityComparer(Of IFact))
		_facts = New HashSet(Of IFact)(facts, comparer)
	End Sub

	Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of IFact).Count
		Get
			Return _facts.Count
		End Get
	End Property

	Public Function Add(fact As IFact) As Boolean Implements IFacts.Add
		Return _facts.Add(Guard.NotNull(fact, NameOf(fact)))
	End Function

	Public Function Add(Of T)(name As String, value As T) As Boolean
		Guard.NotNullOrWhiteSpace(name, NameOf(name))

		Return _facts.Add(New Fact(Of T)(name, value))
	End Function

	Public Function Remove(fact As IFact) As Boolean Implements IFacts.Remove
		Return _facts.Remove(fact)
	End Function

	Public Function Remove(name As String) As Boolean
		Guard.NotNullOrWhiteSpace(name, NameOf(name))

		Return 0 < _facts.RemoveWhere(Function(f) f.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
	End Function

	Public Sub Clear() Implements IFacts.Clear
		_facts.Clear()
	End Sub

	Public Function GetEnumerator() As IEnumerator(Of IFact) Implements IEnumerable(Of IFact).GetEnumerator
		Return _facts.GetEnumerator()
	End Function

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function

	Public Overrides Function ToString() As String
		Return $"{NameOf(Facts)}, {NameOf(Count)}: {Count}"
	End Function
End Class

Public NotInheritable Class RulesEngineParameters
	Implements IRulesEngineParameters

	Public Sub New(Optional skipOnFirstAppliedRule As Boolean = False, Optional skipOnFirstNonTriggeredRule As Boolean = False, Optional skipOnFirstFailedRule As Boolean = False, Optional priorityThreshold As Integer = Integer.MaxValue, Optional loggerFactory As ILoggerFactory = Nothing)
		Me.PriorityThreshold = priorityThreshold
		Me.SkipOnFirstAppliedRule = skipOnFirstAppliedRule
		Me.SkipOnFirstNonTriggeredRule = skipOnFirstNonTriggeredRule
		Me.SkipOnFirstFailedRule = skipOnFirstFailedRule
		Me.LoggerFactory = If(loggerFactory, NullLoggerFactory.Instance)
	End Sub

	Public Sub New(parameters As IRulesEngineParameters, Optional skipOnFirstAppliedRule As Boolean? = Nothing, Optional skipOnFirstNonTriggeredRule As Boolean? = Nothing, Optional skipOnFirstFailedRule As Boolean? = Nothing, Optional priorityThreshold As Integer? = Nothing, Optional loggerFactory As ILoggerFactory = Nothing)
		Me.PriorityThreshold = If(priorityThreshold, parameters.PriorityThreshold)
		Me.SkipOnFirstAppliedRule = If(skipOnFirstAppliedRule, parameters.SkipOnFirstAppliedRule)
		Me.SkipOnFirstNonTriggeredRule = If(skipOnFirstNonTriggeredRule, parameters.SkipOnFirstNonTriggeredRule)
		Me.SkipOnFirstFailedRule = If(skipOnFirstFailedRule, parameters.SkipOnFirstFailedRule)
		Me.LoggerFactory = If(loggerFactory, parameters.LoggerFactory)
	End Sub

	Public ReadOnly Property PriorityThreshold As Integer Implements IRulesEngineParameters.PriorityThreshold
	Public ReadOnly Property SkipOnFirstAppliedRule As Boolean Implements IRulesEngineParameters.SkipOnFirstAppliedRule
	Public ReadOnly Property SkipOnFirstNonTriggeredRule As Boolean Implements IRulesEngineParameters.SkipOnFirstNonTriggeredRule
	Public ReadOnly Property SkipOnFirstFailedRule As Boolean Implements IRulesEngineParameters.SkipOnFirstFailedRule
	Public ReadOnly Property LoggerFactory As ILoggerFactory Implements IRulesEngineParameters.LoggerFactory

	Public Overrides Function ToString() As String
		Dim properties As String() = {NameOf(SkipOnFirstAppliedRule), NameOf(SkipOnFirstNonTriggeredRule), NameOf(SkipOnFirstFailedRule), NameOf(PriorityThreshold)}
		Dim values As String() = {SkipOnFirstAppliedRule.ToString(), SkipOnFirstNonTriggeredRule.ToString(), SkipOnFirstFailedRule.ToString(), PriorityThreshold.ToString()}

		Return $"{NameOf(RulesEngineParameters)} {{{String.Join(", ", properties.Zip(values, Function(prop, value) $"{prop}: {value}"))}}}"
	End Function
End Class

Public Class DefaultRulesEngine
	Implements IRulesEngine

	Public Event BeforeRuleEvaluation As EventHandler(Of RuleEventArgs) Implements IRulesEngine.BeforeRuleEvaluation
	Public Event AfterRuleEvaluation As EventHandler(Of RuleEventArgs) Implements IRulesEngine.AfterRuleEvaluation
	Public Event BeforeRuleExecution As EventHandler(Of RuleEventArgs) Implements IRulesEngine.BeforeRuleExecution
	Public Event AfterRuleExecution As EventHandler(Of RuleEventArgs) Implements IRulesEngine.AfterRuleExecution
	Public Event OnRuleEvaluationError As EventHandler(Of RuleFailureEventArgs) Implements IRulesEngine.OnRuleEvaluationError
	Public Event OnRuleExecutionError As EventHandler(Of RuleFailureEventArgs) Implements IRulesEngine.OnRuleExecutionError
	Public Event BeforeRulesEvaluation As EventHandler(Of RulesEngineEventArgs) Implements IRulesEngine.BeforeRulesEvaluation
	Public Event AfterRulesEvaluation As EventHandler(Of RulesEngineEventArgs) Implements IRulesEngine.AfterRulesEvaluation

	Protected ReadOnly _logger As ILogger

	Public Sub New()
		Me.New(New RulesEngineParameters())
	End Sub

	Public Sub New(parameters As IRulesEngineParameters)
		Me.Parameters = Guard.NotNull(parameters, NameOf(parameters))
		_logger = parameters.LoggerFactory.CreateLogger(Of DefaultRulesEngine)()
	End Sub

	Public ReadOnly Property Parameters As IRulesEngineParameters Implements IRulesEngine.Parameters

	Public Function Evaluate(rules As IRules, facts As IFacts) As IEnumerable(Of KeyValuePair(Of IRule, Boolean)) Implements IRulesEngine.Evaluate
		Guard.NotNull(rules, NameOf(rules))
		Guard.NotNull(facts, NameOf(facts))

		Try
			RaiseEvent BeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

			Return DoCheck(rules, facts)
		Finally
			RaiseEvent AfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
		End Try
	End Function

	Private Iterator Function DoCheck(rules As IRules, facts As IFacts) As IEnumerable(Of KeyValuePair(Of IRule, Boolean))
		Guard.NotNull(rules, NameOf(rules))
		Guard.NotNull(facts, NameOf(facts))

		_logger.LogInformation("Checking rules")

		For Each rule As IRule In rules
			If ShouldBeEvaluated(rule, facts) Then
				Yield New KeyValuePair(Of IRule, Boolean)(rule, rule.Evaluate(facts))
			End If
		Next rule
	End Function

	Private Function ShouldBeEvaluated(rule As IRule, facts As IFacts) As Boolean
		Guard.NotNull(rule, NameOf(rule))
		Guard.NotNull(facts, NameOf(facts))

		Dim e As New RuleEventArgs(rule, facts)

		RaiseEvent BeforeRuleEvaluation(Me, e)

		Return Not e.Cancel
	End Function

	Public Sub Execute(rules As IRules, facts As IFacts) Implements IRulesEngine.Execute
		Guard.NotNull(rules, NameOf(rules))
		Guard.NotNull(facts, NameOf(facts))

		RaiseEvent BeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

		DoFire(rules, facts)

		RaiseEvent AfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
	End Sub

	Protected Overridable Sub DoFire(rules As IRules, facts As IFacts)
		If Not rules.Any() Then
			Return
		End If

		_logger.LogInformation("Beginning evaluation of rules")
		_logger.LogInformation("{Rules}", rules)
		_logger.LogInformation("{Facts}", facts.ToString())


		For Each rule As IRule In rules
			If rule.Priority > Parameters.PriorityThreshold Then
				_logger.LogInformation("Rule priority threshold ({PriorityThreshold}) exceeded at rule '{RuleName}' with priority={Priority}. The next rules will be skipped.", Parameters.PriorityThreshold, rule.Name, rule.Priority)
				Exit For
			End If

			If Not ShouldBeEvaluated(rule, facts) Then
				_logger.LogInformation("Rule '{RuleName}' has been skipped before being evaluated.", rule.Name)
				Continue For
			End If

			Dim evaluationResult As Boolean = False

			Try
				evaluationResult = rule.Evaluate(facts)
			Catch ex As Exception
				_logger.LogError("Rule '{RuleName}' produced error {ex}.", rule.Name, ex)
				RaiseEvent OnRuleEvaluationError(Me, New RuleFailureEventArgs(rule, facts, ex))

				If Parameters.SkipOnFirstNonTriggeredRule Then
					_logger.LogInformation("Subsequent rules will be skipped because {Parameters} is {SkipOnFirstNonTriggeredRule}", NameOf(Parameters.SkipOnFirstNonTriggeredRule), Parameters.SkipOnFirstNonTriggeredRule)
					Exit For
				End If
			End Try

			If evaluationResult Then
				_logger.LogInformation("Rule '{RuleName}' was triggered", rule.Name)
				RaiseEvent AfterRuleEvaluation(Me, New RuleEventArgs(rule, facts))

				Try
					RaiseEvent BeforeRuleExecution(Me, New RuleEventArgs(rule, facts))

					rule.Execute(facts)

					_logger.LogInformation("Rule '{RuleName}' was performed successfully", rule.Name)

					RaiseEvent AfterRuleExecution(Me, New RuleEventArgs(rule, facts))

					If Parameters.SkipOnFirstAppliedRule Then
						_logger.LogInformation("Subsequent rules will be skipped because {Parameters} is {SkipOnFirstAppliedRule}", NameOf(Parameters.SkipOnFirstAppliedRule), Parameters.SkipOnFirstAppliedRule)
						Exit For
					End If

				Catch ex As Exception
					_logger.LogError("Rule '{RuleName}' was performed with error {Exception}", rule.Name, ex)
					RaiseEvent OnRuleExecutionError(Me, New RuleFailureEventArgs(rule, facts, ex))

					If Parameters.SkipOnFirstFailedRule Then
						Exit For
					End If
				End Try
			Else
				_logger.LogInformation("Rule '{RuleName}' evaluated to false and was not executed.", rule.Name)
				RaiseEvent AfterRuleEvaluation(Me, New RuleEventArgs(rule, facts))

				If Parameters.SkipOnFirstNonTriggeredRule Then
					_logger.LogInformation("Subsequent rules will be skipped because {Parameters} is {SkipOnFirstNonTriggeredRule}", NameOf(Parameters.SkipOnFirstNonTriggeredRule), Parameters.SkipOnFirstNonTriggeredRule)
					Exit For
				End If
			End If
		Next rule
	End Sub
End Class

Public NotInheritable Class InferenceRulesEngine
	Implements IRulesEngine

	Public Event BeforeRuleEvaluation As EventHandler(Of RuleEventArgs) Implements IRulesEngine.BeforeRuleEvaluation
	Public Event AfterRuleEvaluation As EventHandler(Of RuleEventArgs) Implements IRulesEngine.AfterRuleEvaluation
	Public Event BeforeRuleExecution As EventHandler(Of RuleEventArgs) Implements IRulesEngine.BeforeRuleExecution
	Public Event AfterRuleExecution As EventHandler(Of RuleEventArgs) Implements IRulesEngine.AfterRuleExecution
	Public Event OnRuleEvaluationError As EventHandler(Of RuleFailureEventArgs) Implements IRulesEngine.OnRuleEvaluationError
	Public Event OnRuleExecutionError As EventHandler(Of RuleFailureEventArgs) Implements IRulesEngine.OnRuleExecutionError
	Public Event BeforeRulesEvaluation As EventHandler(Of RulesEngineEventArgs) Implements IRulesEngine.BeforeRulesEvaluation
	Public Event AfterRulesEvaluation As EventHandler(Of RulesEngineEventArgs) Implements IRulesEngine.AfterRulesEvaluation

	Private ReadOnly _rulesEngine As IRulesEngine
	Private ReadOnly _logger As ILogger

	Public Sub New()
		Me.New(New RulesEngineParameters())
	End Sub

	Public Sub New(parameters As IRulesEngineParameters)
		_rulesEngine = New DefaultRulesEngine(parameters)

		AddHandler _rulesEngine.BeforeRuleEvaluation, Sub(sender As Object, e As RuleEventArgs) RaiseEvent BeforeRuleEvaluation(Me, e)
		AddHandler _rulesEngine.BeforeRuleExecution, Sub(sender As Object, e As RuleEventArgs) RaiseEvent BeforeRuleExecution(Me, e)
		AddHandler _rulesEngine.AfterRuleEvaluation, Sub(sender As Object, e As RuleEventArgs) RaiseEvent AfterRuleEvaluation(Me, e)
		AddHandler _rulesEngine.AfterRuleExecution, Sub(sender As Object, e As RuleEventArgs) RaiseEvent AfterRuleExecution(Me, e)
		AddHandler _rulesEngine.BeforeRulesEvaluation, Sub(sender As Object, e As RulesEngineEventArgs) RaiseEvent BeforeRulesEvaluation(Me, e)
		AddHandler _rulesEngine.AfterRulesEvaluation, Sub(sender As Object, e As RulesEngineEventArgs) RaiseEvent AfterRulesEvaluation(Me, e)
		AddHandler _rulesEngine.OnRuleEvaluationError, Sub(sender As Object, e As RuleFailureEventArgs) RaiseEvent OnRuleEvaluationError(Me, e)
		AddHandler _rulesEngine.OnRuleExecutionError, Sub(sender As Object, e As RuleFailureEventArgs) RaiseEvent OnRuleExecutionError(Me, e)

		_logger = parameters.LoggerFactory.CreateLogger(Of InferenceRulesEngine)()
	End Sub

	Public ReadOnly Property Parameters As IRulesEngineParameters Implements IRulesEngine.Parameters
		Get
			Return _rulesEngine.Parameters
		End Get
	End Property

	Public Sub Execute(rules As IRules, facts As IFacts) Implements IRulesEngine.Execute
		Guard.NotNull(rules, NameOf(rules))
		Guard.NotNull(facts, NameOf(facts))

		Dim selectedRules As IEnumerable(Of IRule)

		Do
			_logger.LogDebug("Selecting candidate rules based on the following {facts}", facts)
			selectedRules = SelectCandidates(rules, facts)

			If selectedRules.Any() Then
				_rulesEngine.Execute(New Rules(selectedRules.ToArray()), facts)
			Else
				_logger.LogDebug("No candidate rules found for {facts}", facts)
			End If
		Loop Until Not selectedRules.Any()
	End Sub

	Private Shared Function SelectCandidates(rules As IRules, facts As IFacts) As IEnumerable(Of IRule)
		Return rules.Where(Function(r) r.Evaluate(facts))
	End Function

	Public Function Evaluate(rules As IRules, facts As IFacts) As IEnumerable(Of KeyValuePair(Of IRule, Boolean)) Implements IRulesEngine.Evaluate
		Guard.NotNull(rules, NameOf(rules))
		Guard.NotNull(facts, NameOf(facts))

		Return _rulesEngine.Evaluate(rules, facts)
	End Function
End Class