Imports Easy_Rules_.NET.API

Namespace Core

    Public Class RuleBuilder
        Private _name As String = Rule.DefaultName
        Private _description As String = Rule.DefaultDescription
        Private _priority As Integer = Rule.DefaultPriority

        Private _condition As Func(Of Facts, Boolean) = Function(F) False
        Private _actions As List(Of Action(Of Facts)) = New List(Of Action(Of Facts))

        Public Function WithName(name As String) As RuleBuilder
            NullGuard.RequireNonNull(name, NameOf(name))
            _name = name
            Return Me
        End Function

        Public Function WithDescription(description As String) As RuleBuilder
            NullGuard.RequireNonNull(description, NameOf(description))
            _description = description
            Return Me
        End Function

        Public Function WithPriority(priority As Integer) As RuleBuilder
            NullGuard.RequireNonNull(priority, NameOf(priority))
            _priority = priority
            Return Me
        End Function

        Public Function ThatWhen(condition As Func(Of Facts, Boolean)) As RuleBuilder
            NullGuard.RequireNonNull(condition, NameOf(condition))
            _condition = condition
            Return Me
        End Function

        Public Function ThenDoes(action As Action(Of Facts)) As RuleBuilder
            NullGuard.RequireNonNull(action, NameOf(action))
            _actions.Add(action)
            Return Me
        End Function

        Public Function Build() As Rule
            Return New DefaultRule(_name, _description, _priority, _condition, _actions.ToArray())
        End Function
    End Class

End Namespace
