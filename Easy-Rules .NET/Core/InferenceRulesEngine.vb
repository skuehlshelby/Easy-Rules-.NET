Imports Easy_Rules_.NET.API
Imports Microsoft.Extensions.Logging

Namespace Core

    Public Class InferenceRulesEngine
        Inherits RulesEngine

        Private ReadOnly _rulesEngine As RulesEngine
        Private ReadOnly _logger As ILogger = GetLogger(Of InferenceRulesEngine)()

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
                _logger.LogDebug("Selecting candidate rules based on the following {facts}", facts)
                selectedRules = SelectCandidates(rules, facts)

                If selectedRules.Any() Then
                    _rulesEngine.Fire(New Rules(selectedRules.ToArray()), facts)
                Else
                    _logger.LogDebug("No candidate rules found for {facts}", facts)
                End If
            Loop Until selectedRules.Count = 0
        End Sub

        Private Shared Function SelectCandidates(rules As Rules, facts As Facts) As HashSet(Of Rule)
            Return rules.Where(Function(r) r.Evaluate(facts)).ToHashSet()
        End Function

        Public Overrides Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            RequireNonNull(rules, NameOf(rules))
            RequireNonNull(facts, NameOf(facts))

            Return _rulesEngine.Check(rules, facts)
        End Function
    End Class

End Namespace

