using System.ComponentModel;

public class ViewModel : INotifyPropertyChanged
{
    // 거실  온도
    private double _tempValue;
    public double TempValue
    {
        get { return _tempValue; }
        set
        {
            if (_tempValue != value)
            {
                _tempValue = value;
                OnPropertyChanged(nameof(TempValue));
            }
        }
    }
    // 화장실 온도
    private double _temp2Value;
    public double Temp2Value
    {
        get { return _temp2Value; }
        set
        {
            if (_temp2Value != value)
            {
                _temp2Value = value;
                OnPropertyChanged(nameof(Temp2Value));
            }
        }
    }

    // 거실 습도
    private double _value1;
    public double Value1
    {
        get { return _value1; }
        set
        {
            if (_value1 != value)
            {
                _value1 = value;
                OnPropertyChanged(nameof(Value1));
            }
        }
    }
    // 화장실 습도
    private double _value2;
    public double Value2
    {
        get { return _value2; }
        set
        {
            if (_value2 != value)
            {
                _value2 = value;
                OnPropertyChanged(nameof(Value2));
            }
        }
    }

    // ... 공통부분?,,,

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
