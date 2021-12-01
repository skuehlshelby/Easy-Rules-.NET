Imports Easy_Rules_.NET.API
Imports Microsoft.Extensions.Logging

Namespace Core

    Public Class DefaultRulesEngine
        Inherits RulesEngine

        Private ReadOnly _logger As ILogger = GetLogger(Of DefaultRulesEngine)()

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(parameters As RulesEngineParameters)
            MyBase.New(parameters)
        End Sub

        Public Overrides Sub Fire(rules As Rules, facts As Facts)
            RequireNonNull(rules, NameOf(rules))
            RequireNonNull(facts, NameOf(facts))

            RaiseBeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

            DoFire(rules, facts)

            RaiseAfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
        End Sub

        Protected Overridable Sub DoFire(rules As Rules, facts As Facts)
            If rules.IsEmpty() Then
                Return
            End If

            _logger.LogInformation("Beginning evaluation of rules.")
            _logger.LogInformation(rules.ToString())
            _logger.LogInformation(facts.ToString())
            

            For Each rule As Rule In rules
                If rule.Priority > Parameters.PriorityThreshold Then
                    _logger.LogInformation($"Rule priority threshold ({Parameters.PriorityThreshold}) exceeded at rule '{rule.Name}' with priority={rule.Priority}. The next rules will be skipped.")
                    Exit For
                End If

                If Not ShouldBeEvaluated(rule, facts) Then
                    _logger.LogInformation($"Rule '{rule.Name}' has been skipped before being evaluated.")
                    Continue For
                End If

                Dim evaluationResult As Boolean = False

                Try
                    evaluationResult = rule.Evaluate(facts)
                Catch ex As Exception
                    _logger.LogError($"Rule '{rule.Name}' produced error {ex}.")
                    RaiseOnEvaluationError(Me, New RuleFailureEventArgs(rule, facts, ex))

                    If Parameters.SkipOnFirstNonTriggeredRule Then
                        _logger.LogInformation($"Subsequent rules will be skipped because {NameOf(Parameters.SkipOnFirstNonTriggeredRule)} is true.")
                        Exit For
                    End If
                End Try

                If evaluationResult Then
                    _logger.LogInformation($"Rule '{rule.Name}' was triggered.")
                    RaiseAfterEvaluate(Me, New RuleEventArgs(rule, facts))

                    Try
                        RaiseBeforeExecute(Me, New RuleEventArgs(rule, facts))

                        rule.Execute(facts)

                        _logger.LogInformation($"Rule '{rule.Name}' was performed successfully.")

                        RaiseAfterExecute(Me, New RuleEventArgs(rule, facts))

                        If Parameters.SkipOnFirstAppliedRule Then
                            _logger.LogInformation($"Subsequent rules will be skipped because {NameOf(Parameters.SkipOnFirstAppliedRule)} is true.")
                            Exit For
                        End If

                    Catch ex As Exception
                        _logger.LogError($"Rule '{rule.Name}' was performed with error {ex}.")
                        RaiseOnExecutionError(Me, New RuleFailureEventArgs(rule, facts, ex))

                        If Parameters.SkipOnFirstFailedRule Then
                            Exit For
                        End If
                    End Try
                Else
                    _logger.LogInformation($"Rule '{rule.Name}' evaluated to false and was not executed.")
                    RaiseAfterEvaluate(Me, New RuleEventArgs(rule, facts))

                    If Parameters.SkipOnFirstNonTriggeredRule Then
                        _logger.LogInformation($"Subsequent rules will be skipped because {NameOf(Parameters.SkipOnFirstNonTriggeredRule)} is true.")
                        Exit For
                    End If
                End If
            Next rule
        End Sub

        Private Function ShouldBeEvaluated(rule As Rule, facts As Facts) As Boolean
            Dim e As RuleEventArgs = New RuleEventArgs(rule, facts)

            RaiseBeforeRuleEvaluation(Me, e)

            Return Not e.Cancel
        End Function

        Public Overrides Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            RequireNonNull(rules, NameOf(rules))
            RequireNonNull(facts, NameOf(facts))

            Try
                RaiseBeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

                Return DoCheck(rules, facts)
            Finally
                RaiseAfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
            End Try
        End Function

        Private Function DoCheck(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            _logger.LogInformation("Checking rules.")

            Dim result As Dictionary(Of Rule, Boolean) = New Dictionary(Of Rule, Boolean)

            For Each rule As Rule In rules
                If ShouldBeEvaluated(rule, facts) Then
                    result.Add(rule, rule.Evaluate(facts))
                End If
            Next rule

            Return result
        End Function
    End Class

End Namespace

