using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Playground.WithLINQ.DbLogger;
using UsableExtensions;

namespace UsableExtensios.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<UsableBenchmark>();
        }
    }

    public class UsableBenchmark
    {
        [Params(1000)]
        public int N;

        [Benchmark]
        public void UsingStatement()
        {
            using (var s1 = new TraceSourceScope(1))
            using (var s2 = new TraceSourceScope(10))
            using (var s3 = new TraceSourceScope(100))
            {
                Trace.WriteLine(s1.Operation + s2.Operation + s3.Operation);
            }
        }

        [Benchmark]
        public void Middleware()
        {
            var middleware =
                from s1 in TraceSource(1)
                from s2 in TraceSource(10)
                from s3 in TraceSource(100)
                select s1.Operation + s2.Operation + s3.Operation;

            Trace.WriteLine(middleware.Run());
        }

        [Benchmark]
        public void Usable_AsUsableOnce()
        {
            var usable =
                from s1 in new TraceSourceScope(1).AsUsableOnece()
                from s2 in new TraceSourceScope(10).AsUsableOnece()
                from s3 in new TraceSourceScope(111).AsUsableOnece()
                select s1.Operation + s2.Operation + s3.Operation;

            Trace.WriteLine(usable.Value());
        }

        [Benchmark]
        public void Usable_CustomUsable()
        {
            var usable =
                from s1 in new UsableTraceSourceScope(1)
                from s2 in (new UsableTraceSourceScope(10) as IUsable<TraceSourceScope>)
                from s3 in new UsableTraceSourceScope(100)
                select s1.Operation + s2.Operation + s3.Operation;

            Trace.WriteLine(usable.Value());
        }

        Middleware<TraceSourceScope> TraceSource(int operation) =>
            f => CreateTraceSourceScope(operation, f);

        public static R CreateTraceSourceScope<R>(int operation, Func<TraceSourceScope, R> func)
        {
            using (var scope = new TraceSourceScope(operation))
            {
                return func(scope);
            }
        }

        public class UsableTraceSourceScope : IUsable<TraceSourceScope>
        {
            private readonly int operation;

            public UsableTraceSourceScope(int operation)
            {
                this.operation = operation;
            }

            public TResult Use<TResult>(Func<TraceSourceScope, TResult> func)
            {
                using (var scope = new TraceSourceScope(operation))
                {
                    return func(scope);
                }
            }
        }
    }

    public class TraceSourceScope : IDisposable
    {
        private bool _isDisposed;

        public int Operation { get; }

        public TraceSourceScope(int operation)
        {
            this.Operation = operation;
            Trace.WriteLine($"Enter: {this.Operation}");
            Trace.Indent();
        }

        public void Dispose()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(nameof(TraceSourceScope));
            }
            Trace.Unindent();
            Trace.WriteLine($"Leave: {this.Operation}");
            this._isDisposed = true;
        }
    }
}