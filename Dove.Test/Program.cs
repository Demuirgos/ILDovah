using Dove.Core;
var result = Parser.Parse<RootDecl.Declaration>(File.ReadAllText("Test.il"));

// TODO : Add MSFT Specific stuff
// TODO : Add TABLES 
// TODO : Make Declaration base check in Source generator check the namespace
Console.WriteLine(result);