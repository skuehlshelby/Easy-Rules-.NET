Imports System.Runtime.CompilerServices

Namespace API

	Public Interface IFact
		ReadOnly Property Name As String
	End Interface

	Public Interface IFact(Of Out T)
		Inherits IFact
		ReadOnly Property Value As T
	End Interface

	Public Interface IFacts
		Inherits IReadOnlyCollection(Of IFact)
		Function Add(fact As IFact) As Boolean
		Function Contains(fact As IFact) As Boolean
		Function Remove(fact As IFact) As Boolean
		Sub Clear()
	End Interface

	Public Interface IFactKey(Of Out T)
		ReadOnly Property Name As String
	End Interface

	Public Interface IRule
		ReadOnly Property Name As String
		ReadOnly Property Description As String
		ReadOnly Property Priority As Integer
		Function Evaluate(facts As IFacts) As Boolean
		Sub Execute(facts As IFacts)
	End Interface

	Public Interface IRules
		Inherits IReadOnlyCollection(Of IRule)
		Function Add(fact As IRule) As Boolean
		Function Contains(fact As IRule) As Boolean
		Function Remove(fact As IRule) As Boolean
		Sub Clear()
	End Interface

	Public Interface IRuleBuilder
		Function WithName(name As String) As IRuleBuilder
		Function WithDescription(description As String) As IRuleBuilder
		Function WithPriority(priority As Integer) As IRuleBuilder
		Function ThatWhen(condition As Func(Of IFacts, Boolean)) As IRuleBuilder
		Function ThenDoes(action As Action(Of IFacts)) As IRuleBuilder
	End Interface

	Public Interface IRulesEngine
		ReadOnly Property Parameters As RulesEngineParameters
		Event AfterRuleEvaluation As EventHandler(Of RuleEventArgs)
		Event AfterRuleExecution As EventHandler(Of RuleEventArgs)
		Event AfterRulesEvaluation As EventHandler(Of RulesEngineEventArgs)
		Event BeforeRuleEvaluation As EventHandler(Of RuleEventArgs)
		Event BeforeRuleExecution As EventHandler(Of RuleEventArgs)
		Event BeforeRulesEvaluation As EventHandler(Of RulesEngineEventArgs)
		Event OnRuleEvaluationError As EventHandler(Of RuleFailureEventArgs)
		Event OnRuleExecutionError As EventHandler(Of RuleFailureEventArgs)
		Function Check(rules As IRules, facts As IFacts) As Dictionary(Of IRule, Boolean)
		Sub Fire(rules As IRules, facts As IFacts)
	End Interface

	Public NotInheritable Class Fact(Of T)
		Implements IFact(Of T)

		Public Sub New(name As String, value As T)
			If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentNullException(NameOf(name))

			Me.Name = name
			Me.Value = value
		End Sub

		Public ReadOnly Property Name As String Implements IFact.Name
		Public ReadOnly Property Value As T Implements IFact(Of T).Value

		Public Overrides Function ToString() As String
			Return $"{NameOf(Fact)}{{{Name}, {Value}}}"
		End Function
	End Class

	Public NotInheritable Class Factss
		Implements IFacts

		Private ReadOnly _facts As HashSet(Of IFact)

		Public Sub New(ParamArray facts() As IFact)
			_facts = New HashSet(Of IFact)(facts, New FactEqualityComparer())
		End Sub

		Public Sub New(facts As IEnumerable(Of IFact))
			_facts = New HashSet(Of IFact)(facts, New FactEqualityComparer())
		End Sub

		Public ReadOnly Property Count As Integer Implements IFacts.Count
			Get
				Return _facts.Count
			End Get
		End Property

		Public Function Add(fact As IFact) As Boolean Implements IFacts.Add
			Return _facts.Add(fact)
		End Function

		Public Function Contains(item As IFact) As Boolean Implements IFacts.Contains
			Return _facts.Contains(item)
		End Function

		Public Function Remove(fact As IFact) As Boolean Implements IFacts.Remove
			Return _facts.Remove(fact)
		End Function

		Public Sub Clear() Implements IFacts.Clear
			_facts.Clear()
		End Sub

		Public Function GetEnumerator() As IEnumerator(Of IFact) Implements IEnumerable(Of IFact).GetEnumerator
			Return _facts.GetEnumerator()
		End Function

		Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
			Return _facts.GetEnumerator()
		End Function

		Private NotInheritable Class FactEqualityComparer
			Inherits EqualityComparer(Of IFact)

			Public Overrides Function Equals(x As IFact, y As IFact) As Boolean
				Return String.Equals(x.Name, y.Name)
			End Function

			Public Overrides Function GetHashCode(obj As IFact) As Integer
				Return obj.Name.GetHashCode()
			End Function
		End Class
	End Class

	Public Module FactExtensions

		<Extension>
		Public Function Add(rules As IRules, rule As Action(Of IRuleBuilder)) As Boolean
			Throw New NotImplementedException()

			Return False
		End Function

		<Extension>
		Public Function Add(Of TValue)(facts As IFacts, name As String, value As TValue) As Boolean
			Return facts.Add(New Fact(Of TValue)(name, value))
		End Function

		<Extension>
		Public Function Contains(facts As IFacts, factName As String) As Boolean
			Return facts.Any(Function(f) f.Name.Equals(factName))
		End Function

		<Extension>
		Public Function GetFact(facts As IFacts, factName As String) As IFact
			Return facts.Single(Function(f) f.Name.Equals(factName))
		End Function

		<Extension>
		Public Function IsTrue(facts As IFacts, factName As String) As Boolean
			Return facts.Contains(factName) AndAlso facts.GetFact(factName).ValueEquals(True)
		End Function

		<Extension>
		Public Function IsTrue(Of TValue)(facts As IFacts, factKey As IFactKey(Of TValue), predicate As Predicate(Of TValue)) As Boolean
			If facts.Contains(factKey.Name) Then
				Dim fact = facts.GetFact(factKey.Name)

				If TypeOf fact Is IFact(Of TValue) Then
					Return predicate.Invoke(DirectCast(fact, IFact(Of TValue)).Value)
				End If
			End If

			Return False
		End Function

		<Extension>
		Public Function ValueEquals(Of T)(fact As IFact, value As T) As Boolean
			Return fact IsNot Nothing AndAlso TypeOf fact Is IFact(Of T) AndAlso DirectCast(fact, IFact(Of T)).Value.Equals(value)
		End Function
	End Module

End Namespace
