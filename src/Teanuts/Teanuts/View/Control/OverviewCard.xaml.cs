using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Teanuts.View.Control;

public sealed partial class OverviewCard : UserControl
{
    public OverviewCard()
    {
        this.InitializeComponent();
    }
    
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(OverviewCard), new PropertyMetadata("Title"));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(OverviewCard), new PropertyMetadata("Text"));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(OverviewCard), new PropertyMetadata("Description"));
    
    public IconElement Icon
    {
        get { return (IconElement)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(IconElement), typeof(OverviewCard), new PropertyMetadata(null));
}
