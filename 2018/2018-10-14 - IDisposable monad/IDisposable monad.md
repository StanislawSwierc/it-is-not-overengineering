# IDisposable monad (IUsable\<T\>) - part 1

Tags: `Functional programming`, `C#`

# Introduction

I've always said that C# is my favorite functional programming language. It is not a functional-first language so you have to be careful, but results are worth it as the FP code tends to be clearer and more maintainable. Recently, I discovered that there was a new book published on the topic of FP in C# - "*Functional Programming in C#: How to write better C# code*" by Enrico Buonanno. I learned about it from a newsletter listing recommended books on Safari Books Online. This was in fact a great recommendation!

[![Functional Programming in C#: How to write better C# code](cover.jpg)][1]

I would recommend this book to everyone who writes software in C#. Even if it doesn't turn you into a FP enthusiast it will definitely change the way you think about certain problems and make you a better C# programmer. It is full of examples, patterns and references to complete source code hosted on GitHub.

The chapter I enjoyed the most was "*Chapter 11. Lazy computations, continuations, and the beauty of monadic composition*". I first learned about how to use monads in C# from an [amazing, 13-part series of blogs by Eric Lippert][3]. Enrico in his book offered slightly different approach. Instead of focusing on the theory, he prepared many examples that guide the reader though a series of problems and solutions to finally land on a beautifully composed program. One of these examples was `Middleware<T>` monad, which is similar to continuation, but adds behaviors both before and after a given function. This was new pattern to me and very intriguing!

Requirement to define behavior before and after some operation is very common. Every time you work with resources that have to be released (e.g. streams) you need to write the setup and teardown code. `Middleware<T>` delegate is defined as a higher order function which accepts a function to act on the resource/state (this pattern was first introduced in the book in section 1.5.1. "*Encapsulating setup and teardown into a HOF*"). When working with `Middleware<T>` callers specify only the function they want to apply to the resource, but they have no control over how the resource is created nor how it is released. This is very powerful pattern because you cannot get it wrong. No matter what you do, teardown code will be executed.

Another great thing about `Middleware<T>` is the fact that you can compose them to form data pipelines. For example you could combine "stopwatch" middleware with "open database connection" middleware and "open transaction scope" middleware to define pipeline for working with databases in your application. I recommend reading the book for other great examples.

# Problem statement

In the book `Middleware<T>` was defined as the following [delegate with a set of extension methods][4].

```cs
public delegate dynamic Middleware<T>(Func<T, dynamic> cont);

public static class Middleware
{
    public static T Run<T>(this Middleware<T> mw) => mw(t => t);

    // [...]

    public static Middleware<RR> SelectMany<T, R, RR>
        (this Middleware<T> @this, Func<T, Middleware<R>> f, Func<T, R, RR> project)
        => cont => @this(t => f(t)(r => cont(project(t, r))));
}
```

There is one important element in this definition worth discussing in detail - `Middleware<T>` accepts and returns `dynamic`! Why is it a big deal? Remember when I praised this pattern for making sure you cannot get it wrong? While it still guarantees that teardown code executes, with `dynamic` there are other traps you may fall into. Author also identified this as a problem:

>The problem is that `T` (the input to the continuation) and R (its output) are not known at the same time. For example, suppose you want to create a `Middleware` instance from a function such as `Connect`, which has signature<br>
><br>
> `public static R Connect<R>(ConnectionString connString, Func<SqlConnection, R> func)`<br>
><br>
>The continuation accepted by Connect takes a `SqlConnection` as input, so we can use Connect to define a `Middleware<SqlConnection>`. That means the `T` type variable in `Middleware<T>` resolves to `SqlConnection`, but we don't yet know what the given continuation will yield, so we can't yet resolve the `R` type variable in `Connect<R>`.<br>
><br>
>**Unfortunately, C# doesn't allow us to "partially apply" type variables; hence, dynamic. So although conceptually we're thinking of combining HOFs of this type**<br>
><br>
>`(T → R) → R`<br>
><br>
>we’re modeling them as follows:<br>
><br>
>`(T → dynamic) → dynamic`<br>
><br>
>Later you'll see that you can still work with Middleware without compromising on type safety.<br>
><br>
>*~Functional Programming in C#: How to write better C# code*

After I read this section it stayed in my head for long. Although Enrico explained how to alleviate this problem with `Run<T>` method I couldn't stop thinking about it. *C# doesn't allow us to "partially apply"*... Does it mean that C# type system isn't flexible enough to define `Middleware<T>`? What a great riddle!

I decided to run some experiments and try to answer the following question:

>Is it possible to define `Middleware<T>` in C# in a type-safe manner without falling back to `dynamic`?

# Solution

I started by trying to understand the bigger picture of what problem `Middleware<T>` is supposed to solve. It defines resource of type `<T>` and accepts a function to apply to the resource. This function can return arbitrary type, which is unknown at the time when `Middleware<T>` is created. This coupling is the main problem. As Enrico correctly pointed out C# doesn't allow us to "partially apply" type so solving this problem will require splitting generic parameters `<T, TResult>` into two constructs. It is impossible to do it with a single delegate, but C# has much more to offer. After all, it is an Object Oriented language.

## Interface

When I was looking for how to represent `Middleware<T>` as object I found an elegant solution which I will call `IUsable<T>` to separate it from the middleware.

```cs
public interface IUsable<T>
{
    TResult Use<TResult>(Func<T, TResult> func);
}
```

In its definition `IUsable<T>` has two type parameters: `T` for the resource type used on the interface and `TResult` for the result of computation used on the `Use` method. This separation is the key to the partial application of type parameters. When you create an instance of `IUsable<T>` you do need to specify one type parameter (partial) and when you want to use the resource with `Use<TResult>` you specify second type parameter. Pretty simple when you think about it.

For this interface to follow monadic composition pattern I also need to define few additional operators.

## Amplification

First of all we do need a way to go from a regular value to `IUsable<T>` (amplify type). This can be achieved with the following extension methods `AsUsable<T>` and `AsUsableOnce<T>`. I decided to create both options because it is very common to use `IDisposable` resource just once. Since there is a disconnect between the time usable is instantiated and the time when the value is actually used I created specialized classes to keep the state.

```cs
public static partial class Usable
{
    public static IUsable<T> AsUsable<T>(this T value) =>
        new ValueUsable<T>(value);

    public static IUsable<T> AsUsableOnece<T>(this T value)
        where T : IDisposable =>
        new UsableOnce<T>(value);

    internal class ValueUsable<T> : IUsable<T>
    {
        private readonly T value;

        public ValueUsable(T value) =>
            this.value = value;

        public TResult Use<TResult>(Func<T, TResult> func) =>
            func(this.value);
    }

    internal class UsableOnce<T> : IUsable<T>
        where T : IDisposable
    {
        private readonly T value;

        public UsableOnce(T value) =>
            this.value = value;

        public TResult Use<TResult>(Func<T, TResult> func)
        {
            using (this.value)
            {
                return func(value);
            }
        }
    }
}
```

## Composition

Second, we need a way to compose two usables. Since this post is about C# I will demonstrate implementation of `SelectMany` with 3 type parameters. Although it is not the signature of the canonical bind operator, this is the code you want in your application as this method unlocks the power of composition with LINQ expression language. Similarly to the amplification section, here I also decided to create a specialized inner class to save usable and selector delegates for the time when usable is used (lazy evaluation).

```cs
public static partial class Usable
{
    public static IUsable<TResult> SelectMany<TOuter, TInner, TResult>(
        this IUsable<TOuter> outerUsable,
        Func<TOuter, IUsable<TInner>> innerUsableSelector,
        Func<TOuter, TInner, TResult> resultSelector)
    {
        return new SelectManyUsable<TOuter, TInner, TResult>(
            outerUsable, innerUsableSelector, resultSelector);
    }

    internal class SelectManyUsable<TOuter, TInner, T> : IUsable<T>
    {
        private readonly IUsable<TOuter> outerUsable;
        private readonly Func<TOuter, IUsable<TInner>> innerUsableSelector;
        private readonly Func<TOuter, TInner, T> resultSelector;

        public SelectManyUsable(
            IUsable<TOuter> outerUsable,
            Func<TOuter, IUsable<TInner>> innerUsableSelector,
            Func<TOuter, TInner, T> resultSelector)
        {
            this.outerUsable = outerUsable;
            this.innerUsableSelector = innerUsableSelector;
            this.resultSelector = resultSelector;
        }

        public TResult Use<TResult>(Func<T, TResult> func)
        {
            return outerUsable.Use(outerScope =>
            {
                return innerUsableSelector(outerScope).Use(innerScope =>
                {
                    return func(resultSelector(outerScope, innerScope));
                });
            });
        }
    }
}
```

## Extraction

Finally we need a way to extract the value from the usable (exit monad). One natural option is actually use the usable and calculate result. I decided, however, to add helper method to easily extract the value from usable. Please notice that it applies identity function and returns resource outside of the `Use<TResult>` scope. This is potentially risky behavior as one could try to extract disposable resource just to discover that it already has been disposed.

```cs
public static partial class Usable
{
    public static T Value<T>(this IUsable<T> usable)
    {
        return usable.Use(value => value);
    }
}
```

That's it! These are the elements we need to replicate behavior of `Middleware<T>` in a type-safe way. Now, let's see how to use it all in an application.

# Example

To demonstrate how to use the `IUsable<T>` I will use the same example that was introduced in the book. It shows how to build a repository (Repository pattern) to access `Orders` stored in SQL database. We will focus on the `Delete` method which should have the following properties:

* Delete operation executes in the `TraceSourceScope` to keep the logs clean.
* When we delete orders we also need to delete order lines within the same transaction.
* All disposable resources are released.

The following code satisfies these requirements. Please notice how `DeleteUsable` was implemented. It is a single LINQ expression which combines trace, connection and transaction to create a right scope for executing SQL statements. All the resources are amplified to `IUsable<T>` with `AsUsableOnce<T>` which guarantees that they are going to be released. The computation in select combines two steps. Finally, there is `Delete` method to simplify the interface in case callers don't need to compose delete operation with other operations in the system.

> *In the next post I will show how we can introduce new `SelectMany` operator to remove `AsUsableOnce` calls.*

```cs
using System;

// NuGet
using Dapper;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging.TraceSource;

namespace UsableRepositories
{
    public class OrdersRepository
    {
        const string deleteLines = "DELETE OrderLines WHERE OrderId = @Id";
        const string deleteOrder = "DELETE Orders WHERE OrderId = @Id";

        private readonly string connectionString;

        public OrdersRepository(string connectionString) =>
            this.connectionString = connectionString;

        public void Delete(Guid id) =>
            DeleteUsable(id).Value();

        public IUsable<int> DeleteUsable(Guid id) =>
            from trace in new TraceSourceScope("Execute: DeleteUsable").AsUsableOnece()
            from conn in new SqlConnection(connectionString).AsUsableOnece()
            from tran in conn.BeginTransaction().AsUsableOnece()
            select conn.Execute(deleteLines, id, tran)
                + conn.Execute(deleteOrder, id, tran);
    }
}
```

# Performance analysis

We've seen how `IUsable<T>` can help with writing succinct code with, but did we have to sacrifice performance to achieve it? To investigate the impact on performance I prepared a benchmark which compares classical `using` statement, `Middleware<T>` and `IUsable<T>` in a typical scenario where you want to execute some operation in the context of 3 disposable resources. I selected `TraceSourceScope` as the resource because it is super fast, thus, the benchmark should highlight the cost of composition rather than the costs of acquiring and releasing resources.

Benchmark was defined with [BenchmarkDotNet][7], which is a great library/runner for .NET.

```cs
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
            Trace.WriteLine(s1.Operation + s2.Operation + s3.Operation;);
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
            from s2 in new UsableTraceSourceScope(10)
            from s3 in new UsableTraceSourceScope(100)
            select s1.Operation + s2.Operation + s3.Operation;

        Trace.WriteLine(usable.Value());
    }


    // [...]
}

```

After running this benchmark for a couple of minutes I received the following result. It was full of surprises!

|              Method |    N |     Mean |    Error |   StdDev |
|-------------------- |----- |---------:|---------:|---------:|
|      UsingStatement | 1000 | 765.5 us | 7.996 us | 7.089 us |
|          Middleware | 1000 | 621.4 us | 8.591 us | 7.615 us |
| Usable_AsUsableOnce | 1000 | 623.0 us | 8.354 us | 7.406 us |
| Usable_CustomUsable | 1000 | 622.0 us | 8.825 us | 7.823 us |

First of all, There was no difference between `Middleware<T>` and `IUsable<T>`. This was a great news because it means that the extra state required to implement `IUsable<T>` balances the impact of `dynamic` in `Middleware<T>`. Second, very practical observation, was that there was no difference between composing usables with `AsUsableOnce` or by creating a custom class. Finally, both `Middleware<T>` and `Usable<T>` showed noticeable improvement from the classical `using` statement. This one is hard for me to trust, but I haven't spot anything wrong in the benchmark to reject this result so I decided to keep it here.

# Conclusions

Though this post I wanted to demonstrate that it is possible to implement the `Middleware<T>` pattern in a type-safe way without falling back to `dynamic`. Solution involved combining Object-Oriented code with Functional Programming concepts to work around limitations of C# type system. Although `IUsable<T>` was defined as an interface and not a delegate, I could define necessary operators to fully implement monad pattern and enable composition through LINQ expressions.

One could think why to go through all this effort to implement this pattern when you can simply use `using` statement to manage the lifetime of your resources. I'll answer with a citation from Enrico's book because I think he captured it well.

> One fundamental difference between the functional and imperative style is that imperative code relies on statements; functional code relies on expressions. In essence, expressions have a value; statements don’t. While expressions such as function calls can have side effects, statements only have side effects, so they don’t compose.<br>
><br>
>*~Functional Programming in C#: How to write better C# code; section 5.5.1. Expressions vs. statements*

Yes, it is all about side-effects and composition! Next time I will show how we can add few specialized operators to help us with `IDisposable` composition. This will allow us to skip the `AsUsableOnece` used throughout examples in this post.

>PS. I'm still looking for a good name for a library. So far I selected "usable" to emphasize relationship to `using` statement because cleaning up (disposing) resources is one of the most important use cases for `IUsable<T>`. If you have some better ideas please leave them in the comments section.





## References:

1. [Functional Programming in C#: How to write better C# code - Amazon][1]
2. [Functional Programming in C#: How to write better C# code - Code samples][2]
3. [Fabulous adventures in coding - Monads][3]
4. [Middleware - Code definition][4]
5. [Middleware - Code examples][5]
6. [Usable Extensions - Source code][6]
7. [BenchmarkDotNet - Powerful .NET library for benchmarking][7]

[1]: https://www.amazon.com/Functional-Programming-write-better-code/dp/1617293954 
[2]: https://github.com/la-yumba/functional-csharp-code
[3]: https://ericlippert.com/category/monads/page/1/
[4]: https://github.com/la-yumba/functional-csharp-code/blob/master/Examples/Chapter11/Middleware/Middleware.cs#L10
[5]: https://github.com/la-yumba/functional-csharp-code/blob/master/Examples/Chapter11/Middleware/DbLogger.cs
[6]: https://github.com/StanislawSwierc/Usable
[7]: https://github.com/dotnet/BenchmarkDotNet