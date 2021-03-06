using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using StoryTeller.Domain;
using StoryTeller.Engine;
using StoryTeller.Model;
using StoryTeller.Samples;
using StoryTeller.UserInterface;
using StoryTeller.UserInterface.Editing;
using StoryTeller.UserInterface.Tests;
using StoryTeller.Workspace;
using StructureMap;
using StructureMap.TypeRules;

namespace StoryTeller.Testing
{
    [TestFixture, Explicit]
    public class Debugging
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion


        [Test]
        public void write_and_open_a_test_editor()
        {
            var project = DataMother.GrammarProject();
            var test = project.LoadTests().FindTest("Sentences");

            var builder = new TestEditorBuilder();
            var document = builder.BuildTestEditor(test, project.LocalRunner().Library);
            document.WriteToFile("editor.htm");

            var path = Path.GetFullPath("editor.htm");

            Process.Start("iexplore.exe", path);
        }


        [Test]
        public void should_find_11_startables()
        {
            Bootstrapper.BootstrapShell();

            ObjectFactory.Model.AllInstances.Where(x => x.ConcreteType == typeof(ScreenConductor)).Each(
                x => Debug.WriteLine(x.PluginType.FullName));

            ObjectFactory.Model.AllInstances.Count(x => x.ConcreteType == typeof(ScreenConductor)).ShouldEqual(1);
        }

        [Test]
        public void try_it()
        {
            typeof(TestPresenter).GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListener<>))
                .Select(x => x.GetGenericArguments()[0])
                .Each(x => Debug.WriteLine(x.FullName));
        }

        [Test]
        public void try_out_json_reading_writing()
        {
            var j = new JObject();
            j["name"] = new JValue("some test name");
            j["lifecycle"] = new JValue("Acceptance");

            Debug.WriteLine(j.ToString());
        }


        [Test]
        public void who_is_listening()
        {
            // Bootstrapper is a just a little helper class to bootstrap
            // my IoC container.  It's very, very advantageous to decouple
            // your IoC container setup away from the Global.asax or the
            // main executable of a WPF/WinForms app for this very kind
            // of scenario
            IContainer container = Bootstrapper.BuildContainer();
            var diagnostics = new ListenerDiagnostics(container);

            diagnostics.Listeners.GroupBy(x => x.EventType).Each(group =>
            {
                Debug.WriteLine("");
                Debug.WriteLine(group.Key.FullName + " is received by:");

                group.Each(x => Debug.WriteLine("     * " + x.ConcreteType));

                Debug.WriteLine("");
            });
        }

        [Test]
        public void write_out_the_project_file_for_match()
        {
            var project = new Project
            {
                BinaryFolder = @"..\..\source\StoryTeller.Samples\bin\debug",
                FileName = "math.xml",
                Name = "Math",
                TestFolder = "math",
                TestRunnerTypeName = typeof(MathTestRunner).AssemblyQualifiedName
            };

            project.Save(@"C:\code\StoryTeller\samples\math.xml");
        }

        [Test]
        public void write_some_html()
        {
            Project project = DataMother.MathProject();
            ITestRunner runner = project.LocalRunner();

            Test test = project.LoadTests().GetAllTests().First();

            runner.RunTest(test);

            test.OpenResultsInBrowser();
        }
    }


    public class ListenerDiagnostics
    {
        private readonly IEnumerable<ListenerToken> _listeners;

        public ListenerDiagnostics(IContainer container)
        {
            _listeners = container.Model.AllInstances
                .Where(x => x.ConcreteType != null)
                .Where(x => x.ConcreteType.ImplementsInterfaceTemplate(typeof(IListener<>)))
                .Select(x => x.ConcreteType)
                .SelectMany(x => tokensForType(x));
        }

        public IEnumerable<ListenerToken> Listeners { get { return _listeners; } }

        private static IEnumerable<ListenerToken> tokensForType(Type type)
        {
            return type.GetInterfaces()
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IListener<>))
                .Select(x => new ListenerToken
                {
                    ConcreteType = type,
                    EventType = x.GetGenericArguments()[0]
                });
        }

        public IEnumerable<Type> WhoListensTo<T>()
        {
            return _listeners
                .Where(x => x.EventType == typeof(T))
                .Select(x => x.ConcreteType);
        }
    }

    public class ListenerToken
    {
        public Type ConcreteType { get; set; }
        public Type EventType { get; set; }
    }


    [TestFixture]
    public class monkey_around
    {
        [Test]
        public void get_example_out_of_fixture1()
        {
            var runner = new TestRunner(x =>
            {
                x.AddFixture<Fixture1>();
                x.AddFixture<Fixture2>();
            });

            var fixture = new Fixture1();

            GrammarStructure paragraph = fixture["EditAndGo"].ToStructure(runner.Library);
            IStep example = paragraph.CreateExample();

            Debug.WriteLine(example);
        }

        [Test]
        public void look_at_illegal_characters()
        {
            Path.GetInvalidPathChars().Each(x => Debug.WriteLine(x));
        }
    }

    public class Fixture1 : Fixture
    {
        public void Edit(string caption)
        {
        }

        public IGrammar EditAndGo()
        {
            return Script("Edit and go", x =>
            {
                x += this["Edit"];
                x += Embed<Fixture2>("Do something");
            });
        }
    }

    public class Fixture2 : Fixture
    {
        public void Add(int x, int y)
        {
        }

        public void Subtract(int x, int y)
        {
        }
    }
}