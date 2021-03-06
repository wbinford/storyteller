using System;
using FubuCore;
using FubuCore.Util;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    public class AutoPerformingTestContext : ITestContext
    {
        #region ITestContext Members

        public object CurrentObject { get; set; }

        public bool Matches(object expected, object actual)
        {
            return new EquivalenceChecker().IsEqual(expected, actual);
        }

        public void Store<T>(T data)
        {
        }

        public T Retrieve<T>()
        {
            throw new NotImplementedException();
        }

        public object Retrieve(Type type)
        {
            throw new NotImplementedException();
        }

        public virtual void IncrementRights()
        {
        }

        public virtual void IncrementWrongs()
        {
        }

        public virtual void IncrementExceptions()
        {
        }

        public virtual void IncrementSyntaxErrors()
        {
        }

        public void ExecuteWithFixture<T>(StepLeaf leaf, ITestPart exceptionTarget) where T : IFixture
        {
        }

        public void RunStep(IGrammar grammar, IStep step)
        {
        }

        public void PerformAction(IStep step, GrammarAction action)
        {
            try
            {
                action(step, this);
            }
            catch (Exception ex)
            {
                IncrementExceptions();
                ResultsFor(step).CaptureException(ex.ToString());
            }
        }

        public void VisitFixtures(IFixtureVisitor visitor)
        {
        }

        private readonly Cache<ITestPart, StepResults> _results = new Cache<ITestPart, StepResults>(step => new StepResults());
        public StepResults ResultsFor(ITestPart part)
        {
            return _results[part];
        }

        public Stringifier Stringifier
        {
            get { return new Stringifier(); }
        }

        public string GetDisplay(object value)
        {
            return new Stringifier().GetString(value);
        }

        public Counts Counts
        {
            get { return new Counts(); }
        }

        public string TraceText
        {
            get { throw new NotImplementedException(); }
        }

        public void Trace(string text)
        {
            throw new NotImplementedException();
        }

        public ObjectFinder Finder { get; set; }

        #endregion
    }
}