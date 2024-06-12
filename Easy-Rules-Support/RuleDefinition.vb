Public NotInheritable Class RuleDefinition
	Public Property Name As String = String.Empty
	Public Property Description As String = Constants.DEFAULT_RULE_DESCRIPTION
	Public Property Priority As Integer = Constants.DEFAULT_RULE_PRIORITY
	Public Property Condition As String = String.Empty
	Public Property Actions As List(Of String) = New List(Of String)()
	Public Property CompositeRuleType As String = String.Empty
	Public Property ComposingRules As List(Of RuleDefinition) = New List(Of RuleDefinition)()
	Public Function IsCompositeRule() As Boolean
		Return ComposingRules IsNot Nothing AndAlso ComposingRules.Any()
	End Function
End Class
