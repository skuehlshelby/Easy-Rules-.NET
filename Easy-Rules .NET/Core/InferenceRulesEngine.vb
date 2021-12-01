Imports Easy_Rules_.NET.API

Namespace Core

    Public Class InferenceRulesEngine
        Inherits RulesEngine

        Private ReadOnly _rulesEngine As RulesEngine

        Public Sub New()
            Me.New(New RulesEngineParameters())
        End Sub

        Public Sub New(parameters As RulesEngineParameters)
            MyBase.New(parameters)
            _rulesEngine = New DefaultRulesEngine(parameters)
        End Sub

        Public Overrides Sub Fire(rules As Rules, facts As Facts)
            RequireNonNull(rules, NameOf(rules))
            RequireNonNull(facts, NameOf(facts))

            Dim selectedRules As ISet(Of Rule)

            Do
                selectedRules = SelectCandidates(rules, facts)

                If selectedRules.Any() Then
                    _rulesEngine.Fire(New Rules(selectedRules.ToArray()), facts)
                Else

                End If
            Loop Until selectedRules.Count = 0
        End Sub

        Private Function SelectCandidates(rules As Rules, facts As Facts) As SortedSet(Of Rule)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            RequireNonNull(rules, NameOf(rules))
            RequireNonNull(facts, NameOf(facts))

            Return _rulesEngine.Check(rules, facts)
        End Function
    End Class

End Namespace

