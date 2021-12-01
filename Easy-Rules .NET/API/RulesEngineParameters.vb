Namespace API

    Public Class RulesEngineParameters
        Implements ICloneable

        Public Const DefaultRulePriorityThreshold As Integer = Integer.MaxValue

        Public Sub New()
            PriorityThreshold = DefaultRulePriorityThreshold
            SkipOnFirstAppliedRule = False
            SkipOnFirstNonTriggeredRule = False
            SkipOnFirstFailedRule = False
        End Sub

        Public Sub New(Optional skipOnFirstAppliedRule As Boolean = False, Optional skipOnFirstNonTriggeredRule As Boolean = False, Optional skipOnFirstFailedRule As Boolean = False, Optional priorityThreshold As Integer = DefaultRulePriorityThreshold)
            Me.PriorityThreshold = priorityThreshold
            Me.SkipOnFirstAppliedRule = skipOnFirstAppliedRule
            Me.SkipOnFirstNonTriggeredRule = skipOnFirstNonTriggeredRule
            Me.SkipOnFirstFailedRule = skipOnFirstFailedRule
        End Sub

        Public Property PriorityThreshold As Integer

        Public Property SkipOnFirstAppliedRule As Boolean

        Public Property SkipOnFirstNonTriggeredRule As Boolean

        Public Property SkipOnFirstFailedRule As Boolean

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

