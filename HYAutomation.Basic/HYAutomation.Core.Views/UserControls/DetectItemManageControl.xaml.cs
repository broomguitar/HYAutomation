using HYAutomation.BaseView;
using HYAutomation.BaseView.ValueConverters;
using HYAutomation.Core.Algorithm.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HYAutomation.Core.Views.UserControls
{
    /// <summary>
    /// DetectItemManageControl.xaml 的交互逻辑
    /// </summary>
    public partial class DetectItemManageControl : UserControl
    {
        public DetectItemManageControl()
        {
            InitializeComponent();
        }

        private void SourceImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DetectionItems != null)
            {
                foreach (var item in (IEnumerable<DetectItemModel>)DetectionItems)
                {
                    if (markerDic.ContainsKey(item.Guid) && !item.DetectItemRegion.IsEmpty)
                    {
                        Rectangle rectangle = markerDic[item.Guid];
                        var bitmapsource = (BitmapSource)TypeImageSource;
                        double scaleX = bitmapsource.PixelWidth / SourceImage.ActualWidth;
                        double scaleY = bitmapsource.PixelHeight / SourceImage.ActualHeight;
                        Canvas.SetLeft(rectangle, item.DetectItemRegion.X / scaleX);
                        Canvas.SetTop(rectangle, item.DetectItemRegion.Y / scaleY);
                        rectangle.Width = item.DetectItemRegion.Width / scaleX;
                        rectangle.Height = item.DetectItemRegion.Height / scaleY;
                    }
                }
            }
        }

        private Rectangle rectangle;
        private Point startLocation;
        private readonly Random random = new Random(Environment.TickCount);
        private readonly List<Brush> brushes = new List<Brush> { Brushes.Green, Brushes.Red, Brushes.Blue, Brushes.Gray, Brushes.Orange, Brushes.Yellow, Brushes.Brown, Brushes.Pink };
        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!IsCanEdit) return;

                if (SelectedDetectionItem == null)
                {
                    startLocation = e.GetPosition(SourceImage);
                    rectangle = new Rectangle
                    {
                        StrokeThickness = 2,
                        Stroke = brushes[random.Next(0, brushes.Count)]
                    };
                    Canvas.SetLeft(rectangle, startLocation.X);
                    Canvas.SetTop(rectangle, startLocation.Y);
                    mainCanvas.Children.Add(rectangle);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
           
            }
        }
        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!IsCanEdit) return;
                var point = e.GetPosition(SourceImage);
                if (point.Equals(startLocation))
                {
                    mainCanvas.Children.Remove(rectangle);
                    return;
                }
                this.ReleaseMouseCapture();
                var bitmapsource = (BitmapSource)TypeImageSource;
                double scaleX = bitmapsource.PixelWidth / SourceImage.ActualWidth;
                double scaleY = bitmapsource.PixelHeight / SourceImage.ActualHeight;
                var rect = new Int32Rect((int)(Canvas.GetLeft(rectangle) * scaleX), (int)(Canvas.GetTop(rectangle) * scaleY), (int)(rectangle.Width * scaleX), (int)(rectangle.Height * scaleY));
                if (SelectedDetectionItem == null)
                {
                    var newData = new DetectItemModel { DetectItemRegion = rect, DetectItemLocation = startLocation };
                    Binding binding = new Binding
                    {
                        Source = newData,
                        Path = new PropertyPath("DetectItemConfig.MarkerBorderBrushStr")
                    };
                    binding.Converter = new StringToBrushConverter();
                    rectangle.SetBinding(Shape.StrokeProperty, binding);
                    Binding bindingVisible = new Binding
                    {
                        Source = newData,
                        Path = new PropertyPath("TargetVisibility"),
                        Mode = BindingMode.OneWay
                    };
                    rectangle.SetBinding(VisibilityProperty, bindingVisible);
                    if (DetectionItems is IList list)
                    {
                        list.Add(newData);
                        if (newData.DetectItemConfig == null)
                        {
                            list.Remove(newData);
                            mainCanvas.Children.Remove(rectangle);
                            return;
                        }
                    }
                    markerDic.Add(newData.Guid, rectangle);
                }
                else
                {
                    SelectedDetectionItem.DetectItemRegion = rect;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
           
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {

        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (!IsCanEdit) return;
                if (e.LeftButton == MouseButtonState.Pressed && rectangle != null)
                {
                    Point endLoaction = e.GetPosition(SourceImage);
                    double width = endLoaction.X - startLocation.X;
                    double height = endLoaction.Y - startLocation.Y;
                    if (width < 0)
                    {
                        Canvas.SetLeft(rectangle, endLoaction.X);
                    }
                    if (height < 0)
                    {
                        Canvas.SetTop(rectangle, endLoaction.Y);
                    }
                    rectangle.Width = Math.Abs(width);
                    rectangle.Height = Math.Abs(height);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.AddLog(ex.ToString(), true);
           
            }
        }
        private readonly Dictionary<Guid, Rectangle> markerDic = new Dictionary<Guid, Rectangle>();
        public ImageSource TypeImageSource
        {
            get { return (ImageSource)GetValue(TypeImageSourceProperty); }
            set { SetValue(TypeImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeImageSourceProperty =
            DependencyProperty.Register("TypeImageSource", typeof(ImageSource), typeof(DetectItemManageControl), new PropertyMetadata(null, new PropertyChangedCallback((o, e) =>
            {
            })));


        public DetectItemModel SelectedDetectionItem
        {
            get { return (DetectItemModel)GetValue(SelectedDetectionItemProperty); }
            set { SetValue(SelectedDetectionItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDetectionItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDetectionItemProperty =
            DependencyProperty.Register("SelectedDetectionItem", typeof(DetectItemModel), typeof(DetectItemManageControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyChanged)));

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DetectItemManageControl imageControl)
            {
                if (e.OldValue != null && e.OldValue is DetectItemModel oldData)
                {
                    if (imageControl.markerDic.ContainsKey(oldData.Guid))
                    {
                        imageControl.markerDic[oldData.Guid].StrokeDashArray = default(DoubleCollection);
                    }
                }
                if (e.NewValue != null && e.NewValue is DetectItemModel newData)
                {
                    if (imageControl.markerDic.ContainsKey(newData.Guid))
                    {
                        imageControl.rectangle = imageControl.markerDic[newData.Guid];
                        imageControl.rectangle.StrokeDashArray = DoubleCollection.Parse("4,2");
                        imageControl.startLocation = newData.DetectItemLocation;
                    }
                }
            }
        }



        public IEnumerable DetectionItems
        {
            get { return (IEnumerable)GetValue(DetectionItemsProperty); }
            set { SetValue(DetectionItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DetectionItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DetectionItemsProperty =
            DependencyProperty.Register("DetectionItems", typeof(IEnumerable), typeof(DetectItemManageControl), new PropertyMetadata(null, new PropertyChangedCallback(DetctionItemsPropertyChanged)));

        private static void DetctionItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DetectItemManageControl imageControl)
            {
                _imageControl = imageControl;
                if (e.OldValue != null && e.OldValue is IEnumerable<DetectItemModel> olditems)
                {
                    if (e.OldValue is System.Collections.Specialized.INotifyCollectionChanged data)
                    {
                        data.CollectionChanged -= Data_CollectionChanged;
                    }
                    foreach (var item in olditems)
                    {
                        if (e.NewValue == null || !(e.NewValue is IEnumerable<DetectItemModel> enumerable) || !enumerable.Contains(item))
                        {
                            if (imageControl.markerDic.ContainsKey(item.Guid))
                            {
                                DelItem(imageControl, item.Guid);
                            }
                        }
                    }
                }
                if (e.NewValue != null && e.NewValue is IEnumerable<DetectItemModel> items)
                {
                    foreach (var item in items)
                    {
                        if (!imageControl.markerDic.ContainsKey(item.Guid) && !item.DetectItemRegion.IsEmpty)
                        {
                            Rectangle rectangle = new Rectangle();
                            var bitmapsource = (BitmapSource)imageControl.TypeImageSource;
                            double scaleX = bitmapsource.PixelWidth / imageControl.ActualWidth;
                            double scaleY = bitmapsource.PixelHeight / imageControl.ActualHeight;
                            Canvas.SetLeft(rectangle, item.DetectItemRegion.X / scaleX);
                            Canvas.SetTop(rectangle, item.DetectItemRegion.Y / scaleY);
                            rectangle.Width = item.DetectItemRegion.Width / scaleX;
                            rectangle.Height = item.DetectItemRegion.Height / scaleY;
                            if (item.DetectItemConfig != null)
                            {
                                rectangle.Stroke = (Brush)new BrushConverter().ConvertFromString(item.DetectItemConfig.MarkerBorderBrushStr);
                            }
                            rectangle.StrokeThickness = 2;
                            Binding binding = new Binding
                            {
                                Source = item,
                                Path = new PropertyPath("DetectItemConfig.MarkerBorderBrushStr")
                            };
                            //rectangle.SetBinding(Shape.StrokeProperty, binding);
                            Binding bindingVisible = new Binding
                            {
                                Source = item,
                                Path = new PropertyPath("TargetVisibility"),
                                Mode = BindingMode.OneWay
                            };
                            rectangle.SetBinding(VisibilityProperty, bindingVisible);
                            AddItem(imageControl, item.Guid, rectangle);
                        }
                    }
                    if (e.NewValue is System.Collections.Specialized.INotifyCollectionChanged data)
                    {
                        data.CollectionChanged += Data_CollectionChanged;
                    }
                }
            }
        }
        private static DetectItemManageControl _imageControl;
        private static void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                if (_imageControl != null)
                {
                    foreach (DetectItemModel item in e.OldItems.OfType<DetectItemModel>())
                    {
                        if (_imageControl.markerDic.ContainsKey(item.Guid))
                        {
                            DelItem(_imageControl, item.Guid);
                        }
                    }
                }
            }
        }

        private static void AddItem(DetectItemManageControl itemManageControl, Guid guid, Rectangle rectangle)
        {
            itemManageControl.mainCanvas.Children.Add(rectangle);
            itemManageControl.markerDic.Add(guid, rectangle);
        }
        private static void DelItem(DetectItemManageControl imageControl, Guid guid)
        {
            imageControl.mainCanvas.Children.Remove(imageControl.markerDic[guid]);
            imageControl.markerDic.Remove(guid);
        }


        public bool IsCanEdit
        {
            get { return (bool)GetValue(IsCanEditProperty); }
            set { SetValue(IsCanEditProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCanEdit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCanEditProperty =
            DependencyProperty.Register("IsCanEdit", typeof(bool), typeof(DetectItemManageControl), new PropertyMetadata(true));
    }
}
