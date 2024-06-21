using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the path to the source PDF:");
        string sourcePdfPath = Console.ReadLine()?.Trim('\"');

        if (!File.Exists(sourcePdfPath))
        {
            Console.WriteLine("File does not exist. Please check the path and try again.");
            return;
        }

        int totalPages;
        try
        {
            using (PdfReader reader = new PdfReader(sourcePdfPath))
            {
                totalPages = reader.NumberOfPages;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading PDF: {ex.Message}");
            return;
        }

        Console.WriteLine($"The PDF has {totalPages} pages.");

        Console.WriteLine("Enter the pages you want to keep (e.g., 1 or 1-15):");
        string pageRange = Console.ReadLine();

        if (!TryParsePageRange(pageRange, totalPages, out int startPage, out int endPage))
        {
            Console.WriteLine("Invalid page range. Please enter a valid page number or range.");
            return;
        }

        Console.WriteLine("Enter the path to save the new PDF, MUST be named different:");
        string outputPdfPath = Console.ReadLine()?.Trim('\"');

        try
        {
            ExtractPages(sourcePdfPath, outputPdfPath, startPage, endPage);
            Console.WriteLine($"Pages {startPage} to {endPage} have been saved to {outputPdfPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting pages: {ex.Message}");
        }
    }

    static bool TryParsePageRange(string pageRange, int totalPages, out int startPage, out int endPage)
    {
        startPage = 0;
        endPage = 0;

        if (string.IsNullOrWhiteSpace(pageRange))
        {
            return false;
        }

        string[] parts = pageRange.Split('-');
        if (parts.Length == 1)
        {
            if (int.TryParse(parts[0], out startPage) && startPage > 0 && startPage <= totalPages)
            {
                endPage = startPage;
                return true;
            }
        }
        else if (parts.Length == 2)
        {
            if (int.TryParse(parts[0], out startPage) && int.TryParse(parts[1], out endPage) &&
                startPage > 0 && endPage >= startPage && endPage <= totalPages)
            {
                return true;
            }
        }

        return false;
    }

    static void ExtractPages(string sourcePdfPath, string outputPdfPath, int startPage, int endPage)
    {
        using (PdfReader reader = new PdfReader(sourcePdfPath))
        {
            using (Document document = new Document())
            {
                using (PdfCopy copy = new PdfCopy(document, new FileStream(outputPdfPath, FileMode.Create)))
                {
                    document.Open();

                    for (int i = startPage; i <= endPage; i++)
                    {
                        PdfImportedPage page = copy.GetImportedPage(reader, i);
                        copy.AddPage(page);
                    }
                }
            }
        }
    }
}
