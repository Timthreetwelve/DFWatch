// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Media;

namespace NumericUpDown;

/*
    2021.02.20/jms
    This code started out from this CodeProject article (posted in 2011):
    https://www.codeproject.com/Articles/151298/WPF-User-Control-NumericBox?msg=5788701#xx5788701xx
    (WPF User Control - NumericBox)

    I change the following:
    - deleted menu and popup components
    - Deleted the timer and associated button click preview events
    - Converted the reguklar Button controls to RepeatButton controls
    - Converted from double to int support
    - Improved the handling of manually entered text
    - Improved general layout of components
    - Changed button style to be more to myu own tastes
*/

[TemplatePart(Name = "PART_NumericTextBox", Type = typeof(TextBox))]
[TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
[TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
/// <summary>
/// WPF User control - IntegerUpDown
/// </summary>
public partial class IntegerUpDown : UserControl
{
#pragma warning disable IDE0017 // Simplify object initialization
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable IDE0052 // Remove unread private members
    #region Variables
    private int value;           // value
    private int increment;       // increment
    private int minimum;         // minimum value
    private int maximum;         // maximum value
    private string valueFormat;  // string format of the value
    #endregion
#pragma warning restore IDE0052 // Remove unread private members

    public IntegerUpDown()
    {
        this.InitializeComponent();
    }

    #region Properties

    new public Brush Foreground
    {
        get { return this.PART_NumericTextBox.Foreground; }
        set { this.PART_NumericTextBox.Foreground = value; }
    }

    //===========================================================
    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register("Minimum", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(int.MinValue, OnMinimumChanged));
    private static void OnMinimumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        IntegerUpDown numericBoxControl = new IntegerUpDown();
        numericBoxControl.minimum = (int)args.NewValue;
    }
    public int Minimum
    {
        get { return (int)this.GetValue(MinimumProperty); }
        set { this.SetValue(MinimumProperty, value); }
    }

    //===========================================================
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register("Maximum", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(int.MaxValue, OnMaximumChanged));
    private static void OnMaximumChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        IntegerUpDown numericBoxControl = new IntegerUpDown();
        numericBoxControl.maximum = (int)args.NewValue;
    }
    public int Maximum
    {
        get { return (int)this.GetValue(MaximumProperty); }
        set { this.SetValue(MaximumProperty, value); }
    }

    //===========================================================
    public static readonly DependencyProperty IncrementProperty =
        DependencyProperty.Register("Increment", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(1, OnIncrementChanged));
    private static void OnIncrementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        IntegerUpDown numericBoxControl = new IntegerUpDown();
        numericBoxControl.increment = (int)args.NewValue;
    }
    public int Increment
    {
        get { return (int)this.GetValue(IncrementProperty); }
        set { this.SetValue(IncrementProperty, value); }
    }

    //===========================================================
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(int), typeof(IntegerUpDown), new PropertyMetadata(new Int32(), OnValueChanged));
    private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        IntegerUpDown numericBoxControl = (IntegerUpDown)sender;
        numericBoxControl.value = (int)args.NewValue;
        numericBoxControl.PART_NumericTextBox.Text = numericBoxControl.value.ToString(numericBoxControl.ValueFormat);
        numericBoxControl.OnValueChanged((int)args.OldValue, (int)args.NewValue);
    }
    public int Value
    {
        get { return (int)this.GetValue(ValueProperty); }
        set { this.SetValue(ValueProperty, value); }
    }

    //===========================================================
    public static readonly DependencyProperty ValueFormatProperty =
        DependencyProperty.Register("ValueFormat", typeof(string), typeof(IntegerUpDown), new PropertyMetadata("0", OnValueFormatChanged));
    private static void OnValueFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        IntegerUpDown numericBoxControl = new IntegerUpDown();
        numericBoxControl.valueFormat = (string)args.NewValue;
    }
    public string ValueFormat
    {
        get { return (string)this.GetValue(ValueFormatProperty); }
        set { this.SetValue(ValueFormatProperty, value); }
    }

    #endregion

    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventHandler<int>), typeof(IntegerUpDown));

    public event RoutedPropertyChangedEventHandler<int> ValueChanged
    {
        add { this.AddHandler(ValueChangedEvent, value); }
        remove { this.RemoveHandler(ValueChangedEvent, value); }
    }

    private void OnValueChanged(int oldValue, int newValue)
    {
        RoutedPropertyChangedEventArgs<int> args = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue);
        args.RoutedEvent = IntegerUpDown.ValueChangedEvent;
        this.RaiseEvent(args);
    }

    #region Events

    private void IncreaseBtn_Click(object sender, RoutedEventArgs e)
    {
        this.IncreaseValue();
    }

    private void DecreaseBtn_Click(object sender, RoutedEventArgs e)
    {
        this.DecreaseValue();
    }

    private void NumericBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textbox = sender as TextBox;
        int caretIndex = textbox.CaretIndex;
        try
        {
            bool error = !int.TryParse(e.Text, out int newvalue);
            string text = textbox.Text;
            if (!error)
            {
                text = text.Insert(textbox.CaretIndex, e.Text);
                error = !int.TryParse(text, out newvalue);
                if (!error)
                {
                    error = (newvalue < this.Minimum || newvalue > this.Maximum);
                }
            }
            if (error)
            {
                SystemSounds.Hand.Play();
                textbox.CaretIndex = caretIndex;
            }
            else
            {
                this.PART_NumericTextBox.Text = text;
                textbox.CaretIndex = caretIndex + e.Text.Length;
                this.Value = newvalue;
            }
        }
        catch (FormatException)
        {
        }
        e.Handled = true;
    }

    private void NumericBox_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
        {
            this.IncreaseValue();
        }
        else if (e.Delta < 0)
        {
            this.DecreaseValue();
        }
    }

    #endregion Events

    #region Private Methods

    //=============================================================
    /// <summary>
    /// Increase value
    /// </summary>
    private void IncreaseValue()
    {
        Value = Math.Min(this.Maximum, this.Value + this.Increment);
    }
    //=============================================================
    /// <summary>
    /// Decrease value
    /// </summary>
    private void DecreaseValue()
    {
        Value = Math.Max(this.Minimum, this.Value - this.Increment);
    }

    #endregion

    #region Overridden Methods

    //=============================================================
    /// <summary>
    /// Apply new templates after setting new style
    /// </summary>
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_IncreaseButton") is RepeatButton btn)
        {
            btn.Click += IncreaseBtn_Click;
        }

        btn = GetTemplateChild("PART_DecreaseButton") as RepeatButton;
        if (btn != null)
        {
            btn.Click += DecreaseBtn_Click;
        }

        if (GetTemplateChild("PART_NumericTextBox") is TextBox tb)
        {
            PART_NumericTextBox = tb;
            PART_NumericTextBox.Text = Value.ToString(ValueFormat);
            PART_NumericTextBox.PreviewTextInput += NumericBox_PreviewTextInput;
            PART_NumericTextBox.MouseWheel += NumericBox_MouseWheel;
        }

        btn = null;
        tb = null;
    }

    #endregion Overridden Methods
#pragma warning restore IDE0017 // Simplify object initialization
#pragma warning restore IDE0090 // Use 'new(...)'

}
