Imports Microsoft.Extensions.Logging

Public MustInherit Class AbstractRuleFactory
	Public Shared ReadOnly Property AllowedCompositeRuleTypes As String() = New String() {NameOf(UnitRuleGroup), NameOf(ConditionalRuleGroup), NameOf(ActivationRuleGroup)}

	Private ReadOnly logger As ILogger(Of AbstractRuleFactory)

	Protected Function CreateRule(ruleDefinition As RuleDefinition) As IRule
		If Guard.NotNull(ruleDefinition, NameOf(ruleDefinition)).IsCompositeRule() Then
			Return CreateCompositeRule(ruleDefinition)
		Else
			Return CreateSimpleRule(ruleDefinition)
		End If
	End Function

	Protected MustOverride Function CreateSimpleRule(ruleDefinition As RuleDefinition) As IRule

	Protected Function CreateCompositeRule(ruleDefinition As RuleDefinition) As IRule
		If Not String.IsNullOrWhiteSpace(ruleDefinition.Condition) Then
			logger.LogWarning("Condition '{Condition}' in composite rule '{RuleDefinition}' of type {CompositeRuleType} will be ignored.",
					 ruleDefinition.Condition,
					 ruleDefinition.Name,
					 ruleDefinition.CompositeRuleType)
		End If

		If ruleDefinition.Actions IsNot Nothing AndAlso ruleDefinition.Actions.Any() Then
			logger.LogWarning("Actions '{Actions}' in composite rule '{RuleDefinition}' of type {CompositeRuleType} will be ignored.",
					 ruleDefinition.Actions,
					 ruleDefinition.Name,
					 ruleDefinition.CompositeRuleType)
		End If

		Dim compositeRule = InstantiateCompositeRule(ruleDefinition)

		With DirectCast(compositeRule, IRules)
			For Each composingRuleDefinition In ruleDefinition.ComposingRules
				.Add(CreateRule(composingRuleDefinition))
			Next
		End With

		Return compositeRule
	End Function

	Private Function InstantiateCompositeRule(ruleDefinition As RuleDefinition) As IRule
		Select Case ruleDefinition.CompositeRuleType
			Case NameOf(UnitRuleGroup)
				Return New UnitRuleGroup(ruleDefinition.Name, ruleDefinition.Description, ruleDefinition.Priority)
			Case NameOf(ActivationRuleGroup)
				Return New ActivationRuleGroup(ruleDefinition.Name, ruleDefinition.Description, ruleDefinition.Priority)
			Case NameOf(ConditionalRuleGroup)
				Return New ConditionalRuleGroup(ruleDefinition.Name, ruleDefinition.Description, ruleDefinition.Priority)
			Case Else
				Throw New ArgumentException($"Invalid composite rule type, must be one of: {String.Join(", ", AllowedCompositeRuleTypes)}")
		End Select
	End Function
End Class
