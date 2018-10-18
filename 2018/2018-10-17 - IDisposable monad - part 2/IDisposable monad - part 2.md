# IDisposable monad (IUsable) - part 2

Tags: `Functional programming`, `C#`

Previously I introduced `IUsable<T>` monad which can be used to compose computations in a safe way and guarantee that all the resources are disposed. In the examples there were many calls to `AsUsableOnce<T>` which were amplifying `IDisposable` types. In this post I will explain how we can add additional operator to simplify that code.

We will start with the same example as before. Please notice the presence of calls to `AsUsableOnce<T>`. They were necessary because the `SelectMany` operator expected `Func<TOuter, IUsable<TInner>>` selector and `AsUsableOnce<T>` was one of the way to provide required return type.

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

Knowing that `IUsable<T>` fits perfectly for the scenarios which use `IDisposable` resources we can optimize for this case. Just as we created `SelectMany` operator to work with `IUsable<T>` we can create a new operator dedicated to `IDisposable`. Since both operators have the same signature we need to put them in a separate classes.


```cs
public static class UsableDisposable
{
    public static IUsable<TResult> SelectMany<TOuter, TInner, TResult>(
        this IUsable<TOuter> outerUsable,
        Func<TOuter, TInner> innerDisposableSelector,
        Func<TOuter, TInner, TResult> resultSelector)
        where TInner : IDisposable
    {
        return new SelectManyDisposableUsable<TOuter, TInner, TResult>(
            outerUsable, innerDisposableSelector, resultSelector);
    }

    private class SelectManyDisposableUsable<TOuter, TInner, T> : IUsable<T>
        where TInner : IDisposable
    {
        private readonly IUsable<TOuter> source;
        private readonly Func<TOuter, TInner> collectionSelector;
        private readonly Func<TOuter, TInner, T> resultSelector;

        public SelectManyDisposableUsable(
            IUsable<TOuter> outerUsable,
            Func<TOuter, TInner> innerDisposableSelector,
            Func<TOuter, TInner, T> resultSelector)
        {
            this.source = outerUsable;
            this.collectionSelector = innerDisposableSelector;
            this.resultSelector = resultSelector;
        }

        public TResult Use<TResult>(Func<T, TResult> func)
        {
            return source.Use(outer =>
            {
                using (var inner = collectionSelector(outer))
                {
                    return func(resultSelector(outer, inner));
                }
            });
        }
    }
}
```

The biggest differences from the operator introduced in the previous post are the selector signature which accepts `TInner` directly and the `where` constraint on the type parameter. Just as before we had to introduce a type to hold the state (`SelectManyDisposableUsable`).

With this operator we can simplify original example to the following form:

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
            from conn in new SqlConnection(connectionString)
            from tran in conn.BeginTransaction()
            select conn.Execute(deleteLines, id, tran)
                + conn.Execute(deleteOrder, id, tran);
    }
}
```

There is still one call to `AsUsableOnce<T>` to enter the monad, but once we are there, we can compose it with `IDisposable` types without leaving it! Of course as long as we are in the monad we benefit from all its properties, which in this case is guaranteed disposal of all the resources.

What is a little surprising and amazing at the same time is that C# compiler can correctly pick the right extension method for each composition pattern. When you compose two usables, it selects one operator, but when you compose usable with `IDisposable` it selects another. There is one quirk when you have a class that implements `IUsable<T>` as compiler might still want to select the `IDisposable` version. This can be easily solved by adding `as IUsable<T>` suffix to force the right operator. This trick would work also if somebody decided to implement both interfaces on the same class.

Operators introduced in the previous and this post are by far the most important for working `IUsable<T>`. In the next post I will introduce other operators which can further simplify the code in specific scenarios.

## References

1. [It is not overengineering - IDisposable monad (IUsable) - part 1][1]
2. [Usable Extensions - Source code][2]

[1]: http://www.itisnotoverengineering.com/2018/10/idisposable-monad-iusable-part-1.html
[2]: https://github.com/StanislawSwierc/Usable
