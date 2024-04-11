Imports Microsoft.Extensions.Logging

Namespace API

    Public MustInherit Class RulesEngine

#Region "Events"

        Public Event BeforeRuleEvaluation As EventHandler(Of RuleEventArgs)

        Public Event AfterRuleEvaluation As EventHandler(Of RuleEventArgs)

        Public Event BeforeRuleExecution As EventHandler(Of RuleEventArgs)

        Public Event AfterRuleExecution As EventHandler(Of RuleEventArgs)

        Public Event OnRuleEvaluationError As EventHandler(Of RuleFailureEventArgs)

        Public Event OnRuleExecutionError As EventHandler(Of RuleFailureEventArgs)

        Public Event BeforeRulesEvaluation As EventHandler(Of RulesEngineEventArgs)

        Public Event AfterRulesEvaluation As EventHandler(Of RulesEngineEventArgs)

        Protected Overridable Sub RaiseBeforeRuleEvaluation(sender As Object, e As RuleEventArgs)
            RaiseEvent BeforeRuleEvaluation(sender, e)
        End Sub

        Protected Overridable Sub RaiseAfterEvaluate(sender As Object, e As RuleEventArgs)
            RaiseEvent AfterRuleEvaluation(sender, e)
        End Sub

        Protected Overridable Sub RaiseBeforeExecute(sender As Object, e As RuleEventArgs)
            RaiseEvent BeforeRuleExecution(sender, e)
        End Sub

        Protected Overridable Sub RaiseAfterExecute(sender As Object, e As RuleEventArgs)
            RaiseEvent AfterRuleExecution(sender, e)
        End Sub

        Protected Overridable Sub RaiseOnEvaluationError(sender As Object, e As RuleFailureEventArgs)
            RaiseEvent OnRuleEvaluationError(sender, e)
        End Sub

        Protected Overridable Sub RaiseOnExecutionError(sender As Object, e As RuleFailureEventArgs)
            RaiseEvent OnRuleExecutionError(sender, e)
        End Sub

        Protected Overridable Sub RaiseBeforeRulesEvaluation(sender As Object, e As RulesEngineEventArgs)
            RaiseEvent BeforeRulesEvaluation(sender, e)
        End Sub

        Protected Overridable Sub RaiseAfterRulesEvaluation(sender As Object, e As RulesEngineEventArgs)
            RaiseEvent AfterRulesEvaluation(sender, e)
        End Sub

#End Region

        Private ReadOnly _parameters As RulesEngineParameters
        Protected ReadOnly _logger As ILogger

        Protected Sub New()
            Me.New(New RulesEngineParameters())
        End Sub

        Protected Sub New(parameters As RulesEngineParameters)
            If parameters Is Nothing Then Throw New ArgumentNullException(NameOf(parameters))

            _parameters = parameters
            _logger = parameters.LoggerFactory.CreateLogger(Of RulesEngine)()
        End Sub

        Public ReadOnly Property Parameters As RulesEngineParameters
            Get
                Return DirectCast(_parameters.Clone(), RulesEngineParameters)
            End Get
        End Property

        Public MustOverride Sub Fire(rules As Rules, facts As Facts)

        Public MustOverride Function Check(rules As Rules, facts As Facts) As Dictionary(Of Rule, Boolean)

    End Class

End Namespace
