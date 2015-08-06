using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Analysis
{
    public partial class Form1 : Form
    {

        string filePath, _dataText;
        StreamReader streamReader;
        StreamWriter streamWriter;
        List<double[]> mfm;
        List<Species> allSpecies;
        int num_landmarks, num_species;
        Random rand;

        public Form1()
        {
            InitializeComponent();
            mfm = new List<double[]>();
            rand = new Random();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();
            filePath = openFileDialog1.FileName;
            textBox1.Text = filePath;
            if (filePath != null)
            {
                streamReader = new StreamReader(filePath);
                richTextBox1.Text = streamReader.ReadToEnd();
                _dataText = richTextBox1.Text;
                allSpecies = getSpeciesFromText(_dataText);
            }
            streamReader.Close();
        }

        public void writeToFile(List<double[]> matrix, int rows, int col, String file)
        {
            streamWriter = new StreamWriter(file, true);
            for (int i = 0; i < rows; i++)
            {
                for (int k = 0; k < col; k++)
                {
                    streamWriter.Write(matrix[i][k].ToString() + " ");
                }
                streamWriter.WriteLine();
            }
            streamWriter.WriteLine();
            streamWriter.Close();
        }

        private List<Species> getSpeciesFromText(string dataText)
        {
            richTextBox1.Clear();
            StringReader strReader = new StringReader(dataText);
            allSpecies = new List<Species>();
            string newline = strReader.ReadLine();
            string[] lineElements = newline.Split(',');
            num_landmarks = lineElements.Length-1;
            string species_name = lineElements[0];
            List<double[]> species_data = new List<double[]>();
            Console.WriteLine(species_name + " " + num_landmarks);
            while (newline != null)
            {
                if (!lineElements[0].Equals(species_name))
                {
                    //store the species
                    allSpecies.Add(new Species(species_data, species_name));
                    richTextBox1.Text += species_name + '\n';
                    species_data = new List<double[]>();
                    species_name = lineElements[0];
                    Console.WriteLine(species_name);
                }

                //process new line of data
                double[] data = new double[num_landmarks];
                for (int k = 1; k < lineElements.Length; k++)
                {
                    data[k-1] = Convert.ToDouble(lineElements[k]);
                }
                species_data.Add(data);
                newline = strReader.ReadLine();
                if (newline != null)
                    lineElements = newline.Split(',');
            }
            //store the last species
            allSpecies.Add(new Species(species_data, species_name)); 
            richTextBox1.Text += species_name + '\n';
            species_data = new List<double[]>();
            species_name = lineElements[0];
            num_species = allSpecies.Count;
            Console.WriteLine(num_species);
            return allSpecies;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            richTextBox1.Clear();
            for (int i = 0; i < allSpecies.Count; i++)
            {
                richTextBox1.Text += allSpecies[i].speciesName + '\n';
                double[] output = calculateMeansForSpecies(allSpecies[i].data, allSpecies[i].data.Count, num_landmarks);
                allSpecies[i].meanForm = output;
                mfm.Add(output);
                for (int k = 0; k < num_landmarks; k++)
                {
                    richTextBox1.Text += output[k] + " ";
                }
                richTextBox1.Text += '\n';
            }
        }

       

        private List<double[]> getDissimilarityMatrix(List<double[]> mfm)
        {
            List<double[]> disMatrix = new List<double[]>();
            for (int i = 0; i < mfm.Count; i++)
            {
                
                double[] line = new double[mfm.Count];
                for (int k = 0; k < mfm.Count; k++)
                {
                    double sum = 0.0;
                    for (int p=0;p<num_landmarks;p++)
                    {
                        sum += Math.Pow(Math.Log((mfm[i][p] / mfm[k][p]), Math.E), 2.0);
                    }
                    line[k] = Math.Sqrt(sum);
                }
                disMatrix.Add(line);
            }
            return disMatrix;
        }

        public double[] calculateMeansForSpecies(List<double[]> matrix, int rows, int cols)
        {
            double[] output = new double[cols];
            
            for (int i = 0; i < cols; i++)
            {
                double sum = 0.0;
                double mean = 0.0;
                for (int k = 0; k < rows; k++)
                {
                    sum += matrix[k][i];
                }
                mean = sum / rows;
                output[i] = mean;
            }
            return output;
        }

        private double[] getRandomIndividualFromSpecies(Species species)
        {
            int randIndex = rand.Next(species.data.Count);
            return species.data[randIndex];
        }

        private List<double[]> getRandomSubset(Species species)
        {
            List<double[]> matrix = new List<double[]>();
            
            int startIndex = rand.Next(species.data.Count);
            int endIndex = rand.Next(species.data.Count);
            if (startIndex > endIndex)
                startIndex = endIndex;
            if (startIndex != endIndex)
            {
                for (int i = 0; i < endIndex - startIndex; i++)
                {
                    matrix.Add(species.data[i]);
                }
            }
            else
                matrix.Add(species.data[startIndex]);
            return matrix;
        }

        //calculate dissimilarity matrix
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.ShowDialog();
            String filename = saveFileDialog1.FileName;
             //do analysis
            progressBar1.Maximum = 999;
            progressBar1.Visible = true;
            for (int x = 0; x < 999; x++)
            {
                List<double[]> mfm = new List<double[]>();
                for (int k = 0; k < allSpecies.Count; k++)
                {
                    List<double[]> randPop = getRandomSubset(allSpecies[k]);
                    mfm.Add(calculateMeansForSpecies(randPop, randPop.Count, num_landmarks));//getRandomIndividualFromSpecies(allSpecies[k]));//
                }
/*                for (int i = 0; i < mfm.Count; i++)
                {
                    for (int k = 0; k < mfm[i].Length; k++)
                    {
                        richTextBox1.Text += Math.Round(mfm[i][k],6) + " ";
                    }
                    richTextBox1.Text += '\n';
                }
                richTextBox1.Text += '\n';
*/                
                List<double[]> matrix = getDissimilarityMatrix(mfm);
/*                for (int p = 0; p < matrix.Count; p++)
                {
                    for (int k = 0; k < matrix[p].Length; k++)
                    {
                        richTextBox1.Text += Math.Round(matrix[p][k],6) + " ";
                    }
                    richTextBox1.Text += '\n';
                }
*/                writeToFile(matrix, matrix.Count, num_species, filename);
/*                for (int i = 0; i < mfm.Count; i++)
                {
                    for (int p = 0; p < matrix[i].Length; p++)
                    {
                        richTextBox1.Text += matrix[i][p] + " ";
                    }
                    richTextBox1.Text += '\n';
                }
                writeToFile(matrix, matrix.Count, num_species, filename);
*/
                progressBar1.Value = x;
            }
            progressBar1.Value = 0;
            richTextBox1.Text = "DONE!";
        }

        //growth matrix
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.ShowDialog();
            String filename = saveFileDialog1.FileName;
            //do analysis
            progressBar1.Maximum = 999;
            progressBar1.Visible = true;
            for (int i = 0; i < 999; i++)
            {
                List<double[]> mfm = new List<double[]>();
                for (int k = 0; k < allSpecies.Count; k++)
                {
                    List<double[]> randPop = getRandomSubset(allSpecies[k]);
                    mfm.Add(calculateMeansForSpecies(randPop, randPop.Count, num_landmarks));
                }
                progressBar1.Value = i;
                List<double[]> matrix = getGrowthRatiosMatrix(mfm);
                richTextBox1.Text += "Run " + i + '\n';
                writeToFile(matrix, matrix.Count, num_landmarks, filename);
            }
            progressBar1.Value = 0;
            richTextBox1.Text = "DONE!";
/*
            List<double[]> mfm = new List<double[]>();
            for (int i = 0; i < allSpecies.Length; i++)
            {
                mfm.Add(calculateMeansForSpecies(allSpecies[i].data, allSpecies[i].data.Count, num_landmarks));
            }
            List<double[]> growthRatios = getGrowthRatiosMatrix(mfm);
            richTextBox1.Clear();
            for (int i = 0; i < growthRatios.Count; i++)
            {
                richTextBox1.Text += allSpecies[i * 2].speciesName + " ";
                for (int k = 0; k < growthRatios[i].Length; k++)
                {
                    richTextBox1.Text += growthRatios[i][k] + " ";
                }
                richTextBox1.Text += '\n';
            }*/
        }

        private List<double[]> getGrowthRatiosMatrix(List<double[]> mfm)
        {
            List<double[]> returnMatrix = new List<double[]>();
            for (int i = 0; i < mfm.Count; i++)
            {
                if (i*2 < (mfm.Count - 1))
                {
                    double[] line = new double[num_landmarks];
                    for (int k = 0; k < num_landmarks; k++)
                    {
                        line[k] = mfm[i*2][k] / mfm[i*2 + 1][k];
                    }
                    returnMatrix.Add(line);
                }
            }
            return returnMatrix;
        }

    }
}
