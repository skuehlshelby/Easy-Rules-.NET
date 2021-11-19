Imports System.Reflection

Namespace Core

    Public Class ActionMethodOrder
        Implements IComparable(Of ActionMethodOrder)

        Public Sub New(order As Integer, method As MethodInfo)
            Me.Order = order
            Me.Method = method
        End Sub

        Public ReadOnly Property Order As Integer

        Public ReadOnly Property Method As MethodInfo

        Public Function CompareTo(other As ActionMethodOrder) As Integer Implements IComparable(Of ActionMethodOrder).CompareTo
            Return other.Order - Order
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            If obj Is Me Then
                Return True
            End If

            If obj.GetHashCode() = GetHashCode() AndAlso TypeOf obj Is ActionMethodOrder Then
                With DirectCast(obj, ActionMethodOrder)
                    Return .Order = Order AndAlso .Method = Method
                End With
            Else
                Return False
            End If
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return HashCode.Combine(Order, Method)
        End Function
    End Class

End Namespace