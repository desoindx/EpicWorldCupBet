using System;
using System.Linq;
using System.Windows.Forms;
using Datas.Entities;

namespace Manager
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            RefreshButton.Click += RefreshButtonOnClick;
            LoadButton.Click += LoadButtonOnClick;
        }

        private void LoadButtonOnClick(object sender, EventArgs eventArgs)
        {
            string fileName;
            using (var dialog = new OpenFileDialog())
            {
                dialog.DefaultExt = ".xlsx";
                dialog.ShowDialog(this);
                fileName = dialog.FileName;
            }

            Importer.Import(fileName);
        }

        private void RefreshButtonOnClick(object sender, EventArgs eventArgs)
        {
            RefreshCompetitionList();
        }

        private void RefreshCompetitionList()
        {
            CompetitionListBox.Items.Clear();
            using (var context = new Entities())
            {
                foreach (var competition in context.CompetitionGames.Select(x => x.CompetitionId).Distinct())
                {
                    int id = competition;
                    CompetitionListBox.Items.Add(context.Competitions.Where(x => x.Id == id).Select(x => x.Name));
                }
            }
        }
    }
}
