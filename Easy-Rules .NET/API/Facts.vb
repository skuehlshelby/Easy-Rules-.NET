Imports RulesEngine.Core

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

        Public Sub Add(Name As String, Value As Object)
            NullGuard.RequireNonNull(Name, NameOf(Name))
            NullGuard.RequireNonNull(Value, NameOf(Value))

            Add(New Fact(Name, Value))
        End Sub

        Public Sub Add(fact As Fact)
            NullGuard.RequireNonNull(fact, NameOf(fact))

            Remove(fact)

            _facts.Add(fact)
        End Sub

        Public Sub Remove(factName As String)
            NullGuard.RequireNonNull(factName, NameOf(factName))

            If HasFact(factName) Then
                Remove(GetFact(factName))
            End If
        End Sub

        Public Sub Remove(fact As Fact)
            NullGuard.RequireNonNull(fact, NameOf(fact))

            _facts.Remove(fact)
        End Sub

        Public Function GetFact(factName As String) As Fact
            NullGuard.RequireNonNull(factName, NameOf(factName))

            Return _facts.Single(Function(fact) fact.Name = factName)
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
