Imports System.Reflection

Namespace Attributes

	Public NotInheritable Class RuleProxy
		Public Shared Function AsRule(rule As Object) As IRule
			If rule Is Nothing Then Throw New ArgumentNullException(NameOf(rule))
			If TypeOf rule Is IRule Then Return DirectCast(rule, IRule)

			RuleDefinitionValidator.ValidateRuleDefinition(rule)

			Dim attribute = rule.GetType().GetCustomAttribute(Of RuleAttribute)()

			Return New ProxyRule(rule, attribute.Name, attribute.Description, attribute.Priority)
		End Function

		Private NotInheritable Class ProxyRule
			Implements IRule

			Private Interface IInvocationStrategy
				Function Invoke(facts As IFacts) As Object
			End Interface

			Private NotInheritable Class ForwardFacts
				Implements IInvocationStrategy

				Private ReadOnly rule As Object
				Private ReadOnly method As MethodBase

				Public Sub New(rule As Object, method As MethodBase)
					If rule Is Nothing Then Throw New ArgumentNullException(NameOf(rule))
					If method Is Nothing Then Throw New ArgumentNullException(NameOf(method))

					Me.rule = rule
					Me.method = method
				End Sub

				Public Function Invoke(facts As IFacts) As Object Implements IInvocationStrategy.Invoke
					Return method.Invoke(rule, New Object() {facts})
				End Function
			End Class

			Private NotInheritable Class ForwardNamedFactsOnly
				Implements IInvocationStrategy

				Private ReadOnly rule As Object
				Private ReadOnly method As MethodBase
				Private ReadOnly factNames As String()

				Public Sub New(rule As Object, method As MethodBase, factNames() As String)
					If rule Is Nothing Then Throw New ArgumentNullException(NameOf(rule))
					If method Is Nothing Then Throw New ArgumentNullException(NameOf(method))
					If factNames Is Nothing Then Throw New ArgumentNullException(NameOf(factNames))

					Me.rule = rule
					Me.method = method
					Me.factNames = factNames
				End Sub

				Public Function Invoke(facts As IFacts) As Object Implements IInvocationStrategy.Invoke
					If Not factNames.All(Function(f) facts.HasFact(f)) Then Return False

					Dim args = factNames.
						Select(Function(p) facts.GetFactByName(p).Value).
						ToArray()

					Return method.Invoke(rule, args)
				End Function
			End Class

			Private NotInheritable Class ForwardNamedFactsAndIFacts
				Implements IInvocationStrategy

				Private ReadOnly rule As Object
				Private ReadOnly method As MethodBase
				Private ReadOnly factNamesFirstHalf As String()
				Private ReadOnly factNamesSecondHalf As String()

				Public Sub New(rule As Object, method As MethodBase, factNamesFirstHalf() As String, factNamesSecondHalf() As String)
					If rule Is Nothing Then Throw New ArgumentNullException(NameOf(rule))
					If method Is Nothing Then Throw New ArgumentNullException(NameOf(method))
					If factNamesFirstHalf Is Nothing Then Throw New ArgumentNullException(NameOf(factNamesFirstHalf))
					If factNamesSecondHalf Is Nothing Then Throw New ArgumentNullException(NameOf(factNamesSecondHalf))

					Me.rule = rule
					Me.method = method
					Me.factNamesFirstHalf = factNamesFirstHalf
					Me.factNamesSecondHalf = factNamesSecondHalf
				End Sub

				Public Function Invoke(facts As IFacts) As Object Implements IInvocationStrategy.Invoke
					If Not factNamesFirstHalf.All(Function(f) facts.HasFact(f)) AndAlso
						Not factNamesSecondHalf.All(Function(f) facts.HasFact(f)) Then Return False

					Dim argsFirstHalf = factNamesFirstHalf.
						Select(Function(p) facts.GetFactByName(p).Value).
						ToArray()

					Dim argsSecondHalf = factNamesSecondHalf.
						Select(Function(p) facts.GetFactByName(p).Value).
						ToArray()

					Dim args = argsFirstHalf.
						Concat(New Object() {facts}).
						Concat(argsSecondHalf).
						ToArray()

					Return method.Invoke(rule, args)
				End Function
			End Class

			Private ReadOnly conditionInvokationStrategy As IInvocationStrategy
			Private ReadOnly actionInvokationStrategies As IInvocationStrategy()

			Public Sub New(rule As Object, name As String, description As String, priority As Integer)
				If name Is Nothing Then Throw New ArgumentNullException(NameOf(name))
				If description Is Nothing Then Throw New ArgumentNullException(NameOf(description))

				Dim ruleType = rule.GetType()

				Dim conditionMethod = ruleType.
					GetMethods().
					Single(Function(m) m.IsDefined(GetType(ConditionAttribute)))

				conditionInvokationStrategy = CreateInvocationStrategy(rule, conditionMethod)

				actionInvokationStrategies = ruleType.
					GetMethods().
					Where(Function(m) m.IsDefined(GetType(ActionAttribute))).
					OrderBy(Function(m) m.GetCustomAttribute(Of ActionAttribute).Order).
					Cast(Of MethodBase)().
					Select(Function(m) CreateInvocationStrategy(rule, m)).
					ToArray()

				Me.Name = name
				Me.Description = description
				Me.Priority = priority
			End Sub

			Private Shared Function CreateInvocationStrategy(rule As Object, method As MethodBase) As IInvocationStrategy
				Dim parameters = method.GetParameters()
				Dim factAttributes = parameters.Select(Function(p) p.GetCustomAttribute(Of FactAttribute)).ToArray()

				If factAttributes.All(Function(f) f IsNot Nothing) Then
					Dim factNames = factAttributes.Select(Function(a) a.FactName).ToArray()
					Return New ForwardNamedFactsOnly(rule, method, factNames)
				ElseIf parameters.Length = 1 AndAlso parameters.Single().ParameterType = GetType(IFacts) Then
					Return New ForwardFacts(rule, method)
				Else
					Dim parametersFirstHalf = parameters.TakeWhile(Function(p) p.ParameterType <> GetType(IFacts))
					Dim parametersSecondHalf = parameters.
						SkipWhile(Function(p) p.ParameterType <> GetType(IFacts)).
						SkipWhile(Function(p) p.ParameterType = GetType(IFacts))
					Dim factNamesFirstHalf = parametersFirstHalf.Select(Function(p) p.GetCustomAttribute(Of FactAttribute).FactName).ToArray()
					Dim factNamesSecondHalf = parametersSecondHalf.Select(Function(p) p.GetCustomAttribute(Of FactAttribute).FactName).ToArray()

					Return New ForwardNamedFactsAndIFacts(rule, method, factNamesFirstHalf, factNamesSecondHalf)
				End If
			End Function

			Public ReadOnly Property Name As String Implements IRule.Name
			Public ReadOnly Property Description As String Implements IRule.Description
			Public ReadOnly Property Priority As Integer Implements IRule.Priority

			Public Function Evaluate(facts As IFacts) As Boolean Implements IRule.Evaluate
				Return DirectCast(conditionInvokationStrategy.Invoke(facts), Boolean)
			End Function

			Public Sub Execute(facts As IFacts) Implements IRule.Execute
				For Each action In actionInvokationStrategies
					action.Invoke(facts)
				Next
			End Sub
		End Class
	End Class

End Namespace