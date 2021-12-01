Public Module NullGuard
    Public Sub RequireNonNull(arg As Object, argName As String)
        If arg Is Nothing Then
            Throw New ArgumentNullException(argName)
        ElseIf TypeOf arg Is String Then
            If String.IsNullOrEmpty(arg.ToString()) Then
                Throw New ArgumentNullException(argName, $"'{argName}' cannot be empty.")
            ElseIf String.IsNullOrWhiteSpace(arg.ToString()) Then
                Throw New ArgumentNullException(argName, $"'{argName}' cannot be white space.")
            End If
        End If
    End Sub
End Module