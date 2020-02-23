using System.Collections.ObjectModel;

namespace Web.Globals 
{
    public class StringGlobals 
    {
        public static readonly string [] CaseKinds = { "bla", "bla"};

        public ReadOnlyCollection<string> test { get; } = new ReadOnlyCollection<string> (
            new string [] { "testing", "this", "out"} 
        ); 
    }
}