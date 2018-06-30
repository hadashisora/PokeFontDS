// Decompiled with JetBrains decompiler
// Type: NARCFileReadingDLL.StringTableSection
// Assembly: NARCFileReadingDLL, Version=2.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F1D310F7-093C-48EB-B9DF-91020A139DAF
// Assembly location: C:\Users\CHEMI6DER\Downloads\pokefonts\NARCFileReadingDLL.dll

using System;
using System.IO;

namespace NARCFileReadingDLL
{
  public class StringTableSection : IStringTableSection
  {
    private StringTableSectionEntry[] m_arrstseEntries;
    private ushort[] m_arrushUnknowns;

    public StringTableSection(BinaryReader brrReader, ushort ushEntriesCount)
    {
            ReadFrom(brrReader, ushEntriesCount);
    }

    public int Size
    {
      get
      {
        int num = 4;
        foreach (StringTableSectionEntry arrstseEntry in m_arrstseEntries)
          num += 8 + arrstseEntry.Size;
        return num + num % 4;
      }
    }

    public IStringTableSectionEntry[] Entries
    {
      get
      {
        return (IStringTableSectionEntry[])m_arrstseEntries;
      }
    }

    public event EventHandler Changed
    {
      add
      {
        foreach (StringTableSectionEntry arrstseEntry in m_arrstseEntries)
          arrstseEntry.Changed += value;
      }
      remove
      {
        foreach (StringTableSectionEntry arrstseEntry in m_arrstseEntries)
          arrstseEntry.Changed -= value;
      }
    }

    public void ReadFrom(BinaryReader brrReader, ushort ushEntriesCount)
    {
      if (brrReader.BaseStream.Length - brrReader.BaseStream.Position < 4L)
        throw new FormatException();
            m_arrstseEntries = new StringTableSectionEntry[(int) ushEntriesCount];
            m_arrushUnknowns = new ushort[(int) ushEntriesCount];
      uint position = (uint) brrReader.BaseStream.Position;
      uint num1 = brrReader.ReadUInt32();
      if (num1 < (uint) (4 + 8 * m_arrstseEntries.Length) || num1 % 4U != 0U)
        throw new FormatException();
      if (brrReader.BaseStream.Length - brrReader.BaseStream.Position < (long) (num1 - 4U))
        throw new FormatException();
      uint num2 = (uint) ((int) position + 4 + 8 * m_arrstseEntries.Length);
      for (int index = 0; index < (int) ushEntriesCount; ++index)
      {
        brrReader.BaseStream.Position = (long) (position + 4U) + (long) (index * 8);
        uint num3 = brrReader.ReadUInt32();
        if ((int) num3 != (int) num2 - (int) position)
          throw new FormatException();
        ushort ushCharacterCount = brrReader.ReadUInt16();
                m_arrushUnknowns[index] = brrReader.ReadUInt16();
        if ((long) (position + num3) + (long) ((int) ushCharacterCount * 2) > brrReader.BaseStream.Length)
          throw new FormatException();
        brrReader.BaseStream.Position = (long) (position + num3);
                m_arrstseEntries[index] = new StringTableSectionEntry(brrReader, ushCharacterCount);
        num2 = (uint) brrReader.BaseStream.Position;
      }
      if ((long) (position + num1) - brrReader.BaseStream.Position >= 4L || (long) (position + num1) - brrReader.BaseStream.Position < 0L)
        throw new FormatException();
    }

    public void WriteTo(BinaryWriter brwWriter)
    {
      uint position = (uint) brwWriter.BaseStream.Position;
      brwWriter.Write((uint)Size);
      uint num1 = (uint) (4 + 8 * m_arrstseEntries.Length);
      for (int index = 0; index < m_arrstseEntries.Length; ++index)
      {
        brwWriter.Write(num1);
        num1 += (uint)m_arrstseEntries[index].Size;
        brwWriter.Write((ushort)m_arrstseEntries[index].Length);
        brwWriter.Write(m_arrushUnknowns[index]);
      }
      foreach (StringTableSectionEntry arrstseEntry in m_arrstseEntries)
        arrstseEntry.WriteTo(brwWriter);
      if ((brwWriter.BaseStream.Position - (long) position) % 4L != 2L)
        return;
      ushort num2 = m_arrstseEntries[m_arrstseEntries.Length - 1].Key;
      for (int index = 0; index < m_arrstseEntries[m_arrstseEntries.Length - 1].Length; ++index)
        num2 = (ushort) (((int) num2 << 3 | (int) num2 >> 13) & (int) ushort.MaxValue);
      brwWriter.Write((ushort) ((uint) ushort.MaxValue ^ (uint) num2));
    }
  }
}
