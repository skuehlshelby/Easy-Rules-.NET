Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Reader
	''' <summary>
	''' Strategy interface for <see cref="RuleDefinition"/> readers.<br/>
	''' See: <br/>
	''' - <seealso cref="JsonRuleDefinitionReader"/><br/>
	''' - <seealso cref="YamlRuleDefinitionReader"/><br/>
	''' </summary>
	Public Interface IRuleDefinitionReader
		''' <summary>
		''' Read a list of rule definitions from a rule descriptor.
		''' <br/><br/>
		''' <strong> The descriptor is expected to contain a collection of rule definitions
		''' even for a single rule.</strong>
		''' </summary>
		''' <param name="reader">Reads the rule descriptors.</param>
		''' <returns>A list of rule definitions</returns>
		Function Read(reader As TextReader) As IEnumerable(Of RuleDefinition)
	End Interface

	Public MustInherit Class AbstractRuleDefinitionReader
		Implements IRuleDefinitionReader

		Public Function Read(reader As TextReader) As IEnumerable(Of RuleDefinition) Implements IRuleDefinitionReader.Read
			Return LoadRules(reader).Select(AddressOf CreateRuleDefinition)
		End Function

		Protected MustOverride Function LoadRules(reader As TextReader) As IEnumerable(Of Dictionary(Of String, Object))

		Protected Function CreateRuleDefinition(dictionary As IDictionary(Of String, Object)) As RuleDefinition
			Dim definition = New RuleDefinition()

			Dim outValue As Object = Nothing

			If dictionary.TryGetValue(NameOf(RuleDefinition.Name).ToLower(), outValue) Then definition.Name = DirectCast(outValue, String)
			If dictionary.TryGetValue(NameOf(RuleDefinition.Description).ToLower(), outValue) Then definition.Description = DirectCast(outValue, String)
			If dictionary.TryGetValue(NameOf(RuleDefinition.Priority).ToLower(), outValue) Then definition.Priority = CType(outValue, Integer)
			If dictionary.TryGetValue(NameOf(RuleDefinition.CompositeRuleType).ToLower(), outValue) Then definition.CompositeRuleType = DirectCast(outValue, String)
			If dictionary.TryGetValue(NameOf(RuleDefinition.Condition).ToLower(), outValue) Then definition.Condition = DirectCast(outValue, String)
			If dictionary.TryGetValue(NameOf(RuleDefinition.Actions).ToLower(), outValue) Then definition.Actions = DirectCast(outValue, List(Of String))

			If String.IsNullOrWhiteSpace(definition.Condition) AndAlso String.IsNullOrWhiteSpace(definition.CompositeRuleType) _
				Then Throw New ArgumentException("The rule condition must be specified")

			If Not definition.Actions.Any() AndAlso String.IsNullOrWhiteSpace(definition.CompositeRuleType) _
				Then Throw New ArgumentException("The rule action(s) must be specified")

			Dim composingRules As New List(Of Object)()
			If dictionary.TryGetValue(NameOf(RuleDefinition.ComposingRules), outValue) Then composingRules = DirectCast(outValue, List(Of Object))

			If composingRules.Any() AndAlso String.IsNullOrWhiteSpace(definition.CompositeRuleType) Then
				Throw New ArgumentException("Non-composite rules cannot have composing rules")
			ElseIf Not composingRules.Any() AndAlso Not String.IsNullOrWhiteSpace(definition.CompositeRuleType) Then
				Throw New ArgumentException("Composite rules must have composing rules specified")
			ElseIf composingRules.Any() Then
				definition.ComposingRules = composingRules.OfType(Of IDictionary(Of String, Object)).Select(AddressOf CreateRuleDefinition).ToList()
			End If

			Return definition
		End Function
	End Class

	Public Class JsonRuleDefinitionReader
		Inherits AbstractRuleDefinitionReader

		Protected Overrides Function LoadRules(reader As TextReader) As IEnumerable(Of Dictionary(Of String, Object))
			Dim results = New List(Of Dictionary(Of String, Object))

			Using jsonReader As New JsonTextReader(reader)
				Dim serializer = JsonSerializer.CreateDefault()
				Dim deserialized = serializer.Deserialize(jsonReader)

				For Each jObject In ToJObects(deserialized)
					results.Add(jObject.Children(Of JProperty)().ToDictionary(Function(p) p.Name, Function(v)
																									  If TypeOf v.Value Is JArray Then
																										  Return DirectCast(v.Value, JArray).ToObject(Of List(Of String))()
																									  End If
																									  Return DirectCast(v.Value, JValue).Value
																								  End Function))
				Next
			End Using

			Return results
		End Function

		Private Function ToJObects(jobject As Object) As IEnumerable(Of JObject)
			If TypeOf jobject Is JArray Then
				Return DirectCast(jobject, JArray).Children(Of JObject)
			ElseIf TypeOf jobject Is JObject Then
				Return New JObject() {DirectCast(jobject, JObject)}
			Else
				Throw New Exception()
			End If
		End Function
	End Class

	Public Class YamlRuleDefinitionReader

	End Class
End Namespace

