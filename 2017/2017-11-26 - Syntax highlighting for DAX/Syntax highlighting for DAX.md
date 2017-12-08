# Syntax highlighting for DAX

I'm not sure if you noticed it, but one of my previous posts had code snippet in DAX and apart from being nicely formatted with [Dax Formatter][2] the syntax was highlighed as well. In order to create that snippet I added DAX support to the amazing library [highlight.js][3] I use to build my blog.

With this contribution you can format your DAX code as well.
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

The pull request to the main project is still pending so you might need to cherry-pick my commit and build it.

https://github.com/isagalaev/highlight.js/pull/1560

Apart from adding it to the main project, I've also contributed it to the fork which is used by Microsoft in their documentation pipeline. Using this fork is another option you have to enable this feature in your websites.

https://github.com/DuncanmaMSFT/highlight.js

Hopefully all pull request will be completed soon and everyone will be able to use it by default.

## References:
1. [It is not overengineering! - How to filter multivalued column in Power BI?][1]
2. [DAX Formatter][2]
3. [highlight.js][3]

[1]: http://www.itisnotoverengineering.com/2017/07/how-to-filter-multivalued-column-in.html
[2]: http://www.daxformatter.com/
[3]: https://highlightjs.org/