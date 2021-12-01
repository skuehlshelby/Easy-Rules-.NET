Namespace API

    Public Class Rules
        Implements IEnumerable(Of Rule)

        Private ReadOnly _rules As List(Of Rule) = New List(Of Rule)

        Public Sub New(ParamArray rules As Rule())
            For Each rule As Rule In rules
                Register(rule)
            Next rule
        End Sub

        Public Sub New(rules As IEnumerable(Of Rule))
            For Each rule As Rule In rules
                Register(rule)
            Next rule
        End Sub

        Public Sub Register(rule As Rule)
            RequireNonNull(rule, NameOf(rule))

            Unregister(rule.Name)

            _rules.Add(rule)

            _rules.Sort()
        End Sub

        Public Sub Unregister(ruleName As String)
            RequireNonNull(ruleName, NameOf(ruleName))

            If HasRule(ruleName) Then
                _rules.Remove(FindRuleByName(ruleName))
            End If
        End Sub

        Private Function HasRule(ruleName As String) As Boolean
            Return _rules.Any(Function(r) r.Name.Equals(ruleName, StringComparison.CurrentCultureIgnoreCase))
        End Function

        Private Function FindRuleByName(ruleName As String) As Rule
            Return _rules.Single(Function(rule) String.Equals(rule.Name, ruleName, StringComparison.CurrentCultureIgnoreCase))
        End Function

        Public Function IsEmpty() As Boolean
            Return Not _rules.Any()
        End Function

        Public Sub Clear()
            _rules.Clear()
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return _rules.Count
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{NameOf(Rules)}: {String.Join(", ", Me)}"
        End Function

        Public Function GetEnumerator() As IEnumerator(Of Rule) Implements IEnumerable(Of Rule).GetEnumerator
            Return _rules.GetEnumerator()
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return _rules.GetEnumerator()
        End Function
    End Class

End Namespace

