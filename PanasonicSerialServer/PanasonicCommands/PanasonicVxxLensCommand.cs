using PanasonicSerialServer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using PanasonicSerialCommon;
using Serilog;

namespace PanasonicSerialServer.PanasonicCommands
{
    public class PanasonicVxxLensCommand : PanasonicVxxCommand
    {
        private const int LensPauseDuration = 9;
        private const string LensOptionPrefix = ":LMLI0=+0000";

        private readonly Dictionary<LensEnum, double> lensAspectRatios;

        private string option = "";

        public override string Option { 
            get => option;
            set => option = ParseLensOption(value);
        }

        public LensEnum LensMemory { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="pauseDuration"></param>
        /// <param name="lensAspectRatios"></param>
        public PanasonicVxxLensCommand(string command, Dictionary<LensEnum, double> lensAspectRatios, int pauseDuration = 0) : base(command, LensPauseDuration)
        {
            this.lensAspectRatios = lensAspectRatios;
        }


        private string ParseLensOption(string optionString)
        {
            int lensMemoryToLoad;

            if (optionString.Length == 1 && optionString.All(char.IsDigit))
            {
                lensMemoryToLoad = int.Parse(optionString) - 1;
            }
            else
            {
                lensMemoryToLoad = MapAspectToLens(optionString);
            }

            this.LensMemory = (LensEnum)lensMemoryToLoad;

            string optionCommand = LensOptionPrefix + lensMemoryToLoad;

            return optionCommand;
        }


        private int MapAspectToLens(string aspectRatioStr)
        {
            // We're given the required aspect ratio from user (or MadVR).
            double aspectRatio = AspectRatio.Parse(aspectRatioStr);


            // Look for closet match
            //
            var closestMatch = this.FindClosestMatch(aspectRatio);
            Log.Information("MapAspectRation({AspectRatio}) Selected aspect: {SelectedAspect}", aspectRatioStr, closestMatch);

             return (int)closestMatch.Key;
        }


        private KeyValuePair<LensEnum, double> FindClosestMatch(double requiredAspect)
        {
            SortedDictionary<LensEnum, double> percentageDifference = new SortedDictionary<LensEnum, double>();

            // Config holds all possible aspect ratios. Check to see which closest matches ours.
            //
            foreach (var lensAspect in this.lensAspectRatios)
            {
                // The exact number we are returned doesn't matter. What we are looking for is a difference
                // as close to 1 as possible.
                if (requiredAspect > lensAspect.Value)
                {
                    percentageDifference[lensAspect.Key] = lensAspect.Value / requiredAspect;
                }
                else
                {
                    percentageDifference[lensAspect.Key] = requiredAspect / lensAspect.Value;
                }
            }
            Log.Debug("FindClosestMatch({AspectRatio}) Sorted aspect differences: {Sorted}", requiredAspect, percentageDifference);

            // Sort descending and take the first value
            var closestMatch = percentageDifference.OrderByDescending(kvp => kvp.Value).First();

            return closestMatch;
        }
    }
}
