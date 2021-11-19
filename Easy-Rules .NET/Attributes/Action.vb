Namespace Attributes

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class Action
        Inherits Attribute

        Public Sub New(Optional order As Integer = 0)
            Me.Order = order
        End Sub

        ''' <summary>
        ''' The order in which the action should be executed.
        ''' </summary>
        Public ReadOnly Property Order As Integer
    End Class

End Namespace