Imports EasyRules.API

Namespace Core

	Public Class RuleBuilder
		Implements IRuleBuilder

		Private _name As String = Rule.DefaultName
		Private _description As String = Rule.DefaultDescription
		Private _priority As Integer = Rule.DefaultPriority

		Private _condition As Func(Of Facts, Boolean) = Function(F) False
		Private ReadOnly _actions As New List(Of Action(Of Facts))

		Public Function WithName(name As String) As RuleBuilder Implements IRuleBuilder.WithName
			If String.IsNullOrWhiteSpace(name) Then Throw New ArgumentException($"'{NameOf(name)}' cannot be null or whitespace.", NameOf(name))

			_name = name
			Return Me
		End Function

		Public Function WithDescription(description As String) As RuleBuilder Implements IRuleBuilder.WithDescription
			If String.IsNullOrWhiteSpace(description) Then Throw New ArgumentException($"'{NameOf(description)}' cannot be null or whitespace.", NameOf(description))

			_description = description
			Return Me
		End Function

		Public Function WithPriority(priority As Integer) As RuleBuilder Implements IRuleBuilder.WithPriority
			_priority = priority
			Return Me
		End Function

		Public Function ThatWhen(condition As Func(Of Facts, Boolean)) As RuleBuilder Implements IRuleBuilder.ThatWhen
			If condition Is Nothing Then Throw New ArgumentNullException(NameOf(condition))

			_condition = condition
			Return Me
		End Function

		Public Function ThenDoes(action As Action(Of Facts)) As RuleBuilder Implements IRuleBuilder.ThenDoes
			If action Is Nothing Then Throw New ArgumentNullException(NameOf(action))

			_actions.Add(action)
			Return Me
		End Function

		Public Function Build() As Rule
			Return New DefaultRule(_name, _description, _priority, _condition, _actions.ToArray())
		End Function
	End Class

End Namespace
