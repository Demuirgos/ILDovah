﻿using static Core;

// Note(Ayman) : Fix whitespace in ToString methods
// Note(Ayman) : Add Extra checks to Nested Self referential Parsers to avoid 
//               infinite loops or stack overflows 
// Note(Ayman) : Wrap partial type (header, members etc) in their containing type


var (source, index) = (".file File.dll", 0);

TestConstruct<ExternClass>(ref index, source);

void TestConstruct<T>(ref int index, string source)
    where T : IDeclaration<T>
    {
        if(IDeclaration<T>.Parse(ref index, source, out T resultVal)) {
            Console.WriteLine(resultVal);
        } else {
            Console.WriteLine("Failed to parse");
        }
    }