Namespace API

    Public Class RulesEngineEventArgs
        Inherits EventArgs

        Public Sub New(rules As Rules, facts As Facts)
            Me.Facts = facts
            Me.Rules = rules
        End Sub

        Public ReadOnly Property Facts As Facts

        Public ReadOnly Property Rules As Rules
    End Class
End Namespace
