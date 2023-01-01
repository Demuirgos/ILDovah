using static Core;

var (source, index) = (".event TimeUpEventHandler startStopEvent {.addon instance void Counter::add_TimeUp() .removeon instance void Counter::remove_TimeUp(class TimeUpEventHandler 'handler') .fire instance void Counter::fire_TimeUpEvent() }", 0);

TestConstruct<Event>(ref index, source);

void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
    {
        if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
            Console.WriteLine(resultVal);
        } else {
            Console.WriteLine("Failed to parse");
        }
    }