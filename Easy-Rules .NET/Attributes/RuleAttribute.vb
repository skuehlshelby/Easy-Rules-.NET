Namespace Attributes

    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
    Public Class RuleAttribute
        Inherits Attribute

        Public Sub New(name As String, Optional description As String = "No Description Provided", Optional priority As Integer = Integer.MaxValue)
            Me.Name = name
            Me.Description = description
            Me.Priority = priority
        End Sub

        ''' <summary>
        ''' The rule name which must be unique within a rules registry.
        ''' </summary>
        Public ReadOnly Property Name As String

        ''' <summary>
        ''' The rule description.
        ''' </summary>
        Public ReadOnly Property Description As String

        ''' <summary>
        ''' The rule priority.
        ''' </summary>
        Public ReadOnly Property Priority As Integer
    End Class

End Namespace