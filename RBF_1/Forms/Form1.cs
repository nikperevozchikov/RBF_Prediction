using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace RBF_1
{
    public partial class Form1 : Form
    {
        private static int min = 30;
        private static int max = 90;

        public Form1()
        {
            InitializeComponent();
        }

        private static int countRow = 100;
        private static int numInput = 2;
        private static int numHidden; // = 3;
        private static int numOutput = 1;
        private static int countIter = 1000;
        private string filename = "iris_train.txt";

        private static Matrix weight; //= new Matrix(numHidden, numOutput);
        private static Matrix centroids; // = new Matrix(numHidden, numInput);        //Центры
        private static Matrix x;
        private static double[] y = new double[numOutput];
        private static Random rnd = new Random();
        private static double er; //= 0; // new double[countRow];      //105
        private static double errTest;
        double[,] D = new double[countRow, numOutput];
        private static double n;
        private static NeuralNetwork rn;
        public static int temp; // = 0;

        public void SetC()
        {
            for (int i = 0; i < centroids.Row; i++)
            {
                for (int j = 0; j < centroids.Column; j++)
                {
                    centroids.Set(i, j, rnd.NextDouble());
                }
            }
        }

        private void button_Sample_Click(object sender, EventArgs e)
        {
            label_err.Text = "Learning error: ";

            label_Test.Text = "Testing error: ";

            //Коэффициент обучение сети
            n = Convert.ToDouble(textBox_n.Text);

            //Объем обучающей выборки
            countRow = Convert.ToInt32(textBox_X.Text);
            numInput = Convert.ToInt32(textBox2.Text);
            numHidden = Convert.ToInt32(textBox_C.Text);
            countIter = Convert.ToInt32(textBox1.Text);
            //чтение входных данных
            if (checkBox1.Checked == true)
                CreateValues("dollar.txt");
            else
                CreateValues();
            double[] d = new double[D.GetLength(0)];
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = D[i, 0];
            }
           
            //Инициализация центроид
            centroids = ReaderWriter.ReadMatrixC(countRow, numInput, numHidden, d, x);

            rn = new NeuralNetwork(countRow, numInput, numHidden, numOutput, x, centroids, n, temp, er);
            rn.countIter = countIter;
            //Инициализация весов
            rn.SetWeight();

            double[] res = new double[countRow];
            // Обучение сети
            rn.TrainNetwork(x, centroids, countRow, numInput, numHidden, numOutput, D, out res);

            if (chart2.Series.Count != 2)
                chart2.Series.Add("");

            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();

            //    chart2.ChartAreas[0].AxisX.Minimum = 30;
            //    chart2.ChartAreas[0].AxisX.Maximum = 50;
            // chart2.ChartAreas[0].AxisX.Interval = 1;
            if (checkBox1.Checked == true)
            {
                chart2.ChartAreas[0].AxisY.Minimum = 30;
                chart2.ChartAreas[0].AxisY.Maximum = 100;
            }
            else
            {
                chart2.ChartAreas[0].AxisY.Minimum = -100;
                chart2.ChartAreas[0].AxisY.Maximum = 1000;
            }



            //  chart2.ChartAreas[0].AxisY.Interval = 10;
            chart2.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            chart2.Series[0].Name = "Received value";
            chart2.Series[1].Name = "Original value";
            chart2.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            chart2.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            for (int i = 1; i < countRow - 1; i++)
            {
                chart2.Series[0].Points.AddXY(i, res[i + 1]);
                chart2.Series[1].Points.AddXY(i, d[i]);
            }

            er = rn.errTrain;
            label_err.Text = "Learning error: " + er;

            temp = rn.temp;


            //  button_Sample.Enabled = false;
            button_test.Enabled = true;
        }

        void RecountD(double[] d)
        {
            int numClass = 0;
            for (int i = 0; i < D.GetLength(0); i++)
            {
                numClass = Convert.ToInt32(d[i]);
                D[i, numClass] = 1;
            }
        }

        void CreateValues()
        {
            x = new Matrix(countRow, numInput);
            D = new double[countRow, 1];
            for (int i = 0; i < countRow; i++)
            {
                for (int j = i; j < numInput + i; j++)
                {
                    x.Set(i, j - i, gV(j));
                }

                D[i, 0] = gV(i + numInput + 1);
            }
        }

        void CreateValues(string filename)
        {
            double[] tmp = ReaderWriter.ReadValues(filename, countRow + numInput + 1);
            x = new Matrix(countRow, numInput);
            D = new double[countRow, 1];
            for (int i = 0; i < countRow; i++)
            {
                for (int j = i; j < numInput + i; j++)
                {
                    x.Set(i, j - i, tmp[j]);
                }

                D[i, 0] = tmp[i + numInput + 1];
            }
        }
        
        double gV(int t)
        {
            return  Math.Pow(t,2);
        }


        private void button_test_Click(object sender, EventArgs e)
        {
            countRow = (int) (countRow * 1.05);
            label_Test.Text = "Testing error: ";

            if (checkBox1.Checked == true)
                CreateValues("dollar.txt");
            else
                CreateValues();
            double[] d = new double[D.GetLength(0)];
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = D[i, 0];
            }

            // Тестирование сети
            errTest = rn.TestNetwork(x, D, countRow, numInput, numOutput, numHidden);

            Matrix result = rn.result;
            //listBox1.Items.Clear();

            if (chart1.Series.Count != 2)
                chart1.Series.Add("");

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();

            if (checkBox1.Checked == true)
            {
                chart1.ChartAreas[0].AxisY.Minimum = 30;
                chart1.ChartAreas[0].AxisY.Maximum = 100;
            }
            else
            {
                chart1.ChartAreas[0].AxisY.Minimum = 0;
                chart1.ChartAreas[0].AxisY.Maximum = 1000;
            }

            chart1.Legends[0].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            chart1.Series[0].Name = "Received value";
            chart1.Series[1].Name = "Original value";
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            for (int i = 1; i < countRow - 1; i++)
            {
                chart1.Series[0].Points.AddXY(i, result.Get(i + 1, 0));
                chart1.Series[1].Points.AddXY(i, d[i]);
            }

            //for (int i = 0; i < result.Row; i++)
            //{
            //    String resultD = "";
            //    String resultY = "";
            //    for (int j = 0; j < result.Column; j++)
            //    {
            //        resultY += result.Get(i, j).ToString() + " ";
            //        resultD += D[i, j].ToString() + " ";
            //    }
            ////    listBox1.Items.Add(resultD + " | " + resultY);
            //}
            label_Test.Text = "Testing error: " + errTest;
        }

        private void textBox_n_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox_C_TextChanged(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double dou = 0;
            //listBox1.Items.Clear();
            for (double q = 0.0001; q < 0.1; q = q + 0.005)
            {
                n = q;
                dou = 0;
                //Объем обучающей выборки
                countRow = Convert.ToInt32(textBox_X.Text);
                numHidden = Convert.ToInt32(textBox_C.Text);
                countIter = Convert.ToInt32(textBox1.Text);
                // чтение входных данных
                x = ReaderWriter.ReadMatrix(filename, countRow, numInput);
                double[] d = ReaderWriter.ReadVector(filename, countRow, numInput);
                D = new double[countRow, numOutput];
                RecountD(d);
                for (int i = 0; i < 10; i++)
                {
                    //Коэффициент обучение сети
                    // n = Convert.ToDouble(textBox_n.Text);
                    countRow = Convert.ToInt32(textBox_X.Text);
                    numHidden = Convert.ToInt32(textBox_C.Text);
                    countIter = Convert.ToInt32(textBox1.Text);
                    // чтение входных данных
                    x = ReaderWriter.ReadMatrix(filename, countRow, numInput);
                    d = ReaderWriter.ReadVector(filename, countRow, numInput);
                    D = new double[countRow, numOutput];
                    RecountD(d);

                    //Инициализация центроид
                    // centroids = ReaderWriter.ReadMatrixC(filename, countRow, numInput, numHidden, d);

                    rn = new NeuralNetwork(countRow, numInput, numHidden, numOutput, x, centroids, n, temp, er);
                    rn.countIter = countIter;
                    //Инициализация весов
                    rn.SetWeight();

                    // Обучение сети
                    //     rn.TrainNetwork(x, centroids, countRow, numInput, numHidden, numOutput, D);

                    er = rn.errTrain;


                    countRow = 30;
                    // чтение входных данных
                    x = ReaderWriter.ReadMatrix("iris_test.txt", countRow, numInput);
                    d = ReaderWriter.ReadVector("iris_test.txt", countRow, numInput);
                    D = new double[countRow, numOutput];
                    RecountD(d);

                    // Тестирование сети
                    errTest = rn.TestNetwork(x, D, countRow, numInput, numOutput, numHidden);
                    dou += errTest;
                    // listBox1.Items.Add(errTest);
                }

                Console.WriteLine("(" + q + ";" + dou / 10 + ")");
                //   Console.WriteLine("(" + q +";" + dou / 10 + ")");
                //listBox1.Items.Add("Итого: " + dou / 10);
            }

            for (double q = 0.1; q < 1; q = q + 0.02)
            {
                n = q;
                dou = 0;
                //Объем обучающей выборки
                countRow = Convert.ToInt32(textBox_X.Text);
                numHidden = Convert.ToInt32(textBox_C.Text);
                countIter = Convert.ToInt32(textBox1.Text);
                // чтение входных данных
                x = ReaderWriter.ReadMatrix(filename, countRow, numInput);
                double[] d = ReaderWriter.ReadVector(filename, countRow, numInput);
                D = new double[countRow, numOutput];
                RecountD(d);
                for (int i = 0; i < 10; i++)
                {
                    //Коэффициент обучение сети
                    // n = Convert.ToDouble(textBox_n.Text);
                    countRow = Convert.ToInt32(textBox_X.Text);
                    numHidden = Convert.ToInt32(textBox_C.Text);
                    countIter = Convert.ToInt32(textBox1.Text);
                    // чтение входных данных
                    x = ReaderWriter.ReadMatrix(filename, countRow, numInput);
                    d = ReaderWriter.ReadVector(filename, countRow, numInput);
                    D = new double[countRow, numOutput];
                    RecountD(d);

                    //Инициализация центроид
                    //  centroids = ReaderWriter.ReadMatrixC(filename, countRow, numInput, numHidden, d);

                    rn = new NeuralNetwork(countRow, numInput, numHidden, numOutput, x, centroids, n, temp, er);
                    rn.countIter = countIter;
                    //Инициализация весов
                    rn.SetWeight();

                    // Обучение сети
                    //   rn.TrainNetwork(x, centroids, countRow, numInput, numHidden, numOutput, D);

                    er = rn.errTrain;


                    countRow = 30;
                    // чтение входных данных
                    x = ReaderWriter.ReadMatrix("iris_test.txt", countRow, numInput);
                    d = ReaderWriter.ReadVector("iris_test.txt", countRow, numInput);
                    D = new double[countRow, numOutput];
                    RecountD(d);

                    // Тестирование сети
                    errTest = rn.TestNetwork(x, D, countRow, numInput, numOutput, numHidden);
                    dou += errTest;
                    // listBox1.Items.Add(errTest);
                }

                Console.WriteLine("(" + q + ";" + dou / 10 + ")");
                //   Console.WriteLine("(" + q +";" + dou / 10 + ")");
                // listBox1.Items.Add("Итого: " + dou / 10);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}