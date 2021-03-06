using System;
using System.Collections.Generic;
using HtmlTags;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Engine.Sets;
using StoryTeller.Model;
using FubuCore;
using System.Linq;

namespace StoryTeller.Html
{



    public class StoryTellerTableTag : TableTag
    {
        private readonly IStep _step;
        private readonly Table _table;
        private TableRowTag _headerRow;

        public StoryTellerTableTag(Table table, IStep step)
        {
            _table = table;
            _step = step;
            AddClass("table");
            Attr("cellpadding", "0").Attr("cellspacing", "0");

            CaptionText(_table.Label);
            AddHeaderRow(x =>
            {
                _headerRow = x;
                _table.Cells.Each(cell => x.Header(cell.Header));
            });
        }

        private IEnumerable<IStep> rows()
        {
            return _table.GetLeaf(_step).AllSteps();
        }

        public void WritePreview(ITestContext context)
        {
            rows().Each(row => { writePreviewRow(row, context); });
        }

        private void writePreviewRow(IStep step, ITestContext context)
        {
            AddBodyRow(row =>
            {
                _table.Cells.Each(cell =>
                {
                    var tag = new CellTag(cell, step);
                    tag.WritePreview(context);

                    row.Cell().Child(tag);
                });
            });
        }

        public void WriteResults(ITestContext context)
        {
            // This code Dru.  If there's an exception in the results, call
            // back to the writeExceptionText(string) method to write in 
            // the exception
            context.ResultsFor(_step).ForExceptionText(writeExceptionText);

            rows().Each(row => writeResultsRow(row, context));
        }

        private void writeResultsRow(IStep step, ITestContext context)
        {
            AddBodyRow(row =>
            {
                _table.Cells.Each(cell =>
                {
                    StepResults results = context.ResultsFor(step);
                    var tag = new CellTag(cell, _step);
                    tag.WriteResults(results, context);

                    // Ditto this line of code
                    results.ForExceptionText(writeExceptionText);

                    row.Cell().Child(tag);
                });
            });
        }

        public void WriteSetVerificationResults(SetVerification verification, ITestContext context)
        {
            if (verification.Ordered)
            {
                _headerRow.InsertFirst(new HtmlTag("th").Text("Order"));
            }

            var results = context.ResultsFor(_step);
            results.ForExceptionText(writeExceptionText);

            var rows = results.GetResult<IList<SetRow>>(_table.LeafName) ?? new List<SetRow>();
            // TODO -- order this the right way

            rows.Each(x =>
            {
                writeVerificationResultRow(x, context, verification.Ordered);
            });
        }

        private void writeVerificationResultRow(SetRow setRow, ITestContext context, bool ordered)
        {
            AddBodyRow(row =>
            {
                if (ordered)
                {
                    row.Cell(setRow.ActualOrder == 0 ? string.Empty : setRow.ActualOrder.ToString());
                }

                _table.Cells.Each(cell =>
                {
                    var display = context.GetDisplay(setRow.Values[cell.Key]);
                    row.Cell(display);
                });

                switch (setRow.Result)
                {
                    case SetMatch.Match:
                        row.AddClass(HtmlClasses.PASS);

                        break;

                    case SetMatch.Extra:
                        row.AddClass(HtmlClasses.FAIL);
                        row.FirstChild().Prepend("Extra:  ");
                        break;

                    case SetMatch.Missing:
                        row.AddClass(HtmlClasses.FAIL);
                        row.FirstChild().Prepend("Missing:  ");
                        break;

                    case SetMatch.OutOfOrder:
                        row.AddClass(HtmlClasses.FAIL);
                        
                        string text = "Expected {0} but was {1}".ToFormat(setRow.ExpectedOrder, setRow.ActualOrder);
                        row.FirstChild().Text(text);
                        break;
                }
            });

            if (setRow.Step != null)
            {
                context.ResultsFor(setRow.Step).ForExceptionText(writeExceptionText);
            }
        }

        private void writeExceptionText(string text)
        {
            AddBodyRow(row =>
            {
                var exceptionTag = new ExceptionTag(text);
                int colSpan = _headerRow.Children.Count;
                row.Cell()
                    .Attr("colspan", colSpan).AddClass(HtmlClasses.EXCEPTION)
                    .Child(exceptionTag);
            });
        }
    }
}