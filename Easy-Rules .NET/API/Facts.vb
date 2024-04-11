Namespace API
	Public Class Facts
		Implements ICollection(Of IFact), IFacts

		Private ReadOnly _facts As ISet(Of IFact) = New HashSet(Of IFact)

		Public ReadOnly Property Count As Integer Implements ICollection(Of IFact).Count, IFacts1.Count
			Get
				Return _facts.Count
			End Get
		End Property

		Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of IFact).IsReadOnly, IFacts1.IsReadOnly
			Get
				Return False
			End Get
		End Property

		Public Function Contains(item As IFact) As Boolean Implements ICollection(Of IFact).Contains, IFacts1.Contains
			Return _facts.Contains(item)
		End Function

		Public Function Contains(factName As String) As Boolean Implements IFacts1.Contains
			Return _facts.Any(Function(fact) String.Equals(fact.Name, factName, StringComparison.CurrentCultureIgnoreCase))
		End Function

		Public Sub Add(fact As IFact) Implements ICollection(Of IFact).Add, IFacts1.Add
			If fact Is Nothing Then Throw New ArgumentNullException(NameOf(fact))

			Remove(fact)

			_facts.Add(fact)
		End Sub

		Public Sub Add(Of T)(name As String, value As T) Implements IFacts1.Add
			If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentException($"'{NameOf(name)}' cannot be null or empty.", NameOf(name))
			If value Is Nothing Then Throw New ArgumentNullException(NameOf(value))

			Add(New Fact(name, value))
		End Sub

		Public Sub Remove(factName As String) Implements IFacts1.Remove
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			If Contains(factName) Then
				Remove(GetFact(factName))
			End If
		End Sub

		Public Function Remove(fact As IFact) As Boolean Implements ICollection(Of IFact).Remove, IFacts1.Remove
			If fact Is Nothing Then Throw New ArgumentNullException(NameOf(fact))

			Return _facts.Remove(fact)
		End Function

		Public Function GetFact(factName As String) As IFact Implements IFacts1.GetFact
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			Return _facts.Single(Function(fact) fact.Name = factName)
		End Function

		Public Function GetFact(Of T)(factName As String, value As T) As Boolean Implements IFacts1.GetFact
			If String.IsNullOrWhiteSpace(factName) Then Throw New ArgumentException($"'{NameOf(factName)}' cannot be null or empty.", NameOf(factName))

			Dim fact = _facts.FirstOrDefault(Function(f) f.Name = factName)

			If fact Is Nothing Then Return False

			Return fact.ValueEquals(value)
		End Function

		Public Function ToDictionary() As Dictionary(Of String, IFact) Implements IFacts1.ToDictionary
			Return _facts.ToDictionary(keySelector:=Function(fact) fact.Name, elementSelector:=Function(fact) fact)
		End Function

		Public Function GetEnumerator() As IEnumerator(Of IFact) Implements IEnumerable(Of IFact).GetEnumerator, IFacts1.GetEnumerator
			Return _facts.GetEnumerator()
		End Function

		Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
			Return DirectCast(_facts, IEnumerable).GetEnumerator()
		End Function

		Public Overrides Function ToString() As String Implements IFacts1.ToString
			Return $"{NameOf(Facts)}: {String.Join(", ", Me)}"
		End Function

	End Class
End Namespace
