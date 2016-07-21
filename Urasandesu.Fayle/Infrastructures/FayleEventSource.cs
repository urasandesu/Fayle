/* 
 * File: FayleEventSource.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */



using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace Urasandesu.Fayle.Infrastructures
{
    [EventSource(Name = "Urasandesu Fayle")]
    public sealed class FayleEventSource : EventSource
    {
        public static readonly FayleEventSource Log = new FayleEventSource();

        public class Keywords
        {
            protected Keywords() { }
            public const EventKeywords Performance = (EventKeywords)1;
            public const EventKeywords Diagnostic = (EventKeywords)2;
        }

        [Event(1, Level = EventLevel.Verbose, Keywords = Keywords.Performance, Message = "{0}")]
        void PerformanceCore(string Message, string MemberName, string FilePath, int Line)
        {
            WriteEvent(1, Message, MemberName, FilePath, Line);
        }

        [NonEvent]
        public void Performance(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Performance))
                PerformanceCore(message, memberName, filePath, line);
        }

        [NonEvent]
        public void Performance<T>(string format, T arg, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Performance))
                PerformanceCore(string.Format(format, arg), memberName, filePath, line);
        }

        [NonEvent]
        public void Performance<T1, T2>(string format, T1 arg1, T2 arg2, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Performance))
                PerformanceCore(string.Format(format, arg1, arg2), memberName, filePath, line);
        }

        [NonEvent]
        public void Performance<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Performance))
                PerformanceCore(string.Format(format, arg1, arg2, arg3), memberName, filePath, line);
        }

        [Event(2, Level = EventLevel.Verbose, Keywords = Keywords.Diagnostic, Message = "{0}")]
        void DiagnosticCore(string Message, string MemberName, string FilePath, int Line)
        {
            WriteEvent(2, Message, MemberName, FilePath, Line);
        }

        [NonEvent]
        public void Diagnostic(string message, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Diagnostic))
                DiagnosticCore(message, memberName, filePath, line);
        }

        [NonEvent]
        public void Diagnostic<T>(string format, T arg, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Diagnostic))
                DiagnosticCore(string.Format(format, arg), memberName, filePath, line);
        }

        [NonEvent]
        public void Diagnostic<T1, T2>(string format, T1 arg1, T2 arg2, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Diagnostic))
                DiagnosticCore(string.Format(format, arg1, arg2), memberName, filePath, line);
        }

        [NonEvent]
        public void Diagnostic<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0)
        {
            if (IsEnabled(EventLevel.Verbose, (EventKeywords)Keywords.Diagnostic))
                DiagnosticCore(string.Format(format, arg1, arg2, arg3), memberName, filePath, line);
        }
    }
}

