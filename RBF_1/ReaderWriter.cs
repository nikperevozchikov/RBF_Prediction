using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RBF_1
{
    class ReaderWriter
    {
        static public Matrix ReadMatrix(string fileName, int countRow, int countColumn)
        {
            string line;
            double sqrtSum = 0;
            double[,] matrix = new double[countRow, countColumn];
            try
            {
                StreamReader sr = new StreamReader(fileName);

                for (int i = 0; i < countRow; i++)
                {
                    double sumSq = 0;
                    line = sr.ReadLine();
                    string[] inData = line.Split(',');
                    for (int j = 0; j < countColumn; j++)
                    {
                        matrix[i, j] = Convert.ToDouble(inData[j].Replace('.', ','));
                        sumSq += Math.Pow(matrix[i, j], 2);
                    }

                    sqrtSum = Math.Sqrt(sumSq);

                    for (int j = 0; j < countColumn; j++)
                    {
                        matrix[i, j] = matrix[i, j] / sqrtSum;
                    }
                }

                sr.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return new Matrix(matrix);
        }

        static public double[] ReadVector(string fileName, int countRow, int countColumn)
        {
            double[] arr = new double[countRow];
            string line;
            try
            {
                StreamReader sr = new StreamReader(fileName);
                for (int i = 0; i < countRow; i++)
                {
                    line = sr.ReadLine();
                    string[] inData = line.Split(',');
                    arr[i] = Convert.ToDouble(inData[countColumn].Replace('.', ','));
                }

                sr.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return arr;
        }

        static public double[] ReadValues(string fileName, int countRow)
        {
            double[] arr = new double[countRow];
            string line;
            try
            {
                StreamReader sr = new StreamReader(fileName);
                for (int i = 0; i < countRow; i++)
                {
                    line = sr.ReadLine();
                    // string[] inData = line.Split(',');
                    arr[i] = Convert.ToDouble(line.Replace('.', ','));
                }

                sr.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return arr;
        }

        static public Matrix ReadMatrixC(int countRow, int countColumn, int numHidden, double[] d, Matrix inp)
        {
            double[,] matrix = new double[countRow, countColumn];
            double sum = 0;
            Matrix c = new Matrix(numHidden, countColumn); //3x4
            try
            {
                // StreamReader sr = new StreamReader(fileName);

                for (int i = 0; i < countRow; i++)
                {
                    double sumSq = 0;
                    for (int j = 0; j < countColumn; j++)
                    {
                        matrix[i, j] = inp.Get(i, j);
                        sumSq += Math.Pow(matrix[i, j], 2);
                    }

                    double sqrtSum = Math.Sqrt(sumSq);
                    for (int j = 0; j < countColumn; j++)
                    {
                        matrix[i, j] = Math.Round(matrix[i, j] / sqrtSum, 5);
                    }
                }


                for (int i = 0; i < countRow; i++) //105
                {
                    for (int j = 0; j < countColumn; j++) //4
                    {
                        for (int q = 0; q < c.Row; q++)
                            c.Set(q, j, c.Get(q, j) + matrix[i, j]);
                    }
                }

                for (int i = 0; i < numHidden; i++)
                {
                    for (int j = 0; j < countColumn; j++)
                    {
                        c.Set(i, j, c.Get(i, j) / (countRow));
                        if (numHidden > 3)
                        {
                            if (i > 2)
                            {
                                c.Set(i, j, (c.Get(i - 1, j) + 40) / (countRow));
                            }
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            return (c);
        }

        public static void WriteData(string fileName, double[] d, double[] y)
        {
            try
            {
                StreamWriter sw = new StreamWriter(fileName);
                for (int i = 0; i < d.Length; i++)
                    sw.WriteLine(d[i] + " : " + Math.Round(y[i], 5));
                sw.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        //public static void PrintVector(double[] vector)
        //{
        //    for (int i = 0; i < vector.Length; i++)
        //    {
        //        Console.WriteLine(vector[i]);
        //    }
        //}
    }
}