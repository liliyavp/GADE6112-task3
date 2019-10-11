using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GADE6112___Task2
{
    public partial class FrmMain : Form
    {
        GameEngine engine;
        Timer timer;
        GameState gameState = GameState.PAUSED;

        public FrmMain()
        {
            InitializeComponent();
            engine = new GameEngine();
            UpdateUI();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            engine.GameLoop();
            UpdateUI();

            if (engine.IsGameOver)
            {
                timer.Stop();
                UpdateUI();
                lblMap.Text = engine.WinningFaction + " WON!\n" + lblMap.Text;
                gameState = GameState.ENDED;
                btnStartPauseSim.Text = "RESTART";
            }
        }

        private void UpdateUI()
        {
            lblMap.Text = engine.MapDisplay;
            lblRound.Text = "Round: " + engine.Round;
            tbxUnitsInfo.Text = engine.GetUnitInfo();
            tbxBuildingsInfo.Text = engine.GetBuildingsInfo();
            lblUnits.Text = "Units (" + engine.NumUnits + ")";
            lblBuildings.Text = "Buildings (" + engine.NumBuildings + ")";
        }

        private void BtnStartPauseSim_Click(object sender, EventArgs e)
        {
            if (gameState == GameState.RUNNING)
            {
                timer.Stop();
                gameState = GameState.PAUSED;
                btnStartPauseSim.Text = "START";
            }
            else{
                if (gameState == GameState.ENDED)
                {
                    engine.Reset();
                }
                timer.Start();
                gameState = GameState.RUNNING;
                btnStartPauseSim.Text = "PAUSE";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            engine.SaveGame();
            lblMap.Text = "GAME SAVED\n" + lblMap.Text;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            engine.LoadGame();
            lblMap.Text = "GAME LOADED\n" + engine.MapDisplay;
        }
    }

    public enum GameState
    {
        RUNNING,
        PAUSED,
        ENDED
    }
}
