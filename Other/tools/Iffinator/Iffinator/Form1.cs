#define IFFINATOR

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using MaterialSkin;
using MaterialSkin.Controls;
using Files;
using Files.FAR1;
using Files.IFF;

namespace Iffinator
{
    public partial class Form1 : MaterialForm
    {
        private IEnumerable<string> m_HouseDataFARs;

        private List<FAR1Archive> m_Archives = new List<FAR1Archive>();
        private List<FAR1Entry> m_FloorEntries = new List<FAR1Entry>();
        private List<FAR1Entry> m_WallEntries = new List<FAR1Entry>();

        private Iff m_CurrentIff;

        public Form1()
        {
            InitializeComponent();

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepOrange800, Primary.Orange900, Primary.Orange500, Accent.LightBlue200, TextShade.WHITE);

            LstFloors.MouseClick += LstFloors_MouseClick;
            LstLanguages.MouseClick += LstLanguages_MouseClick;
            LstLanguages.Visible = false;

            LoadArchives();
        }

        /// <summary>
        /// Loads all the archives containing IFFs, and populates the listview with the names of the IFFs.
        /// </summary>
        private void LoadArchives()
        {
            m_HouseDataFARs = GetFileList("*.far", GlobalSettings.Default.StartupPath + "housedata");

            foreach (string Path in m_HouseDataFARs)
            {
                if (Path.Contains("floor"))
                {
                    FAR1Archive Archive = new FAR1Archive(Path);
                    if (Archive.ReadArchive(false))
                        m_FloorEntries.AddRange(Archive.GrabAllEntries());

                    m_Archives.Add(Archive);
                }
                if (Path.Contains("wall"))
                {
                    FAR1Archive Archive = new FAR1Archive(Path);
                    if (Archive.ReadArchive(false))
                        m_WallEntries.AddRange(Archive.GrabAllEntries());

                    m_Archives.Add(Archive);
                }
            }

            foreach (FAR1Entry Entry in m_FloorEntries)
                LstFloors.Items.Add(new ListViewItem(Entry.Filename));
        }

        /// <summary>
        /// The user clicked on an item in the list of floors.
        /// </summary>
        private void LstFloors_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (ListViewItem Item in LstFloors.Items)
            {
                //We only support selecting one item at a time.
                if (Item.Selected)
                {
                    byte[] Hash = FileUtilities.GenerateHash(Item.Text);
                    int ArchiveIndex = 0;

                    //This is NOT effective, but it's a tool so it doesn't have to be super fast...
                    foreach (FAR1Archive Archive in m_Archives)
                    {
                        if (Archive.ContainsEntry(Hash))
                        {
                            m_CurrentIff = new Iff(RndFloors.GraphicsDevice);
                            m_CurrentIff.Init(Archive.GrabEntry(Hash), false);
                            RndFloors.AddSprite(m_CurrentIff.SPR2s[0]);

                            //TODO: Add support for this.
                            //LblArchive.Text = "Floor is in: " + m_HouseDataFARs[ArchiveIndex];
                            PopulateLanguagesAndStrings();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The user clicked on an item in the list of languages and strings.
        /// </summary>
        private void LstLanguages_MouseClick(object sender, MouseEventArgs e)
        {
            //A floor or wall has only 1 string table.
            STR StringTable = m_CurrentIff.StringTables[0];

            foreach (ListViewItem Item in LstLanguages.Items)
            {
                //We only support selecting one item at a time.
                if (Item.Selected)
                {
                    TxtName.Visible = true;
                    TxtName.Clear();

                    TxtPrice.Visible = true;
                    TxtPrice.Clear();

                    TxtStrings.Visible = true;
                    TxtStrings.Clear();

                    LanguageCodes SelectedCode = (LanguageCodes)Enum.Parse(typeof(LanguageCodes), Item.Text);
                    TxtName.Text = StringTable.GetString(SelectedCode, 0);
                    TxtPrice.Text = StringTable.GetString(SelectedCode, 1);
                    TxtStrings.Text = StringTable.GetString(SelectedCode, 2);
                }
            }
        }

        /// <summary>
        /// Populates the lists of strings and languages.
        /// </summary>
        private void PopulateLanguagesAndStrings()
        {
            LstLanguages.Items.Clear();
            LstLanguages.Visible = true;
            TxtStrings.Visible = true;

            //A floor or wall has only 1 string table.
            STR StringTable = m_CurrentIff.StringTables[0];

            foreach (LanguageCodes LangCode in Enum.GetValues(typeof(LanguageCodes)))
            {
                TranslatedString[] TranslatedStrings = StringTable.GetStringList(LangCode).ToArray();

                if (TranslatedStrings.Length > 0)
                {
                    ListViewItem LstItem = new ListViewItem(new[] { LangCode.ToString(), TranslatedStrings.Length.ToString() });
                    LstLanguages.Items.Add(LstItem);
                }
            }
        }

        #region Helper methods

        /// <summary>
        /// Generates a list of strings containing all the files having the supplied extension in the specified path.
        /// </summary>
        /// <param name="fileSearchPattern">The extension to search for.</param>
        /// <param name="rootFolderPath">The path to search.</param>
        /// <returns>An IEnumerable instance containing a list of strings with the qualified path to the file
        /// corresponding to the <paramref name="fileSearchPattern"/> in the <paramref name="rootFolderPath"/></returns>
        private static IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }

#endregion
    }
}
