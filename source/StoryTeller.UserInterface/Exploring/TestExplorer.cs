using System;
using System.Collections.Generic;
using System.Linq;
using StoryTeller.Domain;
using StoryTeller.UserInterface.Controls;
using StoryTeller.UserInterface.Running;
using StoryTeller.Workspace;

namespace StoryTeller.UserInterface.Exploring
{
    public class TestExplorer : ITestExplorer
                                , IListener<TestAddedMessage>
                                , ITestFilterObserver
                                , IListener<ClearResultsMessage>
                                , IListener<TestRunEvent>
                                , IListener<DeleteTestMessage>
                                , IListener<TestRenamed>
                                , IListener<SuiteAddedMessage>
    {
        private readonly IEventAggregator _events;
        private readonly TestFilter _filter = new TestFilter();
        private readonly SuiteNavigator _navigator;
        private readonly SuiteNodeCache _suiteNodes = new SuiteNodeCache();
        private readonly TestNodeCache _testNodes = new TestNodeCache();

        private readonly IExplorerView _view;
        private Hierarchy _hierarchy;

        public TestExplorer(IExplorerView view, IEventAggregator events, ITestFilterBar filter)
        {
            _view = view;
            _events = events;

            if (filter != null) filter.Observer = this;
            _navigator = new SuiteNavigator
            {
                TestFilter = _filter.Matches,
                SuiteFilter =
                    suite => suite.GetAllTests().FirstOrDefault(_filter.Matches) != null || _filter.ShowEmptySuites()
            };
        }

        #region IListener<ClearResultsMessage> Members

        public void Handle(ClearResultsMessage message)
        {
            _hierarchy.ClearResults();
            eachNode(x => x.Icon = Icon.Unknown);

            ResetFilter();
        }

        #endregion

        #region IListener<DeleteTestMessage> Members

        public void Handle(DeleteTestMessage message)
        {
            TreeNode testNode = _testNodes[message.Test];
            _suiteNodes[message.Parent].Remove(testNode);
        }

        #endregion

        #region IListener<SuiteAddedMessage> Members

        public void Handle(SuiteAddedMessage message)
        {
            TreeNode newNode = _suiteNodes[message.NewSuite];
            _suiteNodes[message.NewSuite.Parent].Add(newNode);
        }

        #endregion

        #region IListener<TestAddedMessage> Members

        public void Handle(TestAddedMessage message)
        {
            TreeNode testNode = _testNodes[message.Test];
            TreeNode suiteNode = _suiteNodes[message.Test.Parent];

            suiteNode.Add(testNode);
        }

        #endregion

        #region IListener<TestRenamed> Members

        public void Handle(TestRenamed message)
        {
            _testNodes[message.Test].ResetText();
        }

        #endregion

        #region IListener<TestRunEvent> Members

        public void Handle(TestRunEvent message)
        {
            Icon icon = Icon.Unknown;
            Test test = message.Test;

            switch (message.State)
            {
                case TestState.Queued:
                    icon = Icon.Pending;
                    break;

                case TestState.Executing:
                    icon = Icon.Pending;
                    if (test.HasResult())
                    {
                        icon = test.WasSuccessful() ? Icon.RunningSuccess : Icon.RunningFailure;
                    }
                    break;

                case TestState.NotQueued:
                    if (message.Test.HasResult())
                    {
                        icon = message.Test.WasSuccessful() ? Icon.Success : Icon.Failed;
                    }
                    break;
            }

            updateIcon(message.Test, icon);
        }

        #endregion

        #region ITestExplorer Members

        public void Handle(Hierarchy message)
        {
            _hierarchy = message;

            _testNodes.ClearAll();
            _suiteNodes.ClearAll();

            ResetFilter();
        }

        public void Start()
        {
            // no-op
        }

        public Icon IconFor(Test test)
        {
            return _testNodes[test].Icon;
        }

        public IEnumerable<Test> TestsMatchingFilter(Suite suite)
        {
            return suite.GetAllTests().Where(_filter.Matches);
        }

        public Hierarchy CurrentHierarchy { get { return _hierarchy; } }

        public void ResetFilter()
        {
            clear();

            TreeNode topNode = NodeFor(_hierarchy);

            _navigator.Visit(_hierarchy, new TreeBuilder(topNode, _testNodes, _suiteNodes));

            _view.ApplyProjectNode(topNode);
        }

        #endregion

        #region ITestFilterObserver Members

        public void ResultStatusChanged(ResultStatus status)
        {
            _filter.ResultStatus = status;
            ResetFilter();
        }

        public void LifecycleChanged(Lifecycle lifecycle)
        {
            _filter.Lifecycle = lifecycle;
            ResetFilter();
        }

        public void RunAll()
        {
            IEnumerable<Test> tests = _hierarchy.GetAllTests().Where(_filter.Matches);
            var message = new ExecuteTestMessage(tests);
            _events.SendMessage(message);
        }

        #endregion

        private void updateIcon(Test test, Icon icon)
        {
            NodeFor(test).Icon = icon;
            var message = new TestIconChanged(test, icon);
            _events.SendMessage(message);
        }

        public TreeNode NodeFor(Test test)
        {
            return _testNodes[test];
        }

        public TreeNode NodeFor(Suite suite)
        {
            return _suiteNodes[suite];
        }

        private void clear()
        {
            eachNode(node => node.ClearParent());
        }

        private void eachNode(Action<TreeNode> action)
        {
            _suiteNodes.Each(action);
            _testNodes.Each(action);
        }
    }
}