Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Logging.Abstractions

Public Module LibraryLogging

    Public Property Factory As ILoggerFactory = New NullLoggerFactory()

    Public Function GetLogger(Of T)() As ILogger
        Return Factory.CreateLogger(Of T)()
    End Function

End Module