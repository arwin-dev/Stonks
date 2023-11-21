﻿using Stonks.Models;
using Stonks.Recognizers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Stonks
{
    public partial class form_chartDisplay : Form
    {
        List<smartCandlestick> stockData = null;
        List<smartCandlestick> tempop = null;
        List<Recognizer> recognizers = null;
        private BindingList<smartCandlestick> candlesticks { get; set; }

        /// <summary>
        /// Constructor for the form.
        /// Initializes the form, sets data, begin, and end dates, and populates UI elements.
        /// </summary>
        /// <param name="data">List of smartCandlestick data</param>
        /// <param name="begin">Start date for the data</param>
        /// <param name="end">End date for the data</param>
        public form_chartDisplay(List<smartCandlestick> data, DateTime begin, DateTime end)
        {
            InitializeComponent();

            stockData = data;
            dateTimePicker_begin.Value = begin;
            dateTimePicker_end.Value = end;

            InitRecognizers();
            InitComboBox();

            var TempData = stockData.FirstOrDefault();
            var period = TempData.period.ToLower() == "day" ? "Daily" : TempData.period.ToString() + "ly";
            
            label_ticker.Text = TempData.ticker;
            label_period.Text = period;
            this.Text = TempData.ticker;

            refreshGrid();
        }

        /// <summary>
        /// This function is called every time there is a change in the date range.
        /// It uses LINQ to filter the data according to the selected dates and binds the filtered data into the chart.
        /// The function also updates the stock name and the price change label.
        /// </summary>
        public void refreshGrid()
        {
            if (candlesticks != null) candlesticks.Clear();
            if (stockData == null) return;
            var tempdata = stockData.Where(x => x.date >= dateTimePicker_begin.Value && x.date <= dateTimePicker_end.Value).ToList();
            tempop = tempdata;
            if (tempdata == null || tempdata.Count == 0)
            {
                MessageBox.Show("Invalid Date Range!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            candlesticks = new BindingList<smartCandlestick>();
            decimal max = 0, min = 9999999;
            foreach (smartCandlestick cs in tempdata)
            {
                if (cs.high > max)
                {
                    max = cs.high;
                }
                
                if (cs.low < min) 
                {
                    min = cs.low;
                }

                candlesticks.Add(cs);

            }

            chart_data.ChartAreas["ChartArea_ohlc"].AxisY.Minimum = (double)min - 10;
            chart_data.ChartAreas["ChartArea_ohlc"].AxisY.Maximum = (double)max + 10;
            chart_data.DataSource = candlesticks;
            chart_data.DataBind();

            var data = stockData.FirstOrDefault();
            

            var change = Math.Round(candlesticks.Last().close - candlesticks.First().close, 2);
            label_priceChange.ForeColor = change < 0 ? Color.Red : Color.Green;

            label_priceChange.Text = change > 0 ? change.ToString() + "$ ↑" : change.ToString() + "$ ↓";
        }

        /// <summary>
        /// Event handler for the click event of the "Refresh" button.
        /// Calls the refreshGrid function to update the chart based on the selected date range.
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void button_refreshBtn_MouseClick(object sender, MouseEventArgs e)
        {
            refreshGrid();
        }

        /// <summary>
        /// Event handler for the selection change in the candlestick patterns dropdown.
        /// Clears existing chart annotations and adds new annotations based on the selected pattern.
        /// </summary>
        /// <param name="sender">The object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void comboBox_patterns_SelectedIndexChanged(object sender, EventArgs e)
        {
            chart_data.Annotations.Clear();
            
            var reco = recognizers[comboBox_patterns.SelectedIndex];

            for(int i = 0; i < tempop.Count ; i++) 
            {
                if (reco.recognizePattern(tempop[i]))
                {
                    if(reco.patternSize == 1)
                    {
                        CreateAnnotation(tempop[i]);
                    }
                    else
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Creates an annotation on the chart for a specific candlestick.
        /// </summary>
        /// <param name="cs">The smartCandlestick for which to create the annotation</param>
        public void CreateAnnotation(smartCandlestick cs) 
        {
            var arrowAnnotation = new ArrowAnnotation();
            arrowAnnotation.AxisX = chart_data.ChartAreas[0].AxisX;
            arrowAnnotation.AxisY = chart_data.ChartAreas[0].AxisY;
            arrowAnnotation.X = cs.date.ToOADate();

            arrowAnnotation.Y = (double)(cs.low) - 5;
            arrowAnnotation.LineWidth = 1;
            arrowAnnotation.Width = 0;
            arrowAnnotation.Height = 5;
            arrowAnnotation.ArrowSize = 2;

            arrowAnnotation.LineColor = cs.isBullish ? Color.Green : Color.Red;

            chart_data.Annotations.Add(arrowAnnotation);
        }

        public void CreateListOfAnnotation(List<smartCandlestick> cs)
        {


        }

        public void InitRecognizers()
        {
            List<Recognizer> lr = new List<Recognizer>();
            lr.Add(new bullishRecognizer(1, "Bullish"));
            lr.Add(new bearishRecognizer(1, "Bearish"));
            lr.Add(new neutralRecognizer(1, "Neutral"));
            lr.Add(new marubozuRecognizer(1, "Marubozu"));
            lr.Add(new dojiRecognizer(1, "Doji"));
            lr.Add(new dragonflyDojiRecognizer(1, "DragonFly Doji"));
            lr.Add(new gravestoneDojiRecognizer(1, "Gravestone Doji"));
            lr.Add(new hammerRecognizer(1, "Hammer"));
            lr.Add(new invertedHammerRecognizer(1, "Inverted Hammer"));
            lr.Add(new peakRecognizer(3, "Peak"));

            recognizers = lr;
        }

        public void InitComboBox()
        {
            List<string> strings = new List<string>();
            foreach (Recognizer r in recognizers) 
            {
                strings.Add(r.patternName);
            }

            comboBox_patterns.DataSource = strings;
        }
    }
}