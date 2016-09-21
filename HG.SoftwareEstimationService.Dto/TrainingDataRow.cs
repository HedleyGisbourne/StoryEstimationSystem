using System.Collections.Generic;

namespace HG.SoftwareEstimationService.Dto
{
    public class TrainingData
    {
        public TrainingData()
        {
            MatrixOfObsevsations = new List<IndependentVariables>();
            IndependentVariables = new List<double>();
        }

        public List<IndependentVariables> MatrixOfObsevsations { get; set; }
        public List<double> IndependentVariables { get; set; }
    }

    public class IndependentVariables : List<int>
    {
        public IndependentVariables(int variableCount)
        {
            AddRange(new int[variableCount]);
        }
    }
}
