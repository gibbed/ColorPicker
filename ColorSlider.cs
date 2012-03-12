/* Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ColorPicker
{
    public partial class ColorSlider : UserControl
    {
        public ColorSlider()
        {
            this.InitializeComponent();
            this.Mode = SliderMode.Channel;
        }

        private int _IgnoreChangedEventCounter;

        private bool IgnoreChangedEvents
        {
            get { return this._IgnoreChangedEventCounter != 0; }
        }

        private void PushIgnoreChangedEvents()
        {
            ++this._IgnoreChangedEventCounter;
        }

        private void PopIgnoreChangedEvents()
        {
            --this._IgnoreChangedEventCounter;
        }

        [Bindable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get { return this.label.Text; }
            set { this.label.Text = value; }
        }

        public enum SliderMode
        {
            Channel,
            Degrees,
            Total,
        }

        private SliderMode _Mode;

        public SliderMode Mode
        {
            get { return this._Mode; }
            set
            {
                switch (value)
                {
                    case SliderMode.Channel:
                    {
                        this.Minimum = 0;
                        this.Maximum = 255;
                        break;
                    }

                    case SliderMode.Degrees:
                    {
                        this.Minimum = 0;
                        this.Maximum = 360;
                        break;
                    }

                    case SliderMode.Total:
                    {
                        this.Minimum = 0;
                        this.Maximum = 100;
                        break;
                    }
                }

                this.Increment = 1;
                this._Mode = value;
            }
        }

        public int Value
        {
            get { return (int)this.numericUpDown.Value; }
            set
            {
                // ReSharper disable RedundantCheckBeforeAssignment
                if (this.numericUpDown.Value != value) // ReSharper restore RedundantCheckBeforeAssignment
                {
                    this.numericUpDown.Value = value;
                }
            }
        }

        private int Increment
        {
            // ReSharper disable UnusedMember.Local
            get { return (int)this.numericUpDown.Increment; }
            // ReSharper restore UnusedMember.Local
            set { this.numericUpDown.Increment = value; }
        }

        private int Minimum
        {
            // ReSharper disable UnusedMember.Local
            get { return (int)this.numericUpDown.Minimum; }
            // ReSharper restore UnusedMember.Local
            set { this.numericUpDown.Minimum = value; }
        }

        private int Maximum
        {
            // ReSharper disable UnusedMember.Local
            get { return (int)this.numericUpDown.Maximum; }
            // ReSharper restore UnusedMember.Local
            set { this.numericUpDown.Maximum = value; }
        }

        [DisplayName("Min Color")]
        public Color MinColor
        {
            get { return this.colorGradient.MinColor; }
            set { this.colorGradient.MinColor = value; }
        }

        [DisplayName("Max Color")]
        public Color MaxColor
        {
            get { return this.colorGradient.MaxColor; }
            set { this.colorGradient.MaxColor = value; }
        }

        [DisplayName("Custom Gradient")]
        public Color[] CustomGradient
        {
            get { return this.colorGradient.CustomGradient; }
            set { this.colorGradient.CustomGradient = value; }
        }

        private void OnUpDownEnter(object sender, EventArgs e)
        {
            this.numericUpDown.Select(0, this.numericUpDown.Text.Length);
        }

        private void OnUpDownLeave(object sender, EventArgs e)
        {
            this.OnValueChanged(sender, e);
        }

        public event EventHandler ValueChanged;

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (this.IgnoreChangedEvents)
            {
                return;
            }

            this.PushIgnoreChangedEvents();

            if (sender == colorGradient)
            {
                int value;

                switch (this.Mode)
                {
                    case SliderMode.Channel:
                    {
                        value = colorGradient.Value;
                        break;
                    }

                    case SliderMode.Degrees:
                    {
                        value = (colorGradient.Value * 360) / 255;
                        break;
                    }

                    case SliderMode.Total:
                    {
                        value = (colorGradient.Value * 100) / 255;
                        break;
                    }

                    default:
                    {
                        throw new Exception();
                    }
                }

                // ReSharper disable RedundantCheckBeforeAssignment
                if (numericUpDown.Value != value) // ReSharper restore RedundantCheckBeforeAssignment
                {
                    numericUpDown.Value = value;
                }
            }
            else if (sender == numericUpDown)
            {
                int value;

                switch (this.Mode)
                {
                    case SliderMode.Channel:
                    {
                        value = (int)numericUpDown.Value;
                        break;
                    }

                    case SliderMode.Degrees:
                    {
                        value = ((int)numericUpDown.Value * 255) / 360;
                        break;
                    }

                    case SliderMode.Total:
                    {
                        value = ((int)numericUpDown.Value * 255) / 100;
                        break;
                    }

                    default:
                    {
                        throw new Exception();
                    }
                }

                // ReSharper disable RedundantCheckBeforeAssignment
                if (colorGradient.Value != value) // ReSharper restore RedundantCheckBeforeAssignment
                {
                    colorGradient.Value = value;
                }
            }

            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }

            this.PopIgnoreChangedEvents();
        }

        private void OnUpDownKeyUp(object sender, KeyEventArgs e)
        {
            this.OnValueChanged(sender, new EventArgs());
        }
    }
}
