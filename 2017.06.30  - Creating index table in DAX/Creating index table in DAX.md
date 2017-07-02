# Creating index table in DAX

Tags: `powerbi`, `dax`

Sometimes, when you create advanced calculations in DAX, you need to create an auxiliary index table which contains increasing number like in the example below.

| Index |
| - |
| 1 |
| 2 |
| ... |

There are several ways one can approach this problem.
Marco Russo described some of them for the general case of creating static tables in his article [\[1\]][1].
Here I would like to present a different approach which works great for the more specific problem of index tables.

## Problem statement
Create a table with one column where rows are increasing from 1 to 5.

| Index |
| - |
| 1 |
| 2 |
| 3 |
| 4 |
| 5 |

## Solution
In Power BI we can create such table with *Power Query* or directly in *DAX* using [*calculated tables*][5]. In the first approach we would use [`List.Numbers`][2] function.
Second, is more challenging as there is no such function in *DAX*.
Luckily, there is `CALENDAR` function which can be used to generate a table!

```dax
IndexTable =
VAR Length = 5
VAR DummyDate = DATE ( 2000, 1, 1 )
RETURN
    CALENDAR ( DummyDate, DummyDate + Length )
```

| Date |
| - |
| 2000-01-01 00:00:00 |
| 2000-01-02 00:00:00 |
| 2000-01-03 00:00:00 |
| 2000-01-04 00:00:00 |
| 2000-01-05 00:00:00 |

Now we just need to clean it up by converting dates to numbers, adding new column, and removing the original column [\[4\]][4].

```dax
IndexTable =
VAR Length = 5
VAR DummyDate = DATE ( 2000, 1, 1 )
RETURN
    FILTER (
        SUMMARIZE (
            ADDCOLUMNS (
                CALENDAR ( DummyDate, DummyDate + Length ),
                "Index", INT ( [Date] ) - INT ( DummyDate )
            ),
            [Index]
        ),
        [Index] > 0
    )
```

## References:
1. [Create Static Tables in DAX Using the DATATABLE Function][1]
2. [Power Query M function reference - List.Numbers][2]
3. [DAX Function Reference - CALENDAR][3]
4. [Best Practices Using SUMMARIZE and ADDCOLUMNS][4]
5. [Calculated tables in Power BI Desktop][5]

[1]: https://www.sqlbi.com/articles/create-static-tables-in-dax-using-the-datatable-function
[2]: https://msdn.microsoft.com/en-us/library/mt260934.aspx
[3]: https://msdn.microsoft.com/en-us/library/dn802546.aspx
[4]: http://www.sqlbi.com/articles/best-practices-using-summarize-and-addcolumns
[5]: https://powerbi.microsoft.com/en-us/documentation/powerbi-desktop-calculated-tables/