namespace VcfToCsvConverter;

internal static class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: VcfToCsvConverter.exe <vcfFilePath> [--print | -p]");
            return;
        }
        
        string vcfFilePath = args[0];
        string csvFilePath = Path.ChangeExtension(vcfFilePath, ".csv");
        
        VcfConverter.Convert(vcfFilePath, csvFilePath);
        
        if (args is [_, "--print"] or [_, "-p"])
        {
            VcfConverter.PrintCsvAsTable(csvFilePath);
        }

    }


}