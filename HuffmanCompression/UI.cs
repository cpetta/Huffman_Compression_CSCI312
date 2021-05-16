using System;
using System.IO;
using System.Text;

class UI
{
    public static void NoArgsMode()
    {
        string inputFile = null;
        string outputFile = null;

        Console.WriteLine("--- (Chris Petta's) Huffman File Compression. ---\n");
        Console.WriteLine("Type /h for help.");
        Console.WriteLine("Type /q to close the program.\n");

        do
        {
            inputFile = VerifyInput("Compress");

            if (inputFile != null && inputFile == "FileDoesntExist")
                Console.WriteLine("The specified file doesn't exist.");

            if (inputFile != null && inputFile != "shutdown" && inputFile != "FileDoesntExist")
                outputFile = VerifyInput("write");

            if (inputFile != null && inputFile != "shutdown" && inputFile != "FileDoesntExist" && outputFile != "shutdown" && outputFile == null)
            {
                // do compression.
            }

            if (inputFile != null && inputFile != "shutdown" && inputFile != "FileDoesntExist" && outputFile != "shutdown" && outputFile != null && inputFile == outputFile)
                Console.WriteLine("\nThe Output file can't have the same name as the input file.\n");

        } while ((inputFile != "shutdown") && (outputFile != "shutdown"));
    }
    static void OutputHelp()
    {
        Console.WriteLine("\nThis program compresses a given file.\n");
        Console.WriteLine("Commandline Usage: \n\t cpcf.exe [file to read] [file to write]\n\n");
        Console.WriteLine("Type /q to close the program.\n");
    }

    public static string VerifyInput(string readOrWrite, string userInput = null)
    {
        switch (userInput)
        {
            case "exit":
            case "quit":
            case "/q":
            case "-q":
                return "shutdown";
            case "/h":
            case "-h":
                OutputHelp();
                return null;
            case "":
            case null:
                return null;

            default: // not a UI command, so check if it's a good input.
                if (readOrWrite == "write")
                    return userInput;
                else if (File.Exists(userInput)) // not writting, so it's asumed we're reading.
                    return userInput;
                else // Reading, and the file to read doesn't exist.
                    return "FileDoesntExist";
        } // end switch
    } // end VerifyInput()
}