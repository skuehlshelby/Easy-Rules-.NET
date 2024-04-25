Imports System.Reflection

Namespace Attributes

	Public NotInheritable Class RuleProxy
		Public Shared Function AsRule(rule As Object) As IRule
			If rule Is Nothing Then Throw New ArgumentNullException(NameOf(rule))
			If TypeOf rule Is IRule Then Return DirectCast(rule, IRule)

			RuleDefinitionValidator.ValidateRuleDefinition(rule)

			Dim attribute = rule.GetType().GetCustomAttribute(Of RuleAttribute)()

			Return New Rule(attribute.Name, attribute.Description, attribute.Priority, Function(f) ForwardFactsToCondition(rule, f), Sub(f) ForwardFactsToActionMethods(rule, f))
		End Function

		Private Shared Function ForwardFactsToCondition(rule As Object, facts As IFacts) As Boolean
			Dim condition = rule.GetType().GetMethods().Single(Function(m) m.GetCustomAttribute(Of ConditionAttribute) IsNot Nothing)
			Dim parameters = condition.GetParameters()
			Dim values As New List(Of Object)

			For Each parameter In parameters
				Dim factName = parameter.GetCustomAttribute(Of FactAttribute).FactName
				Dim fact = facts.GetFactByName(factName)
				Dim valueProperty = fact.GetType().GetProperty(NameOf(IFact(Of Object).Value))
				values.Add(valueProperty.GetValue(fact))
			Next

			Return CType(condition.Invoke(rule, values.ToArray()), Boolean)
		End Function

		Private Shared Sub ForwardFactsToActionMethods(rule As Object, facts As IFacts)
			Dim actions = rule.
				GetType().
				GetMethods().
				Where(Function(m) m.IsDefined(GetType(ActionAttribute))).
				OrderBy(Function(m) m.GetCustomAttribute(Of ActionAttribute).Order)

			Dim values As List(Of Object)

			For Each action In actions
				values = New List(Of Object)

				For Each parameter In action.GetParameters()
					Dim factName = parameter.GetCustomAttribute(Of FactAttribute).FactName
					Dim fact = facts.GetFactByName(factName)
					Dim valueProperty = fact.GetType().GetProperty(NameOf(IFact(Of Object).Value))
					values.Add(valueProperty.GetValue(fact))
				Next

				action.Invoke(rule, values.ToArray())
			Next
		End Sub
	End Class

End Namespace

