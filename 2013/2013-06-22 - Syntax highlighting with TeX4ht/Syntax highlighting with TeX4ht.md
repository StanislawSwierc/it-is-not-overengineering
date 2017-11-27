# Syntax highlighting with TeX4ht

When I was evaluating different options for a blog development I spent some time on the TeX4th.
Although, I haven't chosen this technology I found it very interesting and I would like to share its goodness.

One of the important aspect of all the blogs about programming is how they display source code snippets.
As always there is no one answer how to do it.
Some people just wrap their code in the `<pre>` and `code` tags.
Others care more about the appearance of their posts and highlight the syntax accordingly to the programming language they use.
I wanted the code I share to look good.
That's why I draw my attention the TeX4ht.

## Listings package
In LaTeX there is a *listings* package which can be used to format source code.
It offers environment similar to verbatim but with many parameters to customize the output.

This is an example of how one can add a code block to a LaTeX article.

    \documentclass[11pt]{article}
    \usepackage[utf8]{inputenc}
    
    \usepackage{listings}
    \lstset{
    	language=[Sharp]C,
    	basicstyle=\ttfamily\small,
    	identifierstyle=\sffamily,
    	keywordstyle=\sffamily\bfseries,
    	commentstyle=\rmfamily,
    	stringstyle=\rmfamily\itshape,
        numberstyle=\scriptsize,
    	showstringspaces=false,
    	tabsize=2,
    	numbers=left,
    }
    
    \begin{document}
    \begin{lstlisting}[float, caption={Sample code}]
    class Program
    {
        static void Main(string[] args)
        {
            // This is comment
            var text = "This is text";
            var number = 12345;
            Console.WriteLine(text + number.ToString());
        }
    }
    \end{lstlisting}
    \end{document}

Once compiled to PDF it looks very nice.
Even though everything is black and white every part of the code has its unique style.

<div class="separator" style="clear: both; text-align: center;">
    <a href="http://1.bp.blogspot.com/-Kj8NMXfkGW4/UcYPuj9DVjI/AAAAAAAAAVs/KKOp0hIVt1s/s1600/2013.06.22-1.png" imageanchor="1" style="margin-left: 1em; margin-right: 1em;">
        <img border="0" src="http://1.bp.blogspot.com/-Kj8NMXfkGW4/UcYPuj9DVjI/AAAAAAAAAVs/KKOp0hIVt1s/s1600/2013.06.22-1.png" />
    </a>
</div>

## TeX4ht
The LaTeX document presented in the previous listing can be compiled to the HTML using TeX4ht with the following command

    >>htlatex Sample.tex

Unfortunately the output produced by default is not as pretty as it was in the PDF.
The fonts have their style but the code is no longer aligned.
There is no space between numbers and text.
Comments are not aligned with the rest of the code.


<div class="float">
    <div class="caption"><span class="id">Listing&nbsp;1: </span><span class="content">Sample code</span></div>
    <div class="lstlistingfirst" id="listing-1"><span class="label"><a id="x1-4r1"></a><span class="cmr-8">1</span></span><span class="cmssbx-10">class</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">Program</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-5r2"></a><span class="cmr-8">2</span></span><span class="cmtt-10">{</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-6r3"></a><span class="cmr-8">3</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmssbx-10">static</span><span class="cmtt-10">&nbsp;</span><span class="cmssbx-10">void</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">Main</span><span class="cmtt-10">(</span><span class="cmssbx-10">string</span><span class="cmtt-10">[]</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">args</span><span class="cmtt-10">)</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-7r4"></a><span class="cmr-8">4</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">{</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-8r5"></a><span class="cmr-8">5</span></span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">//</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">This</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">is</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">comment</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-9r6"></a><span class="cmr-8">6</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">var</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">text</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">=</span><span class="cmtt-10">&nbsp;</span><span class="cmti-10">�</span><span class="cmti-10">This</span><span class="cmti-10">&nbsp;</span><span class="cmti-10">is</span><span class="cmti-10">&nbsp;</span><span class="cmti-10">text</span><span class="cmti-10">�</span><span class="cmtt-10">;</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-10r7"></a><span class="cmr-8">7</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">var</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">number</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">=</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">12345;</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-11r8"></a><span class="cmr-8">8</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">Console</span><span class="cmtt-10">.</span><span class="cmss-10">WriteLine</span><span class="cmtt-10">(</span><span class="cmss-10">text</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">+</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">number</span><span class="cmtt-10">.</span><span class="cmss-10">ToString</span><span class="cmtt-10">());</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-12r9"></a><span class="cmr-8">9</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">}</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-13r10"></a><span class="cmr-8">10</span></span><span class="cmtt-10">}</span>
    </div>
</div>

Listing package supports four different modes of alignment.
By default it uses a *fixed* mode where a character is a single unit of output and they are aligned in columns.
This mechanism does not port to HTML.
in order to achieve the similar effect one should use *monospace* fonts.
However this has its own problems because in LaTeX this corresponds to a typewriter (`/ttfamily`) font which cannot be styled.

As I mentioned it in the previous post the best solution I found was at the StaskExchange

* [LaTeX to HTML preserving code coloring from listings](http://tex.stackexchange.com/questions/64054/latex-to-html-preserving-code-coloring-from-listings)

Instead of trying to force TeX4ht to produce different styles for the listing generated with *listings* package it is easier to override the style used in the output.
For this to work all the styles used in the listings should be unique (eg. basicstyle, identifierstyle, ...).
If you look at the `lstset` definition of the first listing, you will see that it satisfies this requirement.

The next step was to define the CSS configuration.
In order to do it I used Internet Explorer Developer Tools to select elements and capture their classes.
Then I was able to create a [private configuration File](https://www.tug.org/applications/tex4ht/mn-commands.html#QQ1-9-41) for the TeX4ht.

    \Preamble{html} 
    \begin{document} 
      % basicstyle
      \Css{div.lstlisting .cmtt-10 {font-family:monospace; color:DimGray}} 
      % identifierstyle
      \Css{div.lstlisting .cmss-10 {font-family:monospace; color:Black}} 
      % keywordstyle
      \Css{div.lstlisting .cmssbx-10 {font-family:monospace; color:Blue}} 
      % commentstyle
      \Css{div.lstlisting .cmr-10 {font-family:monospace; color:Green}} 
      % stringstyle
      \Css{div.lstlisting .cmti-10 {font-family:monospace; color:DarkRed}} 
      % numberstyle
      \Css{div.lstlisting .cmr-8 {display:inline-block; width:20px}} 
    \EndPreamble 

Please notice custom style for the `div.lstlisting` block.
This hasn't been mentioned on the StackExchange but it is required for the line numbering to work. 

In order to include the configuration file I used slightly modified command line.

    >>htlatex Sample.tex Sample.cfg

Finally it all worked.
The listing produced has line numbering.
All the elements of the syntax are highlighted and everything is aligned exactly the same way as in the source code.

<div class="float">
    <div class="caption"><span class="id">Listing&nbsp;1: </span><span class="content">Sample code</span></div>
    <div class="lstlisting" id="listing-1"><span class="label"><a id="x1-4r1"></a><span class="cmr-8">1</span></span><span class="cmssbx-10">class</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">Program</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-5r2"></a><span class="cmr-8">2</span></span><span class="cmtt-10">{</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-6r3"></a><span class="cmr-8">3</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmssbx-10">static</span><span class="cmtt-10">&nbsp;</span><span class="cmssbx-10">void</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">Main</span><span class="cmtt-10">(</span><span class="cmssbx-10">string</span><span class="cmtt-10">[]</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">args</span><span class="cmtt-10">)</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-7r4"></a><span class="cmr-8">4</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">{</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-8r5"></a><span class="cmr-8">5</span></span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">//</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">This</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">is</span><span class="cmr-10">&nbsp;</span><span class="cmr-10">comment</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-9r6"></a><span class="cmr-8">6</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">var</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">text</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">=</span><span class="cmtt-10">&nbsp;</span><span class="cmti-10">�</span><span class="cmti-10">This</span><span class="cmti-10">&nbsp;</span><span class="cmti-10">is</span><span class="cmti-10">&nbsp;</span><span class="cmti-10">text</span><span class="cmti-10">�</span><span class="cmtt-10">;</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-10r7"></a><span class="cmr-8">7</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">var</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">number</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">=</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">12345;</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-11r8"></a><span class="cmr-8">8</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">Console</span><span class="cmtt-10">.</span><span class="cmss-10">WriteLine</span><span class="cmtt-10">(</span><span class="cmss-10">text</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">+</span><span class="cmtt-10">&nbsp;</span><span class="cmss-10">number</span><span class="cmtt-10">.</span><span class="cmss-10">ToString</span><span class="cmtt-10">());</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-12r9"></a><span class="cmr-8">9</span></span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">&nbsp;</span><span class="cmtt-10">}</span><span class="cmtt-10">&nbsp;</span><br>
        <span class="label"><a id="x1-13r10"></a><span class="cmr-8">10</span></span><span class="cmtt-10">}</span>
    </div>
</div>

---
This post with all the resources is available on GitHub:

https://github.com/StanislawSwierc/it-is-not-overengineering/tree/master



<style>
div.float { display: block; padding-left: 10px; padding-right: 10px; background-color: #f5f5f5 }
/* start css.sty */
.cmr-10{font-size:90%;}
.cmr-10x-x-109{}
.cmtt-10{font-size:90%;font-family: monospace;}
.cmr-8{font-size:72%;}
.cmss-10{font-size:90%;  font-family: sans-serif;}
.cmss-10{  font-family: sans-serif;}
.cmssbx-10{font-size:90%; font-family: sans-serif; font-weight: bold;}
.cmssbx-10{ font-family: sans-serif; font-weight: bold;}
.cmti-10{font-size:90%; font-style: italic;}
p.noindent { text-indent: 0em }
td p.noindent { text-indent: 0em; margin-top:0em; }
p.nopar { text-indent: 0em; }
p.indent{ text-indent: 1.5em }
@media print {div.crosslinks {visibility:hidden;}}
a img { border-top: 0; border-left: 0; border-right: 0; }
center { margin-top:1em; margin-bottom:1em; }
td center { margin-top:0em; margin-bottom:0em; }
.Canvas { position:relative; }
img.math{vertical-align:middle;}
li p.indent { text-indent: 0em }
li p:first-child{ margin-top:0em; }
li p:last-child, li div:last-child { margin-bottom:0.5em; }
li p~ul:last-child, li p~ol:last-child{ margin-bottom:0.5em; }
.enumerate1 {list-style-type:decimal;}
.enumerate2 {list-style-type:lower-alpha;}
.enumerate3 {list-style-type:lower-roman;}
.enumerate4 {list-style-type:upper-alpha;}
div.newtheorem { margin-bottom: 2em; margin-top: 2em;}
.obeylines-h,.obeylines-v {white-space: nowrap; }
div.obeylines-v p { margin-top:0; margin-bottom:0; }
.overline{ text-decoration:overline; }
.overline img{ border-top: 1px solid black; }
td.displaylines {text-align:center; white-space:nowrap;}
.centerline {text-align:center;}
.rightline {text-align:right;}
div.verbatim {font-family: monospace; white-space: nowrap; text-align:left; clear:both; }
.fbox {padding-left:3.0pt; padding-right:3.0pt; text-indent:0pt; border:solid black 0.4pt; }
div.fbox {display:table}
div.center div.fbox {text-align:center; clear:both; padding-left:3.0pt; padding-right:3.0pt; text-indent:0pt; border:solid black 0.4pt; }
div.minipage{width:100%;}
div.center, div.center div.center {text-align: center; margin-left:1em; margin-right:1em;}
div.center div {text-align: left;}
div.flushright, div.flushright div.flushright {text-align: right;}
div.flushright div {text-align: left;}
div.flushleft {text-align: left;}
.underline{ text-decoration:underline; }
.underline img{ border-bottom: 1px solid black; margin-bottom:1pt; }
.framebox-c, .framebox-l, .framebox-r { padding-left:3.0pt; padding-right:3.0pt; text-indent:0pt; border:solid black 0.4pt; }
.framebox-c {text-align:center;}
.framebox-l {text-align:left;}
.framebox-r {text-align:right;}
span.thank-mark{ vertical-align: super }
span.footnote-mark sup.textsuperscript, span.footnote-mark a sup.textsuperscript{ font-size:80%; }
div.tabular, div.center div.tabular {text-align: center; margin-top:0.5em; margin-bottom:0.5em; }
table.tabular td p{margin-top:0em;}
table.tabular {margin-left: auto; margin-right: auto;}
td p:first-child{ margin-top:0em; }
td p:last-child{ margin-bottom:0em; }
div.td00{ margin-left:0pt; margin-right:0pt; }
div.td01{ margin-left:0pt; margin-right:5pt; }
div.td10{ margin-left:5pt; margin-right:0pt; }
div.td11{ margin-left:5pt; margin-right:5pt; }
table[rules] {border-left:solid black 0.4pt; border-right:solid black 0.4pt; }
td.td00{ padding-left:0pt; padding-right:0pt; }
td.td01{ padding-left:0pt; padding-right:5pt; }
td.td10{ padding-left:5pt; padding-right:0pt; }
td.td11{ padding-left:5pt; padding-right:5pt; }
table[rules] {border-left:solid black 0.4pt; border-right:solid black 0.4pt; }
.hline hr, .cline hr{ height : 1px; margin:0px; }
.tabbing-right {text-align:right;}
span.TEX {letter-spacing: -0.125em; }
span.TEX span.E{ position:relative;top:0.5ex;left:-0.0417em;}
a span.TEX span.E {text-decoration: none; }
span.LATEX span.A{ position:relative; top:-0.5ex; left:-0.4em; font-size:85%;}
span.LATEX span.TEX{ position:relative; left: -0.4em; }
div.float, div.figure {margin-left: auto; margin-right: auto;}
div.float img {text-align:center;}
div.figure img {text-align:center;}
.marginpar {width:20%; float:right; text-align:left; margin-left:auto; margin-top:0.5em; font-size:85%; text-decoration:underline;}
.marginpar p{margin-top:0.4em; margin-bottom:0.4em;}
table.equation {width:100%;}
.equation td{text-align:center; }
td.equation { margin-top:1em; margin-bottom:1em; } 
td.equation-label { width:5%; text-align:center; }
td.eqnarray4 { width:5%; white-space: normal; }
td.eqnarray2 { width:5%; }
table.eqnarray-star, table.eqnarray {width:100%;}
div.eqnarray{text-align:center;}
div.array {text-align:center;}
div.pmatrix {text-align:center;}
table.pmatrix {width:100%;}
span.pmatrix img{vertical-align:middle;}
div.pmatrix {text-align:center;}
table.pmatrix {width:100%;}
span.bar-css {text-decoration:overline;}
img.cdots{vertical-align:middle;}
.partToc a, .partToc, .likepartToc a, .likepartToc {line-height: 200%; font-weight:bold; font-size:110%;}
.index-item, .index-subitem, .index-subsubitem {display:block}
div.caption {text-indent:-2em; margin-left:3em; margin-right:1em; text-align:left;}
div.caption span.id{font-weight: bold; white-space: nowrap; }
h1.partHead{text-align: center}
p.bibitem { text-indent: -2em; margin-left: 2em; margin-top:0.6em; margin-bottom:0.6em; }
p.bibitem-p { text-indent: 0em; margin-left: 2em; margin-top:0.6em; margin-bottom:0.6em; }
.paragraphHead, .likeparagraphHead { margin-top:2em; font-weight: bold;}
.subparagraphHead, .likesubparagraphHead { font-weight: bold;}
.quote {margin-bottom:0.25em; margin-top:0.25em; margin-left:1em; margin-right:1em; text-align:justify;}
.verse{white-space:nowrap; margin-left:2em}
div.maketitle {text-align:center;}
h2.titleHead{text-align:center;}
div.maketitle{ margin-bottom: 2em; }
div.author, div.date {text-align:center;}
div.thanks{text-align:left; margin-left:10%; font-size:85%; font-style:italic; }
div.author{white-space: nowrap;}
.quotation {margin-bottom:0.25em; margin-top:0.25em; margin-left:1em; }
.abstract p {margin-left:5%; margin-right:5%;}
div.abstract {width:100%;}
.lstlisting .label{margin-right:0.5em; }
div.lstlisting{font-family: monospace; white-space: nowrap; margin-top:0.5em; margin-bottom:0.5em; }
div.lstinputlisting{ font-family: monospace; white-space: nowrap; }
.lstinputlisting .label{margin-right:0.5em;}
div.lstlisting .cmtt-10 {font-family:monospace; color:DimGray}
div.lstlisting .cmss-10 {font-family:monospace; color:Black}
div.lstlisting .cmssbx-10 {font-family:monospace; color:Blue}
div.lstlisting .cmr-10 {font-family:monospace; color:Green}
div.lstlisting .cmti-10 {font-family:monospace; color:DarkRed}
div.lstlisting .cmr-8 {display:inline-block; width:20px}
/* end css.sty */
</style>
