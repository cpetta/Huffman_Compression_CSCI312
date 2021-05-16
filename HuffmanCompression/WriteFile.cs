using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace HuffmanCompression
{
    class WriteFile
    {
        public static void StringToBytes(string outputFile, string toWrite)
        {
            // Delete the old file if it exists.
            if (File.Exists(outputFile))
            {
                try { File.Delete(outputFile); }
                catch (Exception e) { Console.WriteLine("An error occured while trying to delete {0}, \n\n {1}\n\n", outputFile, e.Message); }
            }
            //Create the file.
            try
            {
                byte[] b = new byte[1];
                using (FileStream fs = File.Create(outputFile))
                {
                    //byte[] b = new byte[1024]; // getting a bunch of nulls at the end of the file for some reason...
                    int IterStr = 0;
                    while (IterStr < toWrite.Length) // iterate through the provided string 1024 bytes at a time
                    {
                        try
                        {
                            //for (int i = 0; i < b.Length && IterStr < toWrite.Length; i++) //  for each byte in the 1024 byte (buffer) array.
                            b[0] = Convert.ToByte(toWrite[IterStr]);
                            IterStr++;
                            fs.Write(b, 0, b.Length);
                        }
                        catch (Exception e) { Console.WriteLine("\nAn error occured while writing a part of the file: {0}", e.Message); }
                    }
                }
                Console.WriteLine($"{outputFile} was written sucessfully.");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("\nAn error occured while trying to create: \n\n {0} \n\n (This usually happens when your filename is too long.)\n\n{1}\n", outputFile, e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("{0}", e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\nAn error occured while ether creating or writing {0}: \n\n{1}\n\n", outputFile, e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
        }

        public static void WriteErrorLog(string error, string fileName = "ErrorLog.txt")
        {
            if (!File.Exists(fileName))
            {
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("{0}, {1}:\r\n{2}\r\n", DateTime.Today, DateTime.Now, error);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine("\r\n{0}, {1}:\r\n{2}\r\n", DateTime.Today, DateTime.Now, error);
                }
            }
        }

        public static void FromBytes(string outputFile, byte[] toWrite, bool Append = false)
        {
            // Delete the old file if it exists.
            if (File.Exists(outputFile) && !Append)
            {
                try { File.Delete(outputFile); }
                catch (Exception e) { Console.WriteLine("An error occured while trying to delete {0}, \n\n {1}\n\n", outputFile, e.Message); }
            }
            //Create the file.
            try
            {
                if (Append)
                {
                    using (Stream cw = new FileStream(outputFile, FileMode.Append))
                    {
                        cw.Write(toWrite, 0, toWrite.Length);
                    }
                }
                else
                {
                    using (FileStream fs = File.Create(outputFile))
                    {
                        fs.Write(toWrite, 0, toWrite.Length);
                    }
                }
                Console.WriteLine($"{outputFile} was created sucessfully.");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("\nAn error occured while trying to create: \n\n {0} \n\n (This usually happens when your filename is too long.)\n\n{1}\n", outputFile, e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("{0}", e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\nAn error occured while ether creating or writing {0}: \n\n{1}\n\n", outputFile, e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
        }

        public static void Compressed(string inputFile, string outputFile)
        {
            CharacterFrequency[] charFreq;
            BinaryTreeNode<CharacterFrequency> BuiltTreeRoot;
            BinaryTree<CharacterFrequency> binaryTree;
            Encodings[] encodingsTable;

            charFreq = CharacterFrequency.FindFrequency(inputFile);
            BuiltTreeRoot = BinaryTreeNode<CharacterFrequency>.BuildEncodingTree(charFreq);
            encodingsTable = Encodings.buildEncodingTable(BuiltTreeRoot);

            if (File.Exists(outputFile))
            {
                try { File.Delete(outputFile); }
                catch (Exception e) { Console.WriteLine("An error occured while trying to delete {0}, \n\n {1}\n\n", outputFile, e.Message); }
            }
            try
            {
                //WriteFile.FromBytes(outputFile, Encodings.WriteEncodingTable());
                Console.WriteLine("Starting Compresion, this could take a while...");
                using (FileStream fr = File.OpenRead(inputFile))
                {

                    StringBuilder compressedFileBuilder = new StringBuilder();
                    Queue<char> BitQueue = new Queue<char>();
                    byte byteBuilder = 0;
                    bool byteBuilderIsDone = false;
                    int position = 128;
                    long numberOfCompressedBytes = 0;

                    byte[] b = new byte[1];
                    while (fr.Read(b, 0, b.Length) > 0)
                    {

                        while (BitQueue.Count > 0 && position > 1)
                        {
                            if (BitQueue.Dequeue() == '1')
                                byteBuilder = (byte)(byteBuilder | position);
                            //if (queue == '0' || queue == '1') // The above line will turn the current bit on. if it's a zero it's left off. the position is then moved to the next position.
                            position = position / 2; // go to the next position
                            // could be the source of the strokes, might need to add a bytebuilder is done thing here.
                        }
                        foreach (char bit in encodingsTable[b[0]].Bits) // each number in the encoding.
                        {
                            if (position >= 1) // not end of byte.
                            {
                                if (bit == '1')
                                    byteBuilder = (byte)(byteBuilder | position);

                                position = position / 2; // go to the next position
                            }
                            else // written a whole byte, queue the remaining encoding bits to the next byte.
                            {
                                BitQueue.Enqueue(bit);
                                byteBuilderIsDone = true;
                            }
                        }
                        if (byteBuilderIsDone)
                        {
                            /*//DEBUG
                            Console.Write((char)byteBuilder);
                            Console.Write(' ');
                            Console.WriteLine(Convert.ToString(byteBuilder, 2));
                            *///END DEBUG

                            compressedFileBuilder.Append((char)byteBuilder);

                            byteBuilder = 0;
                            byteBuilderIsDone = false;
                            position = 128;
                        }
                        numberOfCompressedBytes++;
                    } // end inputFile byte reader.
                    if (!byteBuilderIsDone) // if the last byte wasn't accounted for, because it's a partial.
                    {
                        while (BitQueue.Count > 0 && position > 1)
                        {
                            char queue = BitQueue.Dequeue();
                            if (queue == '1')
                                byteBuilder = (byte)(byteBuilder | position);

                            position = position / 2;
                        }

                        /*//DEBUG
                        Console.Write((char)byteBuilder);
                        Console.Write(' ');
                        Console.WriteLine(Convert.ToString(byteBuilder, 2));
                        *///END DEBUG

                        compressedFileBuilder.Append((char)byteBuilder);
                        //numberOfCompressedBytes++;
                    }
                    // should be revised.
                    // Write the length of the compressed bits
                    using (StreamWriter sw = File.CreateText(outputFile))
                    {
                        sw.Write(numberOfCompressedBytes);
                    }
                    // write a delimiter
                    using (Stream cw = new FileStream(outputFile, FileMode.Append))
                    {
                        byte[] temp = new byte[1];
                        cw.Write(temp, 0, temp.Length);
                    }

                    WriteFile.FromBytes(outputFile, Encodings.WriteEncodingTable(), true);
                    // Write the Compresed file.
                    byte[] compresedFile = new byte[compressedFileBuilder.Length];
                    int i = 0;
                    foreach (char c in compressedFileBuilder.ToString())
                    {
                        compresedFile[i] = (byte)c;
                        i++;
                    }
                    using (Stream cw = new FileStream(outputFile, FileMode.Append))
                    {
                        cw.Write(compresedFile, 0, compresedFile.Length);
                    }

                } // End inputfile read
            } // End try
            catch (Exception e)
            {
                Console.WriteLine("\n\n{0}\n\n", e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
        } // End Method Compressed

        public static void Decompressed(string inputFile, string outputFile)
        {
            Console.WriteLine("Starting Decompresion...");
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"{inputFile} doesn't exist.");
            }
            else
            {
                if (File.Exists(outputFile))
                {
                    try { File.Delete(outputFile); }
                    catch (Exception e) { Console.WriteLine("An error occured while trying to delete {0}, \n\n {1}\n\n", outputFile, e.Message); }
                }
                Tuple<long, int, Encodings[]> temp = Encodings.RecoverEncodingTable(inputFile);
                long numCmprsdBytes = temp.Item1;
                int offset = temp.Item2;
                Encodings[] encodingsTable = temp.Item3;

                BinaryTree<Encodings> bt = BinaryTree<Encodings>.encodingsToBinaryTree(encodingsTable);
                BinaryTreeNode<Encodings> currentNode = bt.Root;

                // DEBUG
                //Console.WriteLine("Rebuilt BinaryTree...");
                //bt.preOrder(bt.Root);
                //Console.WriteLine($"Compressed stuff offset = {offset}");
                //Console.WriteLine("Recovered Encodings from compressed file...");

                // Debug
                using (StreamWriter sw = new StreamWriter("0recoveredEncodings.txt"))
                {
                    foreach (Encodings encoding in encodingsTable)
                    {
                        if (!(encoding == null))
                        {
                            sw.Write(encoding);
                            sw.Write("\r\n");
                            //Console.WriteLine(encoding);
                        }
                    }
                }

                //Console.WriteLine("Compressed stuff...");
                //Console.Write(numCmprsdBytes);
                //*/// END DEBUG

                using (FileStream fr = File.OpenRead(inputFile))
                //using (StreamReader sr = new StreamReader(fr, Encoding.UTF8))
                {
                    using (FileStream fs = new FileStream(outputFile, FileMode.Append))
                    //using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {

                        byte[] b = new byte[1];
                        int position = 128;

                        fr.Seek(offset, 0); // start at the offset which we get from recovering the encoding Table.

                        while (fr.Read(b, 0, b.Length) > 0)
                        {
                            // DEBUG
                            //Console.Write((char)b[0]);
                            while (position >= 1 && numCmprsdBytes > 0)
                            {
                                if (position == (byte)(b[0] & position))
                                {
                                    currentNode = currentNode.Left;
                                }
                                if (position != (byte)(b[0] & position))
                                {
                                    currentNode = currentNode.Right;
                                }
                                if (currentNode.Leaf)
                                {
                                    // DEBUG
                                    //Console.Write((char)currentNode.Value.Byte);

                                    fs.WriteByte(currentNode.Value.Byte);
                                    //sw.Write((char)currentNode.Value.Byte);
                                    numCmprsdBytes--;
                                    currentNode = bt.Root;
                                }
                                position = position / 2; // go to the next position
                                //Somethings fishy.......................................................................................................................................................
                            }
                            position = 128;

                        } // end inputFile byte reader.
                    }
                } // End Using FileStream.
            } // End Else (if File.Exists(inputFile))
        } // End Method Decompress

    } // end class WriteFile
}