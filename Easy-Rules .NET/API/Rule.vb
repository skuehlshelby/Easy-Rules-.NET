Namespace API

    Public MustInherit Class Rule
        Implements IEquatable(Of Rule)
        Implements IComparable(Of Rule)


        Public Const DEFAULT_NAME As String = "rule"

        Public Const DEFAULT_DESCRIPTION As String = "description"

        Public Const DEFAULT_PRIORITY As Integer = Integer.MaxValue

        Protected Sub New()
            Me.New(DEFAULT_NAME, DEFAULT_DESCRIPTION, DEFAULT_PRIORITY)
        End Sub

        Protected Sub New(name As String)
            Me.New(name, DEFAULT_DESCRIPTION, DEFAULT_PRIORITY)
        End Sub

        Protected Sub New(name As String, description As String)
            Me.New(name, description, DEFAULT_PRIORITY)
        End Sub

        Protected Sub New(name As String, description As String, priority As Integer)
            Me.Name = name
            Me.Description = description
            Me.Priority = priority
        End Sub

        Public ReadOnly Property Name As String

        Public Property Description As String

        Public Property Priority As Integer

        Public MustOverride Function Evaluate(Facts As Facts) As Boolean

        Public MustOverride Sub Execute(Facts As Facts)

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Overrides Function Equals(obj As Object) As Boolean
            Return Equals(TryCast(obj, Rule))
        End Function

        Public Overloads Function Equals(other As Rule) As Boolean Implements IEquatable(Of Rule).Equals
            Return other IsNot Nothing AndAlso Name = other.Name
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return (Name, Description, Priority).GetHashCode()
        End Function

        Public Shared Operator =(left As Rule, right As Rule) As Boolean
            Return EqualityComparer(Of Rule).Default.Equals(left, right)
        End Operator

        Public Shared Operator <>(left As Rule, right As Rule) As Boolean
            Return Not left = right
        End Operator

        Public Function CompareTo(other As Rule) As Integer Implements IComparable(Of Rule).CompareTo
            If other.Priority > Priority Then
                Return 1
            ElseIf other.Priority < Priority Then
                Return -1
            Else
                Return 0
            End If
        End Function

    End Class

End Namespace
