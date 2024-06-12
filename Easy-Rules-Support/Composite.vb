''' <summary>
''' A conditional rule group is a composite rule where the rule with the highest 
''' priority acts as a condition: If the rule with the highest priority evaluates
''' to <c>true</c>, then we try to evaluate the rest of the rules and execute the ones
''' that evaluate to <c>true</c>.
''' </summary>
Public NotInheritable Class ConditionalRuleGroup
	Implements IRule
	Implements IRules

	Private ReadOnly rules As New HashSet(Of IRule)(EquateRulesByName.Instance)
	Private highestPriorityRule As IRule = Nothing

	Public Sub New(name As String)
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, Constants.DEFAULT_RULE_PRIORITY)
	End Sub

	Public Sub New(name As String, priority As Integer)
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, priority)
	End Sub

	Public Sub New(name As String, description As String)
		Me.New(name, description, Constants.DEFAULT_RULE_PRIORITY)
	End Sub

	Public Sub New(name As String, description As String, priority As Integer)
		Me.Name = Guard.NotNullOrWhiteSpace(name, NameOf(name))
		Me.Description = Guard.NotNullOrWhiteSpace(description, NameOf(description))
		Me.Priority = priority
	End Sub

	Public ReadOnly Property Name As String Implements IRule.Name
	Public ReadOnly Property Description As String Implements IRule.Description
	Public ReadOnly Property Priority As Integer Implements IRule.Priority

	''' <summary>
	''' A conditional rule group will trigger all its composing rules if the condition
	''' of the rule with highest priority evaluates to <c>true</c>.
	''' </summary>
	''' <param name="facts">The facts.</param>
	''' <returns><c>True</c> if the conditions of all composing rules evaluate to <c>true</c>.</returns>
	Public Function Evaluate(facts As IFacts) As Boolean Implements IRule.Evaluate
		Guard.NotNull(facts, NameOf(facts))

		Return highestPriorityRule IsNot Nothing AndAlso highestPriorityRule.Evaluate(facts)
	End Function

	''' <summary>
	''' When a conditional rule group is executed, all rules that evaluated to <c>true</c>
	''' are performed in their natural order, but with the conditional rule
	''' (the one with the highest <see cref="IRule.Priority"/>) first.
	''' </summary>
	''' 
	''' <param name="facts">The facts.</param>
	''' <exception cref="ArgumentNullException"></exception>
	Public Sub Execute(facts As IFacts) Implements IRule.Execute
		Guard.NotNull(facts, NameOf(facts))

		If highestPriorityRule IsNot Nothing Then highestPriorityRule.Execute(facts)

		For Each rule In rules.OrderBy(Function(r) r.Priority).Where(Function(r) r.Evaluate(facts))
			If rule IsNot highestPriorityRule Then
				rule.Execute(facts)
			End If
		Next
	End Sub

	Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of IRule).Count
		Get
			Return rules.Count
		End Get
	End Property

	Public Function Add(rule As IRule) As Boolean Implements IRules.Add
		Guard.NotNull(rule, NameOf(rule))

		If rules.Contains(rule) Then
			Return False
		End If

		If highestPriorityRule Is Nothing Then
			highestPriorityRule = rule
		ElseIf rule.Priority = highestPriorityRule.Priority Then
			Throw New ArgumentException("Only one rule may have the highest priority.")
		ElseIf highestPriorityRule.Priority < rule.Priority Then
			highestPriorityRule = rule
		End If

		Return rules.Add(rule)
	End Function

	Public Function Remove(rule As IRule) As Boolean Implements IRules.Remove
		Guard.NotNull(rule, NameOf(rule))

		If highestPriorityRule Is rule Then
			highestPriorityRule = Nothing
		End If

		Return rules.Remove(rule)
	End Function

	Public Sub Clear() Implements IRules.Clear
		rules.Clear()
		highestPriorityRule = Nothing
	End Sub

	Public Function GetEnumerator() As IEnumerator(Of IRule) Implements IEnumerable(Of IRule).GetEnumerator
		Return rules.OrderBy(Function(r) r.Priority).GetEnumerator()
	End Function

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function
End Class

Public NotInheritable Class UnitRuleGroup
	Implements IRule
	Implements IRules

	Private ReadOnly rules As New HashSet(Of IRule)(EquateRulesByName.Instance)

	Public Sub New(name As String)
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, Constants.DEFAULT_RULE_PRIORITY)
	End Sub

	Public Sub New(name As String, priority As Integer)
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, priority)
	End Sub

	Public Sub New(name As String, description As String)
		Me.New(name, description, Constants.DEFAULT_RULE_PRIORITY)
	End Sub

	Public Sub New(name As String, description As String, priority As Integer)
		Me.Name = Guard.NotNullOrWhiteSpace(name, NameOf(name))
		Me.Description = Guard.NotNullOrWhiteSpace(description, NameOf(description))
		Me.Priority = priority
	End Sub

	Public ReadOnly Property Name As String Implements IRule.Name
	Public ReadOnly Property Description As String Implements IRule.Description
	Public ReadOnly Property Priority As Integer Implements IRule.Priority
	Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of IRule).Count
		Get
			Return rules.Count
		End Get
	End Property

	Public Function Evaluate(facts As IFacts) As Boolean Implements IRule.Evaluate
		Guard.NotNull(facts, NameOf(facts))

		Return rules.All(Function(r) r.Evaluate(facts))
	End Function

	Public Sub Execute(facts As IFacts) Implements IRule.Execute
		Guard.NotNull(facts, NameOf(facts))

		For Each rule In rules
			rule.Execute(facts)
		Next
	End Sub

	Public Function Add(rule As IRule) As Boolean Implements IRules.Add
		Return rules.Add(Guard.NotNull(rule, NameOf(rule)))
	End Function

	Public Function Remove(rule As IRule) As Boolean Implements IRules.Remove
		Return rules.Remove(Guard.NotNull(rule, NameOf(rule)))
	End Function

	Public Sub Clear() Implements IRules.Clear
		rules.Clear()
	End Sub

	Public Function GetEnumerator() As IEnumerator(Of IRule) Implements IEnumerable(Of IRule).GetEnumerator
		Return rules.GetEnumerator()
	End Function

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function
End Class

Public NotInheritable Class ActivationRuleGroup
	Implements IRule
	Implements IRules

	Private ReadOnly rules As New HashSet(Of IRule)(EquateRulesByName.Instance)

	Public Sub New(name As String)
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, Constants.DEFAULT_RULE_PRIORITY)
	End Sub

	Public Sub New(name As String, priority As Integer)
		Me.New(name, Constants.DEFAULT_RULE_DESCRIPTION, priority)
	End Sub

	Public Sub New(name As String, description As String)
		Me.New(name, description, Constants.DEFAULT_RULE_PRIORITY)
	End Sub

	Public Sub New(name As String, description As String, priority As Integer)
		Me.Name = Guard.NotNullOrWhiteSpace(name, NameOf(name))
		Me.Description = Guard.NotNullOrWhiteSpace(description, NameOf(description))
		Me.Priority = priority
	End Sub

	Public ReadOnly Property Name As String Implements IRule.Name
	Public ReadOnly Property Description As String Implements IRule.Description
	Public ReadOnly Property Priority As Integer Implements IRule.Priority
	Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of IRule).Count
		Get
			Return Rules.Count
		End Get
	End Property

	Public Function Evaluate(facts As IFacts) As Boolean Implements IRule.Evaluate
		Return rules.Any(Function(r) r.Evaluate(facts))
	End Function

	Public Sub Execute(facts As IFacts) Implements IRule.Execute
		Dim selectedRule = rules.OrderBy(Function(r) r.Priority).First(Function(r) r.Evaluate(facts))

		If selectedRule IsNot Nothing Then selectedRule.Execute(facts)
	End Sub

	Public Function Add(rule As IRule) As Boolean Implements IRules.Add
		Return rules.Add(Guard.NotNull(rule, NameOf(rule)))
	End Function

	Public Function Remove(rule As IRule) As Boolean Implements IRules.Remove
		Return rules.Remove(Guard.NotNull(rule, NameOf(rule)))
	End Function

	Public Sub Clear() Implements IRules.Clear
		rules.Clear()
	End Sub

	Public Function GetEnumerator() As IEnumerator(Of IRule) Implements IEnumerable(Of IRule).GetEnumerator
		Return rules.GetEnumerator()
	End Function

	Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
		Return GetEnumerator()
	End Function
End Class