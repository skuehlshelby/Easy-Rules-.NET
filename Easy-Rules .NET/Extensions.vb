Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Public Module EasyRulesExtensions
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
	Public Function IsTrue(facts As IFacts, factName As String) As Boolean
		Return facts.IsTrue(Of Boolean)(factName, Function(b) b)
	End Function

	<Extension>
	Public Function IsTrue(Of TFactValue)(facts As IFacts, factName As String, predicate As Predicate(Of TFactValue)) As Boolean
		If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

		Dim fact As IFact = Nothing
		Dim value As TFactValue = Nothing

		If facts.TryGetFactByName(factName, fact) AndAlso fact.TryGetValueAs(value) Then
			Return predicate.Invoke(value)
		Else
			Return False
		End If
	End Function
End Module
