using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GADE6112___Task3
{
    public partial class FrmMain : Form
    {
        GameEngine engine;
        Timer timer;
        GameState gameState = GameState.PAUSED;

        int mapWidth = 20;
        int mapHeight = 20;
        bool engineReset = true;

        public FrmMain()
        {
            InitializeComponent();
            engine = new GameEngine(mapWidth, mapHeight);
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

                numWidth.Enabled = true;
                numHeight.Enabled = true;
                engineReset = false;
            }
        }

        private void UpdateUI()
        {
            lblMap.Text = engine.MapDisplay;
            lblRound.Text = "Round: " + engine.Round;
            tbxUnitsInfo.Text = engine.GetUnitInfo();
            tbxBuildingsInfo.Text = engine.GetBuildingsInfo();
            lblUnits.Text = "Units (" + engine.NumUnitsAlive + "/" + engine.NumUnits + ")";
            lblBuildings.Text = "Buildings (" + engine.NumBuildingsAlive + "/" + engine.NumBuildings + ")";
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
                if (gameState == GameState.ENDED && !engineReset)
                {
                    engine.Reset(mapWidth, mapHeight);
                }
                timer.Start();
                gameState = GameState.RUNNING;
                btnStartPauseSim.Text = "PAUSE";

                numWidth.Enabled = false;
                numHeight.Enabled = false;
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
            numWidth.Value = engine.LoadedMapWidth;
            numHeight.Value = engine.LoadedMapHeight;
        }

        private void NumWidth_ValueChanged(object sender, EventArgs e)
        {
            mapWidth = (int) numWidth.Value;
            engine.Reset(mapWidth, mapHeight);
            engineReset = true;
            UpdateUI();
        }

        private void NumHeight_ValueChanged(object sender, EventArgs e)
        {
            mapHeight = (int) numHeight.Value;
            engine.Reset(mapWidth, mapHeight);
            engineReset = true;
            UpdateUI();
        }
    }

    public enum GameState
    {
        RUNNING,
        PAUSED,
        ENDED
    }
}
