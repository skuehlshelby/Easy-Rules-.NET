Imports System.Runtime.CompilerServices

Public NotInheritable Class Guard
	Private Sub New()
	End Sub

	Public Shared Function NotNull(Of T As Class)(value As T, parameterName As String, <CallerMemberName> Optional callerMemberName As String = "") As T
		If value Is Nothing Then Throw New ArgumentNullException($"'{parameterName}' cannot be null when passed to '{callerMemberName}'.", parameterName)
		Return value
	End Function

	Public Shared Function NotNullOrWhiteSpace(value As String, parameterName As String, <CallerMemberName> Optional callerMemberName As String = "") As String
		If String.IsNullOrWhiteSpace(value) Then Throw New ArgumentException($"'{parameterName}' cannot be null or empty when passed to '{callerMemberName}'.", parameterName)
		Return value
	End Function
End Class
