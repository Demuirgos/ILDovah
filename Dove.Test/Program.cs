using Dove.Core;
var result = Parser.Parse<RootDecl.Declaration.Collection>(File.ReadAllText("Test.il"));

// TODO : Add MSFT Specific stuff
// TODO : Add TABLES 

Console.WriteLine(result);