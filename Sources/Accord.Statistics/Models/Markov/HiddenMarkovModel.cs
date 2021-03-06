// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
// Copyright © Guilherme Pedroso, 2009
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Statistics.Models.Markov
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Accord.Math;
    using Accord.Statistics.Distributions;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;

    /// <summary>
    ///   Discrete-density Hidden Markov Model.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Hidden Markov Models (HMM) are stochastic methods to model temporal and sequence
    ///   data. They are especially known for their application in temporal pattern recognition
    ///   such as speech, handwriting, gesture recognition, part-of-speech tagging, musical
    ///   score following, partial discharges and bioinformatics.</para>
    ///   
    /// <para>
    ///   This page refers to the discrete-density version of the model. For arbitrary
    ///   density (probability distribution) definitions, please see 
    ///   <see cref="HiddenMarkovModel{TDistribution}"/>.
    /// </para>
    ///   
    /// <para>
    ///   Dynamical systems of discrete nature assumed to be governed by a Markov chain emits
    ///   a sequence of observable outputs. Under the Markov assumption, it is also assumed that
    ///   the latest output depends only on the current state of the system. Such states are often
    ///   not known from the observer when only the output values are observable.</para>
    ///   
    ///  <para>
    ///   Assuming the Markov probability, the probability of any sequence of observations 
    ///   occurring when following a given sequence of states can be stated as</para>
    ///      
    ///  <p align="center">
    ///      <img src="..\images\hmm\hmm-joint-probability.png" width="383" height="133" /></p>
    ///      
    ///  <para>
    ///  in which the probabilities <c>p(y<sub>t</sub>|y<sub>t-1</sub>)</c> can be read as the 
    ///  probability of being currently in state <c>y<sub>t</sub></c> given we just were in the
    ///  state<c> y<sub>t-1</sub></c> at the previous instant <c>t-1</c>, and the probability
    ///  <c> p(x<sub>t</sub>|y<sub>t</sub>)</c> can be understood as the probability of observing 
    ///  <c><strong>x<sub>t</sub></strong></c> at instant t given we are currently in the state 
    ///  <c>y<sub>t</sub></c>. To compute those probabilities, we simple use two matrices <strong>
    ///  <c><strong>A</strong></c></strong> and <strong><c><strong>B</strong></c></strong>. 
    ///  The matrix <strong><c><strong>A</strong></c></strong> is the matrix of state probabilities:
    ///  it gives the probabilities <c>p(y<sub>t</sub>|y<sub>t-1</sub>)</c> of jumping from one state
    ///  to the other, and the matrix B is the matrix of observation probabilities, which gives the
    ///  distribution density <c>p(<strong>x<sub>t</sub></strong>|y<sub>t</sub>)</c> associated 
    ///  a given state <c>y<sub>t</sub></c>. In the discrete case, <c><strong><c><strong>
    ///  B</strong></c></strong></c> is really a matrix. In the continuous case, <c><strong>
    ///  B</strong></c> is a vector of probability distributions. The overall model definition
    ///  can then be stated by the tuple</para>
    ///  
    ///  <p align="center">
    ///      <img src="..\images\hmm\hmm-tuple.png" width="159" height="42" /></p>
    ///      
    /// <para>
    ///  in which <em><c><em>n</em></c></em> is an integer representing the total number 
    ///  of states in the system, <strong><c><strong>A</strong></c></strong> is a matrix 
    ///  of transition probabilities, <strong><c><strong>B</strong></c></strong> is either
    ///  a matrix of observation probabilities (in the discrete case) or a vector of probability
    ///  distributions (in the general case) and <c><strong>p</strong></c> is a vector of 
    ///  initial state probabilities determining the probability of starting in each of the 
    ///  possible states in the model.</para>
    ///   
    /// <para>
    ///   Hidden Markov Models attempt to model such systems and allow, among other things,
    ///   <list type="number">
    ///     <item><description>
    ///       To infer the most likely sequence of states that produced a given output sequence,</description></item>
    ///     <item><description>
    ///       Infer which will be the most likely next state (and thus predicting the next output),</description></item>
    ///     <item><description>
    ///       Calculate the probability that a given sequence of outputs originated from the system
    ///       (allowing the use of hidden Markov models for sequence classification).</description></item>
    ///     </list></para>
    ///     
    /// <para>     
    ///   The “hidden” in Hidden Markov Models comes from the fact that the observer does not
    ///   know in which state the system may be in, but has only a probabilistic insight on where
    ///   it should be.</para>
    ///   
    /// <para>
    ///   To learn a Markov model, you can find a list of both <see cref="ISupervisedLearning">
    ///   supervised</see> and <see cref="IUnsupervisedLearning">unsupervised</see> learning 
    ///   algorithms in the <see cref="Accord.Statistics.Models.Markov.Learning"/> namespace.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Linear regression." Wikipedia, the Free Encyclopedia.
    ///       Available at: http://en.wikipedia.org/wiki/Hidden_Markov_model </description></item>
    ///     <item><description>
    ///       Nikolai Shokhirev, Hidden Markov Models. Personal website. Available at:
    ///       http://www.shokhirev.com/nikolai/abc/alg/hmm/hmm.html </description></item>
    ///     <item><description>
    ///       X. Huang, A. Acero, H. Hon. "Spoken Language Processing." pp 396-397. 
    ///       Prentice Hall, 2001.</description></item>
    ///     <item><description>
    ///       Dawei Shen. Some mathematics for HMMs, 2008. Available at:
    ///       http://courses.media.mit.edu/2010fall/mas622j/ProblemSets/ps4/tutorial.pdf </description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>The example below reproduces the same example given in the Wikipedia
    ///   entry for the Viterbi algorithm (http://en.wikipedia.org/wiki/Viterbi_algorithm).
    ///   In this example, the model's parameters are initialized manually. However, it is
    ///   possible to learn those automatically using <see cref="BaumWelchLearning"/>.</para>
    ///   
    /// <code>
    ///   // Create the transition matrix A
    ///   double[,] transition = 
    ///   {  
    ///       { 0.7, 0.3 },
    ///       { 0.4, 0.6 }
    ///   };
    ///   
    ///   // Create the emission matrix B
    ///   double[,] emission = 
    ///   {  
    ///       { 0.1, 0.4, 0.5 },
    ///       { 0.6, 0.3, 0.1 }
    ///   };
    ///   
    ///   // Create the initial probabilities pi
    ///   double[] initial =
    ///   {
    ///       0.6, 0.4
    ///   };
    ///   
    ///   // Create a new hidden Markov model
    ///   HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);
    ///   
    ///   // After that, one could, for example, query the probability
    ///   // of a sequence occurring. We will consider the sequence
    ///   int[] sequence = new int[] { 0, 1, 2 };
    ///   
    ///   // And now we will evaluate its likelihood
    ///   double logLikelihood = hmm.Evaluate(sequence); 
    ///               
    ///   // At this point, the log-likelihood of the sequence
    ///   // occurring within the model is -3.3928721329161653.
    ///   
    ///   // We can also get the Viterbi path of the sequence
    ///   int[] path = hmm.Decode(sequence, out logLikelihood); 
    ///               
    ///   // At this point, the state path will be 1-0-0 and the
    ///   // log-likelihood will be -4.3095199438871337
    ///   </code>
    /// </example>
    /// 
    /// <seealso cref="BaumWelchLearning">Baum-Welch, one of the most famous 
    ///   learning algorithms for Hidden Markov Models.</seealso>
    /// <seealso cref="HiddenMarkovModel{TDistribution}">Arbitrary-density 
    ///   Hidden Markov Model.</seealso>
    /// <seealso cref="Accord.Statistics.Models.Markov.Learning"/>
    /// 
    [Serializable]
    public class HiddenMarkovModel : BaseHiddenMarkovModel, IHiddenMarkovModel, ICloneable
    {

        // Model is defined as M = (A, B, pi)
        private double[,] logB; // emission probabilities

        // The other parameters are defined in HiddenMarkovModelBase
        // private double[,] A; // Transition probabilities
        // private double[] pi; // Initial state probabilities


        // Size of vocabulary
        private int symbols;



        #region Constructors

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public HiddenMarkovModel(ITopology topology, double[,] emissions, bool logarithm = false)
            : base(topology)
        {
            this.logB = emissions;
            this.symbols = logB.GetLength(1);

            if (!logarithm)
                logB = Elementwise.Log(logB);

            for (int i = 0; i < States; i++)
            {
                double stateSum = 0;
                for (int j = 0; j < Symbols; j++)
                    stateSum += Math.Exp(logB[i, j]);

                if (!stateSum.IsEqual(1, 1e-10))
                {
                    throw new ArgumentException("Symbol probabilities for state " + i
                        + " do not sum up to one.", "emissions");
                }
            }
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// 
        public HiddenMarkovModel(ITopology topology, int symbols)
            : this(topology, symbols, false)
        {
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// <param name="random">Whether to initialize emissions with random probabilities
        ///   or uniformly with <c>1 / number of symbols</c>. Default is false (default is
        ///   to use <c>1/symbols</c>).</param>
        /// 
        public HiddenMarkovModel(ITopology topology, int symbols, bool random)
            : base(topology)
        {
            this.symbols = symbols;
            this.logB = new double[States, symbols];

            if (random)
            {
                for (int i = 0; i < States; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < symbols; j++)
                        sum += logB[i, j] = Accord.Math.Random.Generator.Random.NextDouble();

                    for (int j = 0; j < symbols; j++)
                        logB[i, j] /= sum;
                }
            }
            else
            {
                // Initialize B with uniform probabilities
                for (int i = 0; i < States; i++)
                    for (int j = 0; j < symbols; j++)
                        logB[i, j] = 1.0 / symbols;
            }

            logB = Elementwise.Log(logB);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="initial">The initial state probabilities for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public HiddenMarkovModel(double[,] transitions, double[,] emissions, double[] initial, bool logarithm = false)
            : this(new Custom(transitions, initial, logarithm), emissions, logarithm)
        {
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// 
        public HiddenMarkovModel(int states, int symbols)
            : this(new Ergodic(states), symbols)
        {
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// <param name="random">Whether to initialize the model transitions and emissions 
        ///   with random probabilities or uniformly with <c>1 / number of states</c> (for
        ///   transitions) and <c>1 / number of symbols</c> (for emissions). Default is false.</param>
        /// 
        public HiddenMarkovModel(int states, int symbols, bool random)
            : this(new Ergodic(states, random), symbols, random)
        {
        }

        #endregion



        /// <summary>
        ///   Gets the number of symbols in this model's alphabet.
        /// </summary>
        /// 
        public int Symbols
        {
            get { return symbols; }
        }

        /// <summary>
        ///   Gets the log-emission matrix <c>log(B)</c> for this model.
        /// </summary>
        /// 
        public double[,] Emissions
        {
            get { return this.logB; }
        }




        #region Public Methods

        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// 
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        /// 
        public int[] Decode(int[] observations)
        {
            double logLikelihood;
            return Decode(observations, out logLikelihood);
        }

        /// <summary>
        ///   Calculates the most likely sequence of hidden states
        ///   that produced the given observation sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Decoding problem. Given the HMM M = (A, B, pi) and  the observation sequence 
        ///   O = {o1,o2, ..., oK}, calculate the most likely sequence of hidden states Si
        ///   that produced this observation sequence O. This can be computed efficiently
        ///   using the Viterbi algorithm.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="logLikelihood">The log-likelihood along the most likely sequence.</param>
        /// <returns>The sequence of states that most likely produced the sequence.</returns>
        /// 
        public int[] Decode(int[] observations, out double logLikelihood)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            try
            {
                return viterbi(observations, out logLikelihood);
            }
            catch (IndexOutOfRangeException ex)
            {
                checkObservations(ex, observations);
                throw;
            }
        }

        private int[] viterbi(int[] observations, out double logLikelihood)
        {
            // Viterbi-forward algorithm.
            int T = observations.Length;
            int states = States;
            int maxState;
            double maxWeight;
            double weight;

            double[] logPi = Probabilities;
            double[,] logA = Transitions;

            int[,] s = new int[states, T];
            double[,] lnFwd = new double[states, T];


            // Base
            for (int i = 0; i < states; i++)
                lnFwd[i, 0] = logPi[i] + logB[i, observations[0]];

            // Induction
            for (int t = 1; t < T; t++)
            {
                int observation = observations[t];

                for (int j = 0; j < states; j++)
                {
                    maxState = 0;
                    maxWeight = lnFwd[0, t - 1] + logA[0, j];

                    for (int i = 1; i < states; i++)
                    {
                        weight = lnFwd[i, t - 1] + logA[i, j];

                        if (weight > maxWeight)
                        {
                            maxState = i;
                            maxWeight = weight;
                        }
                    }

                    lnFwd[j, t] = maxWeight + logB[j, observation];
                    s[j, t] = maxState;
                }
            }


            // Find maximum value for time T-1
            maxState = 0;
            maxWeight = lnFwd[0, T - 1];

            for (int i = 1; i < states; i++)
            {
                if (lnFwd[i, T - 1] > maxWeight)
                {
                    maxState = i;
                    maxWeight = lnFwd[i, T - 1];
                }
            }


            // Trackback
            int[] path = new int[T];
            path[T - 1] = maxState;

            for (int t = T - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];


            // Returns the sequence probability as an out parameter
            logLikelihood = maxWeight;

            // Returns the most likely (Viterbi path) for the given sequence
            return path;
        }

        /// <summary>
        ///   Calculates the probability of each hidden state for each
        ///   observation in the observation vector.
        /// </summary>
        /// 
        /// <remarks>
        ///   If there are 3 states in the model, and the <paramref name="observations"/>
        ///   array contains 5 elements, the resulting vector will contain 5 vectors of
        ///   size 3 each. Each vector of size 3 will contain probability values that sum
        ///   up to one. By following those probabilities in order, we may decode those
        ///   probabilities into a sequence of most likely states. However, the sequence
        ///   of obtained states may not be valid in the model.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// 
        /// <returns>A vector of the same size as the observation vectors, containing
        ///  the probabilities for each state in the model for the current observation.
        ///  If there are 3 states in the model, and the <paramref name="observations"/>
        ///  array contains 5 elements, the resulting vector will contain 5 vectors of
        ///  size 3 each. Each vector of size 3 will contain probability values that sum
        ///  up to one.</returns>
        /// 
        public double[][] Posterior(int[] observations)
        {
            // Reference: C. S. Foo, CS262 Winter 2007, Lecture 5, Stanford
            // http://ai.stanford.edu/~serafim/CS262_2007/notes/lecture5.pdf

            if (observations == null)
                throw new ArgumentNullException("observations");

            try
            {
                double logLikelihood;

                // Compute forward and backward probabilities
                double[,] lnFwd = ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);
                double[,] lnBwd = ForwardBackwardAlgorithm.LogBackward(this, observations);

                double[][] probabilities = new double[observations.Length][];

                for (int i = 0; i < probabilities.Length; i++)
                {
                    double[] states = probabilities[i] = new double[States];

                    for (int j = 0; j < states.Length; j++)
                        states[j] = Math.Exp(lnFwd[i, j] + lnBwd[i, j] - logLikelihood);
                }

                return probabilities;
            }
            catch (IndexOutOfRangeException ex)
            {
                checkObservations(ex, observations);
                throw;
            }
        }

        /// <summary>
        ///   Calculates the probability of each hidden state for each observation 
        ///   in the observation vector, and uses those probabilities to decode the
        ///   most likely sequence of states for each observation in the sequence 
        ///   using the posterior decoding method. See remarks for details.
        /// </summary>
        /// 
        /// <remarks>
        ///   If there are 3 states in the model, and the <paramref name="observations"/>
        ///   array contains 5 elements, the resulting vector will contain 5 vectors of
        ///   size 3 each. Each vector of size 3 will contain probability values that sum
        ///   up to one. By following those probabilities in order, we may decode those
        ///   probabilities into a sequence of most likely states. However, the sequence
        ///   of obtained states may not be valid in the model.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="path">The sequence of states most likely associated with each
        ///   observation, estimated using the posterior decoding method.</param>
        /// 
        /// <returns>A vector of the same size as the observation vectors, containing
        ///  the probabilities for each state in the model for the current observation.
        ///  If there are 3 states in the model, and the <paramref name="observations"/>
        ///  array contains 5 elements, the resulting vector will contain 5 vectors of
        ///  size 3 each. Each vector of size 3 will contain probability values that sum
        ///  up to one.</returns>
        /// 
        public double[][] Posterior(int[] observations, out int[] path)
        {
            double[][] probabilities = Posterior(observations);

            path = new int[observations.Length];
            for (int i = 0; i < path.Length; i++)
                Accord.Math.Matrix.Max(probabilities[i], out path[i]);

            return probabilities;
        }

        /// <summary>
        ///   Calculates the log-likelihood that this model has generated the given sequence.
        /// </summary>
        /// 
        /// <remarks>
        ///   Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        ///   sequence O = {o1, o2, ..., oK}, calculate the probability that model
        ///   M has generated sequence O. This can be computed efficiently using the
        ///   either the Viterbi or the Forward algorithms.
        /// </remarks>
        /// 
        /// <param name="observations">
        ///   A sequence of observations.
        /// </param>
        /// 
        /// <returns>
        ///   The log-likelihood that the given sequence has been generated by this model.
        /// </returns>
        /// 
        public double Evaluate(int[] observations)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
                return Double.NegativeInfinity;

            try
            {
                // Forward algorithm
                double logLikelihood;

                // Compute forward probabilities
                ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);

                // Return the sequence probability
                return logLikelihood;
            }
            catch (IndexOutOfRangeException ex)
            {
                checkObservations(ex, observations);
                throw;
            }
        }

        /// <summary>
        ///   Calculates the log-likelihood that this model has generated the
        ///   given observation sequence along the given state path.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. </param>
        /// <param name="path">A sequence of states. </param>
        /// 
        /// <returns>
        ///   The log-likelihood that the given sequence of observations has
        ///   been generated by this model along the given sequence of states.
        /// </returns>
        /// 
        public double Evaluate(int[] observations, int[] path)
        {
            if (observations == null)
                throw new ArgumentNullException("observations");

            if (path == null)
                throw new ArgumentNullException("path");

            if (observations.Length == 0)
                return Double.NegativeInfinity;

            try
            {
                double logLikelihood = Probabilities[path[0]] + Emissions[path[0], observations[0]];

                for (int i = 1; i < observations.Length; i++)
                {
                    double a = Transitions[path[i - 1], path[i]];
                    double b = Emissions[path[i], observations[i]];
                    logLikelihood += a + b;
                }

                // Return the sequence probability
                return logLikelihood;
            }
            catch (IndexOutOfRangeException ex)
            {
                checkObservations(ex, observations);
                checkHiddenStates(ex, path);
                throw;
            }
        }



        /// <summary>
        ///   Predicts next observations occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="next">The number of observations to be predicted. Default is 1.</param>
        /// <param name="logLikelihood">The log-likelihood of the given sequence, plus the predicted
        ///   next observations. Exponentiate this value (use the System.Math.Exp function) to obtain
        ///   a <c>likelihood</c> value.</param>
        /// 
        public int[] Predict(int[] observations, int next, out double logLikelihood)
        {
            double[][] logLikelihoods;
            return Predict(observations, next, out logLikelihood, out logLikelihoods);
        }

        /// <summary>
        ///   Predicts next observations occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="next">The number of observations to be predicted. Default is 1.</param>
        /// 
        public int[] Predict(int[] observations, int next)
        {
            double logLikelihood;
            double[][] logLikelihoods;
            return Predict(observations, next, out logLikelihood, out logLikelihoods);
        }

        /// <summary>
        ///   Predicts next observations occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="next">The number of observations to be predicted. Default is 1.</param>
        /// <param name="logLikelihoods">The log-likelihood of the different symbols for each predicted
        ///   next observations. In order to convert those values to probabilities, exponentiate the
        ///   values in the vectors (using the Exp function) and divide each value by their vector's sum.</param>
        /// 
        public int[] Predict(int[] observations, int next, out double[][] logLikelihoods)
        {
            double logLikelihood;
            return Predict(observations, next, out logLikelihood, out logLikelihoods);
        }

        /// <summary>
        ///   Predicts the next observation occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observation that should be coming after the last observation in this sequence.</param>
        /// <param name="logLikelihoods">The log-likelihood of the different symbols for the next observation.
        ///   In order to convert those values to probabilities, exponentiate the values in the vector (using
        ///   the Exp function) and divide each value by the vector sum. This will give the probability of each
        ///   next possible symbol to be the next observation in the sequence.</param>
        /// 
        public int Predict(int[] observations, out double[] logLikelihoods)
        {
            double[][] ll;
            double logLikelihood;
            int prediction = Predict(observations, 1, out logLikelihood, out ll)[0];
            logLikelihoods = ll[0];
            return prediction;
        }

        /// <summary>
        ///   Predicts the next observations occurring after a given observation sequence.
        /// </summary>
        /// 
        /// <param name="observations">A sequence of observations. Predictions will be made regarding 
        ///   the next observations that should be coming after the last observation in this sequence.</param>
        /// <param name="next">The number of observations to be predicted. Default is 1.</param>
        /// <param name="logLikelihoods">The log-likelihood of the different symbols for each predicted
        ///   next observations. In order to convert those values to probabilities, exponentiate the
        ///   values in the vectors (using the Exp function) and divide each value by their vector's sum.</param>
        /// <param name="logLikelihood">The log-likelihood of the given sequence, plus the predicted
        ///   next observations. Exponentiate this value (use the System.Math.Exp function) to obtain
        ///   a <c>likelihood</c> value.</param>
        ///   
        public int[] Predict(int[] observations, int next, out double logLikelihood, out double[][] logLikelihoods)
        {
            int states = States;
            int T = next;
            double[,] lnA = Transitions;

            int[] prediction = new int[next];
            logLikelihoods = new double[next][];

            try
            {
                // Compute forward probabilities for the given observation sequence.
                double[,] lnFw0 = ForwardBackwardAlgorithm.LogForward(this, observations, out logLikelihood);

                // Create a matrix to store the future probabilities for the prediction
                // sequence and copy the latest forward probabilities on its first row.
                double[,] lnFwd = new double[T + 1, states];


                // 1. Initialization
                for (int i = 0; i < states; i++)
                    lnFwd[0, i] = lnFw0[observations.Length - 1, i];

                // 2. Induction
                for (int t = 0; t < T; t++)
                {
                    double[] weights = new double[symbols];
                    for (int s = 0; s < symbols; s++)
                    {
                        weights[s] = Double.NegativeInfinity;

                        for (int i = 0; i < states; i++)
                        {
                            double sum = Double.NegativeInfinity;
                            for (int j = 0; j < states; j++)
                                sum = Special.LogSum(sum, lnFwd[t, j] + lnA[j, i]);
                            lnFwd[t + 1, i] = sum + logB[i, s];

                            weights[s] = Special.LogSum(weights[s], lnFwd[t + 1, i]);
                        }
                    }

                    double sumWeight = Double.NegativeInfinity;
                    for (int i = 0; i < weights.Length; i++)
                        sumWeight = Special.LogSum(sumWeight, weights[i]);
                    for (int i = 0; i < weights.Length; i++)
                        weights[i] -= sumWeight;


                    // Select most probable symbol
                    double maxWeight = weights[0];
                    prediction[t] = 0;
                    for (int i = 1; i < weights.Length; i++)
                    {
                        if (weights[i] > maxWeight)
                        {
                            maxWeight = weights[i];
                            prediction[t] = i;
                        }
                    }

                    // Recompute log-likelihood
                    logLikelihoods[t] = weights;
                    logLikelihood = maxWeight;
                }


                return prediction;
            }
            catch (IndexOutOfRangeException ex)
            {
                checkObservations(ex, observations);
                throw;
            }
        }

        /// <summary>
        ///   Generates a random vector of observations from the model.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// 
        /// <returns>A random vector of observations drawn from the model.</returns>
        /// 
        /// <example>
        /// <code>
        /// Accord.Math.Tools.SetupGenerator(42);
        /// 
        /// // Consider some phrases:
        /// //
        /// string[][] phrases =
        /// {
        ///     new[] { "those", "are", "sample", "words", "from", "a", "dictionary" },
        ///     new[] { "those", "are", "sample", "words" },
        ///     new[] { "sample", "words", "are", "words" },
        ///     new[] { "those", "words" },
        ///     new[] { "those", "are", "words" },
        ///     new[] { "words", "from", "a", "dictionary" },
        ///     new[] { "those", "are", "words", "from", "a", "dictionary" }
        /// };
        /// 
        /// // Let's begin by transforming them to sequence of
        /// // integer labels using a codification codebook:
        /// var codebook = new Codification("Words", phrases);
        /// 
        /// // Now we can create the training data for the models:
        /// int[][] sequence = codebook.Translate("Words", phrases);
        /// 
        /// // To create the models, we will specify a forward topology,
        /// // as the sequences have definite start and ending points.
        /// //
        /// var topology = new Forward(states: 4);
        /// int symbols = codebook["Words"].Symbols; // We have 7 different words
        /// 
        /// // Create the hidden Markov model
        /// HiddenMarkovModel hmm = new HiddenMarkovModel(topology, symbols);
        /// 
        /// // Create the learning algorithm
        /// BaumWelchLearning teacher = new BaumWelchLearning(hmm);
        /// 
        /// // Teach the model about the phrases
        /// double error = teacher.Run(sequence);
        /// 
        /// // Now, we can ask the model to generate new samples
        /// // from the word distributions it has just learned:
        /// //
        /// int[] sample = hmm.Generate(3);
        /// 
        /// // And the result will be: "those", "are", "words".
        /// string[] result = codebook.Translate("Words", sample);
        /// </code>
        /// </example>
        /// 
        public int[] Generate(int samples)
        {
            int[] path; double logLikelihood;
            return Generate(samples, out path, out logLikelihood);
        }

        /// <summary>
        ///   Generates a random vector of observations from the model.
        /// </summary>
        /// 
        /// <param name="samples">The number of samples to generate.</param>
        /// <param name="logLikelihood">The log-likelihood of the generated observation sequence.</param>
        /// <param name="path">The Viterbi path of the generated observation sequence.</param>
        /// 
        /// <example>
        ///   An usage example is available at the <see cref="Generate(int)"/> documentation page.
        /// </example>
        /// 
        /// <returns>A random vector of observations drawn from the model.</returns>
        /// 
        public int[] Generate(int samples, out int[] path, out double logLikelihood)
        {
            double[] transitions = Probabilities;
            double[] emissions;

            int[] observations = new int[samples];
            logLikelihood = 0; // log(1)
            path = new int[samples];


            // For each observation to be generated
            for (int t = 0; t < observations.Length; t++)
            {
                // Navigate randomly on one of the state transitions
                int state = GeneralDiscreteDistribution.Random(Elementwise.Exp(transitions));

                // Generate a sample for the state
                emissions = Emissions.GetRow(state);
                int symbol = GeneralDiscreteDistribution.Random(Elementwise.Exp(emissions));

                // Store the sample
                observations[t] = symbol;
                path[t] = state;

                // Compute log-likelihood up to this point
                logLikelihood += transitions[state] + emissions[symbol];

                // Continue sampling
                transitions = Transitions.GetRow(state);
            }

            return observations;
        }

        /// <summary>
        ///   Converts this <see cref="HiddenMarkovModel">Discrete density Hidden Markov Model</see>
        ///   into a <see cref="HiddenMarkovModel{TDistribution}">arbitrary density model</see>.
        /// </summary>
        public HiddenMarkovModel<GeneralDiscreteDistribution> ToContinuousModel()
        {
            var transitions = (double[,])Transitions.Clone();
            var probabilities = (double[])Probabilities.Clone();

            var emissions = new GeneralDiscreteDistribution[States];
            for (int i = 0; i < emissions.Length; i++)
                emissions[i] = new GeneralDiscreteDistribution(Accord.Math.Matrix.GetRow(Emissions, i));

            return new HiddenMarkovModel<GeneralDiscreteDistribution>(transitions, emissions, probabilities);
        }

        /// <summary>
        ///   Converts this <see cref="HiddenMarkovModel">Discrete density Hidden Markov Model</see>
        ///   to a <see cref="HiddenMarkovModel{TDistribution}">Continuous density model</see>.
        /// </summary>
        public static explicit operator HiddenMarkovModel<GeneralDiscreteDistribution>(HiddenMarkovModel model)
        {
            return model.ToContinuousModel();
        }
        #endregion



        #region Named constructors

        /// <summary>
        ///   Constructs a new discrete-density Hidden Markov Model.
        /// </summary>
        /// 
        /// <param name="transitions">The transitions matrix A for this model.</param>
        /// <param name="emissions">The emissions matrix B for this model.</param>
        /// <param name="probabilities">The initial state probabilities for this model.</param>
        /// <param name="logarithm">Set to true if the matrices are given with logarithms of the
        /// intended probabilities; set to false otherwise. Default is false.</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(double[,] transitions,
            double[,] emissions, double[] probabilities, bool logarithm = false)
        {
            ITopology topology = new Custom(transitions, probabilities, logarithm);

            if (emissions == null)
            {
                throw new ArgumentNullException("emissions");
            }

            if (emissions.GetLength(0) != topology.States)
            {
                throw new ArgumentException(
                    "The emission matrix should have the same number of rows as the number of states in the model.",
                    "emissions");
            }


            // Initialize B using a discrete distribution
            var B = new GeneralDiscreteDistribution[topology.States];
            for (int i = 0; i < B.Length; i++)
                B[i] = new GeneralDiscreteDistribution(Accord.Math.Matrix.GetRow(emissions, i));

            return new HiddenMarkovModel<GeneralDiscreteDistribution>(topology, B);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(ITopology topology, int symbols)
        {
            return CreateGeneric(topology, symbols, false);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// 
        /// <param name="topology">
        ///   A <see cref="Topology"/> object specifying the initial values of the matrix of transition 
        ///   probabilities <c>A</c> and initial state probabilities <c>pi</c> to be used by this model.
        /// </param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// <param name="random">Whether to initialize emissions with random probabilities
        ///   or uniformly with <c>1 / number of symbols</c>. Default is false (default is
        ///   to use <c>1/symbols</c>).</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(ITopology topology, int symbols, bool random)
        {
            if (symbols <= 0)
            {
                throw new ArgumentOutOfRangeException("symbols",
                    "Number of symbols should be higher than zero.");
            }

            double[,] A;
            double[] pi;
            topology.Create(true, out A, out pi);

            // Initialize B with a uniform discrete distribution
            GeneralDiscreteDistribution[] B = new GeneralDiscreteDistribution[topology.States];

            if (random)
            {
                for (int i = 0; i < B.Length; i++)
                {
                    double[] probabilities = new double[symbols];

                    double sum = 0;
                    for (int j = 0; j < probabilities.Length; j++)
                        sum += probabilities[j] = Accord.Math.Random.Generator.Random.NextDouble();

                    for (int j = 0; j < probabilities.Length; j++)
                        probabilities[j] /= sum;

                    B[i] = new GeneralDiscreteDistribution(probabilities);
                }
            }
            else
            {
                for (int i = 0; i < B.Length; i++)
                    B[i] = new GeneralDiscreteDistribution(symbols);
            }


            return new HiddenMarkovModel<GeneralDiscreteDistribution>(A, B, pi, logarithm: true);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// 
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(int states, int symbols)
        {
            return CreateGeneric(new Ergodic(states), symbols);
        }

        /// <summary>
        ///   Constructs a new Hidden Markov Model with discrete state probabilities.
        /// </summary>
        /// 
        /// <param name="states">The number of states for this model.</param>
        /// <param name="symbols">The number of output symbols used for this model.</param>
        /// <param name="random">Whether to initialize emissions with random probabilities
        ///   or uniformly with <c>1 / number of symbols</c>. Default is false (default is
        ///   to use <c>1/symbols</c>).</param>
        /// 
        public static HiddenMarkovModel<GeneralDiscreteDistribution> CreateGeneric(int states, int symbols, bool random)
        {
            return CreateGeneric(new Ergodic(states, random), symbols, random);
        }

        #endregion




        #region IHiddenMarkovModel implementation
        int[] IHiddenMarkovModel.Decode(Array sequence, out double logLikelihood)
        {
            return Decode((int[])sequence, out logLikelihood);
        }

        double IHiddenMarkovModel.Evaluate(Array sequence)
        {
            return Evaluate((int[])sequence);
        }


        /// <summary>
        ///   Calculates the probability of each hidden state for each
        ///   observation in the observation vector.
        /// </summary>
        /// 
        /// <remarks>
        ///   If there are 3 states in the model, and the <paramref name="observations"/>
        ///   array contains 5 elements, the resulting vector will contain 5 vectors of
        ///   size 3 each. Each vector of size 3 will contain probability values that sum
        ///   up to one. By following those probabilities in order, we may decode those
        ///   probabilities into a sequence of most likely states. However, the sequence
        ///   of obtained states may not be valid in the model.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// 
        /// <returns>A vector of the same size as the observation vectors, containing
        ///  the probabilities for each state in the model for the current observation.
        ///  If there are 3 states in the model, and the <paramref name="observations"/>
        ///  array contains 5 elements, the resulting vector will contain 5 vectors of
        ///  size 3 each. Each vector of size 3 will contain probability values that sum
        ///  up to one.</returns>
        /// 
        double[][] IHiddenMarkovModel.Posterior(Array observations)
        {
            return Posterior((int[])observations);
        }

        /// <summary>
        ///   Calculates the probability of each hidden state for each observation 
        ///   in the observation vector, and uses those probabilities to decode the
        ///   most likely sequence of states for each observation in the sequence 
        ///   using the posterior decoding method. See remarks for details.
        /// </summary>
        /// 
        /// <remarks>
        ///   If there are 3 states in the model, and the <paramref name="observations"/>
        ///   array contains 5 elements, the resulting vector will contain 5 vectors of
        ///   size 3 each. Each vector of size 3 will contain probability values that sum
        ///   up to one. By following those probabilities in order, we may decode those
        ///   probabilities into a sequence of most likely states. However, the sequence
        ///   of obtained states may not be valid in the model.
        /// </remarks>
        /// 
        /// <param name="observations">A sequence of observations.</param>
        /// <param name="path">The sequence of states most likely associated with each
        ///   observation, estimated using the posterior decoding method.</param>
        /// 
        /// <returns>A vector of the same size as the observation vectors, containing
        ///  the probabilities for each state in the model for the current observation.
        ///  If there are 3 states in the model, and the <paramref name="observations"/>
        ///  array contains 5 elements, the resulting vector will contain 5 vectors of
        ///  size 3 each. Each vector of size 3 will contain probability values that sum
        ///  up to one.</returns>
        /// 
        double[][] IHiddenMarkovModel.Posterior(Array observations, out int[] path)
        {
            return Posterior((int[])observations, out path);
        }

        #endregion



        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            double[,] A = (double[,])Transitions.Clone();
            double[,] B = (double[,])Emissions.Clone();
            double[] pi = (double[])Probabilities.Clone();

            return new HiddenMarkovModel(A, B, pi, logarithm: true);
        }


        #region Save & Load methods

        /// <summary>
        ///   Saves the hidden Markov model to a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream to which the model is to be serialized.</param>
        /// 
        public void Save(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(stream, this);
        }

        /// <summary>
        ///   Saves the hidden Markov model  to a stream.
        /// </summary>
        /// 
        /// <param name="path">The stream to which the model is to be serialized.</param>
        /// 
        public void Save(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Save(fs);
            }
        }

        /// <summary>
        ///   Loads a hidden Markov model  from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized classifier.</returns>
        /// 
        public static HiddenMarkovModel Load(Stream stream)
        {
            BinaryFormatter b = new BinaryFormatter();
            return (HiddenMarkovModel)b.Deserialize(stream);
        }

        /// <summary>
        ///   Loads a hidden Markov model  from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized model.</returns>
        /// 
        public static HiddenMarkovModel Load(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return Load(fs);
            }
        }

        /// <summary>
        ///   Loads a hidden Markov model  from a stream.
        /// </summary>
        /// 
        /// <param name="stream">The stream from which the model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized model.</returns>
        /// 
        public static HiddenMarkovModel<TDistribution> Load<TDistribution>(Stream stream)
            where TDistribution : IDistribution
        {
            return HiddenMarkovModel<TDistribution>.Load(stream);
        }

        /// <summary>
        ///   Loads a hidden Markov model  from a file.
        /// </summary>
        /// 
        /// <param name="path">The path to the file from which the model is to be deserialized.</param>
        /// 
        /// <returns>The deserialized model.</returns>
        /// 
        public static HiddenMarkovModel<TDistribution> Load<TDistribution>(string path)
            where TDistribution : IDistribution
        {
            return HiddenMarkovModel<TDistribution>.Load(path);
        }

        #endregion



        private void checkHiddenStates(IndexOutOfRangeException ex, int[] path)
        {

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] < 0 || path[i] >= States)
                {
                    throw new ArgumentException("path", "The hidden states vector must "
                    + "only contain values higher than or equal to 0 and less than " + States
                    + ". The value at the position " + i + " is " + path[i] + ".", ex);
                }
            }
        }

        private void checkObservations(IndexOutOfRangeException ex, int[] observations)
        {
            for (int i = 0; i < observations.Length; i++)
            {
                if (observations[i] < 0 || observations[i] >= symbols)
                {
                    throw new ArgumentException("observations", "The observations vector must "
                    + "only contain values higher than or equal to 0 and less than " + symbols
                    + ". The value at the position " + i + " is " + observations[i] + ".", ex);
                }
            }
        }

    }
}
