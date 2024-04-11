Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Logging.Abstractions

Namespace API

    Public NotInheritable Class RulesEngineParameters
        Implements ICloneable

        Public Const DefaultRulePriorityThreshold As Integer = Integer.MaxValue

        Public Sub New()
            PriorityThreshold = DefaultRulePriorityThreshold
            SkipOnFirstAppliedRule = False
            SkipOnFirstNonTriggeredRule = False
            SkipOnFirstFailedRule = False
            LoggerFactory = NullLoggerFactory.Instance
        End Sub

        Public Sub New(Optional skipOnFirstAppliedRule As Boolean = False, Optional skipOnFirstNonTriggeredRule As Boolean = False, Optional skipOnFirstFailedRule As Boolean = False, Optional priorityThreshold As Integer = DefaultRulePriorityThreshold, Optional loggerFactory As ILoggerFactory = Nothing)
            Me.PriorityThreshold = priorityThreshold
            Me.SkipOnFirstAppliedRule = skipOnFirstAppliedRule
            Me.SkipOnFirstNonTriggeredRule = skipOnFirstNonTriggeredRule
            Me.SkipOnFirstFailedRule = skipOnFirstFailedRule
            Me.LoggerFactory = If(loggerFactory, NullLoggerFactory.Instance)
        End Sub

        Public Property PriorityThreshold As Integer
        Public Property SkipOnFirstAppliedRule As Boolean
        Public Property SkipOnFirstNonTriggeredRule As Boolean
        Public Property SkipOnFirstFailedRule As Boolean
        Public Property LoggerFactory As ILoggerFactory

        Public Overrides Function ToString() As String
            Dim properties As String() = {NameOf(SkipOnFirstAppliedRule), NameOf(SkipOnFirstNonTriggeredRule), NameOf(SkipOnFirstFailedRule), NameOf(PriorityThreshold)}
            Dim values As String() = {SkipOnFirstAppliedRule.ToString(), SkipOnFirstNonTriggeredRule.ToString(), SkipOnFirstFailedRule.ToString(), PriorityThreshold.ToString()}

            Return "Engine parameters:" & Environment.NewLine & String.Join(Environment.NewLine, properties.Zip(values, Function(prop, value) $"{prop} = {value}"))
        End Function

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class

End Namespace

