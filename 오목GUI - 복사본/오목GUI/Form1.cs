using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 오목GUI
{
    public partial class Form1 : Form
    {
        [DllImport("Dll_Gomok_logic.dll")]
        public static extern void InitGame();

        [DllImport("Dll_Gomok_logic.dll")]
        public static extern void InputPosition(int x, int y, int player);

        [DllImport("Dll_Gomok_logic.dll")]
        public static extern void GetNextMove(int player, ref int x, ref int y);

        [DllImport("Dll_Gomok_logic.dll")]
        public static extern bool CheckWin(int player);



        private bool start = false;
        private int ai = 0;
        private int player = 0;
        private const int BoardSize = 19;
        private const int CellSize = 30;
        private const int Margin = 30;
        private Brush blackBrush = new SolidBrush(Color.Black);
        private Brush whiteBrush = new SolidBrush(Color.White);
        private int[,] board = new int[BoardSize, BoardSize];

        private void InitializeGame()
        {
            InitGame(); // DLL 함수 호출
            Array.Clear(board, 0, board.Length); // board 배열 초기화
            panel1.Invalidate(); // 판넬을 다시 그려 바둑판 업데이트
        }

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(950, 600);
            panel1.Size = new Size((BoardSize - 1) * CellSize + Margin * 2, (BoardSize - 1) * CellSize + Margin * 2);
            panel1.Paint += Panel1_Paint;
            panel1.MouseClick += Panel1_MouseClick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGame();
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int i = 0; i < BoardSize - 1; i++)
            {
                for (int j = 0; j < BoardSize - 1; j++)
                {
                    int x = Margin + i * CellSize;
                    int y = Margin + j * CellSize;
                    g.DrawRectangle(Pens.Black, x, y, CellSize, CellSize);
                }
            }

            for (int i = 0; i < BoardSize + 1; i++)
            {
                for (int j = 0; j < BoardSize + 1; j++)
                {
                    int x = Margin + i * CellSize;
                    int y = Margin + j * CellSize;

                    if (i == 0 && j > 0 && j < BoardSize + 1)
                        g.DrawString($"{(char)('A' + j - 1)}", Font, Brushes.Black, x + CellSize / 2 - 35, y - CellSize / 2 - 18);
                    else if (j == 0 && i > 0 && i < BoardSize + 1)
                        g.DrawString($"{i}", Font, Brushes.Black, x - CellSize / 2 - 20, y + CellSize / 2 - 10 - CellSize);
                }
            }

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == 1)
                    {
                        g.FillEllipse(blackBrush, Margin + i * CellSize - CellSize / 2, Margin + j * CellSize - CellSize / 2, CellSize, CellSize);
                    }
                    else if (board[i, j] == 2)
                    {
                        g.FillEllipse(whiteBrush, Margin + i * CellSize - CellSize / 2, Margin + j * CellSize - CellSize / 2, CellSize, CellSize);
                    }
                }
            }

            for (int i = 3; i < BoardSize; i += 6)
            {
                for (int j = 3; j < BoardSize; j += 6)
                {
                    g.FillEllipse(Brushes.Black, Margin + i * CellSize - 2, Margin + j * CellSize - 2, 5, 5);
                }
            }
        }

        private void DrawStone(int x, int y, Brush brush)
        {
            Graphics g = panel1.CreateGraphics();
            g.FillEllipse(brush, Margin + x * CellSize - CellSize / 2, Margin + y * CellSize - CellSize / 2, CellSize, CellSize);
            g.Dispose();
        }

        private async Task UserMove(int x, int y)
        {
            if (board[x, y] == 0)
            {
                board[x, y] = player;
                DrawStone(x, y, (player == 1) ? blackBrush : whiteBrush);
                InputPosition(x, y, player);

                // 사용자의 착수를 textBox1에 출력
                textBox1.AppendText($"사용자 : ({x}, {y}) 착수 ...\r\n");

                if (CheckWin(player))
                {
                    MessageBox.Show((player == 1) ? "흑돌이 승리했습니다!" : "백돌이 승리했습니다!");
                    InitGame();
                    return;
                }
                await AIMove();
            }
        }

        private async Task AIVsAI()
        {
            int currentAI = 1; // 초기 AI 설정(흑돌)
            int otherAI = 2;   // 다른 AI 설정(백돌)
            while (!CheckWin(currentAI) && !CheckWin(otherAI))
            {
                await AIMove(currentAI);
                await Task.Delay(5); // AI 착수 간에 시간 지연을 좀 더 길게 설정

                // AI를 스위치
                int temp = currentAI;
                currentAI = otherAI;
                otherAI = temp;

                if (CheckWin(currentAI) || CheckWin(otherAI))
                {
                    MessageBox.Show((CheckWin(currentAI) ? "AI " + currentAI : "AI " + otherAI) + " wins!");
                    InitializeGame();
                    break;
                }
            }
        }

        private async Task AIMove(int aiPlayer)
        {
            int aiX = 0, aiY = 0;
            GetNextMove(aiPlayer, ref aiX, ref aiY);
            Console.WriteLine($"AI Move: x={aiX}, y={aiY}"); // DLL 호출 결과 확인
            if (aiX < 0 || aiX >= BoardSize || aiY < 0 || aiY >= BoardSize)
            {
                MessageBox.Show($"AI {aiPlayer} produced invalid move ({aiX}, {aiY})");
                return;
            }

            if (board[aiX, aiY] == 0)
            {
                board[aiX, aiY] = aiPlayer;
                DrawStone(aiX, aiY, (aiPlayer == 1) ? blackBrush : whiteBrush);
                InputPosition(aiX, aiY, aiPlayer);

                // AI의 착수를 textBox1에 출력
                textBox1.AppendText($"AI {aiPlayer} : ({aiX}, {aiY}) 착수 ...\r\n");

                if (CheckWin(aiPlayer))
                {
                    MessageBox.Show((aiPlayer == 1) ? "흑돌 AI가 승리했습니다!" : "백돌 AI가 승리했습니다!");
                    InitGame();
                    return;
                }
            }
        }

        private async Task AIMove()
        {
            int aiX = 0, aiY = 0;
            GetNextMove(ai, ref aiX, ref aiY);
            Console.WriteLine($"AI Move: x={aiX}, y={aiY}"); // DLL 호출 결과 확인
            if (aiX < 0 || aiX >= BoardSize || aiY < 0 || aiY >= BoardSize)
            {
                MessageBox.Show($"AI produced invalid move ({aiX}, {aiY})");
                return;
            }

            if (board[aiX, aiY] == 0)
            {
                board[aiX, aiY] = ai;
                DrawStone(aiX, aiY, (ai == 1) ? blackBrush : whiteBrush);
                InputPosition(aiX, aiY, ai);


                // AI의 착수를 textBox1에 출력
                textBox1.AppendText($"AI {ai} : ({aiX}, {aiY}) 착수 ...\r\n");
                if (CheckWin(ai))
                {
                    MessageBox.Show((ai == 1) ? "흑돌이 승리했습니다!" : "백돌이 승리했습니다!");
                    InitializeGame();
                    panel1.Invalidate();
                    return;
                }
            }
        }

        private async Task AITakesFirstMove()
        {
            int aiX = 9, aiY = 9; // AI의 첫 수를 (9,9)로 고정

            // 첫 수가 비어있는지 확인 (이미 돌이 있을 경우 다른 위치로 대체할 수 있도록 로직 추가 가능)
            if (board[aiX, aiY] == 0)
            {
                board[aiX, aiY] = ai; // AI의 돌을 바둑판에 저장
                DrawStone(aiX, aiY, (ai == 1) ? blackBrush : whiteBrush); // AI의 돌을 화면에 그림
                InputPosition(aiX, aiY, ai); // AI의 수를 로직에 전달

                // AI의 착수를 textBox1에 출력
                textBox1.AppendText($"AI {ai} : ({aiX}, {aiY}) 첫 착수 ...\r\n");
            }
            else
            {
                // 만약 (9, 9) 위치에 이미 돌이 있다면, 적절한 에러 처리나 다른 위치 찾기 등의 로직이 필요합니다.
                MessageBox.Show($"AI tried to move at occupied position ({aiX}, {aiY}).");
            }
        }

        private async void Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = (e.X - Margin + CellSize / 2) / CellSize;
            int y = (e.Y - Margin + CellSize / 2) / CellSize;

            if (start && player > 0)
            {
                if (x >= 0 && x < BoardSize && y >= 0 && y < BoardSize && board[x, y] == 0)
                {
                    await UserMove(x, y);
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!start)
            {
                start = true;
                textBox1.AppendText("게임이 시작되었습니다...\r\n"); // 게임 시작 상태 출력
                InitializeGame(); // 게임 초기화

                if (radioButton1.Checked)
                {
                    player = 1;
                    ai = 2;
                }
                else if (radioButton2.Checked)
                {
                    player = 2;
                    ai = 1;
                    await AITakesFirstMove(); // AI가 먼저 수를 두도록 함
                }
                else if (radioButton3.Checked)
                {
                    Task.Run(async () => await AIVsAI());
                }
            }
            else
            {
                start = false;
                textBox1.AppendText("게임이 중지되었습니다...\r\n"); // 게임 중지 상태 출력

                InitializeGame(); // 게임 초기화
            }
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}