Imports RulesEngine.API

Namespace Core

    Public Class InferenceRulesEngine
        Inherits API.RulesEngine

        Private ReadOnly _RulesEngine As API.RulesEngine

        Public Sub New()
            Me.New(New RulesEngineParameters())
        End Sub

        Public Sub New(parameters As RulesEngineParameters)
            MyBase.New(parameters)
            _RulesEngine = New DefaultRulesEngine(parameters)
        End Sub

        Public Overrides Sub Fire(rules As Rules, facts As Facts)
            NullGuard.RequireNonNull(rules, NameOf(rules))
            NullGuard.RequireNonNull(facts, NameOf(facts))

            Dim selectedRules As ISet(Of Rule)

            Do
                selectedRules = SelectCandidates(rules, facts)

                If selectedRules.Any() Then
                    _RulesEngine.Fire(New Rules(selectedRules.ToArray()), facts)
                Else

                End If
            Loop Until selectedRules.Count = 0
        End Sub

        Private Function SelectCandidates(rules As Rules, facts As Facts) As SortedSet(Of Rule)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            NullGuard.RequireNonNull(rules, NameOf(rules))
            NullGuard.RequireNonNull(facts, NameOf(facts))

            Return _RulesEngine.Check(rules, facts)
        End Function
    End Class

End Namespace

