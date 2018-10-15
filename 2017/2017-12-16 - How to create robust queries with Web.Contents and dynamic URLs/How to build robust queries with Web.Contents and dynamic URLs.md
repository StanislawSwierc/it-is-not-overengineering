# How to build robust queries with Web.Contents and dynamic URLs

Tags: `Power BI`, `Power Query`, `Web.Contents` 

Power Query is a great language for importing data from plethora of different sources. By far the most versatile source is the `Web.Contents`, which allows you to access data from any web page, REST API, or any HTTP endpoint. It is the building block for many Data Connectors both internal to Power BI and published externally by Power Query enthusiasts.

One not so obvious, thus very frustrating feature of `Web.Contents` function is that it works differently in Power BI Desktop and Power BI Service. When working in Desktop version you can dynamically build the URL and it will work just fine, but when you publish your dataset, you won't be able to refresh it. Many people run into this problem in the past and created several ideas in the Power BI forum related to this problem . At the time I'm writing this, there are 368 votes for the idea to \[[1], [2]\].

All of these ideas are still open because changing this behavior would require a fundamental change in how Power Query engine handled discovery and authentication for different data source. As explained by Curth from Power BI Team in one of the threads\[[3]\].

>*This is currently correct; queries where data access happens inside a function and where the data source is dependent on parameters to the function can't currently be refreshed. That's because we're doing static analysis of the query to discover the data source, and our static analysis can't yet handle this scenario.*

While this behavior might persist for long, there are some good workarounds. For example, Chris Webb explains how one can use `RelativePath` and `Query` options to add dynamicity to the query while preserving static URL, thus, meeting requirements of th static analysis \[[4]\]. In this post I would like to describe a design pattern I applied in one of my projects \[[5]\]. The strongest advantage is that with this approach you can easily refactor your code to make your queries refreshable in Power BI Service.

## Problem statement
Imagine that your data source is GitHub and you want to import all the issues created in the last 30 days. This is dynamic query as it contains element of current time. Is is also close to real life problems as analysis of the recent time period is a very common problem.

```pq
since = DateTimeZone.UtcNow() - #duration(7, 0, 0, 0),
```


```
https://api.github.com/repos/StanislawSwierc/it-is-not-overengineering/issues?since=2017-12-01T00:00:00Z
```



## References:
1. [Power BI Ideas - Web.Contents() should support scheduled refresh even with dynamic URL][1]
2. [Power BI Ideas - Make functions refreshable when the data source is a parameter of the function][2]
3. [Power BI Help - Integrations with Files and Services][3]
4. [Chris Webb's BI Blog - Web.Contents(), M Functions And Dataset Refresh Errors In Power BI][4]
5. [Visual Studio Team Services Marketplace - Open in Power BI][5]


[1]: https://ideas.powerbi.com/forums/265200-power-bi-ideas/suggestions/10927416-web-contents-should-support-scheduled-refresh-ev
[2]: https://ideas.powerbi.com/forums/265200-power-bi-ideas/suggestions/9312540-make-functions-refreshable-when-the-data-source-is
[3]: http://community.powerbi.com/t5/Integrations-with-Files-and/Refreshing-queries-with-functions-doens-t-work/td-p/2450
[4]: https://blog.crossjoin.co.uk/2016/08/23/web-contents-m-functions-and-dataset-refresh-errors-in-power-bi/
[5]: https://marketplace.visualstudio.com/items?itemName=stansw.vsts-open-in-powerbi

