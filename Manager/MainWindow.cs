using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Datas.Entities;

namespace Manager
{
    public partial class MainWindow : Form
    {
        private static string _columnId = "Id";
        private static string _columnTeam = "Team";
        private static string _columnValue = "Value";
        private static string _columnStrength = "Strength";

        public MainWindow()
        {
            InitializeComponent();

            RefreshButton.Click += RefreshButtonOnClick;
            LoadButton.Click += LoadButtonOnClick;
            CompetitionListBox.SelectedIndexChanged += CompetitionListBoxOnSelectedIndexChanged;
            PriceButton.Click += PriceButtonOnClick;
            StrenghtRadioButton.CheckedChanged += StrenghtRadioButtonOnCheckedChanged;
        }

        private void StrenghtRadioButtonOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            Strength.Visible = StrenghtRadioButton.Checked;
        }

        private void PriceButtonOnClick(object sender, EventArgs eventArgs)
        {
            var strenghts = new Dictionary<Team, double>();
            foreach (DataGridViewRow row in teamGridView.Rows)
            {
                strenghts[(Team)row.Cells[_columnId].Value] =
                    double.Parse(row.Cells[_columnStrength].Value.ToString());
            }

            Dictionary<Team, double> results = null;
            if (StrenghtRadioButton.Checked)
            {
                results = Pricer.Pricer.Price(CompetitionListBox.SelectedItem.ToString(), strenghts);
            }
            else
            {
                //            Pricer.Pricer.Price(CompetitionListBox.SelectedItem.ToString(), );
            }

            foreach (DataGridViewRow row in teamGridView.Rows)
            {
                row.Cells[_columnValue].Value = results[(Team)row.Cells[_columnId].Value];
            }
        }

        private void CompetitionListBoxOnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            var selectedItem = CompetitionListBox.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            teamGridView.Rows.Clear();
            using (var context = new Entities())
            {
                var competition = context.Competitions.First(x => selectedItem.ToString() == x.Name);
                foreach (var team in context.Teams.Where(x => x.IdCompetition == competition.Id && x.RealTeam.HasValue && x.RealTeam.Value))
                {
                    var newRowIndex = teamGridView.Rows.Add();
                    var newRow = teamGridView.Rows[newRowIndex];
                    newRow.Cells[_columnId].Value = team;
                    newRow.Cells[_columnTeam].Value = team.Name;
                    newRow.Cells[_columnStrength].Value = 1;
                    newRow.Cells[_columnValue].Value = string.Empty;
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
