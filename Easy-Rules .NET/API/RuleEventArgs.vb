Namespace API

    Public Class RuleEventArgs
        Inherits EventArgs

        Public Sub New(rule As Rule, facts As Facts)
            Me.Facts = facts
            Me.Rule = rule
            Cancel = False
        End Sub

        Public ReadOnly Property Facts As Facts

        Public ReadOnly Property Rule As Rule

        Public Property Cancel As Boolean
    End Class

    Public Class RuleFailureEventArgs
        Inherits RuleEventArgs

        Public Sub New(rule As Rule, facts As Facts, exception As Exception)
            MyBase.New(rule, facts)
            Me.Exception = exception
        End Sub

        Public ReadOnly Property Exception As Exception
    End Class

End Namespace
