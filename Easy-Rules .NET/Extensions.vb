Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Public Module EasyRulesExtensions
	<Extension>
	Public Function [Get](Of T)(facts As IFacts, factKey As IFactKey(Of T)) As T
		Return DirectCast(facts.First(Function(f) f.Name.Equals(factKey.Name, StringComparison.OrdinalIgnoreCase)), IFact(Of T)).Value
	End Function

	<Extension>
	Public Function [Get](Of T)(facts As IFacts, factName As String) As T
		Return DirectCast(facts.First(Function(f) f.Name.Equals(factName, StringComparison.OrdinalIgnoreCase)), IFact(Of T)).Value
	End Function

	<Extension>
	Public Function TryGetValueAs(Of T)(fact As IFact, <Out> ByRef value As T) As Boolean
		If TypeOf fact Is IFact(Of T) Then
			value = DirectCast(fact, IFact(Of T)).Value
			Return True
		Else
			Return False
		End If
	End Function

	<Extension>
	Public Function HasFact(facts As IFacts, factName As String) As Boolean
		If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

		Return facts.Any(Function(f) f.Name.Equals(factName, StringComparison.OrdinalIgnoreCase))
	End Function

	<Extension>
	Public Function GetFactByName(facts As IFacts, factName As String) As IFact
		If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

		Dim fact = facts.FirstOrDefault(Function(f) f.Name.Equals(factName, StringComparison.OrdinalIgnoreCase))

		If fact Is Nothing Then Throw New NoSuchFactException(factName)

		Return fact
	End Function

	<Extension>
	Public Function TryGetFactByName(facts As IFacts, factName As String, <Out> ByRef fact As IFact) As Boolean
		If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

		fact = facts.FirstOrDefault(Function(f) f.Name.Equals(factName, StringComparison.OrdinalIgnoreCase))

		Return fact IsNot Nothing
	End Function

	<Extension>
	Public Function [True](facts As IFacts, factName As String) As Boolean
		Return facts.True(Of Boolean)(factName, Function(b) b)
	End Function

	<Extension>
	Public Function [True](facts As IFacts, factKey As IFactKey(Of Boolean)) As Boolean
		Return facts.True(factKey, Function(b) b)
	End Function

	<Extension>
	Public Function [True](Of TFactValue)(facts As IFacts, factKey As IFactKey(Of TFactValue), predicate As Predicate(Of TFactValue)) As Boolean
		Return facts.True(factKey.Name, predicate)
	End Function

	<Extension>
	Public Function [True](Of TFactValue)(facts As IFacts, factName As String, predicate As Predicate(Of TFactValue)) As Boolean
		If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

		Dim fact As IFact = Nothing
		Dim value As TFactValue = Nothing

		If facts.TryGetFactByName(factName, fact) AndAlso fact.TryGetValueAs(value) Then
			Return predicate.Invoke(value)
		Else
			Return False
		End If
	End Function

	<Extension>
	Public Function Remove(facts As IFacts, factName As String) As Boolean
		Dim fact As IFact = Nothing

		If facts.TryGetFactByName(factName, fact) Then
			Return facts.Remove(fact)
		End If

		Return False
	End Function

	<Extension>
	Public Function Remove(rules As IRules, ruleName As String) As Boolean
		Dim rule As IRule = rules.FirstOrDefault(Function(r) r.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase))

		If rule IsNot Nothing Then
			Return rules.Remove(rule)
		End If

		Return False
	End Function

	<Extension>
	Public Function Key(Of T)(fact As IFact(Of T)) As IFactKey(Of T)
		Return New FactKey(Of T)(fact.Name)
	End Function

	<Extension>
	Public Function Key(Of T)(fact As IFact(Of T), <Out> ByRef factKey As IFactKey(Of T)) As IFact(Of T)
		If fact Is Nothing Then Throw New ArgumentNullException(NameOf(fact))

		factKey = New FactKey(Of T)(fact.Name)
		Return fact
	End Function
End Module
