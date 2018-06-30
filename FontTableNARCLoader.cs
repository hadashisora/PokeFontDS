// FontTableNARCLoader.cs

using NARCFileReadingDLL;
using System.IO;
using System.Windows.Forms;

namespace PokeFontDS
{
    #region FontTableNARCLoaderBase Abstract Class

    public abstract class FontTableNARCLoaderBase
    {
        #region Data Members

        private NARCFile m_nfNARCFile;

        #endregion

        #region Ctor Dtor

        public FontTableNARCLoaderBase()
        {
            FileStream fsNARC = File.OpenRead(Application.StartupPath + @"\temp\root\" + FileName);
            m_nfNARCFile = new NARCFile(new BinaryReader(fsNARC));
            fsNARC.Close();
        }

        #endregion

        #region Properties

        public abstract string FileName { get; }
        public NARCFile NARCFile
        {
            get
            {
                return (m_nfNARCFile);
            }
        }

        #endregion
    }

    #endregion

    #region SimpleFontTableNARCLoader Abstract Class

    public abstract class SimpleFontTableNARCLoader : FontTableNARCLoaderBase
    {
        #region Ctor Dtor

        public SimpleFontTableNARCLoader()
        {
            for (int nIndex = 0; nIndex < NARCFile.FilesCount; ++nIndex)
            {
                if (NARCFile.Files[nIndex].Size == 33101)
                {
                    NARCFile.Files[nIndex] = new SimpleFontTable(new BinaryReader(new ByteArrayStream(NARCFile.Files[nIndex].File)));
                }
            }
        }

        #endregion
    }

    #endregion

    #region NFTRFontTableNARCLoader Abstract Class

    public abstract class NFTRFontTableNARCLoader : FontTableNARCLoaderBase
    {
        #region Ctor Dtor

        public NFTRFontTableNARCLoader()
            : base()
        {
        }

        #endregion
    }

    #endregion

    #region POKEMON_DFontTableNARCLoader Class

    public class POKEMON_DFontTableNARCLoader : SimpleFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"graphic\font.narc";

        #endregion

        #region Ctor Dtor

        public POKEMON_DFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_PFontTableNARCLoader Class

    public class POKEMON_PFontTableNARCLoader : SimpleFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"graphic\font.narc";

        #endregion

        #region Ctor Dtor

        public POKEMON_PFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_PLFontTableNARCLoader Class

    public class POKEMON_PLFontTableNARCLoader : SimpleFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"graphic\pl_font.narc";

        #endregion

        #region Ctor Dtor

        public POKEMON_PLFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_HGFontTableNARCLoader Class

    public class POKEMON_HGFontTableNARCLoader : SimpleFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"a\0\1\6";

        #endregion

        #region Ctor Dtor

        public POKEMON_HGFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_SSFontTableNARCLoader Class

    public class POKEMON_SSFontTableNARCLoader : SimpleFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"a\0\1\6";

        #endregion

        #region Ctor Dtor

        public POKEMON_SSFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_BFontTableNARCLoader Class

    public class POKEMON_BFontTableNARCLoader : NFTRFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"a\0\2\3";

        #endregion

        #region Ctor Dtor

        public POKEMON_BFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_WFontTableNARCLoader Class

    public class POKEMON_WFontTableNARCLoader : NFTRFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"a\0\2\3";

        #endregion

        #region Ctor Dtor

        public POKEMON_WFontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_B2FontTableNARCLoader Class

    public class POKEMON_B2FontTableNARCLoader : NFTRFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"a\0\2\3";

        #endregion

        #region Ctor Dtor

        public POKEMON_B2FontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion

    #region POKEMON_W2FontTableNARCLoader Class

    public class POKEMON_W2FontTableNARCLoader : NFTRFontTableNARCLoader
    {
        #region Const Members

        private const string FILE_NAME = @"a\0\2\3";

        #endregion

        #region Ctor Dtor

        public POKEMON_W2FontTableNARCLoader()
            : base()
        {
        }

        #endregion

        #region Properties

        public override string FileName
        {
            get
            {
                return (FILE_NAME);
            }
        }

        #endregion
    }

    #endregion
}