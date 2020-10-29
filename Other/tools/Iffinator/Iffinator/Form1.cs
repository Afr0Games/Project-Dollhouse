#define IFFINATOR

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
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

        //Zoom level at which to draw walls and floors.
        private int m_ZoomLevel = 0;

        private Iff m_CurrentIff;
        private LanguageCodes m_CurrentLanguage;

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
            LstFloorLanguages.MouseClick += LstFloorLanguages_MouseClick;
            LstFloorLanguages.Visible = false;

            LstWalls.MouseClick += LstWalls_MouseClick;
            LstWallLanguages.MouseClick += LstWallLanguages_MouseClick;
            LstWallLanguages.Visible = false;

            TxtFloorStrings.TextChanged += TxtFloorStrings_TextChanged;
            TxtFloorName.TextChanged += TxtFloorName_TextChanged;
            BtnUpdateText.Click += BtnUpdateText_Click;

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

            foreach (FAR1Entry Entry in m_WallEntries)
                LstWalls.Items.Add(new ListViewItem(Entry.Filename));
        }

        private void LstWalls_MouseClick(object sender, MouseEventArgs e)
        {
            BtnZoomInWall.Visible = true;
            BtnZoomOutWall.Visible = true;

            HideWallTextInterface();

            foreach (ListViewItem Item in LstWalls.Items)
            {
                //We only support selecting one item at a time.
                if (Item.Selected)
                {
                    byte[] Hash = FileUtilities.GenerateHash(Item.Text);
                    int ArchiveIndex = 0;

                    //This is NOT effective, but it's a tool so it doesn't have to be super fast...
                    foreach (FAR1Archive Archive in m_Archives)
                    {
                        ArchiveIndex++;

                        if (Archive.ContainsEntry(Hash))
                        {
                            m_CurrentIff = new Iff(RndWalls.GraphicsDevice);
                            m_CurrentIff.Init(Archive.GrabEntry(Hash), false);
                            RndWalls.AddSprite(m_CurrentIff.SPRs[0]);

                            DirectoryInfo DirInfo = new DirectoryInfo(Archive.Path);
                            LblArchive.Text = "Wall is in: " + DirInfo.Parent + "\\" + Path.GetFileName(Archive.Path);
                            PopulateLanguagesAndStrings();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The user clicked on an item in the list of floors.
        /// </summary>
        private void LstFloors_MouseClick(object sender, MouseEventArgs e)
        {
            BtnZoomIn.Visible = true;
            BtnZoomOut.Visible = true;

            HideFloorTextInterface();

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
                        ArchiveIndex++;

                        if (Archive.ContainsEntry(Hash))
                        {
                            m_CurrentIff = new Iff(RndFloors.GraphicsDevice);
                            m_CurrentIff.Init(Archive.GrabEntry(Hash), false);
                            RndFloors.AddSprite(m_CurrentIff.SPR2s[0]);

                            DirectoryInfo DirInfo = new DirectoryInfo(Archive.Path);
                            LblArchive.Text = "Floor is in: " + DirInfo.Parent + "\\" + Path.GetFileName(Archive.Path);
                            PopulateLanguagesAndStrings();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The user changed an object's description.
        /// </summary>
        private void TxtFloorStrings_TextChanged(object sender, EventArgs e)
        {
            BtnUpdateText.Visible = true;
        }

        /// <summary>
        /// The user changed an object's name.
        /// </summary>
        private void TxtFloorName_TextChanged(object sender, EventArgs e)
        {
            BtnUpdateText.Visible = true;
        }

        /// <summary>
        /// The user clicked the button to update the current text.
        /// </summary>
        private void BtnUpdateText_Click(object sender, EventArgs e)
        {
            STR Strings = m_CurrentIff.StringTables[0];

            TranslatedString TranslatedStr = 
                new TranslatedString() { LangCode = m_CurrentLanguage, TranslatedStr = TxtFloorStrings.Text };
            Strings.AddString(TranslatedStr, ObjectStringIndices.Description);

            m_CurrentIff.AddSTR(Strings.ID, Strings);

            BtnUpdateText.Visible = false;
        }

        /// <summary>
        /// The user clicked on an item in the list of languages and strings.
        /// </summary>
        private void LstFloorLanguages_MouseClick(object sender, MouseEventArgs e)
        {
            LblFloorPrice.Visible = true;
            LblFloorName.Visible = true;

            //A floor or wall has only 1 string table.
            STR StringTable = m_CurrentIff.StringTables[0];

            foreach (ListViewItem Item in LstFloorLanguages.Items)
            {
                //We only support selecting one item at a time.
                if (Item.Selected)
                {
                    TxtFloorName.Visible = true;
                    TxtFloorName.Clear();

                    TxtFloorPrice.Visible = true;
                    TxtFloorPrice.Clear();

                    TxtFloorStrings.Visible = true;
                    TxtFloorStrings.Clear();

                    LanguageCodes SelectedCode = (LanguageCodes)Enum.Parse(typeof(LanguageCodes), Item.Text);
                    m_CurrentLanguage = SelectedCode;
                    TxtFloorName.Text = StringTable.GetString(SelectedCode, ObjectStringIndices.Name);
                    TxtFloorPrice.Text = StringTable.GetString(SelectedCode, ObjectStringIndices.Price);
                    TxtFloorStrings.Text = StringTable.GetString(SelectedCode, ObjectStringIndices.Description);
                }
            }
        }

        /// <summary>
        /// The user clicked on an item in the list of languages and strings.
        /// </summary>
        private void LstWallLanguages_MouseClick(object sender, MouseEventArgs e)
        {
            LblWallPrice.Visible = true;
            LblWallName.Visible = true;

            //A floor or wall has only 1 string table.
            STR StringTable = m_CurrentIff.StringTables[0];

            foreach (ListViewItem Item in LstFloorLanguages.Items)
            {
                //We only support selecting one item at a time.
                if (Item.Selected)
                {
                    TxtWallName.Visible = true;
                    TxtWallName.Clear();

                    TxtWallPrice.Visible = true;
                    TxtWallPrice.Clear();

                    TxtWallStrings.Visible = true;
                    TxtWallStrings.Clear();

                    LanguageCodes SelectedCode = (LanguageCodes)Enum.Parse(typeof(LanguageCodes), Item.Text);
                    m_CurrentLanguage = SelectedCode;
                    TxtWallName.Text = StringTable.GetString(SelectedCode, ObjectStringIndices.Name);
                    TxtWallPrice.Text = StringTable.GetString(SelectedCode, ObjectStringIndices.Price);
                    TxtWallStrings.Text = StringTable.GetString(SelectedCode, ObjectStringIndices.Description);
                }
            }
        }

        /// <summary>
        /// Populates the lists of strings and languages.
        /// </summary>
        private void PopulateLanguagesAndStrings()
        {
            LstFloorLanguages.Items.Clear();
            LstFloorLanguages.Visible = true;

            //A floor or wall has only 1 string table.
            STR StringTable = m_CurrentIff.StringTables[0];

            foreach (LanguageCodes LangCode in Enum.GetValues(typeof(LanguageCodes)))
            {
                TranslatedString[] TranslatedStrings = StringTable.GetStringList(LangCode).ToArray();

                if (TranslatedStrings.Length > 0)
                {
                    ListViewItem LstItem = new ListViewItem(new[] { LangCode.ToString(), TranslatedStrings.Length.ToString() });
                    LstFloorLanguages.Items.Add(LstItem);
                }
            }

            LstWallLanguages.Items.Clear();
            LstWallLanguages.Visible = true;

            //A floor or wall has only 1 string table.
            StringTable = m_CurrentIff.StringTables[0];

            foreach (LanguageCodes LangCode in Enum.GetValues(typeof(LanguageCodes)))
            {
                TranslatedString[] TranslatedStrings = StringTable.GetStringList(LangCode).ToArray();

                if (TranslatedStrings.Length > 0)
                {
                    ListViewItem LstItem = new ListViewItem(new[] { LangCode.ToString(), TranslatedStrings.Length.ToString() });
                    LstWallLanguages.Items.Add(LstItem);
                }
            }
        }

        /// <summary>
        /// User wanted to zoom in a floor.
        /// </summary>
        private void BtnZoomIn_Click(object sender, EventArgs e)
        {
            if (m_ZoomLevel < 2)
                m_ZoomLevel++;

            RndFloors.AddSprite(m_CurrentIff.SPR2s[m_ZoomLevel]);
        }

        /// <summary>
        /// User wanted to zoom out a floor.
        /// </summary>
        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            if (m_ZoomLevel > 0)
                m_ZoomLevel--;

            RndFloors.AddSprite(m_CurrentIff.SPR2s[m_ZoomLevel]);
        }

        /// <summary>
        /// User wanted to zoom in a wall.
        /// </summary>
        private void BtnZoomInWall_Click(object sender, EventArgs e)
        {
            if (m_ZoomLevel < 2)
                m_ZoomLevel++;

            RndWalls.AddSprite(m_CurrentIff.SPRs[m_ZoomLevel]);
        }

        /// <summary>
        /// User wanted to zoom out a wall.
        /// </summary>
        private void BtnZoomOutWall_Click(object sender, EventArgs e)
        {
            if (m_ZoomLevel > 0)
                m_ZoomLevel--;

            RndWalls.AddSprite(m_CurrentIff.SPRs[m_ZoomLevel]);
        }

        #region Helper methods

        /// <summary>
        /// Hides the text interface.
        /// </summary>
        public void HideFloorTextInterface()
        {
            if (BtnUpdateText.Visible)
                BtnUpdateText.Visible = false;
            if (LblFloorName.Visible)
                LblFloorName.Visible = false;
            if (LblFloorPrice.Visible)
                LblFloorPrice.Visible = false;
            if (TxtFloorName.Visible)
                TxtFloorName.Visible = false;
            if (TxtFloorPrice.Visible)
                TxtFloorPrice.Visible = false;
            if (TxtFloorStrings.Visible)
                TxtFloorStrings.Visible = false;
        }

        /// <summary>
        /// Hides the text interface.
        /// </summary>
        public void HideWallTextInterface()
        {
            if (BtnUpdateText.Visible)
                BtnUpdateText.Visible = false;
            if (LblWallName.Visible)
                LblWallName.Visible = false;
            if (LblWallPrice.Visible)
                LblWallPrice.Visible = false;
            if (TxtWallName.Visible)
                TxtWallName.Visible = false;
            if (TxtWallPrice.Visible)
                TxtWallPrice.Visible = false;
            if (TxtWallStrings.Visible)
                TxtWallStrings.Visible = false;
        }

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
