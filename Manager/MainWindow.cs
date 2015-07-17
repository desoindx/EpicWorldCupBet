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
            CompetitionListBox.SelectedIndexChanged += CompetitionListBoxOnSelectedIndexChanged;
            PriceButton.Click += PriceButtonOnClick;
        }

        private void PriceButtonOnClick(object sender, EventArgs eventArgs)
        {
            Pricer.Pricer.Price(CompetitionListBox.SelectedItem.ToString());
        }

        private void CompetitionListBoxOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            teamGridView.Rows.Clear();
            using (var context = new Entities())
            {
                var competition = context.Competitions.First(x => CompetitionListBox.SelectedItem.ToString() == x.Name);
                foreach (var team in context.Teams.Where(x => x.IdCompetition == competition.Id && x.RealTeam.HasValue && x.RealTeam.Value))
                {
                    var newRowIndex = teamGridView.Rows.Add();
                    var newRow = teamGridView.Rows[newRowIndex];
                    newRow.Cells[0].Value = team.Name;
                    newRow.Cells[1].Value = 1;
                    newRow.Cells[2].Value = string.Empty;
                } 
            }
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

            if (!string.IsNullOrEmpty(fileName))
            {
                Importer.Import(fileName);
                RefreshCompetitionList();
            }
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
                    CompetitionListBox.Items.Add(context.Competitions.Where(x => x.Id == id).Select(x => x.Name).First());
                }
            }
        }
    }
}
