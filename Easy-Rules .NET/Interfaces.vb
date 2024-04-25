Imports Microsoft.Extensions.Logging

Public Interface IRule
    ReadOnly Property Name As String
    ReadOnly Property Description As String
    ReadOnly Property Priority As Integer
    Function Evaluate(facts As IFacts) As Boolean
    Sub Execute(facts As IFacts)
End Interface

Public Interface IRules
    Inherits IReadOnlyCollection(Of IRule)
    Function Add(rule As IRule) As Boolean
    Function Remove(rule As IRule) As Boolean
    Sub Clear()
End Interface

Public Interface IFact
    ReadOnly Property Name As String
End Interface

Public Interface IFact(Of Out T)
    Inherits IFact
    ReadOnly Property Value As T
End Interface

Public Interface IFacts
    Inherits IReadOnlyCollection(Of IFact)
    Function Add(fact As IFact) As Boolean
    Function Remove(fact As IFact) As Boolean
    Sub Clear()
End Interface

Public Interface IRulesEngineParameters
    ReadOnly Property PriorityThreshold As Integer
    ReadOnly Property SkipOnFirstAppliedRule As Boolean
    ReadOnly Property SkipOnFirstNonTriggeredRule As Boolean
    ReadOnly Property SkipOnFirstFailedRule As Boolean
    ReadOnly Property LoggerFactory As ILoggerFactory
End Interface

Public Class RuleEventArgs
    Inherits EventArgs

    Public Sub New(rule As IRule, facts As IFacts)
        Me.Facts = facts
        Me.Rule = rule
        Cancel = False
    End Sub

    Public ReadOnly Property Facts As IFacts
    Public ReadOnly Property Rule As IRule
    Public Property Cancel As Boolean
End Class

Public Class RuleFailureEventArgs
    Inherits RuleEventArgs

    Public Sub New(rule As IRule, facts As IFacts, exception As Exception)
        MyBase.New(rule, facts)
        Me.Exception = exception
    End Sub

    Public ReadOnly Property Exception As Exception
End Class

Public Class RulesEngineEventArgs
    Inherits EventArgs

    Public Sub New(rules As IRules, facts As IFacts)
        Me.Facts = facts
        Me.Rules = rules
    End Sub
    Public ReadOnly Property Facts As IFacts

    Public ReadOnly Property Rules As IRules
End Class

Public Interface IRulesEngine
    Event BeforeRuleEvaluation As EventHandler(Of RuleEventArgs)

    Event AfterRuleEvaluation As EventHandler(Of RuleEventArgs)

    Event BeforeRuleExecution As EventHandler(Of RuleEventArgs)

    Event AfterRuleExecution As EventHandler(Of RuleEventArgs)

    Event OnRuleEvaluationError As EventHandler(Of RuleFailureEventArgs)

    Event OnRuleExecutionError As EventHandler(Of RuleFailureEventArgs)

    Event BeforeRulesEvaluation As EventHandler(Of RulesEngineEventArgs)

    Event AfterRulesEvaluation As EventHandler(Of RulesEngineEventArgs)

    ReadOnly Property Parameters As IRulesEngineParameters
    Function Evaluate(rules As IRules, facts As IFacts) As IEnumerable(Of KeyValuePair(Of IRule, Boolean))
    Sub Execute(rules As IRules, facts As IFacts)
End Interface

Public Class NoSuchFactException
    Inherits Exception

    Public Sub New(missingFact As String)
        Me.MissingFact = missingFact
    End Sub

    Public Sub New(message As String, missingFact As String)
        MyBase.New(message)
        Me.MissingFact = missingFact
    End Sub

    Public Sub New(message As String, innerException As Exception, missingFact As String)
        MyBase.New(message, innerException)
        Me.MissingFact = missingFact
    End Sub

    Public ReadOnly Property MissingFact As String
End Class