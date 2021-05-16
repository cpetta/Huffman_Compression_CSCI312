using System;
using System.IO;
using System.Text;

// Todo.
// The encoding Table should use two bytes, the first for the byte to be encoded, and the second for the encoding itself.
namespace HuffmanCompression
{
    class Encodings : IComparable
    {
        private static StringBuilder encodingBuilder = new StringBuilder();
        private static Encodings[] Table = new Encodings[256]; // should be changed to not care about size.

        public byte Byte { get; set; }

        public string Bits { get; set; }

        public Encodings()
        {

        }

        public Encodings(Encodings encoding)
        {
            Byte = encoding.Byte;
            Bits = encoding.Bits;
        }

        public Encodings(byte b, string ConstructerBits)
        {
            Byte = b;
            Bits = ConstructerBits;
        }

        public static Encodings[] buildEncodingTable(BinaryTreeNode<CharacterFrequency> Node)
        {
            if (Node.Left != null)
            {
                encodingBuilder.Append("1"); // ToChange (To something shorter...maybe?)
                buildEncodingTable(Node.Left);
            }
            if (Node.Right != null)
            {
                encodingBuilder.Append("0"); // ToChange (To something shorter...maybe?)
                buildEncodingTable(Node.Right);
            }
            if (Node.Leaf)
            {
                byte index = (byte)Node.Value.Ch;
                Table[index] = new Encodings((byte)Node.Value.Ch, encodingBuilder.ToString());
            }
            if (encodingBuilder.Length > 0)
                encodingBuilder.Remove(encodingBuilder.Length - 1, 1);

            return Table;
        }

        public static Tuple<long, int, Encodings[]> RecoverEncodingTable(string inputFile)
        {
            long ncb = 0; // Number of Compressed Bytes
            int offset = 0;
            Encodings[] encodings = new Encodings[256];

            using (FileStream fr = File.OpenRead(inputFile))
            {
                bool notEndofEncodingTable = true;
                byte[] b1 = new byte[] { byte.MaxValue };
                byte[] b2 = new byte[] { byte.MaxValue };
                byte[] b3 = new byte[] { byte.MaxValue };
                byte currentEncodingIndex = 255;
                StringBuilder encodingBuilder = new StringBuilder();

                string ncbBuilder = "";
                bool ncbDone = false;

                // DEBUG
                //Console.WriteLine("Reading Encoding Table...");

                while (fr.Read(b1, 0, b1.Length) > 0 && notEndofEncodingTable)
                {
                    /*// DEBUG
                    Console.WriteLine((char)b1[0]);
                    Console.WriteLine($"b1 = {b1[0]}");
                    Console.WriteLine($"b2 = {b2[0]}");
                    Console.WriteLine($"b3 = {b3[0]}");
                    */// END DEBUG

                    // If we haven't built the compressed length yet
                    if (!ncbDone)
                    {
                        // if the current byte being read isn't a null
                        if (b1[0] != 0)
                        {
                            //append the byte to the number builder.
                            ncbBuilder = ncbBuilder + (char)b1[0];
                        }
                        else
                        {
                            ncb = long.Parse(ncbBuilder);
                            ncbDone = true;
                        }
                    }
                    else
                    {
                        // if the byte is a '0' and the next byte isn't a null...unless the third byte is 255, which means that the null is at the begining and is only one bit long.
                        if (b1[0] == 48 && (b2[0] != 0 || b3[0] == 255))
                        {
                            encodingBuilder.Append('0');
                        }
                        // if the byte is a '1' and the next byte isn't a null...unless the third byte is 255, which means that the null is at the begining and is only one bit long.
                        if (b1[0] == 49 && (b2[0] != 0 || b3[0] == 255))
                        {
                            encodingBuilder.Append('1');
                        }
                        // If the Null char is present in the encoding table
                        if (b1[0] == 0 && b2[0] == 0) // will also activate when reaching the end of the table.
                        {
                            currentEncodingIndex = b1[0];
                        }
                        // If The Null char is present in the encoding table, at the begining of the file....
                        if (b1[0] == 0 && b2[0] == byte.MaxValue && b3[0] == byte.MaxValue)
                        {
                            currentEncodingIndex = b1[0];
                        }
                        // if the byte is not a '0' or '1' or null (It must be a character of some sort)
                        if (b1[0] != 48 && b1[0] != 49 && b1[0] != 0)
                        {
                            currentEncodingIndex = b1[0];
                        }
                        // if the last byte was a null and the next byte is a '0'
                        if (b2[0] == 0 && b1[0] == 48 && b3[0] != 0)
                        {
                            currentEncodingIndex = b1[0];
                        }
                        // if the last byte was a null and the next byte is a '1'
                        if (b2[0] == 0 && b1[0] == 49 && b3[0] != 0)
                        {
                            currentEncodingIndex = b1[0];
                        }
                        // if the byte is null (the end of an encoding.) And the last byte was '0' or '1'.
                        if (b1[0] == 0 && (b2[0] == 48 || b2[0] == 49) && b3[0] != byte.MaxValue)
                        {
                            encodings[currentEncodingIndex] = new Encodings(currentEncodingIndex, encodingBuilder.ToString());
                            encodingBuilder.Clear();
                        }
                        // if the last three bytes that were read are null. (Found end of encodings)
                        if (b1[0] == 0 && b2[0] == 0 && b3[0] == 0)
                        {
                            notEndofEncodingTable = false;
                        }
                    }
                    // move b to b2, and b2 to b3.
                    b3[0] = b2[0];
                    b2[0] = b1[0];
                    offset++;
                }
            }
            Tuple<long, int, Encodings[]> importantStuff = Tuple.Create(ncb, offset, encodings);
            return importantStuff;
        }

        public override string ToString()
        {
            return Convert.ToChar(Byte) + " (" + Byte + ") " + Bits;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;

            if (obj == null) return false;

            Encodings en = obj as Encodings;
            if (en == null) return false;

            return en.Byte == this.Byte;
        }

        public override int GetHashCode()
        {
            return Byte.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            int value = 0;
            if (obj == null) //|| !this.GetType().Equals(obj.GetType()))
            {
                value = int.MaxValue;
            }
            if (!(obj is Encodings))
                throw new ArgumentException($"{obj} is not a Character Frequency object");

            Encodings temp = obj as Encodings;

            if (Byte < temp.Byte)
                value = int.MinValue;
            else if (Byte.Equals(temp.Byte))
                value = 0;
            else
                value = int.MaxValue;

            return value;
        }
        public static byte[] Transform(byte b)
        {
            int ByteLength = Table[b].Bits.Length / 8;
            int ByteIter = 1;
            byte[] encoding = new byte[ByteLength];
            foreach (char ch in Table[b].Bits)
            {
                if (ch == '1')
                {

                    ByteIter++;
                }
                if (ch == '0')
                {

                    ByteIter++;
                }
            }
            return encoding;
            //return Table[b].Bits;
        }

        public static int TableLength
        {
            get
            {
                int count = 0;
                foreach (Encodings item in Table)
                {
                    if (item != null)
                    {
                        count++;
                        count = count + item.Bits.Length + 1;
                    }
                }
                count = count + 2; // two nulls at the end of the encoding table.
                return count;
            }
        }

        public static byte[] WriteEncodingTable()
        {
            byte[] b = new byte[TableLength];
            int index = 0;
            foreach (Encodings item in Encodings.Table)
            {
                if (item != null)
                {
                    b[index] = item.Byte;
                    index++;
                    for (int i = 0; i < item.Bits.Length; i++)
                    {
                        b[index] = (byte)item.Bits[i];
                        index++;
                    }
                    b[index] = 0;
                    index++;
                }
            }
            return b;
        }
    }
}