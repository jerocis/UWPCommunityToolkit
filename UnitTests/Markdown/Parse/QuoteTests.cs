﻿// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using Microsoft.Toolkit.Parsers.Markdown.Blocks.List;
using Microsoft.Toolkit.Parsers.Markdown.Enums;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class QuoteTests : ParseTestBase
    {
        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_SingleLine()
        {
            AssertEqual(">Quoted text",
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted text" })));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_MultiLine_1()
        {
            AssertEqual(CollapseWhitespace(@"
                > Single line

                >Quote  
                with line break


                 > Spaces
                  > and line continuation

                normal text"),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Single line" }),
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quote\r\nwith line break" }),
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Spaces and line continuation" })),
                new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "normal text" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_MultiLine_Simple()
        {
            AssertEqual(CollapseWhitespace(@"
                before
                >Quoted
                >Quoted, line 2
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted Quoted, line 2 after" })));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_WithHeader()
        {
            AssertEqual(CollapseWhitespace(@"
                >#header
                > #header
                >  #not a header
                >
                >    text
                >
                >     code"),
                new QuoteBlock().AddChildren(
                    new HeaderBlock { HeaderLevel = 1 }.AddChildren(
                        new TextRunInline { Text = "header" }),
                    new HeaderBlock { HeaderLevel = 1 }.AddChildren(
                        new TextRunInline { Text = "header" }),
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = " #not a header" }),
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "   text" }),
                    new CodeBlock { Text = "code" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_Nested()
        {
            AssertEqual(CollapseWhitespace(@"
                >Quoted
                >>Nested quote
                >Still nested
                
                >Not nested"),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Quoted" }),
                    new QuoteBlock().AddChildren(
                        new ParagraphBlock().AddChildren(
                            new TextRunInline { Text = "Nested quote Still nested" })),
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "Not nested" })));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_WithTable()
        {
            AssertEqual(CollapseWhitespace(@"
                This doesn't work:

                > a|b
                -|-
                1|2

                But this does:

                > a|b
                > -|-
                > 1|2"),

                new ParagraphBlock().AddChildren(new TextRunInline { Text = "This doesn't work:" }),
                new TableBlock
                {
                    ColumnDefinitions = new List<TableBlock.TableColumnDefinition>
                    {
                        new TableBlock.TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        new TableBlock.TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                    }
                }.AddChildren(
                    new TableBlock.TableRow().AddChildren(
                        new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "> a" }),
                        new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "b" })),
                    new TableBlock.TableRow().AddChildren(
                        new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "1" }),
                        new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "2" }))),

                new ParagraphBlock().AddChildren(new TextRunInline { Text = "But this does:" }),
                new QuoteBlock().AddChildren(
                    new TableBlock
                    {
                        ColumnDefinitions = new List<TableBlock.TableColumnDefinition>
                        {
                            new TableBlock.TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                            new TableBlock.TableColumnDefinition { Alignment = ColumnAlignment.Unspecified },
                        }
                    }.AddChildren(
                        new TableBlock.TableRow().AddChildren(
                            new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "a" }),
                            new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "b" })),
                        new TableBlock.TableRow().AddChildren(
                            new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "1" }),
                            new TableBlock.TableCell().AddChildren(new TextRunInline { Text = "2" })))));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_Invalid_Table()
        {
            AssertEqual(CollapseWhitespace(@"
                >a|b"),
                new QuoteBlock().AddChildren(
                    new ParagraphBlock().AddChildren(
                        new TextRunInline { Text = "a|b" })));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_WithCode()
        {
            AssertEqual(CollapseWhitespace(@"
                >     code, line 1
                >
                
                >     code, line 4"),

                new QuoteBlock().AddChildren(
                    new CodeBlock { Text = "code, line 1\r\n\r\n\r\ncode, line 4" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Quote_WithList()
        {
            AssertEqual(CollapseWhitespace(@"
                >     code, line 1
                >
                
                >     code, line 4"),

                new QuoteBlock().AddChildren(
                    new CodeBlock { Text = "code, line 1\r\n\r\n\r\ncode, line 4" }));

            AssertEqual(CollapseWhitespace(@"
                > + List item 1
                > + List item 2
                > + List item 3"),
            new QuoteBlock().AddChildren(
                new ListBlock { Style = ListStyle.Bulleted }.AddChildren(
                    new ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 1" })),
                    new ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 2" })),
                    new ListItemBlock().AddChildren(new ParagraphBlock().AddChildren(new TextRunInline { Text = "List item 3" })))));
        }
    }
}