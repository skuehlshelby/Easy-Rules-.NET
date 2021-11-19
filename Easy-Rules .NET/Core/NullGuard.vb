Namespace Core

    Public Class NullGuard
        Public Shared Sub RequireNonNull(arg As Object, argName As String)
            If TypeOf arg Is String Then
                If String.IsNullOrEmpty(DirectCast(arg, String)) Then
                    Throw New ArgumentException($"'{argName}' cannot be null or empty.", argName)
                End If
                If String.IsNullOrWhiteSpace(DirectCast(arg, String)) Then
                    Throw New ArgumentException($"'{argName}' cannot be null or white space.", argName)
                End If
            Else
                If arg Is Nothing Then
                    Throw New ArgumentNullException(argName)
                End If
            End If
        End Sub
    End Class

End Namespace