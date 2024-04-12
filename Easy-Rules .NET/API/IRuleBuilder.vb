Imports EasyRules.Core

Namespace API
	Public Interface IRuleBuilder
		Function ThatWhen(condition As Func(Of Facts, Boolean)) As RuleBuilder
		Function ThenDoes(action As Action(Of Facts)) As RuleBuilder
		Function WithDescription(description As String) As RuleBuilder
		Function WithName(name As String) As RuleBuilder
		Function WithPriority(priority As Integer) As RuleBuilder
	End Interface
End Namespace
