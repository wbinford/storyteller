using System;
using StoryTeller.Engine;

namespace StoryTeller.Samples.Grammars
{
    public class TableFixture : Fixture
    {
        private double _first;
        private double _second;

        public TableFixture()
        {
            this["decisions"] = new Decisions();
        }


        [ExposeAsTable("Adding numbers together", "sum")]
        [return: AliasAs("sum")]
        public int Sum(int x, int y)
        {
            return x + y;
        }

        public IGrammar Divide()
        {
            return Script("Divide numbers", x =>
            {
                x += Do(() => _first = _second = 0);
                x += Read<double>("x", o => _first = o);
                x += Read<double>("y", o => _second = o);
                x += Check("product", () => _first/_second);
            }).AsTable("Subtract numbers");
        }

        public IGrammar BeforeThrowsError()
        {
            return
                Script("Throws an Error before we even start", x => { x += Read<double>("x", o => { }); }).AsTable(
                    "Throw error on the Before").Before(() => { throw new NotImplementedException(); });
        }

        public IGrammar AfterThrowsError()
        {
            return
                Script("Throws an Error before we even start", x => { x += Read<double>("x", o => { }); }).AsTable(
                    "Throw error on the After").After(() => { throw new NotImplementedException(); });
        }
    }


    public class Decisions : DecisionTableGrammar
    {
        private string _first;
        private string _last;

        public Decisions()
            : base("decisions")
        {
        }

        public string FirstName { set { _first = value; } }

        public string LastName { set { _last = value; } }

        public string FullName { get { return _first + " " + _last; } }

        public string LastNameFirst { get { return _last + ", " + _first; } }
    }
}