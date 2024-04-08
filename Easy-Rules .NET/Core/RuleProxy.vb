Imports System.Reflection
Imports System.Runtime.InteropServices
Imports EasyRules.API
Imports EasyRules.Attributes

Namespace Core

    Public NotInheritable Class RuleProxy
        Private Shared ReadOnly Validator As RuleDefinitionValidator = New RuleDefinitionValidator()

        Public Shared Function AsRule(rule As Object) As API.Rule
            If TypeOf rule Is API.Rule Then
                Return DirectCast(rule, API.Rule)
            Else
                Validator.ValidateRuleDefinition(rule)

                Dim builder As RuleBuilder = New RuleBuilder()

                Dim outStr As String = String.Empty

                If TryGetRuleName(rule, outStr) Then
                    builder.WithName(outStr)
                End If

                If TryGetRuleDescription(rule, outStr) Then
                    builder.WithDescription(outStr)
                End If

                Dim outInt As Integer = 0

                If TryGetRulePriority(rule, outInt) Then
                    builder.WithPriority(outInt)
                End If

                builder.ThatWhen(GetCondition(rule))

                For Each action As Action(Of Facts) In GetExecutionActions(rule)
                    builder.ThenDoes(action)
                Next action

                Return builder.Build()
            End If
        End Function

        Private Shared Function TryGetRuleName(rule As Object, <Out()> ByRef name As String) As Boolean
            name = String.Empty

            Try
                Dim ruleAttribute As Attributes.Rule = CType(rule.GetType().GetCustomAttributes(False).Single(Function(attribute) TypeOf attribute Is Attributes.Rule), Attributes.Rule)

                name = ruleAttribute.Name
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Shared Function TryGetRuleDescription(rule As Object, <Out()> ByRef description As String) As Boolean
            description = String.Empty

            Try
                Dim ruleAttribute As Attributes.Rule = CType(rule.GetType().GetCustomAttributes(False).Single(Function(attribute) TypeOf attribute Is Attributes.Rule), Attributes.Rule)

                description = ruleAttribute.Description
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Shared Function TryGetRulePriority(rule As Object, <Out()> ByRef priority As Integer) As Boolean
            Try
                Dim ruleAttribute As Attributes.Rule = CType(rule.GetType().GetCustomAttributes(False).Single(Function(attribute) TypeOf attribute Is Attributes.Rule), Attributes.Rule)

                priority = ruleAttribute.Priority
                Return True
            Catch ex As Exception
                priority = Nothing
                Return False
            End Try
        End Function

        Private Shared Function GetCondition(rule As Object) As Func(Of Facts, Boolean)
            Dim condition As MethodInfo = rule.GetType().GetMethods().Single(Function(method) method.IsDefined(GetType(Condition), False))

            Dim parameters As ParameterInfo() = condition.GetParameters()

            If parameters.Length = 1 AndAlso GetType(Facts).IsAssignableFrom(parameters.Single().ParameterType) Then
                Return CType(condition.CreateDelegate(GetType(Func(Of Facts, Boolean)), rule), Func(Of Facts, Boolean))
            Else
                Return AddressOf New ConditionProxy(condition, rule).ProxyMethod
            End If
        End Function

        Private Class ConditionProxy
            Private ReadOnly _method As MethodInfo
            Private ReadOnly _originalRule As Object
            Private ReadOnly _factNames As List(Of String) = New List(Of String)

            Public Sub New(method As MethodInfo, originalRule As Object)
                _method = method
                _originalRule = originalRule
                _factNames.AddRange(GetFactNames(method))
            End Sub

            Private Shared Function GetFactNames(method As MethodInfo) As IEnumerable(Of String)
                Return method.GetParameters() _
                             .Select(Function(param) param.GetCustomAttribute(Of Attributes.Fact).FactName)
            End Function

            Public Function ProxyMethod(facts As Facts) As Boolean
                Return CBool(_method.Invoke(_originalRule, GetFactsByName(facts, _factNames)))
            End Function

            Private Shared Function GetFactsByName(facts As Facts, names As IEnumerable(Of String)) As Object()
                Return names.Select(Function(name) facts.GetFact(name).Value).ToArray()
            End Function
        End Class

        Private Shared Function GetExecutionActions(rule As Object) As IEnumerable(Of Action(Of Facts))
            Dim actions As IEnumerable(Of MethodInfo) = rule.GetType().GetMethods().Where(Function(method) method.IsDefined(GetType(Action), False))

            Dim returnValue = New List(Of Action(Of Facts))

            For Each actionMethod As MethodInfo In actions
                If actionMethod.GetParameters().Length = 1 _
                        AndAlso GetType(Facts).IsAssignableFrom(actionMethod.GetParameters().Single().ParameterType) Then
                    returnValue.Add(CType(actionMethod.CreateDelegate(GetType(Action(Of Facts)), rule), Action(Of Facts)))
                Else
                    returnValue.Add(AddressOf New ActionProxy(actionMethod, rule).ProxyMethod)
                End If
            Next actionMethod

            Return returnValue
        End Function

        Private Class ActionProxy
            Private ReadOnly _method As MethodInfo
            Private ReadOnly _originalRule As Object
            Private ReadOnly _factNames As List(Of String) = New List(Of String)

            Public Sub New(method As MethodInfo, originalRule As Object)
                _method = method 
                _originalRule = originalRule
                _factNames.AddRange(GetFactNames(method))
            End Sub

            Private Shared Function GetFactNames(method As MethodInfo) As IEnumerable(Of String)
                Return method.GetParameters() _
                             .Select(Function(param) param.GetCustomAttribute(Of Attributes.Fact).FactName)
            End Function

            Public Sub ProxyMethod(facts As Facts)
                _method.Invoke(_originalRule, GetFactsByName(facts, _factNames))
            End Sub

            Private Shared Function GetFactsByName(facts As Facts, names As IEnumerable(Of String)) As Object()
                Return names.Select(Function(name) facts.GetFact(name).Value).ToArray()
            End Function
        End Class
    End Class
End Namespace