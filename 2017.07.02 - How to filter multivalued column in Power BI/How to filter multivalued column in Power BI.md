# How to filter multivalued column in Power BI

Tags: `Power BI`, `DAX`

Recently someone asked me what is the best way to filter on a multi-valued columns in Power BI.
The question was in the context of `Tags` property of the `Work Items - Today` table in the Visual Studio Team Services [\[1\]][1].
Although the original question was very specific, the solution I came up with can be generalized to multivalued columns in any data model.

## What is a multivalued column?
Multivalued column is a database design pattern where instead of normalizing and splitting data across multiple tables you keep multiple values in a single table.
You can see it typically in the data warehouses where normalization would lead to a too granular fact tables.
One of the best examples is the `Categories` column in the `Product` table where want to allow users to select multiple values, but you don't want to create a separate `Categories` table.

In the context of Visual Studio Team Services there is one table where this pattern was applied - `Work Items - Today`.
It contains `Tags` column which is a `"; "` delimited list of tags like in the example below.

| Work Item Id | Title | Tags |
| - | - | - |
| 1 | Add column A to table B | database; milestone1 |
| 2 | Create migration script | database |
| 3 | Improve performance of slow queries | performance; database |
| ... | ... | ... |

## Problem statement

Given table with a multivalued column prepare data model that will allow users to easily filter on distinct values.

For example, we can start with the table below, which has multivalued `Tags` column.

```pq
let
    Source = #table(
        {"Work Item Id", "Title", "Tags"},
        {
            { "1", "Add column A to table B", "database; milestone1" },
            { "2", "Create migration script", "database" },
            { "3", "Improve performance of slow queries", "performance; database" }
        })
in
    Source
```

<div class="separator" style="clear: both; text-align: center;"><a href="https://3.bp.blogspot.com/-D7KsGrf7Um0/WVlGMss39sI/AAAAAAABIY4/oqLcr9G6TfYgN6uBmqOyS4B9CxYwrJH2QCLcBGAs/s1600/exaple_table.png" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="https://3.bp.blogspot.com/-D7KsGrf7Um0/WVlGMss39sI/AAAAAAABIY4/oqLcr9G6TfYgN6uBmqOyS4B9CxYwrJH2QCLcBGAs/s1600/exaple_table.png" data-original-width="513" data-original-height="85" /></a></div>

If we simply selected `Tags` for the slicer it would produce the following result.
Instead of values users could only select combinations that appear in the dataset.
That's **not** what we want.

<div class="separator" style="clear: both; text-align: center;"><a href="https://4.bp.blogspot.com/-j7eXtBxxjEE/WVlGMhp4IOI/AAAAAAABIY8/g2C-7qeC1wQ9UYLsPDrGkDMYgejaTLUmwCLcBGAs/s1600/exaple_badfilter.png" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="https://4.bp.blogspot.com/-j7eXtBxxjEE/WVlGMhp4IOI/AAAAAAABIY8/g2C-7qeC1wQ9UYLsPDrGkDMYgejaTLUmwCLcBGAs/s1600/exaple_badfilter.png" data-original-width="335" data-original-height="151" /></a></div>

A much better design is to extract distinct values from the `Tags` column so that we can build the following slicer.

<div class="separator" style="clear: both; text-align: center;"><a href="https://3.bp.blogspot.com/-8nfx_OGSdCI/WVlGMUQg3LI/AAAAAAABIY0/r6L71Nz3QjsGkwSLro6VZNJ9r7bTohc7gCLcBGAs/s1600/exaple_filter.png" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="https://3.bp.blogspot.com/-8nfx_OGSdCI/WVlGMUQg3LI/AAAAAAABIY0/r6L71Nz3QjsGkwSLro6VZNJ9r7bTohc7gCLcBGAs/s1600/exaple_filter.png" data-original-width="335" data-original-height="157" /></a></div>


## Solution
The solution I would like to show you is based on the post by [SQLJason](http://sqljason.com/author/sql_jason) where he talks about handling delimited rows [\[2\]][2].
I modernized and improved it a little to cover columns of arbitrary length and to avoid contaminating model with auxiliary tables.
The idea stays the same and can be broken down into the following steps.

1. Create temporary index table.
2. Apply `CROSSJOIN` operation and convert source table from *wide* to *long* format.
3. Define relationships.

In my previous post ["Creating index table in DAX"][3] I explained how to create index table for a given *N*.
Here, *N* should be selected as the max number of elements in the multivalued column.

```dax
MaxLength =
VAR Separator = "; "
RETURN
    MAXX (
        'Work Items - Today',
        1 + LEN ( [Tags] )
            - LEN ( SUBSTITUTE ( [Tags], Separator, "" ) )
    )
```

Now we can use this *DAX* expression and create `Indexes` table.

```dax
Indexes =
VAR Separator = "; "
RETURN
    FILTER (
        SUMMARIZE (
            ADDCOLUMNS (
                CALENDAR (
                    DATE ( 2000, 1, 1 ),
                    DATE ( 2000
                        + MAXX (
                            'Work Items - Today',
                            1 + LEN ( [Tags] )
                                - LEN ( SUBSTITUTE ( [Tags], Separator, "" ) )
                        ), 1, 1 )
                ),
                "Index", YEAR ( [Date] ) - 2000
            ),
            [Index]
        ),
        [Index] > 0
    )
```

| Index |
| - |
| 1 |
| 2 |
| 3 |

We do not need to store this table in our model.
Instead, we can simply save it in the *DAX* variable and reuse later.

The final expression consists of the following operations:

1. Save separator in the variable.
2. Create index table.
3. Add `TagsCount` to the `Work Item - Today` table to keep track of the index range.
4. Apply `CROSSJOIN` with `Indexes` table.
5. Filter out indexes that are outside of the range.
6. Use `PATHITEM` to extract single value from the multivalued field by index and save it in `Tag` column.
7. Summarize to reduce set of columns in the output table.

```dax
Tags =
VAR Separator = "; "
VAR Indexes =
    FILTER (
        SUMMARIZE (
            ADDCOLUMNS (
                CALENDAR (
                    DATE ( 2000, 1, 1 ),
                    DATE ( 2000
                        + MAXX (
                            'Work Items - Today',
                            1 + LEN ( [Tags] )
                                - LEN ( SUBSTITUTE ( [Tags], Separator, "" ) )
                        ), 1, 1 )
                ),
                "Index", YEAR ( [Date] ) - 2000
            ),
            [Index]
        ),
        [Index] > 0
    )
RETURN
    SUMMARIZE (
        ADDCOLUMNS (
            FILTER (
                CROSSJOIN (
                    ADDCOLUMNS (
                        'Work Items - Today',
                        "TagsCount", 1
                            + ( LEN ( [Tags] ) - LEN ( SUBSTITUTE ( [Tags], Separator, "" ) ) )
                                / LEN ( Separator )
                    ),
                    Indexes
                ),
                [Index] <= [TagsCount]
            ),
            "Tag", PATHITEM ( SUBSTITUTE ( [Tags], Separator, "|" ), [Index] )
        ),
        [Work Item Id],
        [Tag]
    )
```

It will produce the following result.
This table captures relationship between work items and tags.

| Work Item Id | Tag |
| - | - |
| 1 | database |
| 2 | database |
| 3 | performance |
| 1 | milestone1 |
| 3 | database |

Now we need to define relationships and specify cross filtering direction.
First, task is easy because most likely Power BI will automatically detect the relationship like in the example below.

<div class="separator" style="clear: both; text-align: center;"><a href="https://2.bp.blogspot.com/-8VpiMUBsim8/WVlGM9X100I/AAAAAAABIZA/XrDWcnnp7hQbvmAAcNOcCRMDXUgXK4vfwCLcBGAs/s1600/relationships.png" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="https://2.bp.blogspot.com/-8VpiMUBsim8/WVlGM9X100I/AAAAAAABIZA/XrDWcnnp7hQbvmAAcNOcCRMDXUgXK4vfwCLcBGAs/s1600/relationships.png" data-original-width="562" data-original-height="318" /></a></div>


The automatic relationship is a standard *one-to-many* relationship, which means that it will allow us to filter `Tags` based on `Work Item - Taday` selection.
That is exactly opposite of what we need.

Double-click on the relationship to open the advanced editor and under *"Cross filter direction"* select *"Both"*.

<div class="separator" style="clear: both; text-align: center;"><a href="https://2.bp.blogspot.com/-6udSv6BuSAg/WVlGNB9XYyI/AAAAAAABIZE/JKnf1u2S-akBCBjVr2XBcB2QRcoShJjLwCPcBGAYYCw/s1600/relationships_advanced.png" imageanchor="1" style="margin-left: 1em; margin-right: 1em;"><img border="0" src="https://2.bp.blogspot.com/-6udSv6BuSAg/WVlGNB9XYyI/AAAAAAABIZE/JKnf1u2S-akBCBjVr2XBcB2QRcoShJjLwCPcBGAYYCw/s640/relationships_advanced.png" width="640" height="547" data-original-width="716" data-original-height="612" /></a></div>

Finally, create a new slicer with `Tag` field from the newly created `Tags` table to get the best filtering experience! You can also try out amazing [Smart Filter][4] custom visual, which fits perfectly for this scenario.


## References:
1. [Team Services & TFS - Available data tables in the Power BI Data Connector for Team Services][1]
2. [Split a Delimited Row into Multiple Rows using DAX Queries][2]
3. [It is not overengineering - Creating index table in DAX][3]
4. [Custom visuals for Power BI - Smart Filter by OKViz][4]
5. [SQLBI - Best Practices Using SUMMARIZE and ADDCOLUMNS][5]
6. [Power BI Documentation - Calculated tables in Power BI Desktop][6]

[1]: https://www.visualstudio.com/en-us/docs/report/powerbi/data-connector-available-data
[2]: http://sqljason.com/2013/06/split-delimited-row-into-multiple-rows.html
[3]: http://www.itisnotoverengineering.com/2017/07/creating-index-table-in-dax.html
[4]: https://store.office.com/en-us/app.aspx?assetid=WA104380859&sourcecorrid=7807eed6-5653-43d3-abb5-8c38f68aa677&searchapppos=18&ui=en-US&rs=en-US&ad=US&appredirect=false
[5]: http://www.sqlbi.com/articles/best-practices-using-summarize-and-addcolumns/
[6]: https://powerbi.microsoft.com/en-us/documentation/powerbi-desktop-calculated-tables/