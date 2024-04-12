Namespace API
	Public Class Facts
		Implements IEnumerable(Of Fact)

		Private ReadOnly _facts As ISet(Of Fact) = New HashSet(Of Fact)

		Public Function HasFact(factName As String) As Boolean
			Return _facts.Any(Function(fact) String.Equals(fact.Name, factName, StringComparison.CurrentCultureIgnoreCase))
		End Function

		Public Function HasFact(fact As Fact) As Boolean
			Return _facts.Contains(fact)
		End Function

		Public Sub Add(name As String, value As Object)
			If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentException($"'{NameOf(name)}' cannot be null or empty.", NameOf(name))
			If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))

			Add(New Fact(name, value))
		End Sub

		Public Sub Add(fact As Fact)
			If fact Is Nothing Then Throw New ArgumentNullException(NameOf(fact))

			Remove(fact)

			_facts.Add(fact)
		End Sub

		Public Sub Remove(factName As String)
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			If HasFact(factName) Then
				Remove(GetFact(factName))
			End If
		End Sub

		Public Sub Remove(fact As Fact)
			If fact Is Nothing Then Throw New ArgumentNullException(NameOf(fact))

			_facts.Remove(fact)
		End Sub

		Public Function GetFact(factName As String) As Fact
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			Return _facts.Single(Function(fact) fact.Name = factName)
		End Function

		Public Function IsTrue(factName As String) As Boolean
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			Dim fact = _facts.FirstOrDefault(Function(f) f.Name = factName)

			If fact Is Nothing Then Return False

			Return TypeOf fact.Value Is Boolean AndAlso DirectCast(fact.Value, Boolean)
		End Function

		Public Function IsTrue(Of TFactValue)(factName As String, predicate As Predicate(Of TFactValue)) As Boolean
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			Dim fact = _facts.FirstOrDefault(Function(f) f.Name = factName)

			If fact Is Nothing Then Return False

			Return TypeOf fact.Value Is TFactValue AndAlso predicate.Invoke(DirectCast(fact.Value, TFactValue))
		End Function

		Public Function ToDictionary() As Dictionary(Of String, Fact)
			Return _facts.ToDictionary(keySelector:=Function(fact) fact.Name, elementSelector:=Function(fact) fact)
		End Function

		Public Function GetEnumerator() As IEnumerator(Of Fact) Implements IEnumerable(Of Fact).GetEnumerator
			Return _facts.GetEnumerator()
		End Function

		Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
			Return DirectCast(_facts, IEnumerable).GetEnumerator()
		End Function

		Public Overrides Function ToString() As String
			Return $"{NameOf(Facts)}: {String.Join(", ", Me)}"
		End Function
	End Class
End Namespace
