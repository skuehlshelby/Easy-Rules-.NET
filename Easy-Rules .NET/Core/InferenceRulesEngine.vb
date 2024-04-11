Imports EasyRules.API
Imports Microsoft.Extensions.Logging

Namespace Core

    Public Class InferenceRulesEngine
        Inherits RulesEngine

        Private ReadOnly _rulesEngine As DefaultRulesEngine

        Public Sub New()
            Me.New(New RulesEngineParameters())
        End Sub

        Public Sub New(parameters As RulesEngineParameters)
            MyBase.New(parameters)
            _rulesEngine = New DefaultRulesEngine(parameters)
        End Sub

        Public Overrides Sub Fire(rules As Rules, facts As Facts)
            If rules Is Nothing Then Throw New ArgumentNullException(NameOf(rules))
            If facts Is Nothing Then Throw New ArgumentNullException(NameOf(facts))

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
            Return New HashSet(Of Rule)(rules.Where(Function(r) r.Evaluate(facts)))
        End Function

        Public Overrides Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            If rules Is Nothing Then Throw New ArgumentNullException(NameOf(rules))
            If facts Is Nothing Then Throw New ArgumentNullException(NameOf(facts))

            Return _rulesEngine.Check(rules, facts)
        End Function
    End Class

End Namespace

