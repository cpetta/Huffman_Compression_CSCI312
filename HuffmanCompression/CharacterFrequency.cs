using System;
using System.IO;

namespace HuffmanCompression
{
    class CharacterFrequency : IComparable
    {
        private char ch;
        private byte ascii;
        private ulong frequency = 0;

        public char Ch
        {
            get
            {
                return ch;
            }
            set
            {
                ch = value;
            }
        }

        public ulong Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
            }
        }

        public byte Ascii
        {
            get
            {
                return ascii;
            }
            set
            {
                ascii = value;
            }
        }

        public void increment()
        {
            frequency++;
        }

        public static CharacterFrequency[] FindFrequency(string inputFile)
        {
            CharacterFrequency[] charFreq = new CharacterFrequency[256];
            for (int i = 0; i < charFreq.Length; i++)
            {
                charFreq[i] = new CharacterFrequency();
                charFreq[i].Ch = Convert.ToChar(i);
                charFreq[i].Ascii = Convert.ToByte(i);
            }
            try
            {
                using (FileStream fs = File.OpenRead(inputFile))
                {
                    byte[] b = new byte[1];
                    while (fs.Read(b, 0, b.Length) > 0)
                        charFreq[b[0]].increment();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n\n{0}\n\n", e.Message);
                //WriteFile.WriteErrorLog(e.ToString());
            }
            return charFreq;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CharacterFrequency cf = (CharacterFrequency)obj;
                return (ch == cf.ch);
            }
        }

        public override int GetHashCode()
        {
            return ascii;
        }

        public override string ToString()
        {
            string formattedOutput = ch + " (" + ascii + ") " + frequency /*+ "\r\n"*/;
            return formattedOutput;
        }

        public int CompareTo(object obj)
        {
            int value = 0;
            if (obj == null) //|| !this.GetType().Equals(obj.GetType()))
            {
                value = int.MaxValue;
            }
            if (!(obj is CharacterFrequency))
                throw new ArgumentException($"{obj} is not a Character Frequency object");

            CharacterFrequency temp = obj as CharacterFrequency;

            if (frequency < temp.frequency)
                value = int.MinValue;
            else if (frequency.Equals(temp.frequency))
                value = 0;
            else
                value = int.MaxValue;

            return value; // _ch.CompareTo(temp._ch); // charfreq object is being input, need to imploment how to compair the two.
        }

    } // end class CharacterFrequency
}