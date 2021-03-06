using System.IO;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Execution;
using StoryTeller.UserInterface;
using StoryTeller.UserInterface.Eventing;
using StoryTeller.UserInterface.Projects;
using StoryTeller.Workspace;
using StructureMap;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Execution
{
    [TestFixture]
    public class TestEngineIntegrationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            ProjectPersistor.DeleteHistoryFile();

            Bootstrapper.ForceRestart();
            var events = new SimpleEventAggregator();
            ObjectFactory.Inject<IEventAggregator>(events);

            var observer = ObjectFactory.GetInstance<UserInterfaceTestObserver>();

            project = DataMother.MathProject();

            project.GetBinaryFolder().ShouldEqual(Path.GetFullPath(@"..\..\..\StoryTeller.Samples\bin\debug"));

            hierarchy = project.LoadTests();

            events.RemoveAllListeners();
            listener = ObjectFactory.GetInstance<StubLibraryListener>();


            engine = ObjectFactory.GetInstance<TestEngine>();


            engine.Handle(new ProjectLoaded(project));

            engine.WaitForProcessing();
        }

        #endregion

        private Project project;
        private Hierarchy hierarchy;
        private TestEngine engine;
        private StubLibraryListener listener;


        [Test]
        public void broadcasts_a_FixtureLibraryLoaded_message_in_recycle()
        {
            listener.TheMessage.ShouldNotBeNull();
            listener.TheMessage.Library.ShouldBeTheSameAs(engine.Library);
        }

        [Test]
        public void reloading_a_project_does_not_blow_up()
        {
            engine.HandleMessage(new ForceEnvironmentRecycle());

            Test test = hierarchy.FindTest("Good Add at Top");
            engine.RunTest(test);

            test.LastResult.ExecutionTime.ShouldBeGreaterThan(0);

            test.LastResult.Counts.ShouldEqual(1, 0, 0, 0);
        }

        [Test]
        public void run_exception_test()
        {
            Test test = hierarchy.FindTest("Mixed/SubMixed/SubMixedThatThrows");
            engine.RunTest(test);

            test.LastResult.ExecutionTime.ShouldBeGreaterThan(0);

            test.LastResult.Counts.ShouldEqual(1, 0, 1, 0);
        }

        [Test]
        public void run_failed_test()
        {
            Test test = hierarchy.FindTest("Bad Add at Top");
            engine.RunTest(test);

            test.LastResult.ExecutionTime.ShouldBeGreaterThan(0);

            test.LastResult.Counts.ShouldEqual(1, 3, 0, 0);
        }

        [Test]
        public void run_successful_test()
        {
            Test test = hierarchy.FindTest("Good Add at Top");
            engine.RunTest(test);

            test.LastResult.ExecutionTime.ShouldBeGreaterThan(0);

            test.LastResult.Counts.ShouldEqual(1, 0, 0, 0);
        }

        [Test]
        public void write_out_the_description_to_a_grammar_structure()
        {
            engine.Library.Find(new TPath("Math/MultiplyBy")).Description.ShouldEqual(
                "This grammar multiplies two numbers together");
        }
    }


    public class StubLibraryListener : IListener<BinaryRecycleFinished>
    {
        public BinaryRecycleFinished TheMessage;

        #region IListener<BinaryRecycleFinished> Members

        public void Handle(BinaryRecycleFinished message)
        {
            TheMessage = message;
        }

        #endregion
    }
}