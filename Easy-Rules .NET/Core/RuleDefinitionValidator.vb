Imports System.Reflection
Imports RulesEngine.API

Namespace Core
    Public Class RuleDefinitionValidator
        Public Sub ValidateRuleDefinition(rule As Object)
            CheckRuleClass(rule)
            CheckConditionMethod(rule)
            CheckActionMethods(rule)
        End Sub

        Private Sub CheckRuleClass(rule As Object)
            If Not IsRuleClassWellDefined(rule) Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' is not annotated with '{NameOf(Attributes.Rule)}'.")
            End If
        End Sub

        Private Sub CheckConditionMethod(rule As Object)
            Dim conditionMethods As List(Of MethodInfo) = GetMethodsAnnotatedWith(Of Attributes.Condition)(rule)

            If Not conditionMethods.Any() Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' must have a public method annotated with '{NameOf(Attributes.Condition)}'.")
            End If

            If conditionMethods.Count > 1 Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' must have exactly one public method annotated with '{NameOf(Attributes.Condition)}'.")
            End If

            Dim condition As MethodInfo = conditionMethods.Single()

            If Not IsConditionMethodWellDefined(condition) Then
                Throw New ArgumentException($"Condition method '{condition.Name}' defined in rule '{rule.GetType().Name}' must be public, must return a boolean value, and may have parameters annotated with @Fact (and/or exactly one parameter of type Facts or one of its sub-types).")
            End If
        End Sub

        Private Sub CheckActionMethods(rule As Object)
            Dim actionMethods As List(Of MethodInfo) = GetMethodsAnnotatedWith(Of Attributes.Action)(rule)

            If Not actionMethods.Any() Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' must have at least one public method annotated with '{NameOf(Attributes.Action)}'.")
            End If

            For Each actionMethod As MethodInfo In actionMethods
                If Not IsActionMethodWellDefined(actionMethod) Then
                    Throw New ArgumentException($"Action method '{actionMethod.Name}' defined in rule '{rule.GetType().Name}' must be public, must return void, and may have parameters annotated with @Fact (and/or exactly one parameter of type Facts or one of its sub-types).")
                End If
            Next actionMethod
        End Sub

        Private Function IsRuleClassWellDefined(rule As Object) As Boolean
            Return rule.GetType.IsDefined(GetType(Attributes.Rule), False)
        End Function

        Private Function IsConditionMethodWellDefined(method As MethodInfo) As Boolean
            Return method.IsPublic AndAlso method.ReturnType.Equals(GetType(Boolean)) AndAlso HasValidParameters(method)
        End Function

        Private Function HasValidParameters(method As MethodInfo) As Boolean
            Dim parameters As ParameterInfo() = method.GetParameters()

            If parameters.Length = 0 Then
                Return True
            ElseIf parameters.All(Function(parameter) parameter.IsDefined(GetType(Attributes.Fact))) Then
                Return True
            ElseIf parameters.Length = 1 Then
                Return GetType(Facts).IsAssignableFrom(parameters.Single().ParameterType())
            Else
                Return False
            End If
        End Function

        Private Function IsActionMethodWellDefined(method As MethodInfo) As Boolean
            Return method.IsPublic AndAlso method.ReturnType.Equals(GetType(Void)) AndAlso HasValidParameters(method)
        End Function

        Private Function IsPriorityMethodWellDefined(method As MethodInfo) As Boolean
            Return method.IsPublic AndAlso method.ReturnType.Equals(GetType(Integer)) AndAlso HasValidParameters(method)
        End Function

        Private Function GetMethodsAnnotatedWith(Of T As Attribute)(rule As Object) As List(Of MethodInfo)
            Return rule.GetType().GetMethods().Where(Function(method) method.IsDefined(GetType(T))).ToList()
        End Function
    End Class
End Namespace