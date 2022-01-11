using System;
using System.Drawing;
using System.Windows.Forms;

namespace KatyProject
{

    public partial class Form1 : Form
    {

        public Form1()
        {

            InitializeComponent();
            frame = new Bitmap(ClientSize.Width, ClientSize.Height);// Объявляем изображение для подготовки кадра
            g = CreateGraphics();  //Основнвя графика
            gF = Graphics.FromImage(frame);  // Графика кадра
            board = new Crustal[xCol, yCol]; // Игровое поле из массива кристалов
            rand = new Random();
            selPos = new Point(-5, -5);
        }

        Point selPos = new Point();
        int xCol = 11, yCol = 9; //  размеры поля

        bool allStand = false; //Все стоят на своих местах
        Image frame;// Изображение для подготовки кадра
        Graphics g, gF; // Объекты графики
        Crustal[,] board; // Игровое поле из массива кристалов
        Random rand;
        void Spawn() // Проверка всех ячеек для спавна
        {
            for (int j = 0; j < yCol; j++)
                for (int i = 0; i < xCol; i++)  // Перебор всех элементов
                {
                    if (j == 0) // Если это верхний ряд
                    {
                        if (board[i, j] == null) // и данный элемент отсутствует
                        {
                            board[i, j] = new Crustal(i, rand.Next(5)); // мы создаем новый
                        }
                    }
                    else // в остальных случаях
                    {
                        if (board[i, j] == null) // если данный элемент отсутствует
                        {
                            board[i, j] = board[i, j - 1]; // привязываем тот что повыше к этой координате
                            board[i, j - 1] = null;
                            board[i, j].newPos(i,j);
                        }
                    }
                }
        }

        void Render() // просчитываем перемещение каждой ячейки и отрисовываем их
        {
            allStand = true; // допустим все стоят на своих местах
            for (int j = 0; j < yCol; j++)
                for (int i = 0; i < xCol; i++)  // Перебор всех элементов
                {
                    if (board[i, j] != null)
                    {
                        allStand = board[i, j].Check() ? allStand : false;
                        board[i, j].Render(gF);
                        if (selPos == new Point(i, j))
                            gF.DrawRectangle(new Pen(Color.Red), new Rectangle(i*60,j*60,60,60));
                    }
                }
            
            g.DrawImage(frame, 0, 0);
        }

        private void timer1_Tick(object sender, EventArgs e) // Каждый тик таймера
        {
            gF.Clear(Color.Black); // Отчищаем
            for (int i = 1; i < xCol; i++)
                gF.DrawLine(new Pen(Color.Aqua), 60 * i, 0, 60 * i, Height);
            for (int i = 1; i < yCol; i++)
                gF.DrawLine(new Pen(Color.Aqua), 0, 60 * i, Width, 60 * i); //Рисуем линии
            Spawn(); //вызываем спавн (выше)
            Render();


        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (allStand)
            {
                int xP = e.X / 60;
                int yP = e.Y / 60;
                if (selPos == new Point (-5,-5)) selPos = new Point(e.X / 60, e.Y / 60);
                else if (selPos == new Point(xP, yP)) selPos = new Point (-5,-5);
                else if (board[selPos.X, selPos.Y].Dist(board[xP, yP].cur) <= 60)
                {
                    (board[xP, yP], board[selPos.X, selPos.Y]) = (board[selPos.X, selPos.Y], board[xP, yP]);
                    board[xP, yP].newPos(xP, yP);
                    board[selPos.X, selPos.Y].newPos(selPos.X, selPos.Y);
                }
            }
        }
    }
    class Crustal
    {
        int spd = 4; // шаг Кристала на кадр

        int num;
        public bool stand;
        public Point pos, cur; // позиции (по нынешняя и целевая)

        Color myColor; 

        public Crustal(int xPos, int num)
        {
            pos = new Point(xPos * 60 + 30, 0);
            cur = new Point(xPos * 60 + 30, 30);
            this.num = num;
            myColor = num switch
            {
                0 => Color.White,
                1 => Color.Red,
                2 => Color.Blue,
                3 => Color.Green,
                4 => Color.Yellow
            };
        }
        public bool Check()
        {
            if (pos == cur)
                stand = true;
            else
            {
                int rstX = cur.X - pos.X; 
                int rstY = cur.Y - pos.Y; 
                if (Math.Abs(rstX) >= spd && rstX != 0) pos.X += spd * rstX / Math.Abs(rstX); else pos.X = cur.X;
                if (Math.Abs(rstY) >= spd && rstY != 0) pos.Y += spd * rstY / Math.Abs(rstY); else pos.Y = cur.Y;
            }
            return stand;
        }
        public double Dist (Point p) => Math.Sqrt(Math.Pow(p.X - pos.X,2)+Math.Pow(p.Y - pos.Y,2));
        public void newPos(int X, int Y) => cur = new Point(X * 60 + 30, Y * 60 + 30);
        public void Render(Graphics frameGr)
        {
            frameGr.DrawEllipse(new Pen(myColor), pos.X - 30, pos.Y - 30, 60, 60);
        }
    }
}
