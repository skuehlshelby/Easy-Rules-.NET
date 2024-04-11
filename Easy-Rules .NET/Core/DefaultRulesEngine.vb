Imports EasyRules.API
Imports Microsoft.Extensions.Logging

Namespace Core

    Public Class DefaultRulesEngine
        Inherits RulesEngine

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(parameters As RulesEngineParameters)
            MyBase.New(parameters)
        End Sub

        Public Overrides Sub Fire(rules As Rules, facts As Facts)
            If rules Is Nothing Then Throw New ArgumentNullException(NameOf(rules))
            If facts Is Nothing Then Throw New ArgumentNullException(NameOf(facts))

            RaiseBeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

            DoFire(rules, facts)

            RaiseAfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
        End Sub

        Protected Overridable Sub DoFire(rules As Rules, facts As Facts)
            If rules.IsEmpty() Then
                Return
            End If

            _logger.LogInformation("Beginning evaluation of rules")
            _logger.LogInformation("{Rules}", rules)
            _logger.LogInformation("{Facts}", facts.ToString())


            For Each rule As Rule In rules
                If rule.Priority > Parameters.PriorityThreshold Then
                    _logger.LogInformation("Rule priority threshold ({PriorityThreshold}) exceeded at rule '{RuleName}' with priority={Priority}. The next rules will be skipped.", Parameters.PriorityThreshold, rule.Name, rule.Priority)
                    Exit For
                End If

                If Not ShouldBeEvaluated(rule, facts) Then
                    _logger.LogInformation("Rule '{RuleName}' has been skipped before being evaluated.", rule.Name)
                    Continue For
                End If

                Dim evaluationResult As Boolean = False

                Try
                    evaluationResult = rule.Evaluate(facts)
                Catch ex As Exception
                    _logger.LogError($"Rule '{rule.Name}' produced error {ex}.")
                    RaiseOnEvaluationError(Me, New RuleFailureEventArgs(rule, facts, ex))

                    If Parameters.SkipOnFirstNonTriggeredRule Then
                        _logger.LogInformation("Subsequent rules will be skipped because {Parameters} is {SkipOnFirstNonTriggeredRule}", NameOf(Parameters.SkipOnFirstNonTriggeredRule), Parameters.SkipOnFirstNonTriggeredRule)
                        Exit For
                    End If
                End Try

                If evaluationResult Then
                    _logger.LogInformation("Rule '{RuleName}' was triggered", rule.Name)
                    RaiseAfterEvaluate(Me, New RuleEventArgs(rule, facts))

                    Try
                        RaiseBeforeExecute(Me, New RuleEventArgs(rule, facts))

                        rule.Execute(facts)

                        _logger.LogInformation("Rule '{RuleName}' was performed successfully", rule.Name)

                        RaiseAfterExecute(Me, New RuleEventArgs(rule, facts))

                        If Parameters.SkipOnFirstAppliedRule Then
                            _logger.LogInformation("Subsequent rules will be skipped because {Parameters} is {SkipOnFirstAppliedRule}", NameOf(Parameters.SkipOnFirstAppliedRule), Parameters.SkipOnFirstAppliedRule)
                            Exit For
                        End If

                    Catch ex As Exception
                        _logger.LogError("Rule '{RuleName}' was performed with error {Exception}", rule.Name, ex)
                        RaiseOnExecutionError(Me, New RuleFailureEventArgs(rule, facts, ex))

                        If Parameters.SkipOnFirstFailedRule Then
                            Exit For
                        End If
                    End Try
                Else
                    _logger.LogInformation("Rule '{RuleName}' evaluated to false and was not executed.", rule.Name)
                    RaiseAfterEvaluate(Me, New RuleEventArgs(rule, facts))

                    If Parameters.SkipOnFirstNonTriggeredRule Then
                        _logger.LogInformation("Subsequent rules will be skipped because {Parameters} is {SkipOnFirstNonTriggeredRule}", NameOf(Parameters.SkipOnFirstNonTriggeredRule), Parameters.SkipOnFirstNonTriggeredRule)
                        Exit For
                    End If
                End If
            Next rule
        End Sub

        Private Function ShouldBeEvaluated(rule As Rule, facts As Facts) As Boolean
            If rule Is Nothing Then Throw New ArgumentNullException(NameOf(Rules))
            If facts Is Nothing Then Throw New ArgumentNullException(NameOf(facts))

            Dim e As RuleEventArgs = New RuleEventArgs(rule, facts)

            RaiseBeforeRuleEvaluation(Me, e)

            Return Not e.Cancel
        End Function

        Public Overrides Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            If rules Is Nothing Then Throw New ArgumentNullException(NameOf(rules))
            If facts Is Nothing Then Throw New ArgumentNullException(NameOf(facts))

            Try
                RaiseBeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

                Return DoCheck(rules, facts)
            Finally
                RaiseAfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
            End Try
        End Function

        Private Function DoCheck(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            If rules Is Nothing Then Throw New ArgumentNullException(NameOf(rules))
            If facts Is Nothing Then Throw New ArgumentNullException(NameOf(facts))

            _logger.LogInformation("Checking rules")

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

