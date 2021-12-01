Imports Easy_Rules_.NET.API

Namespace Core

    Public Class BasicRule
        Inherits Rule
        Implements IComparable(Of Rule)

        Public Sub New()
        End Sub

        Public Sub New(name As String)
            MyBase.New(name)
        End Sub

        Public Sub New(name As String, description As String)
            MyBase.New(name, description)
        End Sub

        Public Sub New(name As String, description As String, priority As Integer)
            MyBase.New(name, description, priority)
        End Sub

        Public Overrides Sub Execute(Facts As Facts)

        End Sub

        Public Overrides Function Evaluate(Facts As Facts) As Boolean
            Return False
        End Function
    End Class

End Namespace