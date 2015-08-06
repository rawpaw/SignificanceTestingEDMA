using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analysis
{
    class Species
    {
        public List<double[]> data;
        public double[] meanForm; 
        public string speciesName;

        public Species() 
        {
          
        }

        public Species(List<double[]> matrix, string name)
        {
            data = matrix;
            speciesName = name;
        }
    }
}
