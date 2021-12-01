Imports System.Reflection
Imports Easy_Rules_.NET.API
Imports Easy_Rules_.NET.Attributes

Namespace Core
    Public Class RuleDefinitionValidator
        Public Sub ValidateRuleDefinition(rule As Object)
            CheckRuleClass(rule)
            CheckConditionMethod(rule)
            CheckActionMethods(rule)
            CheckPriorityMethod(rule)
        End Sub

        Private Shared Sub CheckRuleClass(rule As Object)
            If Not IsRuleClassWellDefined(rule) Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' is not annotated with '{NameOf(Attributes.Rule)}'.")
            End If
        End Sub

        Private Shared Sub CheckConditionMethod(rule As Object)
            Dim conditionMethods As List(Of MethodInfo) = GetMethodsAnnotatedWith(Of Condition)(rule)

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

        Private Shared Sub CheckActionMethods(rule As Object)
            Dim actionMethods As List(Of MethodInfo) = GetMethodsAnnotatedWith(Of Action)(rule)

            If Not actionMethods.Any() Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' must have at least one public method annotated with '{NameOf(Action)}'.")
            End If

            For Each actionMethod As MethodInfo In actionMethods
                If Not IsActionMethodWellDefined(actionMethod) Then
                    Throw New ArgumentException($"Action method '{actionMethod.Name}' defined in rule '{rule.GetType().Name}' must be public, must return void, and may have parameters annotated with @Fact (and/or exactly one parameter of type Facts or one of its sub-types).")
                End If
            Next actionMethod
        End Sub

        Private Shared Sub CheckPriorityMethod(rule As Object)
            Dim priorityMethods As List(Of MethodInfo) = GetMethodsAnnotatedWith(Of Priority)(rule)

            If priorityMethods.Count = 0 Then
                Return
            End If

            If priorityMethods.Count > 1 Then
                Throw New ArgumentException($"Rule '{rule.GetType().Name}' must have no more than one public method annotated with '{NameOf(Priority)}'.")
            End If

            Dim priorityMethod As MethodInfo = priorityMethods.Single()

            If Not IsPriorityMethodWellDefined(priorityMethod) Then
                Throw New ArgumentException($"Priority method '{priorityMethod.Name}' defined in rule '{rule.GetType().Name}' must be public, must return an integer, and must have no input parameters.")
            End If
        End Sub

        Private Shared Function IsRuleClassWellDefined(rule As Object) As Boolean
            Return rule.GetType.IsDefined(GetType(Attributes.Rule), False)
        End Function

        Private Shared Function IsConditionMethodWellDefined(method As MethodInfo) As Boolean
            Return _
                method.IsPublic AndAlso 
                method.ReturnType Is GetType(Boolean) AndAlso 
                HasValidParameters(method)
        End Function

        Private Shared Function IsActionMethodWellDefined(method As MethodInfo) As Boolean
            Return _ 
                method.IsPublic AndAlso 
                method.ReturnType Is GetType(Void) AndAlso 
                HasValidParameters(method)
        End Function

        Private Shared Function HasValidParameters(method As MethodBase) As Boolean
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

        Private Shared Function IsPriorityMethodWellDefined(method As MethodInfo) As Boolean
            Return _
                method.IsPublic AndAlso 
                method.ReturnType Is GetType(Integer) AndAlso
                Not method.GetParameters().Any()
        End Function

        Private Shared Function GetMethodsAnnotatedWith(Of T As Attribute)(rule As Object) As List(Of MethodInfo)
            Return rule.GetType().GetMethods().Where(Function(method) method.IsDefined(GetType(T))).ToList()
        End Function
    End Class
End Namespace