using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;

[assembly: AssemblyTitle("Xamarin.Forms.Platform.Tizen")]
[assembly: AssemblyDescription("Tizen Backend for Xamarin.Forms")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCulture("")]

//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

[assembly: Dependency(typeof(ResourcesProvider))]
[assembly: Dependency(typeof(Deserializer))]
[assembly: Dependency(typeof(NativeBindingService))]
[assembly: Dependency(typeof(NativeValueConverterService))]

[assembly: ExportRenderer(typeof(Layout), typeof(LayoutRenderer))]
[assembly: ExportRenderer(typeof(ScrollView), typeof(ScrollViewRenderer))]
[assembly: ExportRenderer(typeof(CarouselPage), typeof(CarouselPageRenderer))]
[assembly: ExportRenderer(typeof(ContentPage), typeof(ContentPageRenderer))]
[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]
[assembly: ExportRenderer(typeof(MasterDetailPage), typeof(MasterDetailPageRenderer))]
[assembly: ExportRenderer(typeof(TabbedPage), typeof(TabbedPageRenderer))]

[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer))]
[assembly: ExportRenderer(typeof(Button), typeof(ButtonRenderer))]
[assembly: ExportRenderer(typeof(Image), typeof(ImageRenderer))]
[assembly: ExportRenderer(typeof(Slider), typeof(SliderRenderer))]
[assembly: ExportRenderer(typeof(Picker), typeof(PickerRenderer))]
[assembly: ExportRenderer(typeof(Frame), typeof(FrameRenderer))]
[assembly: ExportRenderer(typeof(Stepper), typeof(StepperRenderer))]
[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerRenderer))]
[assembly: ExportRenderer(typeof(TimePicker), typeof(TimePickerRenderer))]
[assembly: ExportRenderer(typeof(ProgressBar), typeof(ProgressBarRenderer))]
[assembly: ExportRenderer(typeof(Switch), typeof(SwitchRenderer))]
[assembly: ExportRenderer(typeof(ListView), typeof(ListViewRenderer))]
[assembly: ExportRenderer(typeof(BoxView), typeof(BoxViewRenderer))]
[assembly: ExportRenderer(typeof(ActivityIndicator), typeof(ActivityIndicatorRenderer))]
[assembly: ExportRenderer(typeof(SearchBar), typeof(SearchBarRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(EntryRenderer))]
[assembly: ExportRenderer(typeof(Editor), typeof(EditorRenderer))]
[assembly: ExportRenderer(typeof(TableView), typeof(TableViewRenderer))]
[assembly: ExportRenderer(typeof(EvasObjectWrapper), typeof(EvasObjectWrapperRenderer))]
[assembly: ExportRenderer(typeof(WebView), typeof(WebViewRenderer))]

[assembly: ExportImageSourceHandler(typeof(FileImageSource), typeof(FileImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(StreamImageSource), typeof(StreamImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(UriImageSource), typeof(UriImageSourceHandler))]

[assembly: ExportCell(typeof(TextCell), typeof(TextCellRenderer))]
[assembly: ExportCell(typeof(ImageCell), typeof(ImageCellRenderer))]
[assembly: ExportCell(typeof(SwitchCell), typeof(SwitchCellRenderer))]
[assembly: ExportCell(typeof(EntryCell), typeof(EntryCellRenderer))]
[assembly: ExportCell(typeof(ViewCell), typeof(ViewCellRenderer))]

[assembly: ExportHandler(typeof(TapGestureRecognizer), typeof(TapGestureHandler))]
[assembly: ExportHandler(typeof(PinchGestureRecognizer), typeof(PinchGestureHandler))]
[assembly: ExportHandler(typeof(PanGestureRecognizer), typeof(PanGestureHandler))]