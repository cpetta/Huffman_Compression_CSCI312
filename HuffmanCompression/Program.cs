using System;
using System.IO;
using System.Linq;

namespace HuffmanCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                string mode = args[0];
                string inputFile = args[1];

                if (File.Exists(inputFile))
                {
                    if (mode == "-c")
                    {
                        try
                        {
                            WriteFile.Compressed(inputFile, inputFile + ".che");
                        }
                        catch(Exception e)
                        {
                            //Console.WriteLine(e.Message);
                            Console.WriteLine(e);
                        }
                    }   
                    else
                    if (mode == "-d")
                    {
                        if (isFileFormatChe(inputFile))
                        {
                            string outputFile = inputFile.Remove(inputFile.Length - 3, 3);
                            if (File.Exists(outputFile))
                            {
                                if (outputFile.Contains('.'))
                                    outputFile = outputFile.Insert(outputFile.IndexOf('.'), "(Decompressed)");
                                else
                                    outputFile = outputFile + "(Decompressed)";
                            }
                            try
                            { 
                                WriteFile.Decompressed(inputFile, outputFile);
                            }
                            catch (Exception e)
                            {
                                //Console.WriteLine(e.Message);
                                Console.WriteLine(e);
                            }

                        }
                        else
                        {
                            Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} Can only decompress files with the .che file extention.");
                        }
                    }
                    // DEBUG
                    else if (mode == "-cd")
                    {
                        DebugMode(inputFile);
                    }
                    else
                    {
                        Console.WriteLine($"Unsupported mode specified, {mode}");
                    }
                    // END DEBUG
                }
                else //(Input File doesn't exist)
                {
                    Console.WriteLine("\nThe input file {inputFile} doesn't exist\n");
                }
            }
            else // if (args != 0 || 2)
            {
                Console.WriteLine($"USAGE: {AppDomain.CurrentDomain.FriendlyName} [Input File]");
            }
            #if DEBUG
                Console.WriteLine("Done");
                Console.ReadLine();
            #endif
        }

        public static bool isFileFormatChe(string input)
        {
            if (input.Length < 4) return false;
            if (input[input.Length - 4] != '.') return false;
            if (input[input.Length - 3] != 'c') return false;
            if (input[input.Length - 2] != 'h') return false;
            if (input[input.Length - 1] != 'e') return false;
            return true;
        }

        public static void DebugMode(string inputFile)
        {
            try
            {
                WriteFile.Compressed(inputFile, inputFile + ".che");
                WriteFile.Decompressed(inputFile + ".che", "0DecompressedOutoutDebug.txt");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                Console.WriteLine(e);
            }
            using (StreamWriter sw = new StreamWriter("0originalEncodings.txt"))
            {
                foreach (byte b in Encodings.WriteEncodingTable())
                {
                    if (b == 0)
                    {
                        //Console.WriteLine((char)b);
                        sw.WriteLine();
                    }

                    else if (b != 48 && b != 49)
                        //Console.Write($"{(char)b} ({b}) ");
                        sw.Write($"{(char)b} ({b}) ");
                    else
                        sw.Write((char)b);
                    //Console.Write((char)b);
                }
            }

            using (StreamWriter sw = new StreamWriter("0OriginalBinaryTree.txt"))
            {
                //binaryTree = new BinaryTree<CharacterFrequency>(BuiltTreeRoot);
                //binaryTree.preOrder(binaryTree.Root);
            }

            using (StreamWriter sw = new StreamWriter("0RecoveredBinaryTree.txt"))
            {

            }
        }
    } // end class Compression
}


