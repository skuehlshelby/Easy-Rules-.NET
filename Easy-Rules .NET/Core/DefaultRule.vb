Imports EasyRules.API

Namespace Core

    Public Class DefaultRule
        Inherits BasicRule

        Private ReadOnly _condition As Func(Of Facts, Boolean)
        Private ReadOnly _actions As List(Of Action(Of Facts))

        Public Sub New(name As String, description As String, priority As Integer, condition As Func(Of Facts, Boolean), ParamArray actions As Action(Of Facts)())
            MyBase.New(name, description, priority)
            _condition = condition
            _actions = New List(Of Action(Of Facts))(actions)
        End Sub

        Public Overrides Function Evaluate(facts As Facts) As Boolean
            Return _condition.Invoke(facts)
        End Function

        Public Overrides Sub Execute(facts As Facts)
            _actions.ForEach(Sub(action) action.Invoke(facts))
        End Sub
    End Class

End Namespace
