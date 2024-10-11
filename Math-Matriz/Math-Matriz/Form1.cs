using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Math_Matriz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private List<double[,]> matrices = new List<double[,]>();
        private int rows;
        private int cols;
        private int operacionCounter = 0; // Contador para filas y columnas
        private string operacionActual = ""; // Operación actual

        private void InicializarDataGridView()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            for (int i = 0; i < cols; i++)
            {
                dataGridView1.Columns.Add($"Col{i}", $"Columna {i + 1}");
            }

            for (int i = 0; i < rows; i++)
            {
                dataGridView1.Rows.Add();
            }
        }

        private void GuardarMatrix()
        {
            double[,] matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (dataGridView1.Rows[i].Cells[j].Value != null &&
                        double.TryParse(dataGridView1.Rows[i].Cells[j].Value.ToString(), out double value))
                    {
                        matrix[i, j] = value;
                    }
                }
            }

            matrices.Add(matrix);
        }

        private double[,] AddMatrices(List<double[,]> matrices)
        {
            var firstMatrix = matrices[0];

            for (int i = 1; i < matrices.Count; i++)
            {
                if (firstMatrix.GetLength(0) != matrices[i].GetLength(0) ||
                    firstMatrix.GetLength(1) != matrices[i].GetLength(1))
                {
                    MessageBox.Show("Las matrices deben tener las mismas dimensiones para sumar.");
                    return null;
                }

                for (int row = 0; row < firstMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < firstMatrix.GetLength(1); col++)
                    {
                        firstMatrix[row, col] += matrices[i][row, col];
                    }
                }
            }

            return firstMatrix;
        }

        private double[,] SubtractMatrices(List<double[,]> matrices)
        {
            var firstMatrix = matrices[0];

            for (int i = 1; i < matrices.Count; i++)
            {
                if (firstMatrix.GetLength(0) != matrices[i].GetLength(0) ||
                    firstMatrix.GetLength(1) != matrices[i].GetLength(1))
                {
                    MessageBox.Show("Las matrices deben tener las mismas dimensiones para restar.");
                    return null;
                }

                for (int row = 0; row < firstMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < firstMatrix.GetLength(1); col++)
                    {
                        firstMatrix[row, col] -= matrices[i][row, col];
                    }
                }
            }

            return firstMatrix;
        }

        private double[,] MultiplyMatrices(List<double[,]> matrices)
        {
            if (matrices.Count < 2)
            {
                MessageBox.Show("Se necesitan al menos 2 matrices para multiplicar.");
                return null;
            }

            var result = matrices[0]; // Iniciar con la primera matriz

            for (int i = 1; i < matrices.Count; i++)
            {
                var secondMatrix = matrices[i];

                if (result.GetLength(1) != secondMatrix.GetLength(0))
                {
                    MessageBox.Show("El número de columnas de la primera matriz debe ser igual al número de filas de la segunda.");
                    return null;
                }

                double[,] tempResult = new double[result.GetLength(0), secondMatrix.GetLength(1)];

                for (int row = 0; row < tempResult.GetLength(0); row++)
                {
                    for (int col = 0; col < tempResult.GetLength(1); col++)
                    {
                        for (int k = 0; k < result.GetLength(1); k++)
                        {
                            tempResult[row, col] += result[row, k] * secondMatrix[k, col];
                        }
                    }
                }

                result = tempResult; // Actualizar el resultado
            }

            return result;
        }

        private void DisplayResult(double[,] result)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            int rows = result.GetLength(0);
            int cols = result.GetLength(1);

            for (int i = 0; i < cols; i++)
            {
                dataGridView1.Columns.Add($"Col{i}", $"Columna {i + 1}");
            }

            for (int i = 0; i < rows; i++)
            {
                var rowValues = new object[cols];
                for (int j = 0; j < cols; j++)
                {
                    rowValues[j] = result[i, j];
                }
                dataGridView1.Rows.Add(rowValues);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtDimenciones.Text, out double value))
            {
                if (operacionCounter == 0)
                {
                    // Guardar número de filas
                    rows = (int)value;
                    MessageBox.Show($"Filas definidas: {rows}");
                }
                else if (operacionCounter == 1)
                {
                    // Guardar número de columnas
                    cols = (int)value;
                    MessageBox.Show($"Columnas definidas: {cols}");

                    // Inicializar DataGridView
                    InicializarDataGridView();
                    operacionCounter = -1; // Reiniciar para la siguiente entrada
                }
                operacionCounter++;
                txtDimenciones.Clear();
            }
            else
            {
                MessageBox.Show("Ingrese un número válido.");
            }
        }

        private void ClearDataGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            operacionCounter = 0; // Reiniciar contador
        }

        private void btnSuma_Click(object sender, EventArgs e)
        {
            operacionActual = "suma";
            GuardarMatrix();
            ClearDataGrid();
        }

        private void btnResta_Click(object sender, EventArgs e)
        {
            operacionActual = "resta";
            GuardarMatrix();
            ClearDataGrid();
        }

        private void btnMultiplicacion_Click(object sender, EventArgs e)
        {
            operacionActual = "multiplicacion";
            GuardarMatrix();
            ClearDataGrid();
        }

        private void btnIgual_Click(object sender, EventArgs e)
        {
            // Guarda la matriz actual antes de realizar la operación
            GuardarMatrix();

            if (matrices.Count < 2)
            {
                MessageBox.Show("Se necesitan al menos 2 matrices para realizar la operación.");
                return;
            }

            double[,] resultado = null;

            // Verifica qué operación se desea realizar
            switch (operacionActual)
            {
                case "suma":
                    resultado = AddMatrices(matrices);
                    break;
                case "resta":
                    resultado = SubtractMatrices(matrices);
                    break;
                case "multiplicacion":
                    resultado = MultiplyMatrices(matrices);
                    break;
                default:
                    MessageBox.Show("Seleccione una operación válida.");
                    return;
            }

            if (resultado != null)
            {
                DisplayResult(resultado);
            }
            else
            {
                MessageBox.Show("Error al realizar la operación.");
            }

            matrices.Clear(); // Limpiar matrices después de mostrar el resultado
            operacionActual = ""; // Resetear operación
        }

        int x;
        int y;

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                x = e.X;y = e.Y;
            }
            else
            {
                Left = Left + (e.X - x);
                Top = Top + (e.Y - y);
            }
        }

    }
}
