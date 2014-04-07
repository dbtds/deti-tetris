﻿using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Tetris.TetrisModule;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private Rectangle[,] tetrisTable;
        private Rectangle[,] nextBlockTable;
        private TetrisM tetris = TetrisM.getInstance();

        public MainWindow()
        {
            InitializeComponent();
            createGrid();

            /* Block paint means that needs to be painted */
            tetris.blockPaint += new TetrisM.BlockRepaintEventHandler(blockPaintEvent);
            /* Block moved means that needs to be cleared */
            tetris.blockMoved += new TetrisM.BlockMovedEventHandler(blockMovedEvent);
            /* Table change means that needs a total repaint */
            tetris.tableChanged += new TetrisM.TableChangedEventHandler(tableChanged);
            /* Event is called when a row gets completed */
            tetris.rowComplete += new TetrisM.RowCompleteEventHandler(rowCompleteEvent);
            /* Refresh clock */
            tetris.clockTick += new TetrisM.ClockTickEventHandler(clockTick);
            /* Event is called when game ends (time out or game over) */
            tetris.gameEnd += new TetrisM.GameEndEventHandler(gameEnded);
            /* Event is called when next block changes */
            tetris.nextBlockChanged += new TetrisM.NextBlockChangedEventHandler(nextBlockChanged);

            paintGrid();
            tetris.startGame();
        }
        public void clockTick(TimeSpan cTime)
        {
            TimeLeft.Content = String.Format("{0:m\\:ss}", cTime);
        }
        public void blockMovedEvent(Block currentBlock)
        {
            Point2D [] list = currentBlock.getList();
            Point2D pos = currentBlock.getPosition();
            for (int i = 0; i < list.Length; i++)
            {
                if (TetrisM.validPos(pos.X + list[i].X, pos.Y + list[i].Y))
                    tetrisTable[pos.X + list[i].X, pos.Y + list[i].Y].Fill = 
                        new SolidColorBrush(TetrisM.emptyBlock);
            }
        }
        public void blockPaintEvent(Block currentBlock)
        {
            Point2D[] list = currentBlock.getList();
            Point2D pos = currentBlock.getPosition();
            for (int i = 0; i < list.Length; i++)
            {
                if (TetrisM.validPos(pos.X + list[i].X, pos.Y + list[i].Y))
                    tetrisTable[pos.X + list[i].X, pos.Y + list[i].Y].Fill = 
                        new SolidColorBrush(currentBlock.getColor());
            }
        }
        private void createGrid()
        {
            MainGrid.Background = new SolidColorBrush(Colors.Black);

            tetrisTable = new Rectangle[TetrisM.NC, TetrisM.NR];
            TetrisGrid.Columns = TetrisM.NC;
            TetrisGrid.Rows = TetrisM.NR;

            TetrisGridBack.Columns = TetrisGrid.Columns;
            TetrisGridBack.Rows = TetrisGrid.Rows;

            for (int i = 0; i < TetrisM.NR; i++)
            {
                for (int j = 0; j < TetrisM.NC; j++)
                {
                    tetrisTable[j, i] = new Rectangle();
                    tetrisTable[j, i].Margin = new Thickness(0.5);
                    TetrisGrid.Children.Add(tetrisTable[j, i]);

                    TetrisGridBack.Children.Add(new Rectangle
                    {
                        Fill = new SolidColorBrush(Colors.WhiteSmoke),
                        Margin = new Thickness(1)
                    });
                }
            }

            GridNext.Background = new SolidColorBrush(Colors.Black);
            nextBlockTable = new Rectangle[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nextBlockTable[j, i] = new Rectangle();
                    nextBlockTable[j, i].Margin = new Thickness(0.5);
                    NextBlockGrid.Children.Add(nextBlockTable[j, i]);

                    NextBlockBackground.Children.Add(new Rectangle
                    {
                        Fill = new SolidColorBrush(Colors.WhiteSmoke),
                        Margin = new Thickness(1)
                    });
                }
            }
        }
        private void paintGrid()
        {
            Color[,] table = tetris.getTable();
            for (int i = 0; i < TetrisM.NC; i++)
            {
                for (int j = 0; j < TetrisM.NR; j++)
                {
                    tetrisTable[i, j].Fill = new SolidColorBrush(table[i, j]);
                }
            }
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    tetris.moveCurrentBlock(TetrisM.Actions.DOWN); break;
                case Key.Left:
                    tetris.moveCurrentBlock(TetrisM.Actions.LEFT); break;
                case Key.Right:
                    tetris.moveCurrentBlock(TetrisM.Actions.RIGHT); break;
                case Key.Up:
                    tetris.moveCurrentBlock(TetrisM.Actions.ROTATE); break;
                default:
                    return;
            }
        }
        public void rowCompleteEvent(int row) {
            paintGrid();
        }
        public void tableChanged() {
            paintGrid();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void nextBlockChanged(Block nextBlock)
        {
            Point2D[] list = nextBlock.getList();
            Color c = nextBlock.getColor();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nextBlockTable[i, j].Fill = new SolidColorBrush(Colors.Transparent);
                }
            }
            for (int i = 0; i < list.Length; i++)
            {
                nextBlockTable[list[i].X + 1, list[i].Y + 2].Fill = new SolidColorBrush(c);
            }
        }

        public void gameEnded()
        {
            MessageBox.Show("~ Game Over ~");
            this.Close();
        }
    }
}
