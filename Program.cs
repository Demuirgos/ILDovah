// See https://aka.ms/new-console-template for more information
string source1 = "123:testing_ground";
int index1 = 0;

if(CodeLabel.Parse(ref index1, source1, out CodeLabel idVal1)) {
    Console.WriteLine(idVal1);
}

if(DataLabel.Parse(ref index1, source1, out DataLabel idVal2)) {
    Console.WriteLine(idVal2);
}