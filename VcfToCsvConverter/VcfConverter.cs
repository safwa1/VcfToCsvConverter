using System.Text;

namespace VcfToCsvConverter;

public static class VcfConverter
{
    public static bool Convert(string vcfFilePath, string csvFilePath)
    {
        try
        {
            List<Contact> contacts = ReadVcfFile(vcfFilePath);
            WriteCsvFile(csvFilePath, contacts);
            Console.WriteLine($"Conversion successful. CSV file saved at: {csvFilePath}");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }

    private static List<Contact> ReadVcfFile(string filePath)
    {
        List<Contact> contacts = [];
        Contact? currentContact = null;

        foreach (var line in File.ReadLines(filePath))
        {
            if (line.StartsWith("BEGIN:VCARD"))
            {
                currentContact = new Contact();
            }
            else if (line.StartsWith("END:VCARD"))
            {
                if (currentContact != null)
                {
                    contacts.Add(currentContact);
                    currentContact = null;
                }
            }
            else if (currentContact != null)
            {
                Console.WriteLine(line);
                Console.WriteLine("=====================");
                if (line.StartsWith("FN:"))
                {
                    var name = line[3..];
                    currentContact.FullName = name;
                }
                else if (line.StartsWith("TEL"))
                {
                    var phoneNumber = ExtractPhoneNumber(line);
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        currentContact.PhoneNumbers.Add(phoneNumber);
                    }
                }
                else if (line.StartsWith("EMAIL"))
                {
                    var email = ExtractPhoneNumber(line);
                    if (!string.IsNullOrEmpty(email))
                    {
                        currentContact.Emails.Add(email);
                    }
                }
            }
        }

        return contacts;
    }

    private static string? ExtractPhoneNumber(string input)
    {
        string searchTerm = "pref:";
        int index = input.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase);

        if (index != -1)
        {
            // Extract substring from "pref:" to the end of the string
            return input[(index + searchTerm.Length)..];
        }

        return null;
    }
    
    private static void WriteCsvFile(string filePath, List<Contact> contacts)
    {
        var sb = new StringBuilder();
        sb.AppendLine("FullName,PhoneNumbers");

        foreach (var contact in contacts)
        {
            var phones = string.Join(";", contact.PhoneNumbers);
            sb.AppendLine($"\"{Escape(contact.FullName)}\",\"{Escape(phones)}\"");
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }
    
    private static string Escape(string? input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        if (input.Contains('"') || input.Contains(','))
        {
            return $"\"{input.Replace("\"", "\"\"")}\"";
        }
        return input;
    }

    public static void PrintCsvAsTable(string csvFilePath)
    {
        if (!File.Exists(csvFilePath))
        {
            Console.WriteLine("CSV file not found.");
            return;
        }

        var lines = File.ReadAllLines(csvFilePath);
        if (lines.Length == 0)
        {
            Console.WriteLine("CSV file is empty.");
            return;
        }

        // Split headers and data
        string[] headers = lines[0].Split(',');
        var data = new List<string[]>();
        for (int i = 1; i < lines.Length; i++)
        {
            data.Add(lines[i].Split(','));
        }

        // Calculate column widths
        int[] columnWidths = new int[headers.Length];
        for (int i = 0; i < headers.Length; i++)
        {
            columnWidths[i] = headers[i].Length;
            foreach (var row in data)
            {
                if (row[i].Length > columnWidths[i])
                {
                    columnWidths[i] = row[i].Length;
                }
            }
        }

        // Print headers
        for (int i = 0; i < headers.Length; i++)
        {
            Console.Write(headers[i].PadRight(columnWidths[i] + 2));
        }

        Console.WriteLine();

        // Print separator
        for (int i = 0; i < headers.Length; i++)
        {
            Console.Write(new string('-', columnWidths[i]).PadRight(columnWidths[i] + 2));
        }

        Console.WriteLine();

        // Print data rows
        foreach (var row in data)
        {
            for (int i = 0; i < row.Length; i++)
            {
                Console.Write(row[i].PadRight(columnWidths[i] + 2));
            }

            Console.WriteLine();
        }
    }
}