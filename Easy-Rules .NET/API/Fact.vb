Imports EasyRules.Core

Namespace API
    Public Class Fact
        Implements IEquatable(Of Fact)

        Public Sub New(name As String, value As Object)
            ArgumentException.ThrowIfNullOrWhiteSpace(name, NameOf(name))
            ArgumentNullException.ThrowIfNull(value, NameOf(value))
            Me.Name = name
            Me.Value = value
        End Sub

        Public ReadOnly Property Name As String
        Public ReadOnly Property Value As Object

        Public Function Unwrap(Of T)() As T
            Return CType(Value, T)
        End Function

        Public Overrides Function ToString() As String
            Return $"Fact: {Name} = {Value}"
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Return Equals(TryCast(obj, Fact))
        End Function

        Public Overloads Function Equals(other As Fact) As Boolean Implements IEquatable(Of Fact).Equals
            Return other IsNot Nothing AndAlso other.Name = Name
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Name.GetHashCode()
        End Function

        Public Shared Function NameFactAfterBaseClass(Of T As Class)() As String
            Dim type As Type = GetType(T)
            Dim name As String

            Do
                name = type.Name
                type = type.BaseType
            Loop Until type Is GetType(Object)

            Return name
        End Function

    End Class
End Namespace