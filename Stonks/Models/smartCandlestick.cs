﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stonks.Models
{
    /// <summary>
    /// The smartCandlestick class inherits from aCandlestick and adds additional properties
    /// and methods to represent candlestick pattern analysis.
    /// </summary>
    public class smartCandlestick : aCandlestick
    {
        //Smart Candlestick Properties
        public decimal range { get; set; }
        public decimal bodyRange { get; set; }
        public decimal topPrice { get; set; }
        public decimal bottomPrice { get; set; }
        public decimal topTail { get; set; }
        public decimal bottomTail { get; set; }
        public bool isBullish { get; set; }
        public bool isBearish { get; set; }
        public bool isMarubozu { get; set; }
        public bool isNeutral { get; set; }
        public bool isDoji { get; set; }
        public bool isDragonFlyDoji { get; set; }
        public bool isGravestoneDoji { get; set; }
        public bool isHammer { get; set; }
        public bool isInvertedHammer { get; set; }

        /// <summary>
        /// Constructor for the smartCandlestick class that calls the base class constructor
        /// and calculates smart properties based on aCandlestick properties.
        /// </summary>
        /// <param name="values">An array of values containing candlestick data</param>
        public smartCandlestick(string[] values) : base(values)
        {
            // Calculate smart properties based on aCandlestick properties
            range = high - low;
            bodyRange = Math.Abs(close - open);
            topPrice = Math.Max(open, close);
            bottomPrice = Math.Min(open, close);
            topTail = high - Math.Max(open, close);
            bottomTail = Math.Min(open, close) - low;

            isBullish = close > open;
            isBearish = open > close;
            isNeutral = bodyRange < (0.05m * range);

            double lowerShadowRatio = (double)(close - low) / (double)(high - low);
            double upperShadowRatio = (double)(high - close) / (double)(high - low);

            isDoji = bodyRange <= (high - low) * 0.05m;
            isMarubozu = (bodyRange/range) >= 0.95m;
            isDragonFlyDoji = lowerShadowRatio >= 0.98 && upperShadowRatio < 0.02;
            isGravestoneDoji = isDoji && topTail > (0.70m * range);
            isHammer = bottomTail > (high - low) * 0.6m && topTail < (high - low) * 0.1m;
            isInvertedHammer = topTail > (high - low) * 0.6m && bottomTail < (high - low) * 0.1m;
        }

    }
}
