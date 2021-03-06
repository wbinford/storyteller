using NUnit.Framework;
using StoryTeller.Domain;

namespace StoryTeller.Testing
{
    [TestFixture]
    public class HierarchyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            hierarchy = new Hierarchy("some project");
        }

        #endregion

        private Hierarchy hierarchy;

        [Test]
        public void add_test_by_one_deep_path()
        {
            Test test = hierarchy.AddTest("suite1/test1");
            test.Name.ShouldEqual("test1");
            hierarchy.FindSuite("suite1").FindTest("test1").ShouldBeTheSameAs(test);

            hierarchy.FindTest("suite1/test1").ShouldBeTheSameAs(test);
        }

        [Test]
        public void add_test_by_path_with_no_slashes()
        {
            Test test = hierarchy.AddTest("test1");

            test.Name.ShouldEqual("test1");
            hierarchy.Contains(test).ShouldBeTrue();
            hierarchy.ChildSuites.Length.ShouldEqual(0);
        }

        [Test]
        public void add_test_by_two_deep_path_twice()
        {
            Test test1 = hierarchy.AddTest("suite1/suite2/test1");
            Test test2 = hierarchy.AddTest("suite1/suite2/test2");

            hierarchy.FindTest("suite1/suite2/test1").ShouldBeTheSameAs(test1);
            hierarchy.FindTest("suite1/suite2/test2").ShouldBeTheSameAs(test2);

            hierarchy.FindSuite("suite1").FindSuite("suite2").FindTest("test1").ShouldBeTheSameAs(test1);
            hierarchy.FindSuite("suite1").FindSuite("suite2").FindTest("test2").ShouldBeTheSameAs(test2);
        }

        [Test]
        public void get_path_is_just_an_empty_string()
        {
            hierarchy.GetPath().Locator.ShouldBeEmpty();
        }
    }
}