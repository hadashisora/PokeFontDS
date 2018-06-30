// Decompiled with JetBrains decompiler
// Type: NARCFileReadingDLL.CGLPFrame
// Assembly: NARCFileReadingDLL, Version=2.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F1D310F7-093C-48EB-B9DF-91020A139DAF
// Assembly location: C:\Users\CHEMI6DER\Downloads\pokefonts\NARCFileReadingDLL.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace NARCFileReadingDLL
{
  public class CGLPFrame : NitroFileFrameBase
  {
    public const string MAGIC = "CGLP";
    private byte m_bMaxWidth;
    private byte m_bHeight;
    private ushort m_ushLength;
    private byte m_bUnknown;
    private byte m_bKerning;
    private byte m_bBitsPerPixel;
    private byte m_bOrientation;
    private List<CGLPFrame.Character> m_lstcharCharacters;

    public CGLPFrame(BinaryReader brrReader)
      : base(brrReader)
    {
    }

    public override string Magic
    {
      get
      {
        return "CGLP";
      }
      set
      {
        if (value != "CGLP")
          throw new FormatException();
      }
    }

    public IFontTableItem[] Items
    {
      get
      {
        return (IFontTableItem[])m_lstcharCharacters.ToArray();
      }
    }

    public byte Height
    {
      get
      {
        return m_bHeight;
      }
    }

    public byte MaxWidth
    {
      get
      {
        return m_bMaxWidth;
      }
    }

    public byte MaxHeight
    {
      get
      {
        return m_bHeight;
      }
    }

    protected override byte[] GetContent()
    {
      BinaryWriter brwWriter = new BinaryWriter((Stream) new ByteArrayStream());
      brwWriter.Write(m_bMaxWidth);
      brwWriter.Write(m_bHeight);
      brwWriter.Write(m_ushLength);
      brwWriter.Write(m_bUnknown);
      brwWriter.Write(m_bKerning);
      brwWriter.Write(m_bBitsPerPixel);
      brwWriter.Write(m_bOrientation);
      foreach (CGLPFrame.Character lstcharCharacter in m_lstcharCharacters)
        lstcharCharacter.WriteTo(brwWriter);
      brwWriter.BaseStream.Position = 0L;
      return new BinaryReader(brwWriter.BaseStream).ReadBytes((int) brwWriter.BaseStream.Length);
    }

    protected override void SetContent(byte[] arrbContent)
    {
      BinaryReader brrReader = new BinaryReader((Stream) new ByteArrayStream(arrbContent));
            m_bMaxWidth = brrReader.ReadByte();
            m_bHeight = brrReader.ReadByte();
            m_ushLength = brrReader.ReadUInt16();
            m_bUnknown = brrReader.ReadByte();
            m_bKerning = brrReader.ReadByte();
            m_bBitsPerPixel = brrReader.ReadByte();
            m_bOrientation = brrReader.ReadByte();
      if ((brrReader.BaseStream.Length - brrReader.BaseStream.Position) % (long)m_ushLength != 0L)
        throw new FormatException();
            m_lstcharCharacters = new List<CGLPFrame.Character>((int) (brrReader.BaseStream.Length - brrReader.BaseStream.Position) / (int)m_ushLength);
      for (int index = 0; index < m_lstcharCharacters.Capacity; ++index)
                m_lstcharCharacters.Add(new CGLPFrame.Character(brrReader, (int)m_ushLength, m_bMaxWidth, m_bHeight, m_bBitsPerPixel));
    }

    public void AddNewItem()
    {
            m_lstcharCharacters.Add(new CGLPFrame.Character(m_bMaxWidth, m_bHeight, m_bBitsPerPixel));
    }

    public class Character : IFontTableItem, IContainsSpaceWidth
    {
      private VALUE[,] m_arrvValues;
      private byte m_bySpaceWidth;
      private byte m_byWidth;
      private byte m_byUnknown;
      private byte m_byMaxWidth;
      private byte m_byMaxHeight;
      private byte m_byBPP;

      public Character(byte byMaxWidth, byte byMaxHeight, byte byBPP)
      {
                m_bySpaceWidth = (byte) 0;
                m_byWidth = (byte) 0;
                m_byUnknown = (byte) 0;
                m_byMaxWidth = byMaxWidth;
                m_byMaxHeight = byMaxHeight;
                m_byBPP = byBPP;
                m_arrvValues = new VALUE[(int) byMaxHeight, (int) byMaxWidth];
        for (int index1 = 0; index1 < (int) byMaxHeight; ++index1)
        {
          for (int index2 = 0; index2 < (int) byMaxWidth; ++index2)
                        m_arrvValues[index1, index2] = VALUE.VALUE_0;
        }
      }

      public Character(BinaryReader brrReader, int nSize, byte byMaxWidth, byte byMaxHeight, byte byBPP)
      {
        if (nSize < 3)
          throw new FormatException();
        if ((long) nSize > brrReader.BaseStream.Length - brrReader.BaseStream.Position)
          throw new FormatException();
        if ((int) byMaxHeight * (int) byMaxWidth / (8 / (int) byBPP) > nSize - 3)
          throw new FormatException();
                m_bySpaceWidth = brrReader.ReadByte();
                m_byWidth = brrReader.ReadByte();
        if ((int)m_byWidth > (int) byMaxWidth)
          throw new FormatException();
                m_byUnknown = brrReader.ReadByte();
                m_byMaxWidth = byMaxWidth;
                m_byMaxHeight = byMaxHeight;
                m_byBPP = byBPP;
                m_arrvValues = new VALUE[(int) byMaxHeight, (int) byMaxWidth];
        int num1 = 0;
        byte num2 = 0;
        for (int index1 = 0; index1 < (int) byMaxHeight; ++index1)
        {
          for (int index2 = 0; index2 < (int) byMaxWidth; ++index2)
          {
            if (num1 == 0)
              num2 = brrReader.ReadByte();
                        m_arrvValues[index1, index2] = (VALUE) ((double) ((int) num2 / (int) Math.Pow(Math.Pow(2.0, (double) byBPP), (double) (3 - num1))) % Math.Pow(2.0, (double) byBPP));
            num1 = (num1 + 1) % (8 / (int)m_byBPP);
          }
        }
      }

      public int Size
      {
        get
        {
          return 3 + m_arrvValues.Length / 4;
        }
      }

      public VALUE[,] Item
      {
        get
        {
          return (VALUE[,])m_arrvValues.Clone();
        }
        set
        {
          if (value == null)
            throw new FormatException();
          if (value.GetLength(0) != (int)m_byMaxHeight || value.GetLength(1) != (int)m_byMaxWidth)
            throw new FormatException();
                    m_arrvValues = value;
        }
      }

      public byte SpaceWidth
      {
        get
        {
          return m_bySpaceWidth;
        }
        set
        {
                    m_byUnknown += (byte) ((uint) value - (uint)m_bySpaceWidth);
                    m_bySpaceWidth = value;
        }
      }

      public byte Width
      {
        get
        {
          return m_byWidth;
        }
        set
        {
          if ((int) value > (int)m_byMaxWidth)
            throw new FormatException();
                    m_byUnknown += (byte) ((uint) value - (uint)m_byWidth);
                    m_byWidth = value;
        }
      }

      public byte Unknown
      {
        get
        {
          return m_byUnknown;
        }
      }

      public byte Height
      {
        get
        {
          return m_byMaxHeight;
        }
      }

      public void WriteTo(BinaryWriter brwWriter)
      {
        brwWriter.Write(m_bySpaceWidth);
        brwWriter.Write(m_byWidth);
        brwWriter.Write(m_byUnknown);
        int num1 = 0;
        byte num2 = 0;
        for (int index1 = 0; index1 < (int)m_byMaxHeight; ++index1)
        {
          for (int index2 = 0; index2 < (int)m_byMaxWidth; ++index2)
          {
            num2 += (byte) ((double) (byte)m_arrvValues[index1, index2] * Math.Pow(Math.Pow(2.0, (double)m_byBPP), (double) (3 - num1)));
            num1 = (num1 + 1) % 4;
            if (num1 == 0)
            {
              brwWriter.Write(num2);
              num2 = (byte) 0;
            }
          }
        }
        if (num1 == 0)
          return;
        brwWriter.Write(num2);
      }
    }
  }
}
