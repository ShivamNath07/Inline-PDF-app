using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PdfValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Change these paths as needed
                string pdfFolderPath = @"C:\Users\52470252\Downloads\inline APp\In\";
                string inputFilePath = @"C:\Users\52470252\Downloads\inline APp\Out\Filenames.txt";
                string outputFilePath = Path.Combine(
                    Path.GetDirectoryName(inputFilePath),
                    "OutputReport.txt");

                // Read expected names from TXT
                HashSet<string> expectedNames = new HashSet<string>(
                File.ReadAllLines(inputFilePath)
               .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Path.GetFileNameWithoutExtension(x.Trim())),
                StringComparer.OrdinalIgnoreCase);

                // Read actual PDF names without extension
                HashSet<string> actualPdfNames = new HashSet<string>(
                    Directory.GetFiles(pdfFolderPath, "*.pdf")
                        .Select(x => Path.GetFileNameWithoutExtension(x)),
                    StringComparer.OrdinalIgnoreCase);

                // Missing PDFs
                List<string> missingPdfs = expectedNames
                    .Except(actualPdfNames, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList();

                // Extra PDFs
                List<string> extraPdfs = actualPdfNames
                    .Except(expectedNames, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .Select(x => x + ".pdf")
                    .ToList();

                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    writer.WriteLine("===== MISSING PDFs =====");
                    writer.WriteLine();

                    if (missingPdfs.Any())
                    {
                        foreach (string file in missingPdfs)
                            writer.WriteLine(file);
                    }
                    else
                    {
                        writer.WriteLine("None");
                    }

                    writer.WriteLine();
                    writer.WriteLine("===== EXTRA PDFs FOUND IN FOLDER =====");
                    writer.WriteLine();

                    if (extraPdfs.Any())
                    {
                        foreach (string file in extraPdfs)
                            writer.WriteLine(file);
                    }
                    else
                    {
                        writer.WriteLine("None");
                    }
                }
                Console.WriteLine("Expected Names Count : " + expectedNames.Count);
                Console.WriteLine("PDF Files Count      : " + actualPdfNames.Count);
                Console.WriteLine("Missing Count        : " + missingPdfs.Count);
                Console.WriteLine("Extra Count          : " + extraPdfs.Count);
                Console.WriteLine("Validation completed.");
                Console.WriteLine("Output generated: " + outputFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}