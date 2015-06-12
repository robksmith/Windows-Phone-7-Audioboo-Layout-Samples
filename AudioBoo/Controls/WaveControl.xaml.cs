
#region Usings

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace AudioBoo.Controls
{
    public partial class WaveControl : UserControl
    {
        #region Fields

        // grid dimensions
        const int gridWidth = 12;
        const int gridHeight = 7;

        // Store circles in a list for now
        List<WaveCircleControl> circlesList = new List<WaveCircleControl>();

        // May be better in an array though
        WaveCircleControl[,] circlesArray = new WaveCircleControl[gridWidth, gridHeight];

        // Brushes
        SolidColorBrush brushDefault = new SolidColorBrush(Color.FromArgb(0xFF, 0xbe, 0xca, 0xcb));
        SolidColorBrush brushDarker = new SolidColorBrush(Color.FromArgb(0xFF, 0x68, 0x7f, 0x8a));
        SolidColorBrush pink1 = new SolidColorBrush(Color.FromArgb(0xFF, 0x7b, 0x60, 0x85));
        SolidColorBrush pink2 = new SolidColorBrush(Color.FromArgb(0xFF, 0x92, 0x3c, 0x7f));
        SolidColorBrush pink3 = new SolidColorBrush(Color.FromArgb(0xFF, 0xb8, 0x00, 0x76));

        //public static readonly DependencyProperty WaveGainProperty =
        //    DependencyProperty.Register("WaveGain", typeof (double), typeof (WaveControl), new PropertyMetadata(default(double)));

        //public double WaveGain
        //{
        //    get
        //    {
        //        return (double) GetValue(WaveGainProperty);
        //    } 
            
        //    set
        //    {
        //        SetValue(WaveGainProperty, value);
        //        Update();
        //    }
        //}


        #endregion


        #region Constructors

        public WaveControl()
        {
            InitializeComponent();

            int col = 0;
            int row = 0;

            // Grab each circle from the xaml and store them in our array and list
            foreach (UserControl c in WaveGrid.Children)
            {
                if (c is WaveCircleControl)
                {
                    WaveCircleControl circle = c as WaveCircleControl;

                    // add to list
                    circlesList.Add(circle);

                    // add to array as well
                    circlesArray[col, row] = circle;

                    // move on depending on the order of them in the page
                    row += 1;
                    if (row >= gridHeight)
                    {
                        col += 1;
                        row = 0;
                    }
                }
            }
        }

        #endregion


        #region Updates

        /// <summary>
        /// Update colour / movement of the circles
        /// </summary>
        public void Update(double gain)
        {
            // Debug text
            TextGain.Text = string.Format("gain {0:0.##}", gain);

            // Reset the colours
            ResetColours();

            // If the gain is too low then don't draw any wave
            if (gain <= 0.05) return;
            if (gain > 1.0) gain = 1.0;

            double rnd = 1.0f;

            // Fill columns to left
            FillSurroundingColumn(0, gain - (rnd * 0.6));
            FillSurroundingColumn(1, gain - (rnd * 0.5));
            FillSurroundingColumn(2, gain - (rnd * 0.4));
            FillSurroundingColumn(3, gain - (rnd * 0.3));
            FillSurroundingColumn(4, gain - (rnd * 0.2));
            

            // fill center column with pink up to the gain
            FillCenterColumn(5, gain);

            // Fill columns to right
            FillSurroundingColumn(6, gain - (rnd * 0.2));
            FillSurroundingColumn(7, gain - (rnd * 0.3));
            FillSurroundingColumn(8, gain - (rnd * 0.4));
            FillSurroundingColumn(9, gain - (rnd * 0.5));
            FillSurroundingColumn(10, gain - (rnd * 0.6));
            FillSurroundingColumn(11, gain - (rnd * 0.7));
        }

        private void FillCenterColumn(int col, double gain)
        {
            // work out the height (row) from the gain
            int row = (int)((double)(gridHeight * gain));

            // The pinkest
            FillCircle(col, row, pink3);

            // darker pink
            row -= 1;
            FillCircle(col, row, pink2);

            // light pink
            row -= 1;
            FillCircle(col, row, pink1);

            // light pink
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);
        }

        private void FillSurroundingColumn(int col, double limit)
        {
            // work out row from the gain
            int row = (int)((double)(gridHeight * limit));

            FillCircle(col, row, brushDarker);

            row -= 1;
            FillCircle(col, row, brushDarker);

            row -= 1;
            FillCircle(col, row, brushDarker);

            // light pink
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);

            // dark gray
            row -= 1;
            FillCircle(col, row, brushDarker);
        }

        private void FillCircle(int col, int row, SolidColorBrush brush)
        {
            if (row < gridHeight && row >=0)
            {
                circlesArray[col, row].Circle.Fill = brush;
            }
        }

        /// <summary>
        /// Reset all colour in the circle
        /// </summary>
        public void ResetColours()
        {
            foreach (WaveCircleControl circle in circlesList)
            {
                circle.Circle.Fill = brushDefault; 
            }
        }

        #endregion

    }
}
