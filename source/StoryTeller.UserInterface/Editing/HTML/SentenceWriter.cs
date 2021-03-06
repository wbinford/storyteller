using System;
using System.Collections.Generic;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;

namespace StoryTeller.UserInterface.Editing.HTML
{
    public class SentenceWriter : ISentenceVisitor
    {
        private readonly ICellBuilderLibrary _builders;
        private readonly GrammarTag _grammar;

        public SentenceWriter(GrammarTag grammar, ICellBuilderLibrary builders)
        {
            _grammar = grammar;
            _builders = builders;
        }


        public void Label(Label label)
        {
            _grammar.Add("span").Text(label.Text);
        }

        public void TextInput(TextInput input)
        {
            _builders.AddCell(_grammar, input.Cell);
        }

        public void Write()
        {
            _grammar.AddClass(GrammarConstants.SENTENCE);
            _grammar.Grammar.As<Sentence>().Parts.Each(x => x.AcceptVisitor(this));
        }
    }
}