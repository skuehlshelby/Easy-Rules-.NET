Namespace Attributes

    <AttributeUsage(AttributeTargets.Parameter, AllowMultiple:=False, Inherited:=True)>
    Public Class Fact
        Inherits Attribute

        Public Sub New(factName As String)
            Me.FactName = factName
        End Sub

        Public ReadOnly Property FactName As String
    End Class

End Namespace
