Imports RulesEngine.API

Namespace Core

    Public Class DefaultRulesEngine
        Inherits API.RulesEngine

        Private ReadOnly _Logger As Logging.Log = New Logging.Log

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(parameters As RulesEngineParameters)
            MyBase.New(parameters)
        End Sub

        Public Overrides Sub Fire(rules As Rules, facts As Facts)
            NullGuard.RequireNonNull(rules, NameOf(rules))
            NullGuard.RequireNonNull(facts, NameOf(facts))

            RaiseBeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

            DoFire(rules, facts)

            RaiseAfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
        End Sub

        Protected Overridable Sub DoFire(rules As Rules, facts As Facts)
            If rules.IsEmpty() Then
                Return
            End If

            _Logger.WriteEntry(rules.ToString())
            _Logger.WriteEntry(facts.ToString())
            _Logger.WriteEntry("Begin rules evaluation:")

            For Each rule As Rule In rules
                If rule.Priority > Parameters.PriorityThreshold Then
                    _Logger.WriteEntry($"Rule priority threshold ({Parameters.PriorityThreshold}) exceeded at rule '{rule.Name}' with priority={rule.Priority}, next rules will be skipped.")
                    Exit For
                End If

                If Not ShouldBeEvaluated(rule, facts) Then
                    _Logger.WriteEntry($"Rule '{rule.Name}' has been skipped before being evaluated.")
                    Continue For
                End If

                Dim evaluationResult As Boolean = False

                Try
                    evaluationResult = rule.Evaluate(facts)
                Catch ex As Exception
                    _Logger.WriteEntry($"Rule '{rule.Name}' produced error {ex}.")
                    RaiseOnEvaluationError(Me, New RuleFailureEventArgs(rule, facts, ex))

                    If Parameters.SkipOnFirstNonTriggeredRule Then
                        _Logger.WriteEntry($"Subsequent rules will be skipped because {NameOf(Parameters.SkipOnFirstNonTriggeredRule)} is true.")
                        Exit For
                    End If
                End Try

                If evaluationResult Then
                    _Logger.WriteEntry($"Rule '{rule.Name}' was triggered.")
                    RaiseAfterEvaluate(Me, New RuleEventArgs(rule, facts))

                    Try
                        RaiseBeforeExecute(Me, New RuleEventArgs(rule, facts))

                        rule.Execute(facts)

                        _Logger.WriteEntry($"Rule '{rule.Name}' was performed successfully.")

                        RaiseAfterExecute(Me, New RuleEventArgs(rule, facts))

                        If Parameters.SkipOnFirstAppliedRule Then
                            _Logger.WriteEntry($"Subsequent rules will be skipped because {NameOf(Parameters.SkipOnFirstAppliedRule)} is true.")
                            Exit For
                        End If

                    Catch ex As Exception
                        _Logger.WriteEntry($"Rule '{rule.Name}' was performed with error {ex}.")
                        RaiseOnEvaluationError(Me, New RuleFailureEventArgs(rule, facts, ex))

                        If Parameters.SkipOnFirstFailedRule Then
                            Exit For
                        End If
                    End Try
                Else
                    _Logger.WriteEntry($"Rule '{rule.Name}' evaluated to false and was not executed.")
                    RaiseAfterEvaluate(Me, New RuleEventArgs(rule, facts))

                    If Parameters.SkipOnFirstNonTriggeredRule Then
                        _Logger.WriteEntry($"Subsequent rules will be skipped because {NameOf(Parameters.SkipOnFirstNonTriggeredRule)} is true.")
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
            NullGuard.RequireNonNull(rules, NameOf(rules))
            NullGuard.RequireNonNull(facts, NameOf(facts))

            Try
                RaiseBeforeRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))

                Return DoCheck(rules, facts)
            Finally
                RaiseAfterRulesEvaluation(Me, New RulesEngineEventArgs(rules, facts))
            End Try
        End Function

        Private Function DoCheck(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)
            _Logger.WriteEntry("Checking rules.")

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

