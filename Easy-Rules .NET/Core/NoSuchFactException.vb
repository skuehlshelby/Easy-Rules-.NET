Imports System.Runtime.Serialization

Namespace Core

    Public Class NoSuchFactException
        Inherits Exception

        Public Sub New(missingFact As String)
            Me.MissingFact = missingFact
        End Sub

        Public Sub New(message As String, missingFact As String)
            MyBase.New(message)
            Me.MissingFact = missingFact
        End Sub

        Public Sub New(message As String, innerException As Exception, missingFact As String)
            MyBase.New(message, innerException)
            Me.MissingFact = missingFact
        End Sub

        Public ReadOnly Property MissingFact As String
    End Class

End Namespace